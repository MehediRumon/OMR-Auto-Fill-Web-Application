using Microsoft.AspNetCore.Mvc;
using OMRAutoFillApp.Models.ViewModels;
using OMRAutoFillApp.Services;
using System;
using System.Linq;

namespace OMRAutoFillApp.Controllers
{
    public class OMRController : Controller
    {
        private readonly IOMRFillEngineService _omrFillEngine;

        public OMRController(IOMRFillEngineService omrFillEngine)
        {
            _omrFillEngine = omrFillEngine;
        }

        public IActionResult Index()
        {
            return View(new OMRFillViewModel());
        }

        [HttpPost]
        public IActionResult FillOMR(OMRFillViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                // Validate uploaded files
                if (model.TemplateImage == null || model.TemplateImage.Length == 0)
                {
                    ModelState.AddModelError("", "Please upload a valid template image");
                    return View("Index", model);
                }

                if (model.TemplateConfiguration == null || model.TemplateConfiguration.Length == 0)
                {
                    ModelState.AddModelError("", "Please upload a valid template configuration");
                    return View("Index", model);
                }

                // Parse MCQ answers if provided
                string[]? mcqAnswers = null;
                if (!string.IsNullOrWhiteSpace(model.McqAnswers))
                {
                    mcqAnswers = model.McqAnswers
                        .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(a => a.Trim().ToUpper())
                        .ToArray();
                }

                // Generate filled OMR using uploaded files
                using var imageStream = model.TemplateImage.OpenReadStream();
                using var configStream = model.TemplateConfiguration.OpenReadStream();

                var filledOMR = _omrFillEngine.FillOMR(
                    imageStream,
                    configStream,
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
                return View("Index", model);
            }
        }
    }
}
