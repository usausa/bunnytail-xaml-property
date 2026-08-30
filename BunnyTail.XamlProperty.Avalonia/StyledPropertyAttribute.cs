namespace BunnyTail.XamlProperty;

using System;

using Avalonia.Data;

[AttributeUsage(AttributeTargets.Property)]
public sealed class StyledPropertyAttribute : Attribute
{
    public object? DefaultValue { get; set; }

    public string? DefaultValueExpression { get; set; }

    public string? DefaultValueMember { get; set; }

    public BindingMode DefaultBindingMode { get; set; }

    public bool Inherits { get; set; }

    public bool EnableDataValidation { get; set; }

    public string? Coerce { get; set; }

    public string? Validate { get; set; }
}
