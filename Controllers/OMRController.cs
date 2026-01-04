using Microsoft.AspNetCore.Mvc;
using OMRAutoFillApp.Models.ViewModels;
using OMRAutoFillApp.Services;
using System;
using System.Linq;

namespace OMRAutoFillApp.Controllers
{
    public class OMRController : Controller
    {
        private readonly ITemplateLoaderService _templateLoader;
        private readonly IOMRFillEngineService _omrFillEngine;

        public OMRController(ITemplateLoaderService templateLoader, IOMRFillEngineService omrFillEngine)
        {
            _templateLoader = templateLoader;
            _omrFillEngine = omrFillEngine;
        }

        public IActionResult Index()
        {
            var templates = _templateLoader.GetAllTemplates();
            ViewBag.Templates = templates;
            return View(new OMRFillViewModel());
        }

        [HttpPost]
        public IActionResult FillOMR(OMRFillViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var templates = _templateLoader.GetAllTemplates();
                ViewBag.Templates = templates;
                return View("Index", model);
            }

            try
            {
                // Parse MCQ answers if provided
                string[]? mcqAnswers = null;
                if (!string.IsNullOrWhiteSpace(model.McqAnswers))
                {
                    mcqAnswers = model.McqAnswers
                        .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(a => a.Trim().ToUpper())
                        .ToArray();
                }

                // Generate filled OMR
                var filledOMR = _omrFillEngine.FillOMR(
                    model.TemplateId,
                    model.RollNumber,
                    model.RegistrationNumber,
                    mcqAnswers
                );

                // Return the filled OMR as a downloadable file
                return File(filledOMR, "image/png", $"OMR_{model.RollNumber}_{DateTime.Now:yyyyMMddHHmmss}.png");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error generating OMR: {ex.Message}");
                var templates = _templateLoader.GetAllTemplates();
                ViewBag.Templates = templates;
                return View("Index", model);
            }
        }

        [HttpGet]
        public IActionResult GetTemplateInfo(string templateId)
        {
            var metadata = _templateLoader.GetTemplateMetadata(templateId);
            if (metadata == null)
                return NotFound();

            var config = _templateLoader.LoadTemplateConfiguration(templateId);
            if (config == null)
                return NotFound();

            return Json(new
            {
                templateType = config.TemplateType,
                rollDigits = config.Roll?.Digits ?? 0,
                regDigits = config.Reg?.Digits ?? 0,
                mcqCount = config.Mcq?.QuestionCount ?? 0,
                options = config.Mcq?.Options ?? new System.Collections.Generic.List<string>()
            });
        }
    }
}
