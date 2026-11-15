namespace BunnyTail.XamlProperty.Generator.Models;

internal sealed record PropertyChangedModel(
    string MethodName,
    bool IsMethodGroup,
    bool HasParameters,
    string OldParameterType,
    string NewParameterType);
