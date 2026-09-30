namespace BunnyTail.XamlProperty.Generator.Models;

using Microsoft.CodeAnalysis;

using SourceGenerateHelper;

internal sealed record AttachedPropertyModel(
    // Containing type
    string Namespace,
    string ClassName,
    EquatableArray<ContainingTypeModel> ContainingTypes,
    string TypeKeyword,
    bool IsStaticClass,
    // Accessor
    Accessibility GetAccessibility,
    string GetSignature,
    string GetParameterName,
    string? SetSignature,
    string SetTargetName,
    string SetValueName,
    // Property
    string PropertyName,
    bool IsNewField,
    string TargetType,
    string ValueType,
    string TypeofType,
    bool RequireCast,
    // Metadata
    string? DefaultValue,
    string? MetadataOptions,
    string? PropertyChanged,
    // Generation
    EquatableArray<string> Usings,
    bool IsFallback,
    bool IsSetFallback,
    bool GetTargetNullable,
    bool SetTargetNullable);
