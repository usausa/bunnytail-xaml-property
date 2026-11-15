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
    // Callback
    PropertyChangedModel? PropertyChanged,
    // Generation
    EquatableArray<string> Usings,
    bool IsFallback);
