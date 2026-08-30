namespace BunnyTail.XamlProperty.Generator;

using Microsoft.CodeAnalysis;

internal static class Diagnostics
{
    public static DiagnosticDescriptor InvalidPropertyDefinition { get; } = new(
        id: "BTXP0001",
        title: "Invalid property definition",
        messageFormat: "[DependencyProperty] property must be partial. property=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor StaticPropertyNotSupported { get; } = new(
        id: "BTXP0002",
        title: "Static property not supported",
        messageFormat: "[DependencyProperty] static property is not supported. property=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InvalidPropertyAccessor { get; } = new(
        id: "BTXP0003",
        title: "Invalid property accessor",
        messageFormat: "[DependencyProperty] property must have get/set without modifiers. property=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor ContainingTypeNotPartial { get; } = new(
        id: "BTXP0004",
        title: "Containing type not partial",
        messageFormat: "[DependencyProperty] containing type must be partial. property=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InvalidContainingType { get; } = new(
        id: "BTXP0005",
        title: "Invalid containing type",
        messageFormat: "[DependencyProperty] containing type is not DependencyObject. property=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor GenericTypeNotSupported { get; } = new(
        id: "BTXP0006",
        title: "Generic type not supported",
        messageFormat: "[DependencyProperty] generic containing type is not supported. property=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor DefaultValueConflict { get; } = new(
        id: "BTXP0007",
        title: "DefaultValue conflict",
        messageFormat: "[DependencyProperty] DefaultValue and DefaultValueExpression conflict. property=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor CallbackMethodNotFound { get; } = new(
        id: "BTXP0008",
        title: "Callback method not found",
        messageFormat: "[DependencyProperty] callback method is not found. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InvalidCallbackMethod { get; } = new(
        id: "BTXP0009",
        title: "Invalid callback method",
        messageFormat: "[DependencyProperty] callback method signature is invalid. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InvalidDefaultValueMember { get; } = new(
        id: "BTXP0011",
        title: "Invalid default value member",
        messageFormat: "[DependencyProperty] DefaultValueMember is not a static member of the property type. member=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor InvalidDefaultValue { get; } = new(
        id: "BTXP0010",
        title: "Invalid default value",
        messageFormat: "[DependencyProperty] DefaultValue is not a supported constant. property=[{0}]",
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
        messageFormat: "[AttachedProperty] target type is not DependencyObject. method=[{0}]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
