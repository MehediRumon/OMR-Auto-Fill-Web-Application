using System.Collections.Generic;
using System.Xml.Serialization;

namespace OMRAutoFillApp.Models
{
    /// <summary>
    /// Legacy OMR configuration format with Page root element.
    /// This format is used by older OMR systems for grid-based reading/generation.
    /// </summary>
    [XmlRoot("Page")]
    public class LegacyOMRConfiguration
    {
        [XmlAttribute("name")]
        public string Name { get; set; } = string.Empty;
        
        [XmlElement("design")]
        public LegacyDesign? Design { get; set; }
    }
    
    public class LegacyDesign
    {
        [XmlAttribute("designFor")]
        public string DesignFor { get; set; } = string.Empty;
        
        [XmlAttribute("mappingStyle")]
        public string MappingStyle { get; set; } = string.Empty;
        
        [XmlAttribute("gridX")]
        public int GridX { get; set; }
        
        [XmlAttribute("gridY")]
        public int GridY { get; set; }
        
        [XmlAttribute("blackPixelPercent")]
        public int BlackPixelPercent { get; set; }
        
        [XmlElement("omrInfo")]
        public LegacyOMRInfo? OmrInfo { get; set; }
        
        [XmlElement("omr")]
        public List<LegacyOMR> OMRs { get; set; } = new List<LegacyOMR>();
    }
    
    public class LegacyOMRInfo
    {
        [XmlAttribute("direction")]
        public string Direction { get; set; } = string.Empty;
        
        [XmlAttribute("charSet")]
        public string CharSet { get; set; } = string.Empty;
        
        [XmlAttribute("lines")]
        public int Lines { get; set; }
        
        [XmlAttribute("memberName")]
        public string MemberName { get; set; } = string.Empty;
    }
    
    public class LegacyOMR
    {
        [XmlAttribute("omrNo")]
        public string OmrNo { get; set; } = string.Empty;
        
        [XmlElement("region")]
        public List<LegacyRegion> Regions { get; set; } = new List<LegacyRegion>();
        
        [XmlElement("imagePart")]
        public List<LegacyImagePart> ImageParts { get; set; } = new List<LegacyImagePart>();
    }
    
    public class LegacyRegion
    {
        [XmlAttribute("isVirtual")]
        public string IsVirtual { get; set; } = string.Empty;
        
        [XmlAttribute("defaultValue")]
        public string DefaultValue { get; set; } = string.Empty;
        
        [XmlAttribute("memberName")]
        public string MemberName { get; set; } = string.Empty;
        
        [XmlAttribute("direction")]
        public string Direction { get; set; } = string.Empty;
        
        [XmlAttribute("charSet")]
        public string CharSet { get; set; } = string.Empty;
        
        [XmlAttribute("lines")]
        public int Lines { get; set; }
        
        [XmlAttribute("useSeparator")]
        public string UseSeparator { get; set; } = string.Empty;
        
        [XmlAttribute("useErrorChar")]
        public string UseErrorChar { get; set; } = string.Empty;
        
        [XmlElement("startCircle")]
        public LegacyCircle? StartCircle { get; set; }
        
        [XmlElement("startPadding")]
        public LegacyPadding? StartPadding { get; set; }
        
        [XmlElement("spacing")]
        public LegacySpacing? Spacing { get; set; }
        
        [XmlElement("borderRemovePercent")]
        public LegacyBorderRemovePercent? BorderRemovePercent { get; set; }
    }
    
    public class LegacyImagePart
    {
        [XmlAttribute("id")]
        public string Id { get; set; } = string.Empty;
        
        [XmlAttribute("direction")]
        public string Direction { get; set; } = string.Empty;
    }
    
    public class LegacyCircle
    {
        [XmlAttribute("x")]
        public int X { get; set; }
        
        [XmlAttribute("y")]
        public int Y { get; set; }
    }
    
    public class LegacyPadding
    {
        [XmlAttribute("x")]
        public double X { get; set; }
        
        [XmlAttribute("y")]
        public double Y { get; set; }
    }
    
    public class LegacySpacing
    {
        [XmlAttribute("x")]
        public double X { get; set; }
        
        [XmlAttribute("y")]
        public double Y { get; set; }
    }
    
    public class LegacyBorderRemovePercent
    {
        [XmlAttribute("useBR")]
        public string UseBR { get; set; } = string.Empty;
        
        [XmlAttribute("top")]
        public double Top { get; set; }
        
        [XmlAttribute("left")]
        public double Left { get; set; }
        
        [XmlAttribute("bottom")]
        public double Bottom { get; set; }
        
        [XmlAttribute("right")]
        public double Right { get; set; }
    }
}
