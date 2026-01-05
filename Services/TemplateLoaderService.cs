using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using OMRAutoFillApp.Models;

namespace OMRAutoFillApp.Services
{
    public interface ITemplateLoaderService
    {
        List<TemplateMetadata> GetAllTemplates();
        TemplateMetadata? GetTemplateMetadata(string templateId);
        TemplateConfiguration? LoadTemplateConfiguration(string templateId);
    }

    public class TemplateLoaderService : ITemplateLoaderService
    {
        private readonly string _templatesBasePath;
        private readonly List<TemplateMetadata> _templateCache;

        public TemplateLoaderService(IWebHostEnvironment environment)
        {
            _templatesBasePath = Path.Combine(environment.ContentRootPath, "Templates");
            _templateCache = new List<TemplateMetadata>();
            LoadTemplateMetadata();
        }

        private void LoadTemplateMetadata()
        {
            // Load MCQ templates
            var mcqPath = Path.Combine(_templatesBasePath, "MCQ");
            if (Directory.Exists(mcqPath))
            {
                foreach (var configFile in Directory.GetFiles(mcqPath, "*.json"))
                {
                    var config = LoadConfig(configFile);
                    if (config != null)
                    {
                        var templateName = Path.GetFileNameWithoutExtension(configFile);
                        var imageFile = Directory.GetFiles(mcqPath, $"{templateName}.*")
                            .FirstOrDefault(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg"));

                        if (imageFile != null)
                        {
                            _templateCache.Add(new TemplateMetadata
                            {
                                TemplateId = config.TemplateId,
                                TemplateName = templateName,
                                TemplateType = "MCQ",
                                BaseImagePath = imageFile,
                                ConfigurationPath = configFile
                            });
                        }
                    }
                }
            }

            // Load SAQ templates
            var saqPath = Path.Combine(_templatesBasePath, "SAQ");
            if (Directory.Exists(saqPath))
            {
                foreach (var configFile in Directory.GetFiles(saqPath, "*.json"))
                {
                    var config = LoadConfig(configFile);
                    if (config != null)
                    {
                        var templateName = Path.GetFileNameWithoutExtension(configFile);
                        var imageFile = Directory.GetFiles(saqPath, $"{templateName}.*")
                            .FirstOrDefault(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg"));

                        if (imageFile != null)
                        {
                            _templateCache.Add(new TemplateMetadata
                            {
                                TemplateId = config.TemplateId,
                                TemplateName = templateName,
                                TemplateType = "SAQ",
                                BaseImagePath = imageFile,
                                ConfigurationPath = configFile
                            });
                        }
                    }
                }
            }
        }

        private TemplateConfiguration? LoadConfig(string path)
        {
            try
            {
                var json = File.ReadAllText(path);
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

        public List<TemplateMetadata> GetAllTemplates()
        {
            return _templateCache;
        }

        public TemplateMetadata? GetTemplateMetadata(string templateId)
        {
            return _templateCache.FirstOrDefault(t => t.TemplateId == templateId);
        }

        public TemplateConfiguration? LoadTemplateConfiguration(string templateId)
        {
            var metadata = GetTemplateMetadata(templateId);
            if (metadata == null) return null;

            return LoadConfig(metadata.ConfigurationPath);
        }
    }
}
