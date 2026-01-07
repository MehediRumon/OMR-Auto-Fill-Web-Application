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
            return ConvertToTemplateConfiguration(legacyConfig, StandardPageWidth, StandardPageHeight);
        }
        
        /// <summary>
        /// Converts a legacy OMR configuration to the current template configuration format,
        /// using the actual image dimensions for accurate coordinate scaling.
        /// </summary>
        public static TemplateConfiguration ConvertToTemplateConfiguration(
            LegacyOMRConfiguration legacyConfig, 
            int actualImageWidth, 
            int actualImageHeight)
        {
            if (legacyConfig?.Design == null)
            {
                throw new ArgumentException("Legacy configuration is invalid or missing design element.");
            }
            
            var templateConfig = new TemplateConfiguration
            {
                TemplateId = GenerateTemplateId(legacyConfig),
                TemplateType = DetermineTemplateType(legacyConfig),
                Dpi = DefaultDpi,
                ReferenceSize = new ReferenceSize
                {
                    Width = actualImageWidth,
                    Height = actualImageHeight
                }
            };
            
            // Convert roll and registration configurations from the first OMR section
            var firstOmr = legacyConfig.Design.OMRs.FirstOrDefault();
            if (firstOmr != null)
            {
                templateConfig.Roll = ConvertRollConfiguration(firstOmr, legacyConfig.Design, actualImageWidth, actualImageHeight);
                templateConfig.Reg = ConvertRegistrationConfiguration(firstOmr, legacyConfig.Design, actualImageWidth, actualImageHeight);
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
            
            // Check if there are imageParts (SAQ answer images)
            var hasImageParts = legacyConfig.Design?.OMRs
                ?.SelectMany(omr => omr.ImageParts ?? Enumerable.Empty<LegacyImagePart>())
                .Any() ?? false;
            
            if (hasImageParts)
            {
                return "SAQ";
            }
            
            // Legacy templates typically only contain roll/registration fields and are SAQ templates
            // Default to SAQ as it's safer (doesn't require MCQ answers)
            return "SAQ";
        }
        
        private static RollConfiguration? ConvertRollConfiguration(LegacyOMR omr, LegacyDesign design, int imageWidth, int imageHeight)
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
                    rollRegion.StartPadding?.X ?? 0.0,
                    rollRegion.StartPadding?.Y ?? 0.0,
                    rollRegion.Spacing?.X ?? 0.5,
                    rollRegion.Spacing?.Y ?? 0.5,
                    rollRegion.Direction,
                    rollRegion.BorderRemovePercent,
                    design.GridX,
                    design.GridY,
                    imageWidth,
                    imageHeight
                );
                
                config.Columns = columns;
            }
            
            return config;
        }
        
        private static RegistrationConfiguration? ConvertRegistrationConfiguration(LegacyOMR omr, LegacyDesign design, int imageWidth, int imageHeight)
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
                    regRegion.StartPadding?.X ?? 0.0,
                    regRegion.StartPadding?.Y ?? 0.0,
                    regRegion.Spacing?.X ?? 0.5,
                    regRegion.Spacing?.Y ?? 0.5,
                    regRegion.Direction,
                    regRegion.BorderRemovePercent,
                    design.GridX,
                    design.GridY,
                    imageWidth,
                    imageHeight
                );
                
                config.Columns = columns;
            }
            
            return config;
        }
        
        private static List<DigitCoordinate> GenerateDigitColumns(
            int numberOfColumns,
            int startX,
            int startY,
            double startPaddingX,
            double startPaddingY,
            double spacingX,
            double spacingY,
            string direction,
            LegacyBorderRemovePercent? borderRemove,
            int gridX,
            int gridY,
            int actualImageWidth,
            int actualImageHeight)
        {
            var columns = new List<DigitCoordinate>();
            
            // Calculate effective dimensions after border removal
            double effectiveWidth = actualImageWidth;
            double effectiveHeight = actualImageHeight;
            double offsetX = 0;
            double offsetY = 0;
            
            // If borderRemovePercent is specified and useBR is true, adjust dimensions
            if (borderRemove != null && borderRemove.UseBR.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                // Border removal percentages
                double leftPercent = borderRemove.Left / 100.0;
                double rightPercent = borderRemove.Right / 100.0;
                double topPercent = borderRemove.Top / 100.0;
                double bottomPercent = borderRemove.Bottom / 100.0;
                
                // Validate border percentages don't exceed reasonable limits
                if (leftPercent + rightPercent >= 0.95 || topPercent + bottomPercent >= 0.95)
                {
                    throw new ArgumentException("Border removal percentages are too large (would leave less than 5% of image).");
                }
                
                // Calculate offset (where the effective area starts)
                offsetX = actualImageWidth * leftPercent;
                offsetY = actualImageHeight * topPercent;
                
                // Calculate effective dimensions (area after border removal)
                effectiveWidth = actualImageWidth * (1 - leftPercent - rightPercent);
                effectiveHeight = actualImageHeight * (1 - topPercent - bottomPercent);
            }
            
            // Validate grid dimensions
            if (gridX <= 0 || gridY <= 0)
            {
                throw new ArgumentException($"Invalid grid dimensions: gridX={gridX}, gridY={gridY}. Must be positive values.");
            }
            
            // Calculate grid cell size using effective dimensions
            double cellWidth = effectiveWidth / gridX;
            double cellHeight = effectiveHeight / gridY;
            
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
                        // startCircle already points to bubble center, startPadding is fractional offset
                        double gridPosX = startX + col * spacingX + startPaddingX;
                        double gridPosY = startY + digit * spacingY + startPaddingY;
                        
                        // Convert to pixel position and add border offset
                        x = (int)(gridPosX * cellWidth + offsetX);
                        y = (int)(gridPosY * cellHeight + offsetY);
                    }
                    else
                    {
                        // Horizontal direction: digits go right, columns go down
                        double gridPosX = startX + digit * spacingX + startPaddingX;
                        double gridPosY = startY + col * spacingY + startPaddingY;
                        
                        // Convert to pixel position and add border offset
                        x = (int)(gridPosX * cellWidth + offsetX);
                        y = (int)(gridPosY * cellHeight + offsetY);
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
