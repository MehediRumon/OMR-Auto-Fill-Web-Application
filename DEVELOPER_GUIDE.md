# OMR Auto-Fill Web Application - Developer Guide

## Overview

This ASP.NET Core MVC application automatically fills OMR (Optical Mark Recognition) answer sheets by overlaying filled bubbles based on user input. The system uses a **user-upload model** where users upload their own template images and XML configurations, providing maximum flexibility without requiring pre-configured templates.

## Project Structure

```
OMRAutoFillApp/
├── Controllers/
│   ├── HomeController.cs          # Redirects to OMR controller
│   └── OMRController.cs            # Main controller for OMR operations
├── Models/
│   ├── TemplateConfiguration.cs    # Template configuration models
│   ├── TemplateMetadata.cs         # Template metadata (legacy)
│   └── ViewModels/
│       └── OMRFillViewModel.cs     # View model for file uploads and form input
├── Services/
│   ├── TemplateLoaderService.cs    # Legacy template loader (no longer used)
│   └── OMRFillEngineService.cs     # Generic OMR fill engine
├── Templates/                      # Sample templates for reference
│   ├── MCQ/                        
│   │   ├── *.json                  # Sample configurations
│   │   └── *.png                   # Sample blank template images
│   └── SAQ/                        
│       ├── *.json                  # Sample configurations
│       └── *.png                   # Sample blank template images
├── Views/
│   └── OMR/
│       └── Index.cshtml            # Upload interface with file inputs
└── wwwroot/                        # Static files (CSS, JS, libraries)
```

## Key Features

### Upload-Based Architecture
- Users upload their own OMR template images (PNG/JPG)
- Users upload XML configuration files with bubble coordinates
- No server-side template storage or pre-configuration needed
- Complete flexibility for any template format

### OMR Fill Engine
- Generic engine works with uploaded templates
- Draws filled circles at coordinates specified in configuration
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

## Using the Application

### For End Users

Users no longer need to select pre-configured templates. Instead, they:

1. **Prepare template files:**
   - Create or obtain a blank OMR template image (PNG/JPG)
   - Create an XML configuration file with bubble coordinates (see `Templates/XML_FORMAT_REFERENCE.md`)

2. **Upload files:**
   - Upload the template image through the web interface
   - Upload the configuration XML file

3. **Fill information:**
   - Enter roll number and registration number
   - Enter MCQ answers (if applicable)

4. **Generate OMR:**
   - Click the generate button
   - Download the filled OMR sheet

### Creating Template Configurations

To create a new template configuration, follow the XML format used by the application (full reference in `Templates/XML_FORMAT_REFERENCE.md`).

#### MCQ Template Example:
```xml
<?xml version="1.0" encoding="utf-8"?>
<TemplateConfiguration>
  <TemplateId>MCQ_2024_01</TemplateId>
  <TemplateType>MCQ</TemplateType>
  <Dpi>300</Dpi>
  <Roll>
    <Digits>6</Digits>
    <Columns>
      <Column>
        <Positions>
          <Position digit="0" x="100" y="100" />
          <Position digit="1" x="100" y="120" />
          <Position digit="2" x="100" y="140" />
          <Position digit="3" x="100" y="160" />
          <Position digit="4" x="100" y="180" />
          <Position digit="5" x="100" y="200" />
          <Position digit="6" x="100" y="220" />
          <Position digit="7" x="100" y="240" />
          <Position digit="8" x="100" y="260" />
          <Position digit="9" x="100" y="280" />
        </Positions>
      </Column>
      <!-- Repeat Column for each digit position -->
    </Columns>
  </Roll>
  <Reg>
    <Digits>8</Digits>
    <Columns>
      <!-- Same structure as Roll -->
    </Columns>
  </Reg>
  <Mcq>
    <QuestionCount>100</QuestionCount>
    <Options>
      <Option>A</Option>
      <Option>B</Option>
      <Option>C</Option>
      <Option>D</Option>
    </Options>
    <Coordinates>
      <Question number="1">
        <OptionPositions>
          <Position option="A" x="120" y="520" />
          <Position option="B" x="145" y="520" />
          <Position option="C" x="170" y="520" />
          <Position option="D" x="195" y="520" />
        </OptionPositions>
      </Question>
      <!-- Repeat for all questions -->
    </Coordinates>
  </Mcq>
</TemplateConfiguration>
```

#### SAQ Template Example:
```xml
<?xml version="1.0" encoding="utf-8"?>
<TemplateConfiguration>
  <TemplateId>SAQ_2025_01</TemplateId>
  <TemplateType>SAQ</TemplateType>
  <Dpi>300</Dpi>
  <Roll>
    <Digits>6</Digits>
    <Columns>
      <!-- Same structure as MCQ -->
    </Columns>
  </Roll>
  <Reg>
    <Digits>8</Digits>
    <Columns>
      <!-- Same structure as MCQ -->
    </Columns>
  </Reg>
</TemplateConfiguration>
```

### Determining Coordinates

To find the coordinates for bubbles in your template:

1. **Manual Method**: Open the template image in an image editor (like GIMP, Photoshop, or Paint.NET) and note the X, Y coordinates of bubble centers.

2. **Coordinate Format**: Each coordinate is `[x, y]` where:
   - `x` = horizontal position (pixels from left)
   - `y` = vertical position (pixels from top)

3. **Tips**:
   - Coordinates should point to the center of each bubble
   - Use consistent spacing for easier configuration
   - Test with a sample OMR to verify accuracy

### Testing Your Template

1. Open the application in your browser
2. Upload your template image and configuration file
3. Fill in test values for roll number, registration, and answers
4. Generate and download a test OMR
5. Verify that all bubbles are filled correctly at the right positions

## Configuration Details

### Template Configuration Properties

#### Root Level
- `TemplateId` (string): Unique identifier for the template
- `TemplateType` (string): "MCQ" or "SAQ"
- `Dpi` (int): Resolution, typically 300

#### Roll Configuration
- `Digits` (int): Number of roll number digits
- `Columns` (array): One entry per digit position
  - Each column lists digit values (0-9) as `<Position digit="0-9" x=".." y=".." />`

#### Registration Configuration
- `Digits` (int): Number of registration number digits
- `Columns` (array): One entry per digit position
  - Same `<Position>` structure as Roll

#### MCQ Configuration (MCQ templates only)
- `QuestionCount` (int): Total number of questions
- `Options` (array): Valid answer options (e.g., A, B, C, D)
- `Coordinates`: Maps question numbers to option coordinates via `<Question number="...">` and `<Position option="...">`

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
- Configuration file must exist and be valid XML
- Corresponding image file must exist
- Coordinates must be valid [x, y] pairs in the XML positions

## Troubleshooting

### Template Not Appearing
- Check that both .xml and image file exist with matching names
- Verify XML is valid (use an XML validator)
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

1. **Template-Driven**: All behavior configured through XML
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
