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
    string Signature,
    string PropertyName,
    bool IsNewField,
    string PropertyType,
    string TypeofType,
    bool RequireCast,
    // Metadata
    string? DefaultValue,
    string? DefaultBindingMode,
    // Callback
    PropertyChangedModel? PropertyChanged,
    PropertyChangedModel? PropertyChanging,
    CoerceModel? Coerce,
    ValidateModel? Validate,
    // Generation
    EquatableArray<string> Usings,
    bool IsFallback);
