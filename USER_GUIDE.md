# OMR Auto-Fill System - Quick Start Guide

## What is OMR Auto-Fill?

OMR Auto-Fill is a web application that automatically fills Optical Mark Recognition (OMR) answer sheets. Instead of manually filling bubbles with a pen or pencil, you can:

1. Select a template (exam format)
2. Enter your information (roll number, registration number, answers)
3. Download a pre-filled OMR sheet ready for submission

## How to Use

### Step 1: Access the Application

Open your web browser and navigate to the application URL (provided by your administrator or instructor).

### Step 2: Select a Template

From the **Select Template** dropdown, choose the appropriate template for your exam:
- **MCQ Templates**: For multiple-choice question exams
- **SAQ Templates**: For short answer/written exams

The application will display template information including:
- Template type (MCQ or SAQ)
- Required roll number digits
- Required registration number digits
- Number of MCQ questions (for MCQ templates)
- Valid answer options (for MCQ templates)

### Step 3: Enter Roll Number

In the **Roll Number** field, enter your roll number:
- Must be numeric (digits only)
- Must match the exact number of digits required by the template
- Example: If template requires 6 digits, enter `123456`

### Step 4: Enter Registration Number

In the **Registration Number** field, enter your registration number:
- Must be numeric (digits only)
- Must match the exact number of digits required by the template
- Example: If template requires 8 digits, enter `98765432`

### Step 5: Enter MCQ Answers (MCQ Templates Only)

If you selected an MCQ template, you'll see an **MCQ Answers** field:
- Enter your answers separated by commas
- Use only the valid options shown (e.g., A, B, C, D)
- Must provide answers for all questions
- Example: `A,B,C,D,A,B,C,D,A,B`

**Note**: SAQ templates don't require MCQ answers as they're for written responses.

### Step 6: Generate and Download

Click the **Generate & Download Filled OMR** button.

Your browser will automatically download a PNG image file named like `OMR_123456_20260104120000.png` with all the bubbles filled according to your input.

### Step 7: Print and Submit

- Open the downloaded image file
- Print it on standard A4 or Letter size paper
- Review to ensure all bubbles are filled correctly
- Submit to your exam center or instructor

## Tips and Best Practices

### Before Generating
- ✅ Double-check your roll number and registration number
- ✅ Verify all MCQ answers are correct
- ✅ Ensure you've selected the correct template
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
