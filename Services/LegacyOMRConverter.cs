using System;
using System.Collections.Generic;
using System.Linq;
using OMRAutoFillApp.Models;

namespace OMRAutoFillApp.Services
{
    /// <summary>
    /// Converts legacy OMR configuration (Page format) to the current TemplateConfiguration format.
    /// </summary>
    public class LegacyOMRConverter
    {
        private const int DefaultDpi = 300;
        private const int StandardPageWidth = 2480;  // Standard A4 width at 300 DPI
        private const int StandardPageHeight = 3508; // Standard A4 height at 300 DPI
        private const string RollNumberMemberName = "rollNo";
        private const string RegistrationNumberMemberName = "registrationNo";
        
        /// <summary>
        /// Converts a legacy OMR configuration to the current template configuration format.
        /// </summary>
        public static TemplateConfiguration ConvertToTemplateConfiguration(LegacyOMRConfiguration legacyConfig)
        {
            if (legacyConfig?.Design == null)
            {
                throw new ArgumentException("Legacy configuration is invalid or missing design element.");
            }
            
            var templateConfig = new TemplateConfiguration
            {
                TemplateId = GenerateTemplateId(legacyConfig),
                TemplateType = DetermineTemplateType(legacyConfig),
                Dpi = DefaultDpi
            };
            
            // Convert roll and registration configurations from the first OMR section
            var firstOmr = legacyConfig.Design.OMRs.FirstOrDefault();
            if (firstOmr != null)
            {
                templateConfig.Roll = ConvertRollConfiguration(firstOmr, legacyConfig.Design);
                templateConfig.Reg = ConvertRegistrationConfiguration(firstOmr, legacyConfig.Design);
            }
            
            // MCQ configuration would be empty for legacy SAQ templates
            // The legacy format doesn't have MCQ bubbles, only written answer areas
            
            return templateConfig;
        }
        
        private static string GenerateTemplateId(LegacyOMRConfiguration legacyConfig)
        {
            // Use the design name or page name as template ID
            var designFor = legacyConfig.Design?.DesignFor ?? legacyConfig.Name;
            return string.IsNullOrEmpty(designFor) ? "LEGACY_TEMPLATE" : designFor;
        }
        
        private static string DetermineTemplateType(LegacyOMRConfiguration legacyConfig)
        {
            // Check if the design name or regions indicate SAQ (Short Answer Questions)
            var designFor = legacyConfig.Design?.DesignFor?.ToLower() ?? "";
            if (designFor.Contains("saq") || designFor.Contains("written"))
            {
                return "SAQ";
            }
            
            // Check if there are SAQ answer regions
            var hasSaqAnswers = legacyConfig.Design?.OMRs
                .SelectMany(omr => omr.Regions)
                .Any(r => r.MemberName?.ToLower().Contains("saq") == true) ?? false;
            
            if (hasSaqAnswers)
            {
                return "SAQ";
            }
            
            return "MCQ"; // Default to MCQ if unclear
        }
        
        private static RollConfiguration? ConvertRollConfiguration(LegacyOMR omr, LegacyDesign design)
        {
            var rollRegion = omr.Regions.FirstOrDefault(r => 
                r.MemberName?.Equals(RollNumberMemberName, StringComparison.OrdinalIgnoreCase) == true);
            
            if (rollRegion == null)
            {
                return null;
            }
            
            var config = new RollConfiguration
            {
                Digits = rollRegion.Lines,
                Columns = new List<DigitCoordinate>()
            };
            
            // Convert region to columns with digit positions
            if (rollRegion.StartCircle != null)
            {
                var columns = GenerateDigitColumns(
                    rollRegion.Lines,
                    rollRegion.StartCircle.X,
                    rollRegion.StartCircle.Y,
                    rollRegion.Spacing?.X ?? 0.5,
                    rollRegion.Spacing?.Y ?? 0.5,
                    rollRegion.Direction,
                    design.GridX,
                    design.GridY
                );
                
                config.Columns = columns;
            }
            
            return config;
        }
        
        private static RegistrationConfiguration? ConvertRegistrationConfiguration(LegacyOMR omr, LegacyDesign design)
        {
            var regRegion = omr.Regions.FirstOrDefault(r => 
                r.MemberName?.Equals(RegistrationNumberMemberName, StringComparison.OrdinalIgnoreCase) == true);
            
            if (regRegion == null)
            {
                return null;
            }
            
            var config = new RegistrationConfiguration
            {
                Digits = regRegion.Lines,
                Columns = new List<DigitCoordinate>()
            };
            
            // Convert region to columns with digit positions
            if (regRegion.StartCircle != null)
            {
                var columns = GenerateDigitColumns(
                    regRegion.Lines,
                    regRegion.StartCircle.X,
                    regRegion.StartCircle.Y,
                    regRegion.Spacing?.X ?? 0.5,
                    regRegion.Spacing?.Y ?? 0.5,
                    regRegion.Direction,
                    design.GridX,
                    design.GridY
                );
                
                config.Columns = columns;
            }
            
            return config;
        }
        
        private static List<DigitCoordinate> GenerateDigitColumns(
            int numberOfColumns,
            int startX,
            int startY,
            double spacingX,
            double spacingY,
            string direction,
            int gridX,
            int gridY)
        {
            var columns = new List<DigitCoordinate>();
            
            // Calculate grid cell size using standard A4 dimensions at 300 DPI
            double cellWidth = (double)StandardPageWidth / gridX;
            double cellHeight = (double)StandardPageHeight / gridY;
            
            for (int col = 0; col < numberOfColumns; col++)
            {
                var column = new DigitCoordinate
                {
                    Positions = new List<CoordinatePosition>()
                };
                
                // Generate positions for digits 0-9
                for (int digit = 0; digit <= 9; digit++)
                {
                    int x, y;
                    
                    if (direction.Equals("y", StringComparison.OrdinalIgnoreCase))
                    {
                        // Vertical direction: digits go down, columns go right
                        x = (int)((startX + col * spacingX) * cellWidth);
                        y = (int)((startY + digit * spacingY) * cellHeight);
                    }
                    else
                    {
                        // Horizontal direction: digits go right, columns go down
                        x = (int)((startX + digit * spacingX) * cellWidth);
                        y = (int)((startY + col * spacingY) * cellHeight);
                    }
                    
                    column.Positions.Add(new CoordinatePosition
                    {
                        Digit = digit.ToString(),
                        X = x,
                        Y = y
                    });
                }
                
                columns.Add(column);
            }
            
            return columns;
        }
    }
}
