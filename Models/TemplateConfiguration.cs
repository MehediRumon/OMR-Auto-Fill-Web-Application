using System.Collections.Generic;
using System.Xml.Serialization;

namespace OMRAutoFillApp.Models
{
    [XmlRoot("TemplateConfiguration")]
    public class TemplateConfiguration
    {
        [XmlElement("TemplateId")]
        public string TemplateId { get; set; } = string.Empty;
        
        [XmlElement("TemplateType")]
        public string TemplateType { get; set; } = string.Empty;
        
        [XmlElement("Dpi")]
        public int Dpi { get; set; } = 300;
        
        [XmlElement("Roll")]
        public RollConfiguration? Roll { get; set; }
        
        [XmlElement("Reg")]
        public RegistrationConfiguration? Reg { get; set; }
        
        [XmlElement("Mcq")]
        public McqConfiguration? Mcq { get; set; }
    }

    public class RollConfiguration
    {
        [XmlElement("Digits")]
        public int Digits { get; set; }
        
        [XmlArray("Columns")]
        [XmlArrayItem("Column")]
        public List<DigitCoordinate>? Columns { get; set; }
    }

    public class RegistrationConfiguration
    {
        [XmlElement("Digits")]
        public int Digits { get; set; }
        
        [XmlArray("Columns")]
        [XmlArrayItem("Column")]
        public List<DigitCoordinate>? Columns { get; set; }
    }

    public class McqConfiguration
    {
        [XmlElement("QuestionCount")]
        public int QuestionCount { get; set; }
        
        [XmlArray("Options")]
        [XmlArrayItem("Option")]
        public List<string> Options { get; set; } = new List<string>();
        
        [XmlArray("Coordinates")]
        [XmlArrayItem("Question")]
        public List<QuestionCoordinate> Coordinates { get; set; } = new List<QuestionCoordinate>();
    }

    public class DigitCoordinate
    {
        [XmlArray("Positions")]
        [XmlArrayItem("Position")]
        public List<CoordinatePosition> Positions { get; set; } = new List<CoordinatePosition>();
    }

    public class CoordinatePosition
    {
        [XmlAttribute("digit")]
        public string Digit { get; set; } = string.Empty;
        
        [XmlAttribute("x")]
        public int X { get; set; }
        
        [XmlAttribute("y")]
        public int Y { get; set; }
    }

    public class QuestionCoordinate
    {
        [XmlAttribute("number")]
        public string Number { get; set; } = string.Empty;
        
        [XmlArray("OptionPositions")]
        [XmlArrayItem("Position")]
        public List<OptionPosition> OptionPositions { get; set; } = new List<OptionPosition>();
    }

    public class OptionPosition
    {
        [XmlAttribute("option")]
        public string Option { get; set; } = string.Empty;
        
        [XmlAttribute("x")]
        public int X { get; set; }
        
        [XmlAttribute("y")]
        public int Y { get; set; }
    }
}
