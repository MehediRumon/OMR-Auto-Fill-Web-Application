# OMR Auto-Fill Web Application - Developer Guide

## Overview

This ASP.NET Core MVC application automatically fills OMR (Optical Mark Recognition) answer sheets by overlaying filled bubbles based on user input. The system is template-driven, supporting both MCQ (Multiple Choice Questions) and SAQ (Short Answer Questions) templates without requiring code changes for new templates.

## Project Structure

```
OMRAutoFillApp/
├── Controllers/
│   ├── HomeController.cs          # Redirects to OMR controller
│   └── OMRController.cs            # Main controller for OMR operations
├── Models/
│   ├── TemplateConfiguration.cs    # Template configuration models
│   ├── TemplateMetadata.cs         # Template metadata
│   └── ViewModels/
│       └── OMRFillViewModel.cs     # View model for form input
├── Services/
│   ├── TemplateLoaderService.cs    # Loads and caches templates
│   └── OMRFillEngineService.cs     # Generic OMR fill engine
├── Templates/
│   ├── MCQ/                        # MCQ template files
│   │   ├── *.json                  # Template configurations
│   │   └── *.png                   # Blank template images
│   └── SAQ/                        # SAQ template files
│       ├── *.json                  # Template configurations
│       └── *.png                   # Blank template images
├── Views/
│   └── OMR/
│       └── Index.cshtml            # Main OMR page
└── wwwroot/                        # Static files (CSS, JS, libraries)
```

## Key Features

### Template-Driven Architecture
- All OMR behavior is controlled by JSON configuration files
- No code changes required for new templates
- Supports unlimited MCQ and SAQ templates

### OMR Fill Engine
- Generic engine works with any template configuration
- Draws filled circles at predefined coordinates
- Supports:
  - Roll number auto-fill
  - Registration number auto-fill
  - MCQ answer auto-fill (MCQ templates only)

### Image Processing
- Uses SixLabors.ImageSharp for image manipulation
- Fixed bubble radius: 6 pixels
- Preserves original template quality
- Outputs PNG format

## How to Run

### Prerequisites
- .NET 8.0 or later
- SixLabors.ImageSharp package
- SixLabors.ImageSharp.Drawing package

### Running the Application
```bash
cd /path/to/OMRAutoFillApp
dotnet restore
dotnet build
dotnet run
```

The application will be available at `http://localhost:5000` (or the port specified in launchSettings.json).

## Adding New Templates

### Step 1: Create Template Configuration JSON

Create a JSON file in the appropriate directory:
- MCQ templates: `Templates/MCQ/your_template_name.json`
- SAQ templates: `Templates/SAQ/your_template_name.json`

#### MCQ Template Example:
```json
{
  "templateId": "MCQ_2024_01",
  "templateType": "MCQ",
  "dpi": 300,
  "roll": {
    "digits": 6,
    "columns": [
      {
        "0": [100, 100],
        "1": [100, 120],
        "2": [100, 140],
        ...
        "9": [100, 280]
      },
      // Repeat for each digit position
    ]
  },
  "reg": {
    "digits": 8,
    "columns": [
      {
        "0": [320, 100],
        "1": [320, 120],
        ...
        "9": [320, 280]
      },
      // Repeat for each digit position
    ]
  },
  "mcq": {
    "questionCount": 100,
    "options": ["A", "B", "C", "D"],
    "coordinates": {
      "1": {
        "A": [120, 520],
        "B": [145, 520],
        "C": [170, 520],
        "D": [195, 520]
      },
      "2": {
        "A": [120, 550],
        ...
      },
      // Repeat for all questions
    }
  }
}
```

#### SAQ Template Example:
```json
{
  "templateId": "SAQ_2025_01",
  "templateType": "SAQ",
  "dpi": 300,
  "roll": {
    "digits": 6,
    "columns": [
      // Same format as MCQ
    ]
  },
  "reg": {
    "digits": 8,
    "columns": [
      // Same format as MCQ
    ]
  }
}
```

### Step 2: Add Blank Template Image

Create or place a blank OMR template image in the same directory with the same name:
- For `Templates/MCQ/my_template.json`, add `Templates/MCQ/my_template.png` (or .jpg)
- For `Templates/SAQ/my_template.json`, add `Templates/SAQ/my_template.png` (or .jpg)

### Step 3: Determine Coordinates

To find the coordinates for bubbles:

1. **Manual Method**: Open the template image in an image editor (like GIMP, Photoshop, or Paint.NET) and note the X, Y coordinates of bubble centers.

2. **Coordinate Format**: Each coordinate is `[x, y]` where:
   - `x` = horizontal position (pixels from left)
   - `y` = vertical position (pixels from top)

3. **Tips**:
   - Coordinates should point to the center of each bubble
   - Use consistent spacing for easier configuration
   - Test with a sample OMR to verify accuracy

### Step 4: Test Your Template

1. Restart the application (the template loader caches templates on startup)
2. Select your new template from the dropdown
3. Fill in the required fields
4. Generate and download a test OMR
5. Verify that all bubbles are filled correctly

## Configuration Details

### Template Configuration Properties

#### Root Level
- `templateId` (string): Unique identifier for the template
- `templateType` (string): "MCQ" or "SAQ"
- `dpi` (int): Resolution, typically 300

#### Roll Configuration
- `digits` (int): Number of roll number digits
- `columns` (array): One entry per digit position
  - Each column maps digit values (0-9) to coordinates [x, y]

#### Registration Configuration
- `digits` (int): Number of registration number digits
- `columns` (array): One entry per digit position
  - Each column maps digit values (0-9) to coordinates [x, y]

#### MCQ Configuration (MCQ templates only)
- `questionCount` (int): Total number of questions
- `options` (array): Valid answer options (e.g., ["A", "B", "C", "D"])
- `coordinates` (object): Maps question numbers to option coordinates
  - Key: Question number as string ("1", "2", etc.)
  - Value: Object mapping options to coordinates

## Validation Rules

### Input Validation
- Roll number must be numeric and match the template's digit count
- Registration number must be numeric and match the template's digit count
- MCQ answers must:
  - Be provided for MCQ templates
  - Match the question count
  - Use only valid options defined in the template

### Template Validation
- Template ID must be unique
- Configuration file must exist and be valid JSON
- Corresponding image file must exist
- Coordinates must be valid [x, y] arrays

## Troubleshooting

### Template Not Appearing
- Check that both .json and image file exist with matching names
- Verify JSON is valid (use a JSON validator)
- Restart the application to reload templates

### Bubbles Not Filled Correctly
- Verify coordinates point to bubble centers
- Check that coordinate values are within image bounds
- Ensure roll/reg digit counts match configuration

### Download Fails
- Check that image file exists and is readable
- Verify ImageSharp packages are installed
- Check for error messages in browser console or server logs

## Architecture

### Services

#### TemplateLoaderService
- Loads all templates on startup
- Caches template metadata for fast access
- Provides methods to:
  - Get all templates
  - Get specific template metadata
  - Load template configuration

#### OMRFillEngineService
- Generic engine that works with any template
- Validates input against template requirements
- Draws filled bubbles at configured coordinates
- Returns PNG byte array for download

### Design Principles

1. **Template-Driven**: All behavior configured through JSON
2. **Zero Code Changes**: New templates require only configuration
3. **Separation of Concerns**: Clear separation between template loading, validation, and filling
4. **Type Safety**: Strong typing with C# models
5. **Scalability**: Can support unlimited templates

## Future Enhancements

Phase 2 features to consider:
- PDF support for template images and output
- Batch generation from Excel/CSV upload
- Admin panel for template management
- Visual coordinate mapping tool
- Template versioning
- Audit logging
- OMR scanning and evaluation capabilities

## License

This project follows the license specified in the repository.
