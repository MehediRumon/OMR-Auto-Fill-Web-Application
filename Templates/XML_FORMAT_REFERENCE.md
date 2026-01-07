# XML Configuration Format Reference

This document describes the XML formats supported for OMR template configuration files.

## Supported Formats

This application supports **TWO** XML configuration formats:

1. **Current Format** (`<TemplateConfiguration>` root) - Recommended for new templates
2. **Legacy Format** (`<Page>` root) - Automatically converted from older OMR systems

## Important Notes

1. **Encoding**: Always use `utf-8` encoding
2. **No Namespaces**: Do not add xmlns attributes
3. **Valid XML**: Ensure your XML is well-formed (matching open/close tags)
4. **Auto-Detection**: The application automatically detects which format you're using

---

## Current Format (Recommended)

### Root Element
Must be `<TemplateConfiguration>` (case-sensitive)

## MCQ Template Format

```xml
<?xml version="1.0" encoding="utf-8"?>
<TemplateConfiguration>
  <TemplateId>YOUR_TEMPLATE_ID</TemplateId>
  <TemplateType>MCQ</TemplateType>
  <Dpi>300</Dpi>
  <ReferenceSize width="2480" height="3508" />
  <Roll>
    <Digits>6</Digits>
    <Columns>
      <Column>
        <Positions>
          <Position digit="0" x="100" y="100" />
          <Position digit="1" x="100" y="120" />
          <!-- ... positions for digits 2-9 ... -->
        </Positions>
      </Column>
      <!-- Repeat Column for each digit position (6 columns for 6-digit roll) -->
    </Columns>
  </Roll>
  <Reg>
    <Digits>8</Digits>
    <Columns>
      <Column>
        <Positions>
          <Position digit="0" x="320" y="100" />
          <Position digit="1" x="320" y="120" />
          <!-- ... positions for digits 2-9 ... -->
        </Positions>
      </Column>
      <!-- Repeat Column for each digit position (8 columns for 8-digit registration) -->
    </Columns>
  </Reg>
  <Mcq>
    <QuestionCount>10</QuestionCount>
    <Options>
      <Option>A</Option>
      <Option>B</Option>
      <Option>C</Option>
      <Option>D</Option>
    </Options>
    <Coordinates>
      <Question number="1">
        <OptionPositions>
          <Position option="A" x="100" y="350" />
          <Position option="B" x="130" y="350" />
          <Position option="C" x="160" y="350" />
          <Position option="D" x="190" y="350" />
        </OptionPositions>
      </Question>
      <!-- Repeat Question for each question (10 in this example) -->
    </Coordinates>
  </Mcq>
</TemplateConfiguration>
```

## SAQ Template Format

```xml
<?xml version="1.0" encoding="utf-8"?>
<TemplateConfiguration>
  <TemplateId>YOUR_TEMPLATE_ID</TemplateId>
  <TemplateType>SAQ</TemplateType>
  <Dpi>300</Dpi>
  <ReferenceSize width="2480" height="3508" />
  <Roll>
    <Digits>6</Digits>
    <Columns>
      <Column>
        <Positions>
          <Position digit="0" x="100" y="100" />
          <Position digit="1" x="100" y="120" />
          <!-- ... positions for digits 2-9 ... -->
        </Positions>
      </Column>
      <!-- Repeat Column for each digit position -->
    </Columns>
  </Roll>
  <Reg>
    <Digits>8</Digits>
    <Columns>
      <Column>
        <Positions>
          <Position digit="0" x="320" y="100" />
          <Position digit="1" x="320" y="120" />
          <!-- ... positions for digits 2-9 ... -->
        </Positions>
      </Column>
      <!-- Repeat Column for each digit position -->
    </Columns>
  </Reg>
</TemplateConfiguration>
```

## Element Descriptions

### Root Elements
- `<TemplateConfiguration>`: Root element (required)
- `<TemplateId>`: Unique identifier for the template (required)
- `<TemplateType>`: Either "MCQ" or "SAQ" (required)
- `<Dpi>`: DPI of the template image, default 300 (optional)
- `<ReferenceSize>`: Reference pixel dimensions of the template image. Attributes: `width`, `height`. Required for reliable scaling across uploads.

### Roll Number Configuration
- `<Roll>`: Roll number bubble configuration (required)
- `<Digits>`: Number of digits in roll number (required)
- `<Columns>`: Container for digit columns (required)
- `<Column>`: One column per digit position (required, count must match Digits)
- `<Positions>`: Container for digit positions 0-9 (required)
- `<Position>`: Bubble position for each digit 0-9 (required, 10 per column)
  - Attributes: `digit` (0-9), `x` (X coordinate), `y` (Y coordinate)

### Registration Number Configuration
- `<Reg>`: Registration number bubble configuration (required)
- Same structure as Roll configuration

### MCQ Configuration (MCQ Templates Only)
- `<Mcq>`: MCQ answer bubble configuration (optional, only for MCQ templates)
- `<QuestionCount>`: Number of MCQ questions (required if Mcq present)
- `<Options>`: Container for valid options (required if Mcq present)
- `<Option>`: One element per valid option (e.g., A, B, C, D) (required)
- `<Coordinates>`: Container for question coordinates (required if Mcq present)
- `<Question>`: One element per question (required, count must match QuestionCount)
  - Attribute: `number` (question number starting from 1)
- `<OptionPositions>`: Container for option bubble positions (required)
- `<Position>`: Bubble position for each option (required)
  - Attributes: `option` (A, B, C, etc.), `x` (X coordinate), `y` (Y coordinate)

## Common Mistakes

1. **Wrong Root Element**: Using `<Template>` instead of `<TemplateConfiguration>`
2. **XML Namespace**: Adding `xmlns=` attribute to root element
3. **Case Sensitivity**: Elements are case-sensitive (`<Roll>` not `<roll>`)
4. **Mismatched Tags**: Ensure all opening tags have matching closing tags
5. **Missing Attributes**: Position elements must have `digit`/`option`, `x`, and `y` attributes
6. **Wrong Count**: Number of Column elements must match Digits value
7. **Wrong Digits**: Each Column must have exactly 10 Position elements (digits 0-9)
8. **Encoding**: Not specifying `encoding="utf-8"` in XML declaration

## Validation Checklist

Before uploading your XML configuration:
- [ ] XML declaration includes `encoding="utf-8"`
- [ ] Root element is `<TemplateConfiguration>` (current format) OR `<Page>` (legacy format)
- [ ] For Current Format: TemplateId, TemplateType are present
- [ ] For Current Format: Roll/Digits matches number of Roll/Columns/Column elements
- [ ] For Current Format: Each Roll Column has 10 Position elements (digits 0-9)
- [ ] For Current Format: Reg/Digits matches number of Reg/Columns/Column elements
- [ ] For Current Format: Each Reg Column has 10 Position elements (digits 0-9)
- [ ] For Current Format MCQ: QuestionCount matches number of Question elements
- [ ] For Current Format MCQ: Each Question has positions for all Options
- [ ] All Position elements have required attributes (digit/option, x, y)
- [ ] No syntax errors (use XML validator tool)

## Legacy Format Support

### Overview

The application automatically detects and converts legacy XML configurations with `<Page>` root element from older OMR systems.

### Legacy Format Structure

```xml
<?xml version="1.0" encoding="utf-8"?>
<Page name="UMS">
  <design designFor="TemplateName" mappingStyle="grid-y-read-generate" gridX="61" gridY="91" blackPixelPercent="60">
    <omr omrNo="1">
      <region direction="y" charSet="0123456789" lines="11" memberName="rollNo">
        <startCircle x="31" y="45" />
        <startPadding x="0" y="0" />
        <spacing x="0.5" y="0.5" />
      </region>
      <region direction="y" charSet="0123456789" lines="7" memberName="registrationNo">
        <startCircle x="49" y="45"/>
        <startPadding x="0.5" y="0"/>
        <spacing x="0.5" y="0.5"/>
      </region>
    </omr>
  </design>
</Page>
```

### How It Works

1. The application detects the `<Page>` root element
2. Automatically parses the legacy format
3. Converts grid-based coordinates to pixel coordinates
4. Generates bubble positions for roll and registration numbers
5. Uses the converted configuration for filling bubbles

### Key Legacy Elements

- **`<Page name="...">`**: Root element with page name
- **`<design>`**: Contains grid parameters (gridX, gridY) and OMR sections
- **`<omr omrNo="1">`**: OMR sheet section (can have multiple pages)
- **`<region memberName="rollNo">`**: Roll number input region with grid coordinates
- **`<region memberName="registrationNo">`**: Registration number input region
- **`<startCircle>`**: Starting grid position (x, y in grid units)
- **`<spacing>`**: Spacing between bubbles (in grid units)
- **`direction="y"`**: Vertical layout (digits go down, columns go right)

### Notes on Legacy Conversion

- Grid coordinates are converted to pixel coordinates assuming standard A4 size at 300 DPI
- The `lines` attribute determines the number of digit columns
- SAQ templates (Short Answer Questions) are automatically detected
- Virtual regions with default values are processed but may not affect bubble filling

## Sample Files

Complete working examples are available in:
- `Templates/MCQ/sample_mcq_template.xml` - MCQ template with 10 questions (current format)
- `Templates/SAQ/sample_saq_template.xml` - SAQ template (current format, no MCQ section)

## Getting Help

The application now supports both current and legacy XML formats. If you encounter errors:

**Supported Root Elements:**
- `<TemplateConfiguration>` - Current format (recommended)
- `<Page>` - Legacy format (automatically converted)

**Common Issues:**
1. XML namespace attributes (xmlns) - Remove all xmlns attributes
2. Malformed XML - Check for matching opening/closing tags
3. Empty or invalid XML file
4. Unsupported root element - Use either `<TemplateConfiguration>` or `<Page>`

Use an online XML validator to check your XML syntax before uploading.
