# XML Configuration Format Reference

This document describes the correct XML format for OMR template configuration files.

## Important Notes

1. **Encoding**: Always use `utf-8` encoding
2. **Root Element**: Must be `<TemplateConfiguration>` (case-sensitive)
3. **No Namespaces**: Do not add xmlns attributes
4. **Valid XML**: Ensure your XML is well-formed (matching open/close tags)

## MCQ Template Format

```xml
<?xml version="1.0" encoding="utf-8"?>
<TemplateConfiguration>
  <TemplateId>YOUR_TEMPLATE_ID</TemplateId>
  <TemplateType>MCQ</TemplateType>
  <Dpi>300</Dpi>
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
- [ ] Root element is `<TemplateConfiguration>`
- [ ] TemplateId, TemplateType are present
- [ ] Roll/Digits matches number of Roll/Columns/Column elements
- [ ] Each Roll Column has 10 Position elements (digits 0-9)
- [ ] Reg/Digits matches number of Reg/Columns/Column elements
- [ ] Each Reg Column has 10 Position elements (digits 0-9)
- [ ] For MCQ: QuestionCount matches number of Question elements
- [ ] For MCQ: Each Question has positions for all Options
- [ ] All Position elements have required attributes (digit/option, x, y)
- [ ] No syntax errors (use XML validator tool)

## Sample Files

Complete working examples are available in:
- `Templates/MCQ/sample_mcq_template.xml` - MCQ template with 10 questions
- `Templates/SAQ/sample_saq_template.xml` - SAQ template (no MCQ section)

## Getting Help

If you encounter the error "There is an error in XML document (1, 2)", it typically means:
1. The root element name is incorrect
2. There's an XML namespace issue
3. The XML is not well-formed (missing closing tag, etc.)

Use an online XML validator to check your XML syntax before uploading.
