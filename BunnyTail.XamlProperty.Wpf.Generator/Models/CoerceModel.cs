namespace BunnyTail.XamlProperty.Generator.Models;

internal sealed record CoerceModel(
    string MethodName,
    bool IsMethodGroup,
    bool IsStatic,
    string ParameterType);
