namespace BunnyTail.XamlProperty.Generator.Models;

using Microsoft.CodeAnalysis;

using SourceGenerateHelper;

internal sealed record PropertyModel(
    // Containing type
    string Namespace,
    string ClassName,
    EquatableArray<ContainingTypeModel> ContainingTypes,
    // Property signature
    Accessibility PropertyAccessibility,
    string PropertyName,
    string PropertyType,
    // Metadata
    string? DefaultValue,
    string? DefaultBindingMode,
    bool Inherits,
    bool EnableDataValidation,
    // Callback
    CoerceModel? Coerce,
    ValidateModel? Validate);
