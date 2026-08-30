namespace BunnyTail.XamlProperty;

using System;

using Microsoft.Maui.Controls;

[AttributeUsage(AttributeTargets.Method)]
public sealed class AttachedPropertyAttribute : Attribute
{
    public object? DefaultValue { get; set; }

    public string? DefaultValueExpression { get; set; }

    public string? DefaultValueMember { get; set; }

    public BindingMode DefaultBindingMode { get; set; }

    public string? PropertyChanged { get; set; }
}
