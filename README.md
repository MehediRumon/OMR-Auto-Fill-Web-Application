OMR Auto-Fill Web Application (ASP.NET)
1. Project Overview
1.1 Project Name

OMR Auto-Fill System

1.2 Objective

To develop a web-based ASP.NET application that automatically fills OMR answer sheets by overlaying filled bubbles based on user input, while supporting a large and growing number of MCQ and SAQ templates, without requiring code changes for new templates.

2. Key Design Principles (MANDATORY)

Template-Driven Architecture
All OMR behavior is controlled by template configuration, not hard-coded logic.

Unlimited Templates
The system must support:

Many MCQ templates

Many SAQ (written) templates

No Bubble Detection
The system does not scan or detect bubbles.
It draws filled circles at predefined coordinates.

Zero Code Change for New Templates
Adding a new OMR template must require only configuration and assets, not new code.

3. Scope
3.1 In Scope

Upload blank OMR template (image/PDF)

Template selection (MCQ / SAQ)

Roll number auto-fill

Registration number auto-fill

MCQ answer auto-fill (MCQ templates only)

Download filled OMR

Support for many template layouts

3.2 Out of Scope

Handwriting recognition

OMR scanning / evaluation

Bubble detection

Automatic layout detection

4. Users

Exam administrators

Coaching centers

Teachers

Test organizers

5. Template Types
5.1 MCQ Templates

MCQ templates may differ by:

Number of questions (e.g., 50, 80, 100)

Options per question (A–D or A–E)

Bubble positions

Roll/Reg digit counts

Page layout

5.2 SAQ (Written) Templates

SAQ templates may differ by:

Roll/Reg bubble layout

Page size

Written answer area

📌 SAQ templates only fill Roll and Registration fields

6. Template Definition (CRITICAL)

Each template is defined by:

Field	Description
TemplateId	Unique identifier
TemplateName	Display name
TemplateType	MCQ / SAQ
BaseImagePath	Blank OMR image or PDF
DPI	Fixed (300)
RollDigitCount	Number of roll digits
RegDigitCount	Number of reg digits
MCQCount	MCQ templates only
Options	MCQ options (A–D / A–E)
CoordinateConfig	JSON file
7. Template Configuration Format (STANDARD)
7.1 MCQ Template JSON
{
  "templateId": "MCQ_2024_01",
  "templateType": "MCQ",
  "dpi": 300,

  "roll": {
    "digits": 10,
    "columns": [
      { "0": [410,820], "1": [410,800], "2": [410,780], "3": [410,760], "4": [410,740], "5": [410,720], "6": [410,700], "7": [410,680], "8": [410,660], "9": [410,640] }
    ]
  },

  "reg": {
    "digits": 8,
    "columns": [
      { "0": [520,820], "1": [520,800], "2": [520,780], "3": [520,760], "4": [520,740], "5": [520,720], "6": [520,700], "7": [520,680], "8": [520,660], "9": [520,640] }
    ]
  },

  "mcq": {
    "questionCount": 100,
    "options": ["A","B","C","D"],
    "coordinates": {
      "1": { "A": [120,520], "B": [145,520], "C": [170,520], "D": [195,520] }
    }
  }
}

7.2 SAQ Template JSON
{
  "templateId": "SAQ_2025_01",
  "templateType": "SAQ",
  "dpi": 300,

  "roll": { "...": "..." },
  "reg": { "...": "..." }
}

8. Functional Requirements
8.1 Template Selection

User must select a template before filling

Templates grouped by:

MCQ

SAQ

8.2 Input Validation

Roll and Reg must be numeric

Length validated per template

MCQ answer count must match template

8.3 OMR Filling Logic

Draw solid black circles

Fixed radius (5–7 px)

No scaling or rotation

Preserve original DPI

8.4 Output

Download filled OMR

Image format (Phase-1)

PDF format (Phase-2)

9. Non-Functional Requirements
Performance

≤ 2 seconds per OMR

Accuracy

≥ 99.99% positional accuracy

Reliability

Original templates remain unchanged

Security

Uploaded files deleted after use

Templates are read-only

10. System Architecture
Browser
  ↓
ASP.NET UI
  ↓
Template Loader (JSON)
  ↓
Generic OMR Fill Engine
  ↓
Image/PDF Generator
  ↓
Download

11. SAQ Handling Rules

Only Roll & Reg auto-filled

Written areas untouched

Same template workflow as MCQ

12. Error Handling

Invalid template selection → warning

Input mismatch → block generation

Missing MCQ answers → configurable behavior

13. Template Management
Phase-1

Templates stored as:

Image/PDF

JSON config

Uploaded manually to server

Phase-2

Admin panel

Visual coordinate mapping

Template versioning

14. Future Enhancements

Batch generation (Excel upload)

ZIP download

Admin UI

OMR scanning & evaluation

Audit logging

15. Acceptance Criteria

✔ Multiple MCQ & SAQ templates supported
✔ No code change for new templates
✔ Scanner-safe output
✔ Correct Roll, Reg, MCQ filling

16. Final Summary

Many templates → data-driven JSON

MCQ & SAQ separated by type

One generic engine

No layout detection

Enterprise-ready design

17. Quick Start (Overall)

- Prerequisites: .NET 8 SDK
- Run locally: `dotnet restore && dotnet run`
- In the UI, upload:
  - Blank OMR template image (PNG/JPG) sized exactly as authored
  - Template configuration:
    - Current format: `<TemplateConfiguration>` XML
    - Legacy format: `<Page>` XML (grid-based)
      - Grid coordinates are applied as-is (no extra centering)
      - Ensure the XML’s image dimensions and DPI settings match the uploaded blank image
      - Check roll/registration regions for `startCircle`, `startPadding`, and `spacing`
- Enter roll and registration numbers with the same digit counts specified in the XML configuration.
- (MCQ only) Enter answers if the template is MCQ.
- Generate & download PNG. If bubbles look offset, verify:
  - Image size/DPI match the XML
  - Border removal percentages are correct
  - `startCircle`, `startPadding`, and `spacing` in the XML point to bubble centers
