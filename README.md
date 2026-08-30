# BunnyTail.XamlProperty

Property source generator for WPF, MAUI and Avalonia.

## NuGet

| Package | Note |
|-|-|
| [![NuGet](https://img.shields.io/nuget/v/BunnyTail.XamlProperty.Wpf.svg)](https://www.nuget.org/packages/BunnyTail.XamlProperty.Wpf/) | `DependencyProperty` for WPF |
| [![NuGet](https://img.shields.io/nuget/v/BunnyTail.XamlProperty.Maui.svg)](https://www.nuget.org/packages/BunnyTail.XamlProperty.Maui/) | `BindableProperty` for MAUI |
| [![NuGet](https://img.shields.io/nuget/v/BunnyTail.XamlProperty.Avalonia.svg)](https://www.nuget.org/packages/BunnyTail.XamlProperty.Avalonia/) | `StyledProperty` for Avalonia |

Every package uses the `BunnyTail.XamlProperty` namespace, and only one of them is referenced from a project.

## Property

Add the attribute to a partial property, and the property field and the property implementation are generated.
The attribute is named after the term used by each framework.

```csharp
// WPF
public partial class GaugeControl : FrameworkElement
{
    [DependencyProperty(DefaultValue = 0d, Options = FrameworkPropertyMetadataOptions.AffectsRender, PropertyChanged = nameof(OnLevelChanged), Coerce = nameof(CoerceLevel))]
    public partial double Level { get; set; }

    private void OnLevelChanged(double oldValue, double newValue) { }

    private double CoerceLevel(double value) => Math.Clamp(value, 0d, 100d);
}
```

```csharp
// MAUI
[BindableProperty(DefaultBindingMode = BindingMode.TwoWay, PropertyChanged = nameof(Invalidate))]
public partial string? Label { get; set; }
```

```csharp
// Avalonia
[StyledProperty(Inherits = true)]
public partial string? Label { get; set; }
```

| Option | Wpf | Maui | Avalonia |
|-|-|-|-|
| `DefaultValue` | ○ | ○ | ○ |
| `DefaultValueExpression` | ○ | ○ | ○ |
| `DefaultValueMember` | ○ | ○ | ○ |
| `Options` | ○ | - | - |
| `DefaultBindingMode` | - | ○ | ○ |
| `PropertyChanged` | ○ | ○ | - |
| `PropertyChanging` | - | ○ | - |
| `Coerce` | ○ | ○ | ○ |
| `Validate` | ○ | ○ | ○ |
| `Inherits` | - | - | ○ |
| `EnableDataValidation` | - | - | ○ |

A callback method can be declared in the containing type or in a base type, so an inherited method can be specified directly.
A method that matches the callback delegate of the framework is used as a method group, so an existing callback can be specified without a change.

Avalonia does not have `PropertyChanged`, because it handles property change by overriding `OnPropertyChanged`.

## AttachedProperty

Add `[AttachedProperty]` to a `static partial` getter, and the property field and the accessor implementations are generated.
A setter is generated when a matching `Set` method is declared.

```csharp
public static partial class Focus
{
    [AttachedProperty(DefaultValue = false)]
    public static partial bool GetSuppress(DependencyObject obj);

    public static partial void SetSuppress(DependencyObject obj, bool value);
}
```

The property name is the getter name without the `Get` prefix, the value type is the return type,
and the target type is the parameter type.

## Note

Requires C# 13 or later, because partial properties are used.

A generated property is not visible to another source generator in the same project.
On MAUI, a property referenced from XAML can not be generated when `MauiXamlInflator` is `SourceGen`.

## Diagnostics

See [Diagnostics.md](Diagnostics.md).
