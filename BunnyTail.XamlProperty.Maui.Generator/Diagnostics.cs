namespace BunnyTail.XamlProperty.Generator;

using Microsoft.CodeAnalysis;

internal static class Diagnostics
{
    public static DiagnosticDescriptor InvalidPropertyDefinition { get; } = new(
        id: "BTXP0001",
        title: "Invalid property definition",
        messageFormat: "[BindableProperty] property must be partial. property=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor StaticPropertyNotSupported { get; } = new(
        id: "BTXP0002",
        title: "Static property not supported",
        messageFormat: "[BindableProperty] static property is not supported. property=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InvalidPropertyAccessor { get; } = new(
        id: "BTXP0003",
        title: "Invalid property accessor",
        messageFormat: "[BindableProperty] property must have get/set without modifiers. property=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor ContainingTypeNotPartial { get; } = new(
        id: "BTXP0004",
        title: "Containing type not partial",
        messageFormat: "[BindableProperty] containing type must be partial. property=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InvalidContainingType { get; } = new(
        id: "BTXP0005",
        title: "Invalid containing type",
        messageFormat: "[BindableProperty] containing type is not BindableObject. property=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor GenericTypeNotSupported { get; } = new(
        id: "BTXP0006",
        title: "Generic type not supported",
        messageFormat: "[BindableProperty] generic containing type is not supported. property=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor DefaultValueConflict { get; } = new(
        id: "BTXP0007",
        title: "DefaultValue conflict",
        messageFormat: "[BindableProperty] DefaultValue and DefaultValueExpression conflict. property=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor CallbackMethodNotFound { get; } = new(
        id: "BTXP0008",
        title: "Callback method not found",
        messageFormat: "[BindableProperty] callback method is not found. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InvalidCallbackMethod { get; } = new(
        id: "BTXP0009",
        title: "Invalid callback method",
        messageFormat: "[BindableProperty] callback method signature is invalid. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InvalidDefaultValueMember { get; } = new(
        id: "BTXP0011",
        title: "Invalid default value member",
        messageFormat: "[BindableProperty] DefaultValueMember is not a static member of the property type. member=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InvalidDefaultValue { get; } = new(
        id: "BTXP0010",
        title: "Invalid default value",
        messageFormat: "[BindableProperty] DefaultValue is not a supported constant. property=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
    public static DiagnosticDescriptor InvalidAccessorDefinition { get; } = new(
        id: "BTXP0012",
        title: "Invalid accessor definition",
        messageFormat: "[AttachedProperty] method must be static partial Get accessor. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InvalidTargetType { get; } = new(
        id: "BTXP0013",
        title: "Invalid target type",
        messageFormat: "[AttachedProperty] target type is not BindableObject. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
