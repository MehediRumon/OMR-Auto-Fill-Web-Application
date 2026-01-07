# OMR Auto-Fill Web Application (ASP.NET Core MVC)

An ASP.NET Core MVC application that automatically fills Optical Mark Recognition (OMR) answer sheets by overlaying filled bubbles on a blank template image. The app is fully template-driven: add a new template by uploading an image and an XML configuration file—no code changes required.

## Quick Start

### Prerequisites
- .NET 8 SDK or later
- Git (optional, for cloning)

### Run Locally
```bash
git clone https://github.com/MehediRumon/OMR-Auto-Fill-Web-Application.git
cd OMR-Auto-Fill-Web-Application
dotnet restore
dotnet build
dotnet run
```
The app runs at `http://localhost:5000` (or the port configured in `launchSettings.json`).

## How to Use
1. **Prepare files**
   - Blank OMR template image (`.png`/`.jpg`)
   - XML configuration file defining bubble coordinates (see [XML format reference](Templates/XML_FORMAT_REFERENCE.md))
2. **Open the web UI** at `http://localhost:5000`.
3. **Upload** the template image and XML configuration.
4. **Enter data**: roll number, registration number, and MCQ answers (comma-separated) when applicable.
5. **Generate & download** the filled OMR (PNG).

Sample templates: `Templates/MCQ/sample_mcq_template.*` and `Templates/SAQ/sample_saq_template.*`.

## Template Configuration (XML)
- Root element: `<TemplateConfiguration>` (current format)  
- Supports both MCQ and SAQ templates  
- Legacy `<Page>` XML is auto-detected and converted  
- Full schema and examples: [Templates/XML_FORMAT_REFERENCE.md](Templates/XML_FORMAT_REFERENCE.md)

### Minimal MCQ Example
```xml
<?xml version="1.0" encoding="utf-8"?>
<TemplateConfiguration>
  <TemplateId>MCQ_SAMPLE</TemplateId>
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
      <!-- repeat Column for each digit position -->
    </Columns>
  </Roll>
  <Reg>
    <Digits>8</Digits>
    <Columns>
      <!-- same structure as Roll -->
    </Columns>
  </Reg>
  <Mcq>
    <QuestionCount>10</QuestionCount>
    <Options>
      <Option>A</Option><Option>B</Option><Option>C</Option><Option>D</Option>
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
      <!-- repeat for each question -->
    </Coordinates>
  </Mcq>
</TemplateConfiguration>
```

## Project Structure
```
OMRAutoFillApp/
├── Controllers/          # MVC controllers
├── Models/               # Data models and view models
├── Services/             # OMR fill engine and loaders
├── Templates/            # Sample images and XML configs (MCQ/SAQ)
├── Views/                # Razor views
└── wwwroot/              # Static assets (CSS/JS)
```

## Documentation
- [User Guide](USER_GUIDE.md) – step-by-step usage for end users
- [Developer Guide](DEVELOPER_GUIDE.md) – architecture, development, and troubleshooting
- [XML Format Reference](Templates/XML_FORMAT_REFERENCE.md) – full schema and examples
- [Project Summary](PROJECT_SUMMARY.md) – implementation highlights and status

## System Highlights (Reference)
- Template-driven architecture; unlimited MCQ & SAQ templates
- No bubble detection; draws filled circles at predefined coordinates
- Zero code change needed for new templates (upload image + XML)
- Validation: numeric roll/reg, option validation, question count checks
- Output: PNG (PDF planned for future phase)
