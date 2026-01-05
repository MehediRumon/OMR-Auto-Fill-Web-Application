using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using OMRAutoFillApp.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace OMRAutoFillApp.Services
{
    public interface IOMRFillEngineService
    {
        byte[] FillOMR(Stream templateImageStream, Stream configurationStream, string rollNumber, string registrationNumber, string[]? mcqAnswers = null);
    }

    public class OMRFillEngineService : IOMRFillEngineService
    {
        private const int BubbleRadius = 6; // 5-7 px as per spec
        private const int StreamReaderBufferSize = 1024;
        private const int MinimumXmlContentLength = 50; // Minimum characters for a valid XML document

        public byte[] FillOMR(Stream templateImageStream, Stream configurationStream, string rollNumber, string registrationNumber, string[]? mcqAnswers = null)
        {
            // Load configuration from uploaded XML
            var config = LoadConfiguration(configurationStream);

            // Validate inputs
            ValidateInputs(config, rollNumber, registrationNumber, mcqAnswers);

            // Load the uploaded template image
            using var image = Image.Load(templateImageStream);

            // Fill roll number
            FillRollNumber(image, config, rollNumber);

            // Fill registration number
            FillRegistrationNumber(image, config, registrationNumber);

            // Fill MCQ answers (only for MCQ templates)
            if (config.TemplateType.Equals("MCQ", StringComparison.OrdinalIgnoreCase) && mcqAnswers != null)
            {
                FillMCQAnswers(image, config, mcqAnswers);
            }

            // Convert to byte array
            using var ms = new MemoryStream();
            image.SaveAsPng(ms);
            return ms.ToArray();
        }

        private TemplateConfiguration LoadConfiguration(Stream configStream)
        {
            try
            {
                configStream.Position = 0; // Reset stream position to beginning
                
                // First, peek at the XML to check for common issues
                var xmlContent = ReadStreamContent(configStream);
                ValidateXmlFormat(xmlContent);
                
                // Reset stream and deserialize
                configStream.Position = 0;
                var serializer = new XmlSerializer(typeof(TemplateConfiguration));
                var config = serializer.Deserialize(configStream) as TemplateConfiguration;
                
                // Validate that essential fields are populated
                if (config == null)
                {
                    throw new InvalidOperationException("XML deserialization returned null. The root element must be '<TemplateConfiguration>'.");
                }
                
                if (string.IsNullOrEmpty(config.TemplateId))
                {
                    throw new InvalidOperationException("TemplateId is missing in configuration. Please add a <TemplateId> element.");
                }
                
                if (config.Roll == null || config.Roll.Columns == null || config.Roll.Columns.Count == 0)
                {
                    throw new InvalidOperationException("Roll configuration is missing or invalid. Please add a <Roll> section with <Columns>.");
                }
                
                if (config.Reg == null || config.Reg.Columns == null || config.Reg.Columns.Count == 0)
                {
                    throw new InvalidOperationException("Registration configuration is missing or invalid. Please add a <Reg> section with <Columns>.");
                }
                
                return config;
            }
            catch (InvalidOperationException ex)
            {
                // Re-throw validation errors - message already includes reference to documentation
                throw new ArgumentException($"Failed to load template configuration: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Provide more detailed error message for other exceptions
                throw new ArgumentException($"Failed to load template configuration: {ex.Message}. Please ensure your XML file follows the correct format. Refer to Templates/XML_FORMAT_REFERENCE.md for examples.", ex);
            }
        }

        private string ReadStreamContent(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: StreamReaderBufferSize, leaveOpen: true);
            return reader.ReadToEnd();
        }

        private void ValidateXmlFormat(string xmlContent)
        {
            // Check if XML is empty or too short
            if (string.IsNullOrWhiteSpace(xmlContent) || xmlContent.Length < MinimumXmlContentLength)
            {
                throw new InvalidOperationException("The XML file appears to be empty or invalid.");
            }

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                // Check root element
                if (doc.DocumentElement == null)
                {
                    throw new InvalidOperationException("The XML file has no root element.");
                }

                var rootElementName = doc.DocumentElement.Name;
                
                // Check for common incorrect root elements
                if (rootElementName == "Page")
                {
                    throw new InvalidOperationException(
                        "Invalid XML format: The root element is '<Page>' but this application expects '<TemplateConfiguration>'. " +
                        "You appear to be using a configuration file from a different OMR system. " +
                        "Please convert your configuration to the format specified in Templates/XML_FORMAT_REFERENCE.md."
                    );
                }
                
                if (rootElementName != "TemplateConfiguration")
                {
                    throw new InvalidOperationException(
                        $"Invalid XML format: The root element is '<{rootElementName}>' but must be '<TemplateConfiguration>'. " +
                        "Please refer to Templates/XML_FORMAT_REFERENCE.md for the correct format."
                    );
                }

                // Check for XML namespaces (common issue)
                if (doc.DocumentElement.NamespaceURI != string.Empty)
                {
                    throw new InvalidOperationException(
                        "Invalid XML format: The XML file contains namespaces (xmlns attributes). " +
                        "Please remove all xmlns attributes from your XML file. " +
                        "The root element should be: <TemplateConfiguration> (without any xmlns attributes)."
                    );
                }
            }
            catch (XmlException ex)
            {
                throw new InvalidOperationException(
                    $"The XML file is malformed: {ex.Message}. " +
                    "Please ensure your XML is well-formed with proper opening and closing tags.",
                    ex
                );
            }
        }

        private void ValidateInputs(TemplateConfiguration config, string rollNumber, string registrationNumber, string[]? mcqAnswers)
        {
            // Validate roll number
            if (config.Roll != null && rollNumber.Length != config.Roll.Digits)
                throw new ArgumentException($"Roll number must be exactly {config.Roll.Digits} digits");

            // Validate registration number
            if (config.Reg != null && registrationNumber.Length != config.Reg.Digits)
                throw new ArgumentException($"Registration number must be exactly {config.Reg.Digits} digits");

            // Validate MCQ answers
            if (config.TemplateType.Equals("MCQ", StringComparison.OrdinalIgnoreCase))
            {
                if (mcqAnswers == null || mcqAnswers.Length == 0)
                    throw new ArgumentException("MCQ answers are required for MCQ templates");

                if (config.Mcq != null && mcqAnswers.Length != config.Mcq.QuestionCount)
                    throw new ArgumentException($"Expected {config.Mcq.QuestionCount} MCQ answers, but got {mcqAnswers.Length}");

                // Validate each answer is a valid option
                if (config.Mcq != null)
                {
                    foreach (var answer in mcqAnswers)
                    {
                        if (!config.Mcq.Options.Contains(answer))
                            throw new ArgumentException($"Invalid answer option: {answer}. Valid options are: {string.Join(", ", config.Mcq.Options)}");
                    }
                }
            }
        }

        private void FillRollNumber(Image image, TemplateConfiguration config, string rollNumber)
        {
            if (config.Roll == null || config.Roll.Columns == null) return;

            for (int i = 0; i < rollNumber.Length && i < config.Roll.Columns.Count; i++)
            {
                var digit = rollNumber[i].ToString();
                var column = config.Roll.Columns[i];

                if (column.Positions != null)
                {
                    var position = column.Positions.FirstOrDefault(p => p.Digit == digit);
                    if (position != null)
                    {
                        DrawBubble(image, position.X, position.Y);
                    }
                }
            }
        }

        private void FillRegistrationNumber(Image image, TemplateConfiguration config, string registrationNumber)
        {
            if (config.Reg == null || config.Reg.Columns == null) return;

            for (int i = 0; i < registrationNumber.Length && i < config.Reg.Columns.Count; i++)
            {
                var digit = registrationNumber[i].ToString();
                var column = config.Reg.Columns[i];

                if (column.Positions != null)
                {
                    var position = column.Positions.FirstOrDefault(p => p.Digit == digit);
                    if (position != null)
                    {
                        DrawBubble(image, position.X, position.Y);
                    }
                }
            }
        }

        private void FillMCQAnswers(Image image, TemplateConfiguration config, string[] mcqAnswers)
        {
            if (config.Mcq == null || config.Mcq.Coordinates == null) return;

            for (int i = 0; i < mcqAnswers.Length; i++)
            {
                var questionNumber = (i + 1).ToString();
                var answer = mcqAnswers[i];

                var question = config.Mcq.Coordinates.FirstOrDefault(q => q.Number == questionNumber);
                if (question != null && question.OptionPositions != null)
                {
                    var optionPos = question.OptionPositions.FirstOrDefault(o => o.Option == answer);
                    if (optionPos != null)
                    {
                        DrawBubble(image, optionPos.X, optionPos.Y);
                    }
                }
            }
        }

        private void DrawBubble(Image image, int x, int y)
        {
            image.Mutate(ctx =>
            {
                ctx.Fill(Color.Black, new EllipsePolygon(x, y, BubbleRadius));
            });
        }
    }
}
