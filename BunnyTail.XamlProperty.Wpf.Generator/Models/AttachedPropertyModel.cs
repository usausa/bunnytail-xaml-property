namespace BunnyTail.XamlProperty.Generator.Models;

using Microsoft.CodeAnalysis;

using SourceGenerateHelper;

internal sealed record AttachedPropertyModel(
    // Containing type
    string Namespace,
    string ClassName,
    EquatableArray<ContainingTypeModel> ContainingTypes,
    bool IsStaticClass,
    // Accessor
    Accessibility GetAccessibility,
    string GetMethodName,
    string? SetMethodName,
    Accessibility SetAccessibility,
    // Property
    string PropertyName,
    string TargetType,
    string ValueType,
    string TypeofType,
    bool RequireCast,
    // Metadata
    string? DefaultValue,
    string? MetadataOptions,
    string? PropertyChanged);
