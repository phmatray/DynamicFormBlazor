using FluentValidation;
using FormCraft.DemoBlazorApp.Components.Shared;
using FormCraft.DemoBlazorApp.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FormCraft.DemoBlazorApp.Components.Pages;

public partial class FluentValidationDemo : ComponentBase
{
    private CustomerModel _model = new();
    private IFormConfiguration<CustomerModel>? _formConfig;
    private bool _submitted;
    private bool _isSubmitting;
    private int _validationAttempts;
    private int _successfulValidations;

    private readonly List<FormGuidelines.GuidelineItem> _sidebarFeatures =
    [
        new()
        {
            Icon = Icons.Material.Filled.Merge,
            Color = Color.Primary,
            Text = "Seamless integration with existing validators"
        },

        new()
        {
            Icon = Icons.Material.Filled.Rule,
            Color = Color.Secondary,
            Text = "Support for complex validation rules"
        },

        new()
        {
            Icon = Icons.Material.Filled.CloudSync,
            Color = Color.Tertiary,
            Text = "Async validation support"
        },

        new()
        {
            Icon = Icons.Material.Filled.AccountTree,
            Color = Color.Info,
            Text = "Nested object validation"
        },

        new()
        {
            Icon = Icons.Material.Filled.Message,
            Color = Color.Success,
            Text = "Custom error messages"
        },

        new()
        {
            Icon = Icons.Material.Filled.FilterAlt,
            Color = Color.Warning,
            Text = "Conditional validation logic"
        }
    ];

    private readonly List<GuidelineItem> _apiGuidelineTableItems =
    [
        new()
        {
            Feature = "WithFluentValidation()",
            Usage = "Uses validator registered in DI",
            Example = "field.WithFluentValidation(x => x.Email)"
        },

        new()
        {
            Feature = "WithFluentValidator()",
            Usage = "Uses specific validator instance",
            Example = "field.WithFluentValidator(validator, x => x.Name)"
        },

        new()
        {
            Feature = "Register Validators",
            Usage = "Add to dependency injection",
            Example = "services.AddScoped<IValidator<Model>, ModelValidator>()"
        },

        new()
        {
            Feature = "Combine Validators",
            Usage = "Mix with FormCraft validators",
            Example = "field.Required().WithFluentValidation(x => x.Name)"
        },

        new()
        {
            Feature = "Nested Validation",
            Usage = "Supports SetValidator rules",
            Example = "RuleFor(x => x.Address).SetValidator(new AddressValidator())"
        },

        new()
        {
            Feature = "Async Rules",
            Usage = "Supports MustAsync",
            Example = "RuleFor(x => x.Email).MustAsync(async (email, ct) => ...)"
        }
    ];

    protected override void OnInitialized()
    {
        // Create form configuration using FluentValidation
        _formConfig = FormBuilder<CustomerModel>
            .Create()
            .AddField(x => x.Name, field => field
                .WithLabel("Customer Name")
                .WithPlaceholder("Enter your full name")
                .WithFluentValidation(x => x.Name)
                .WithHelpText("Name must be between 3 and 50 characters"))
            .AddField(x => x.Email, field => field
                .WithLabel("Email Address")
                .WithPlaceholder("your.email@example.com")
                .WithFluentValidation(x => x.Email)
                .WithHelpText("We'll never share your email with anyone"))
            .AddField(x => x.Age, field => field
                .WithLabel("Age")
                .WithPlaceholder("18")
                .WithFluentValidation(x => x.Age)
                .WithHelpText("Must be 18 or older"))
            .Build();
    }

    private async Task HandleValidSubmit(CustomerModel model)
    {
        _isSubmitting = true;
        _validationAttempts++;
        StateHasChanged();

        // Simulate async operation
        await Task.Delay(1500);

        _submitted = true;
        _successfulValidations++;
        _isSubmitting = false;
        StateHasChanged();
    }

    private Task HandleFieldChanged((string fieldName, object? value) args)
    {
        // Track field changes if needed
        return Task.CompletedTask;
    }

    private void ResetForm()
    {
        _model = new CustomerModel();
        _submitted = false;
        StateHasChanged();
    }

    private List<FormSuccessDisplay.DataDisplayItem> GetDataDisplayItems()
    {
        return new List<FormSuccessDisplay.DataDisplayItem>
        {
            new() { Label = "Customer Name", Value = _model.Name },
            new() { Label = "Email Address", Value = _model.Email },
            new() { Label = "Age", Value = _model.Age.ToString() },
            new() { Label = "Validation Type", Value = "FluentValidation" },
            new() { Label = "Validator Class", Value = "CustomerValidator" }
        };
    }

    private string GetCodeExample()
    {
        return @"// 1. Define your FluentValidation validator
public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(""Name is required"")
            .MinimumLength(3).WithMessage(""Name must be at least 3 characters"")
            .MaximumLength(50).WithMessage(""Name cannot exceed 50 characters"");
            
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(""Email is required"")
            .EmailAddress().WithMessage(""Invalid email format"");
            
        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(18).WithMessage(""Must be 18 or older"");
    }
}

// 2. Register the validator in DI
services.AddScoped<IValidator<Customer>, CustomerValidator>();

// 3. Use in FormCraft
var formConfig = FormBuilder<Customer>
    .Create()
    .AddField(x => x.Name, field => field
        .WithLabel(""Customer Name"")
        .WithFluentValidation(x => x.Name))
    .AddField(x => x.Email, field => field
        .WithLabel(""Email Address"")
        .WithFluentValidation(x => x.Email))
    .Build();";
    }

    // Model class
    public class CustomerModel
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public int Age { get; set; }
    }

    // FluentValidation validator
    public class CustomerValidator : AbstractValidator<CustomerModel>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(3).WithMessage("Name must be at least 3 characters")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Please enter a valid email address");

            RuleFor(x => x.Age)
                .GreaterThanOrEqualTo(18).WithMessage("You must be 18 or older");
        }
    }
}