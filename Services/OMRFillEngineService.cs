using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using OMRAutoFillApp.Models;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace OMRAutoFillApp.Services
{
    public interface IOMRFillEngineService
    {
        byte[] FillOMR(Stream templateImageStream, Stream configurationStream, string rollNumber, string registrationNumber, string[]? mcqAnswers = null, bool debugOverlay = false);
    }

    public class OMRFillEngineService : IOMRFillEngineService
    {
        private const float BubbleRadius = 15; // Base bubble radius (pixels) sized for 300 DPI templates before scaling
        private const int StreamReaderBufferSize = 1024;
        private const int MinimumXmlContentLength = 50; // Minimum characters for a valid XML document
        private const int MinimumReferenceSize = 1;
        private const float MinimumBubbleRadius = 1f;

        public byte[] FillOMR(Stream templateImageStream, Stream configurationStream, string rollNumber, string registrationNumber, string[]? mcqAnswers = null, bool debugOverlay = false)
        {
            // Load the uploaded template image first (we need dimensions for legacy format)
            using var image = Image.Load(templateImageStream);
            NormalizeImage(image);
            
            // Load configuration from uploaded XML, passing image dimensions
            var config = LoadConfiguration(configurationStream, image.Width, image.Height);

            // Validate inputs
            ValidateInputs(config, rollNumber, registrationNumber, mcqAnswers);

            var (scaleX, scaleY) = CalculateScaleFactors(image, config);
            var scaledBubbleRadius = CalculateScaledBubbleRadius(scaleX, scaleY);

            // Fill roll number
            FillRollNumber(image, config, rollNumber, scaleX, scaleY, scaledBubbleRadius);

            // Fill registration number
            FillRegistrationNumber(image, config, registrationNumber, scaleX, scaleY, scaledBubbleRadius);

            // Fill MCQ answers (only for MCQ templates)
            if (config.TemplateType.Equals("MCQ", StringComparison.OrdinalIgnoreCase) && mcqAnswers != null)
            {
                FillMCQAnswers(image, config, mcqAnswers, scaleX, scaleY, scaledBubbleRadius);
            }

            if (debugOverlay)
            {
                DrawDebugOverlay(image, config, scaleX, scaleY, scaledBubbleRadius);
            }

            // Convert to byte array
            using var ms = new MemoryStream();
            image.SaveAsPng(ms);
            return ms.ToArray();
        }

        private TemplateConfiguration LoadConfiguration(Stream configStream, int imageWidth, int imageHeight)
        {
            try
            {
                configStream.Position = 0; // Reset stream position to beginning
                
                // First, peek at the XML to determine format
                var xmlContent = ReadStreamContent(configStream);
                var rootElementName = GetRootElementName(xmlContent);
                
                // Reset stream and deserialize based on format
                configStream.Position = 0;
                TemplateConfiguration config;
                
                if (rootElementName == "Page")
                {
                    // Legacy format - deserialize and convert using actual image dimensions
                    var legacySerializer = new XmlSerializer(typeof(LegacyOMRConfiguration));
                    var legacyConfig = legacySerializer.Deserialize(configStream) as LegacyOMRConfiguration;
                    
                    if (legacyConfig == null)
                    {
                        throw new InvalidOperationException("Failed to deserialize legacy XML configuration.");
                    }
                    
                    // Convert legacy format to current format with actual image dimensions for accurate scaling
                    config = LegacyOMRConverter.ConvertToTemplateConfiguration(legacyConfig, imageWidth, imageHeight);
                }
                else if (rootElementName == "TemplateConfiguration")
                {
                    // Current format - deserialize directly
                    var serializer = new XmlSerializer(typeof(TemplateConfiguration));
                    var deserializedConfig = serializer.Deserialize(configStream) as TemplateConfiguration;
                    
                    if (deserializedConfig == null)
                    {
                        throw new InvalidOperationException("XML deserialization returned null. The root element must be '<TemplateConfiguration>'.");
                    }
                    
                    config = deserializedConfig;
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Unsupported XML format: The root element is '<{rootElementName}>'. " +
                        "This application supports '<TemplateConfiguration>' (current format) or '<Page>' (legacy format). " +
                        "Please refer to Templates/XML_FORMAT_REFERENCE.md for the correct format."
                    );
                }
                
                // Validate that essential fields are populated
                ValidateConfiguration(config);
                
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
                throw new ArgumentException($"Failed to load template configuration: {ex.Message}. Please ensure your XML file follows a supported format.", ex);
            }
        }
        
        private void ValidateConfiguration(TemplateConfiguration config)
        {
            if (string.IsNullOrEmpty(config.TemplateId))
            {
                throw new InvalidOperationException("TemplateId is missing in configuration.");
            }
            
            if (config.Roll == null || config.Roll.Columns == null || config.Roll.Columns.Count == 0)
            {
                throw new InvalidOperationException("Roll configuration is missing or invalid.");
            }
            
            if (config.Reg == null || config.Reg.Columns == null || config.Reg.Columns.Count == 0)
            {
                throw new InvalidOperationException("Registration configuration is missing or invalid.");
            }
        }

        private string ReadStreamContent(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: StreamReaderBufferSize, leaveOpen: true);
            return reader.ReadToEnd();
        }

        private string GetRootElementName(string xmlContent)
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

                // Check for XML namespaces (common issue)
                if (doc.DocumentElement.NamespaceURI != string.Empty)
                {
                    throw new InvalidOperationException(
                        "Invalid XML format: The XML file contains namespaces (xmlns attributes). " +
                        "Please remove all xmlns attributes from your XML file."
                    );
                }
                
                return doc.DocumentElement.Name;
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

        private void FillRollNumber(Image image, TemplateConfiguration config, string rollNumber, float scaleX, float scaleY, float bubbleRadius)
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
                        DrawBubble(image, position.X, position.Y, scaleX, scaleY, bubbleRadius);
                    }
                }
            }
        }

        private void FillRegistrationNumber(Image image, TemplateConfiguration config, string registrationNumber, float scaleX, float scaleY, float bubbleRadius)
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
                        DrawBubble(image, position.X, position.Y, scaleX, scaleY, bubbleRadius);
                    }
                }
            }
        }

        private void FillMCQAnswers(Image image, TemplateConfiguration config, string[] mcqAnswers, float scaleX, float scaleY, float bubbleRadius)
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
                        DrawBubble(image, optionPos.X, optionPos.Y, scaleX, scaleY, bubbleRadius);
                    }
                }
            }
        }

        private void DrawBubble(Image image, int originalX, int originalY, float scaleX, float scaleY, float bubbleRadius)
        {
            var scaledX = originalX * scaleX;
            var scaledY = originalY * scaleY;

            image.Mutate(ctx =>
            {
                ctx.Fill(Color.Black, new EllipsePolygon(scaledX, scaledY, bubbleRadius));
            });
        }

        private static void NormalizeImage(Image image)
        {
            image.Mutate(ctx =>
            {
                ctx.AutoOrient();
            });
        }

        private static (float scaleX, float scaleY) CalculateScaleFactors(Image image, TemplateConfiguration config)
        {
            var referenceWidth = config.ReferenceSize?.Width > 0 ? config.ReferenceSize.Width : image.Width;
            var referenceHeight = config.ReferenceSize?.Height > 0 ? config.ReferenceSize.Height : image.Height;

            if (referenceWidth < MinimumReferenceSize || referenceHeight < MinimumReferenceSize)
            {
                return (1f, 1f);
            }

            return (image.Width / (float)referenceWidth, image.Height / (float)referenceHeight);
        }

        private static float CalculateScaledBubbleRadius(float scaleX, float scaleY)
        {
            var constrainedScale = Math.Min(scaleX, scaleY);
            return Math.Max(MinimumBubbleRadius, BubbleRadius * constrainedScale);
        }

        private static void DrawDebugOverlay(Image image, TemplateConfiguration config, float scaleX, float scaleY, float bubbleRadius)
        {
            var markerRadius = Math.Max(2f, bubbleRadius * 0.35f);
            var fontSize = Math.Max(8f, markerRadius * 2);
            Font? font = null;

            var fontFamilies = SystemFonts.Families.ToList();

            if (fontFamilies.Count > 0)
            {
                var family = fontFamilies[0];

                try
                {
                    font = SystemFonts.CreateFont(family.Name, fontSize);
                }
                catch (InvalidOperationException ex)
                {
                    Debug.WriteLine($"Debug overlay font fallback failed: {ex.Message}");
                }
                catch (ArgumentException ex)
                {
                    Debug.WriteLine($"Debug overlay font fallback failed: {ex.Message}");
                }
            }

            image.Mutate(ctx =>
            {
                if (config.Roll?.Columns != null)
                {
                    for (int colIndex = 0; colIndex < config.Roll.Columns.Count; colIndex++)
                    {
                        var column = config.Roll.Columns[colIndex];
                        if (column.Positions == null) continue;

                        foreach (var position in column.Positions)
                        {
                            var x = position.X * scaleX;
                            var y = position.Y * scaleY;
                            DrawDebugMarker(ctx, x, y, markerRadius, font, $"R{colIndex + 1}:{position.Digit}");
                        }
                    }
                }

                if (config.Reg?.Columns != null)
                {
                    for (int colIndex = 0; colIndex < config.Reg.Columns.Count; colIndex++)
                    {
                        var column = config.Reg.Columns[colIndex];
                        if (column.Positions == null) continue;

                        foreach (var position in column.Positions)
                        {
                            var x = position.X * scaleX;
                            var y = position.Y * scaleY;
                            DrawDebugMarker(ctx, x, y, markerRadius, font, $"G{colIndex + 1}:{position.Digit}");
                        }
                    }
                }

                if (config.Mcq?.Coordinates != null)
                {
                    foreach (var question in config.Mcq.Coordinates)
                    {
                        if (question.OptionPositions == null) continue;

                        foreach (var option in question.OptionPositions)
                        {
                            var x = option.X * scaleX;
                            var y = option.Y * scaleY;
                            DrawDebugMarker(ctx, x, y, markerRadius, font, $"{question.Number}{option.Option}");
                        }
                    }
                }
            });
        }

        private static void DrawDebugMarker(IImageProcessingContext ctx, float x, float y, float markerRadius, Font? font, string label)
        {
            ctx.Fill(Color.Red, new EllipsePolygon(x, y, markerRadius));

            if (font != null)
            {
                ctx.DrawText(label, font, Color.Red, new PointF(x + markerRadius + 1, y - markerRadius));
            }
        }
    }
}
