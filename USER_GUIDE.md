# OMR Auto-Fill System - Quick Start Guide

## What is OMR Auto-Fill?

OMR Auto-Fill is a web application that automatically fills Optical Mark Recognition (OMR) answer sheets. Instead of manually filling bubbles with a pen or pencil, you can:

1. Upload your OMR template image
2. Upload the template configuration (XML file with bubble coordinates)
3. Enter your information (roll number, registration number, answers)
4. Download a pre-filled OMR sheet ready for submission

## How to Use

### Step 1: Access the Application

Open your web browser and navigate to the application URL (provided by your administrator or instructor).

### Step 2: Prepare Your Template Files

Before using the application, you need two files:

1. **Template Image**: Your blank OMR template (PNG or JPG format)
2. **Configuration File**: An XML file containing bubble coordinates for your template

You can download sample templates and configurations from the [GitHub repository](https://github.com/MehediRumon/OMR-Auto-Fill-Web-Application/tree/main/Templates).

### Step 3: Upload Template Image

Click on **Upload OMR Template Image** and select your blank OMR template image file:
- Accepted formats: PNG, JPG, JPEG
- This is the blank template that will be filled with your information

### Step 4: Upload Template Configuration

Click on **Upload Template Configuration** and select your XML configuration file:
- File format: XML (.xml)
- Contains bubble coordinates and template specifications
- See configuration format in the instructions section

### Step 5: Enter Roll Number

In the **Roll Number** field, enter your roll number:
- Must be numeric (digits only)
- Must match the digit count specified in your configuration file
- Example: If configuration requires 6 digits, enter `123456`

### Step 6: Enter Registration Number

In the **Registration Number** field, enter your registration number:
- Must be numeric (digits only)
- Must match the digit count specified in your configuration file
- Example: If configuration requires 8 digits, enter `98765432`

### Step 7: Enter MCQ Answers (Optional)

If you're using an MCQ template, enter your answers in the **MCQ Answers** field:
- Enter your answers separated by commas
- Use only the valid options specified in your configuration (e.g., A, B, C, D)
- Must provide answers for all questions
- Example: `A,B,C,D,A,B,C,D,A,B`

**Note**: Leave this field blank for SAQ (written answer) templates.

### Step 8: Generate and Download

Click the **Generate & Download Filled OMR** button.

Your browser will automatically download a PNG image file named like `OMR_123456_20260105120000.png` with all the bubbles filled according to your input.

### Step 9: Print and Submit

- Open the downloaded image file
- Print it on standard A4 or Letter size paper
- Review to ensure all bubbles are filled correctly
- Submit to your exam center or instructor

## Configuration File Format

Your XML configuration file should follow this format:

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
          ...
        </Positions>
      </Column>
      <!-- Repeat for each digit position -->
    </Columns>
  </Roll>
  <Reg>
    <Digits>8</Digits>
    <Columns>
      <!-- Similar structure to Roll -->
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
          <Position option="A" x="120" y="520" />
          <Position option="B" x="145" y="520" />
          ...
        </OptionPositions>
      </Question>
      <!-- Repeat for all questions -->
    </Coordinates>
  </Mcq>
</TemplateConfiguration>
```

See the [Developer Guide](DEVELOPER_GUIDE.md) for detailed configuration instructions.

## Tips and Best Practices

### Before Generating
- ✅ Prepare your template image and configuration file
- ✅ Double-check your roll number and registration number
- ✅ Verify all MCQ answers are correct
- ✅ Ensure your configuration matches your template
- ✅ Make sure your answer count matches the question count

### After Downloading
- ✅ Open the file to preview before printing
- ✅ Verify all bubbles are clearly filled
- ✅ Check that numbers and answers are correct
- ✅ Print at 100% scale (no scaling or fitting)
- ✅ Use good quality paper for printing

### Common Mistakes to Avoid
- ❌ Don't enter letters in roll/registration number fields
- ❌ Don't use spaces or special characters in numbers
- ❌ Don't skip questions in MCQ answers
- ❌ Don't use invalid options (e.g., 'E' when only A-D are valid)
- ❌ Don't resize the printed OMR sheet

## Example Usage

### Example 1: MCQ Exam
```
Template: MCQ_Sample_Template
Roll Number: 123456
Registration Number: 98765432
MCQ Answers: A,B,C,D,A,B,C,D,A,B

Result: Downloaded OMR with all 10 questions answered
```

### Example 2: SAQ Exam
```
Template: SAQ_Sample_Template
Roll Number: 654321
Registration Number: 12345678
MCQ Answers: (not required for SAQ)

Result: Downloaded OMR with only roll and registration filled
        (written answer area left blank for manual writing)
```

## Troubleshooting

### Problem: Form shows validation errors
**Solution**: Check that:
- All required fields are filled
- Roll/registration numbers match required digit count
- MCQ answers match question count (for MCQ templates)
- Only valid options are used in MCQ answers

### Problem: Download doesn't start
**Solution**: 
- Check your browser's download settings
- Ensure pop-ups are not blocked
- Try a different browser

### Problem: Bubbles appear misaligned
**Solution**: 
- Ensure you're printing at 100% scale
- Check printer settings (no "fit to page")
- Report to administrator if issue persists

### Problem: Wrong template selected
**Solution**: 
- Simply select the correct template from dropdown
- Re-enter your information
- Generate again

## Support

If you encounter any issues or have questions:
1. Check this guide for solutions
2. Verify your input meets all requirements
3. Contact your exam administrator or instructor
4. Report technical issues to the system administrator

## System Requirements

### For Users
- Modern web browser (Chrome, Firefox, Edge, Safari)
- Internet connection
- PDF viewer or image viewer for previewing
- Printer for final output

### Recommended Browsers
- Google Chrome (latest version)
- Mozilla Firefox (latest version)
- Microsoft Edge (latest version)
- Safari (latest version)

## Privacy and Data

- Your information is processed in real-time
- No data is stored permanently on the server
- Generated OMR sheets are sent directly to your browser
- Template images and configurations are stored on the server

## Important Notes

1. **Print Quality**: Always use good quality paper and ensure printer is functioning properly
2. **No Editing After Print**: Do not manually edit bubbles after printing (may cause scanning issues)
3. **Template Accuracy**: Use the exact template specified by your exam administrator
4. **Backup**: Keep a copy of your downloaded OMR before printing
5. **Submission**: Follow your institution's OMR submission guidelines

## Frequently Asked Questions (FAQ)

**Q: Can I edit the OMR after downloading?**
A: You can generate a new OMR with corrected information. Don't manually edit the downloaded image.

**Q: What if I make a mistake?**
A: Simply enter the correct information and generate a new OMR.

**Q: Can I save my information for later?**
A: No, the application doesn't store your information. You need to enter it each time.

**Q: What format is the output?**
A: The output is a PNG image file that can be printed.

**Q: How do I know which template to use?**
A: Your exam administrator or instructor will specify the template name.

**Q: Can I use this on mobile?**
A: Yes, but it's recommended to use a desktop/laptop for best experience and printing.

**Q: What if my template isn't in the list?**
A: Contact your administrator to add the template to the system.

---

**Version**: 1.0  
**Last Updated**: January 2026
