namespace BunnyTail.XamlProperty.Generator;

using Microsoft.CodeAnalysis;

using SourceGenerateHelper;

internal static class Diagnostics
{
    public static DiagnosticDescriptor InvalidPropertyDefinition { get; } = new(
        id: "BTXP0001",
        title: "Invalid property definition",
        messageFormat: "[BindableProperty] property must be partial. property=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);

    public static DiagnosticDescriptor StaticPropertyNotSupported { get; } = new(
        id: "BTXP0002",
        title: "Static property not supported",
        messageFormat: "[BindableProperty] static property is not supported. property=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);

    public static DiagnosticDescriptor InvalidPropertyAccessor { get; } = new(
        id: "BTXP0003",
        title: "Invalid property accessor",
        messageFormat: "[BindableProperty] property must have get/set without modifiers. property=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);

    public static DiagnosticDescriptor ContainingTypeNotPartial { get; } = new(
        id: "BTXP0004",
        title: "Containing type not partial",
        messageFormat: "[{0}] containing type must be partial, and must not be file-local. member=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);

    public static DiagnosticDescriptor InvalidContainingType { get; } = new(
        id: "BTXP0005",
        title: "Invalid containing type",
        messageFormat: "[BindableProperty] containing type is not BindableObject. property=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);

    public static DiagnosticDescriptor GenericTypeNotSupported { get; } = new(
        id: "BTXP0006",
        title: "Generic type not supported",
        messageFormat: "[{0}] generic containing type is not supported. member=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);

    public static DiagnosticDescriptor DefaultValueConflict { get; } = new(
        id: "BTXP0007",
        title: "DefaultValue conflict",
        messageFormat: "[{0}] default value is specified more than once. member=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);

    public static DiagnosticDescriptor CallbackMethodNotFound { get; } = new(
        id: "BTXP0008",
        title: "Callback method not found",
        messageFormat: "[{0}] callback method is not found. method=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);

    public static DiagnosticDescriptor InvalidCallbackMethod { get; } = new(
        id: "BTXP0009",
        title: "Invalid callback method",
        messageFormat: "[{0}] callback method signature is invalid. method=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);

    public static DiagnosticDescriptor InvalidDefaultValueMember { get; } = new(
        id: "BTXP0011",
        title: "Invalid default value member",
        messageFormat: "[{0}] DefaultValueMember is not a static member of the value type. member=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);

    public static DiagnosticDescriptor InvalidDefaultValue { get; } = new(
        id: "BTXP0010",
        title: "Invalid default value",
        messageFormat: "[{0}] default value can not be written, or does not convert to the value type. member=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);
    public static DiagnosticDescriptor InvalidAccessorDefinition { get; } = new(
        id: "BTXP0012",
        title: "Invalid accessor definition",
        messageFormat: "[AttachedProperty] method must be static partial Get accessor. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);

    public static DiagnosticDescriptor InvalidTargetType { get; } = new(
        id: "BTXP0013",
        title: "Invalid target type",
        messageFormat: "[AttachedProperty] target type is not BindableObject. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);

    public static DiagnosticDescriptor InvalidSetterDefinition { get; } = new(
        id: "BTXP0014",
        title: "Invalid setter definition",
        messageFormat: "[AttachedProperty] partial Set method must be static void and take the target and the value of the getter. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);

    public static DiagnosticDescriptor FieldNameConflict { get; } = new(
        id: "BTXP0015",
        title: "Field name conflict",
        messageFormat: "[{0}] field name is already used in the type, and only a throwing implementation is generated. field=[{1}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);

    public static DiagnosticDescriptor HintNameCollision { get; } = new(
        id: "BTXP0016",
        title: "Type name differs only in case",
        messageFormat: "[{0}] type name differs only in case from another type, and its source is not generated. type=[{1}], other=[{2}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: DiagnosticTags.NotSuppressible);
}
