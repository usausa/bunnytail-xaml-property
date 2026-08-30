namespace BunnyTail.XamlProperty;

using System;

using Avalonia.Data;

[AttributeUsage(AttributeTargets.Method)]
public sealed class AttachedPropertyAttribute : Attribute
{
    public object? DefaultValue { get; set; }

    public string? DefaultValueExpression { get; set; }

    public string? DefaultValueMember { get; set; }

    public BindingMode DefaultBindingMode { get; set; }

    public bool Inherits { get; set; }
}
