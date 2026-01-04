using System.Collections.Generic;

namespace OMRAutoFillApp.Models
{
    public class TemplateConfiguration
    {
        public string TemplateId { get; set; } = string.Empty;
        public string TemplateType { get; set; } = string.Empty;
        public int Dpi { get; set; } = 300;
        public RollConfiguration? Roll { get; set; }
        public RegistrationConfiguration? Reg { get; set; }
        public McqConfiguration? Mcq { get; set; }
    }

    public class RollConfiguration
    {
        public int Digits { get; set; }
        public List<Dictionary<string, int[]>>? Columns { get; set; }
    }

    public class RegistrationConfiguration
    {
        public int Digits { get; set; }
        public List<Dictionary<string, int[]>>? Columns { get; set; }
    }

    public class McqConfiguration
    {
        public int QuestionCount { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public Dictionary<string, Dictionary<string, int[]>> Coordinates { get; set; } = new Dictionary<string, Dictionary<string, int[]>>();
    }
}
