using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace OMRAutoFillApp.Models.ViewModels
{
    public class OMRFillViewModel
    {
        [Required(ErrorMessage = "Please upload an OMR template")]
        public IFormFile? TemplateImage { get; set; }

        [Required(ErrorMessage = "Please upload template configuration")]
        public IFormFile? TemplateConfiguration { get; set; }

        [Required(ErrorMessage = "Roll number is required")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Roll number must contain only digits")]
        public string RollNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Registration number is required")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Registration number must contain only digits")]
        public string RegistrationNumber { get; set; } = string.Empty;

        // For MCQ templates only - comma-separated answers (e.g., "A,B,C,D,A,...")
        public string? McqAnswers { get; set; }
    }
}
