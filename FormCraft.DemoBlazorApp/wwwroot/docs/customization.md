# Customization

Learn how to customize FormCraft to fit your specific needs.

## Custom Field Renderers

Create your own field renderers for specialized input types.

### Creating a Custom Renderer

Implement the `IFieldRenderer` interface:

```csharp
public class CustomFieldRenderer : IFieldRenderer
{
    public bool CanRender(Type fieldType, string? fieldSubType = null)
    {
        return fieldType == typeof(MyCustomType);
    }

    public void RenderField(
        RenderTreeBuilder builder, 
        object model, 
        IFieldConfiguration<object, object> field, 
        EventCallback<object?> onValueChanged,
        EventCallback onDependencyChanged)
    {
        // Your custom rendering logic here
        builder.OpenComponent<MyCustomComponent>(0);
        builder.AddAttribute(1, "Value", field.GetValue(model));
        builder.AddAttribute(2, "ValueChanged", onValueChanged);
        builder.CloseComponent();
    }
}
```

### Registering Custom Renderers

Add your renderer to the service collection:

```csharp
builder.Services.AddScoped<IFieldRenderer, CustomFieldRenderer>();
```

## Custom Validators

Create reusable validation logic for your specific business rules.

### Simple Custom Validator

```csharp
public class BusinessRuleValidator<TModel> : IFieldValidator<TModel, string>
{
    public async Task<ValidationResult> ValidateAsync(TModel model, string value, IServiceProvider services)
    {
        // Your validation logic
        if (await IsValidBusinessRule(value))
        {
            return ValidationResult.Success();
        }
        
        return ValidationResult.Error("Value violates business rule");
    }
    
    private async Task<bool> IsValidBusinessRule(string value)
    {
        // Implement your business logic
        return await Task.FromResult(true);
    }
}
```

### Using Custom Validators

```csharp
.AddField(x => x.BusinessCode, field => field
    .WithValidator<BusinessRuleValidator<MyModel>>())
```

## Custom Themes

### MudBlazor Theme Integration

FormCraft inherits MudBlazor's theming system:

```csharp
// In Program.cs
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
});

// Custom theme
var theme = new MudTheme()
{
    Palette = new PaletteLight()
    {
        Primary = "#1976d2",
        Secondary = "#dc004e",
        // ... other colors
    }
};
```

### Custom CSS Classes

Apply custom styling to forms and fields:

```csharp
.AddField(x => x.SpecialField, field => field
    .WithCssClass("my-special-field"))
```

```css
.my-special-field {
    background: linear-gradient(45deg, #f0f0f0, #ffffff);
    border-radius: 8px;
    padding: 1rem;
}
```

## Layout Customization

### Custom Form Layouts

Create your own layout enum and logic:

```csharp
public enum MyCustomLayout
{
    Sidebar,
    Wizard,
    Accordion
}

// Custom layout logic
public static string GetCustomLayoutClass(MyCustomLayout layout)
{
    return layout switch
    {
        MyCustomLayout.Sidebar => "d-flex",
        MyCustomLayout.Wizard => "wizard-container",
        MyCustomLayout.Accordion => "accordion-form",
        _ => ""
    };
}
```

### Field Groups

Organize fields into logical groups:

```csharp
.AddField(x => x.PersonalInfo, field => field
    .WithGroup("Personal Information"))
.AddField(x => x.ContactInfo, field => field
    .WithGroup("Contact Details"))
```

## Advanced Customization

### Custom Form Builder

Extend the FormBuilder with your own methods:

```csharp
public static class MyFormBuilderExtensions
{
    public static FormBuilder<TModel> AddCurrencyField<TModel>(
        this FormBuilder<TModel> builder,
        Expression<Func<TModel, decimal>> expression,
        string label,
        string currency = "USD") where TModel : new()
    {
        return builder.AddField(expression, field => field
            .WithLabel(label)
            .WithAttribute("currency", currency)
            .WithValidator(value => value >= 0, "Amount must be positive"));
    }
}
```

### Custom Component Templates

Override default rendering with custom Blazor components:

```csharp
.AddField(x => x.ComplexData, field => field
    .WithCustomTemplate(context => builder =>
    {
        builder.OpenComponent<MyComplexComponent>(0);
        builder.AddAttribute(1, "Data", context.Value);
        builder.AddAttribute(2, "OnChanged", context.ValueChanged);
        builder.CloseComponent();
    }))
```

## Configuration Options

### Global Settings

Configure default behaviors:

```csharp
builder.Services.Configure<FormCraftOptions>(options =>
{
    options.DefaultLayout = FormLayout.Horizontal;
    options.ShowRequiredIndicator = true;
    options.RequiredIndicator = "*";
    options.ValidateOnFieldChanged = true;
});
```

### Localization

Support multiple languages:

```csharp
// Resource files: Resources/FormLabels.en.resx, Resources/FormLabels.fr.resx
.AddField(x => x.Name, field => field
    .WithLabel(Localizer["NameLabel"])
    .Required(Localizer["NameRequired"]))
```