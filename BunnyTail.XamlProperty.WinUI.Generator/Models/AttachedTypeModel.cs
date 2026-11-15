namespace BunnyTail.XamlProperty.Generator.Models;

using SourceGenerateHelper;

internal sealed record AttachedTypeModel(
    string Namespace,
    string ClassName,
    EquatableArray<ContainingTypeModel> ContainingTypes,
    EquatableArray<AttachedPropertyModel> Properties);
