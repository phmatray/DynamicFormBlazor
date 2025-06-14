using FormCraft.DemoBlazorApp.Components.Shared;
using FormCraft.DemoBlazorApp.Models;
using MudBlazor;

namespace FormCraft.DemoBlazorApp.Components.Pages;

public partial class FileUploadDemo
{
    private JobApplicationModel _model = new();
    private IFormConfiguration<JobApplicationModel> _formConfiguration = null!;
    private bool _isSubmitted;

    private readonly List<GuidelineItem> _apiGuidelineTableItems =
    [
        new()
        {
            Feature = "Single File",
            Usage = "For uploading one file",
            Example = ".AddFileUploadField(x => x.Resume)"
        },
        new()
        {
            Feature = "Multiple Files",
            Usage = "For uploading multiple files",
            Example = ".AddMultipleFileUploadField(x => x.Documents)"
        },
        new()
        {
            Feature = "File Types",
            Usage = "Restrict accepted formats",
            Example = "acceptedFileTypes: new[] { \".pdf\", \".doc\" }"
        },
        new()
        {
            Feature = "File Size",
            Usage = "Set maximum file size",
            Example = "maxFileSize: 5 * 1024 * 1024 // 5MB"
        },
        new()
        {
            Feature = "Max Files",
            Usage = "Limit number of files",
            Example = "maxFiles: 3"
        }
    ];

    private readonly List<FormGuidelines.GuidelineItem> _sidebarFeatures =
    [
        new()
        {
            Icon = Icons.Material.Filled.Upload,
            Color = Color.Primary,
            Text = "Single file upload for resume"
        },
        new()
        {
            Icon = Icons.Material.Filled.FileCopy,
            Color = Color.Secondary,
            Text = "Multiple file upload for certificates"
        },
        new()
        {
            Icon = Icons.Material.Filled.CheckCircle,
            Color = Color.Success,
            Text = "File type validation"
        },
        new()
        {
            Icon = Icons.Material.Filled.Storage,
            Color = Color.Info,
            Text = "File size constraints"
        },
        new()
        {
            Icon = Icons.Material.Filled.DragIndicator,
            Color = Color.Warning,
            Text = "Drag and drop support"
        }
    ];

    protected override void OnInitialized()
    {
        _formConfiguration = FormBuilder<JobApplicationModel>.Create()
            .AddField(x => x.FullName, field => field
                .WithLabel("Full Name")
                .WithPlaceholder("Enter your full name")
                .Required("Full name is required"))
            .AddField(x => x.Email, field => field
                .WithLabel("Email")
                .Required("Email is required")
                .WithEmailValidation())
            .AddField(x => x.Phone, field => field
                .WithLabel("Phone")
                .Required("Phone number is required"))
            .AddField(x => x.Resume, field => field
                .WithLabel("Upload Resume")
                .Required("Resume is required"))
            .AddField(x => x.Certificates, field => field
                .AsMultipleFileUpload(
                    maxFiles: 3,
                    acceptedFileTypes: [".pdf", ".jpg", ".png"],
                    maxFileSize: 2 * 1024 * 1024) // 2MB per file
                .WithLabel("Upload Certificates"))
            .AddField(x => x.Position, field => field
                .WithLabel("Position")
                .WithOptions(
                    ("developer", "Software Developer"),
                    ("designer", "UI/UX Designer"),
                    ("manager", "Project Manager"),
                    ("analyst", "Business Analyst")))
            .AddField(x => x.CoverLetter, field => field
                .AsTextArea(lines: 5, maxLength: 1000)
                .WithLabel("Cover Letter")
                .WithPlaceholder("Tell us why you're a great fit for this position...")
                .Required("Please provide a cover letter"))
            .AddField(x => x.AgreeToTerms, field => field
                .WithLabel("I agree to the terms and conditions")
                .Required("You must agree to proceed with your application"))
            .Build();
    }

    private async Task HandleSubmit(JobApplicationModel model)
    {
        // In a real application, you would handle file uploads here
        // For demo purposes, we'll just show success
        _isSubmitted = true;
        await Task.CompletedTask;
    }

    private void ResetForm()
    {
        _model = new JobApplicationModel();
        _isSubmitted = false;
        StateHasChanged();
    }

    private List<FormSuccessDisplay.DataDisplayItem> GetDataDisplayItems()
    {
        var items = new List<FormSuccessDisplay.DataDisplayItem>
        {
            new() { Label = "Full Name", Value = _model.FullName },
            new() { Label = "Email", Value = _model.Email },
            new() { Label = "Phone", Value = _model.Phone ?? "Not provided" },
            new() { Label = "Position", Value = _model.Position }
        };

        if (_model.Resume != null)
        {
            var resumeSize = _model.Resume.Size / 1024.0;
            items.Add(new FormSuccessDisplay.DataDisplayItem
            {
                Label = "Resume",
                Value = $"{_model.Resume.Name} ({resumeSize:F1} KB)"
            });
        }

        if (_model.Certificates?.Any() == true)
        {
            items.Add(new FormSuccessDisplay.DataDisplayItem
            {
                Label = "Certificates",
                Value = $"{_model.Certificates.Count} file(s) uploaded"
            });
        }

        items.Add(new FormSuccessDisplay.DataDisplayItem
        {
            Label = "Terms Accepted",
            Value = _model.AgreeToTerms ? "Yes" : "No"
        });

        return items;
    }

    private string GetGeneratedCode()
    {
        const string code = @"_formConfiguration = FormBuilder<JobApplicationModel>.Create()
    .AddRequiredTextField(x => x.FullName, ""Full Name"", ""Enter your full name"")
    .AddEmailField(x => x.Email)
    .AddPhoneField(x => x.Phone!, required: true)
    .AddFileUploadField(x => x.Resume!, ""Upload Resume"",
        acceptedFileTypes: ["".pdf"", "".doc"", "".docx""],
        maxFileSize: 5 * 1024 * 1024, // 5MB
        required: true)
    .AddMultipleFileUploadField(x => x.Certificates!, ""Upload Certificates"",
        maxFiles: 3,
        acceptedFileTypes: ["".pdf"", "".jpg"", "".png""],
        maxFileSize: 2 * 1024 * 1024) // 2MB per file
    .AddDropdownField(x => x.Position, ""Position"",
        (""developer"", ""Software Developer""),
        (""designer"", ""UI/UX Designer""),
        (""manager"", ""Project Manager""),
        (""analyst"", ""Business Analyst""))
    .AddField(x => x.CoverLetter, field => field
        .AsTextArea(lines: 5, maxLength: 1000)
        .WithLabel(""Cover Letter"")
        .WithPlaceholder(""Tell us why you're a great fit for this position..."")
        .Required(""Please provide a cover letter""))
    .AddCheckboxField(x => x.AgreeToTerms, ""I agree to the terms and conditions"",
        ""You must agree to proceed with your application"")
    .Build();

// Use in Razor component
<FormCraftComponent
    TModel=""JobApplicationModel"" 
    Model=""@_model"" 
    Configuration=""@_formConfiguration""
    OnValidSubmit=""@HandleSubmit"" />";

        return code;
    }
}