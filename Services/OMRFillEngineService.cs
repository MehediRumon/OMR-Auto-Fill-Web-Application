using System;
using System.IO;
using System.Linq;
using System.Text.Json;
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

        public byte[] FillOMR(Stream templateImageStream, Stream configurationStream, string rollNumber, string registrationNumber, string[]? mcqAnswers = null)
        {
            // Load configuration from uploaded JSON
            var config = LoadConfiguration(configurationStream);
            if (config == null)
                throw new ArgumentException("Invalid template configuration");

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

        private TemplateConfiguration? LoadConfiguration(Stream configStream)
        {
            try
            {
                using var reader = new StreamReader(configStream);
                var json = reader.ReadToEnd();
                return JsonSerializer.Deserialize<TemplateConfiguration>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch
            {
                return null;
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

                if (column.ContainsKey(digit))
                {
                    var coordinates = column[digit];
                    DrawBubble(image, coordinates[0], coordinates[1]);
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

                if (column.ContainsKey(digit))
                {
                    var coordinates = column[digit];
                    DrawBubble(image, coordinates[0], coordinates[1]);
                }
            }
        }

        private void FillMCQAnswers(Image image, TemplateConfiguration config, string[] mcqAnswers)
        {
            if (config.Mcq == null) return;

            for (int i = 0; i < mcqAnswers.Length; i++)
            {
                var questionNumber = (i + 1).ToString();
                var answer = mcqAnswers[i];

                if (config.Mcq.Coordinates.ContainsKey(questionNumber))
                {
                    var questionCoordinates = config.Mcq.Coordinates[questionNumber];
                    if (questionCoordinates.ContainsKey(answer))
                    {
                        var coordinates = questionCoordinates[answer];
                        DrawBubble(image, coordinates[0], coordinates[1]);
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
