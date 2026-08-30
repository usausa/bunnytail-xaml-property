namespace BunnyTail.XamlProperty.Generator.Models;

internal sealed record ValidateModel(
    string MethodName,
    bool IsMethodGroup,
    bool IsStatic,
    string ParameterType);
