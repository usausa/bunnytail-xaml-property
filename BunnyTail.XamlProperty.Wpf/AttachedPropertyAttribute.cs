namespace BunnyTail.XamlProperty;

using System;
using System.Windows;

[AttributeUsage(AttributeTargets.Method)]
public sealed class AttachedPropertyAttribute : Attribute
{
    public object? DefaultValue { get; set; }

    public string? DefaultValueExpression { get; set; }

    public string? DefaultValueMember { get; set; }

    public FrameworkPropertyMetadataOptions Options { get; set; }

    public string? PropertyChanged { get; set; }
}
