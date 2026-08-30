# BunnyTail.XamlProperty

Property source generator for WPF, MAUI and Avalonia.

## 📦 NuGet

| Package | Note |
|-|-|
| [![NuGet](https://img.shields.io/nuget/v/BunnyTail.XamlProperty.Wpf.svg)](https://www.nuget.org/packages/BunnyTail.XamlProperty.Wpf/) | `DependencyProperty` for WPF |
| [![NuGet](https://img.shields.io/nuget/v/BunnyTail.XamlProperty.Maui.svg)](https://www.nuget.org/packages/BunnyTail.XamlProperty.Maui/) | `BindableProperty` for MAUI |
| [![NuGet](https://img.shields.io/nuget/v/BunnyTail.XamlProperty.Avalonia.svg)](https://www.nuget.org/packages/BunnyTail.XamlProperty.Avalonia/) | `StyledProperty` for Avalonia |

## ⚙️ Property

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
|-|:-:|:-:|:-:|
| `DefaultValue` | ✅ | ✅ | ✅ |
| `DefaultValueExpression` | ✅ | ✅ | ✅ |
| `DefaultValueMember` | ✅ | ✅ | ✅ |
| `Options` | ✅ | ❌ | ❌ |
| `DefaultBindingMode` | ❌ | ✅ | ✅ |
| `PropertyChanged` | ✅ | ✅ | ❌ |
| `PropertyChanging` | ❌ | ✅ | ❌ |
| `Coerce` | ✅ | ✅ | ✅ |
| `Validate` | ✅ | ✅ | ✅ |
| `Inherits` | ❌ | ❌ | ✅ |
| `EnableDataValidation` | ❌ | ❌ | ✅ |

Avalonia does not have `PropertyChanged`, because it handles property change by overriding `OnPropertyChanged`.

## 🔗 AttachedProperty

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

## ⚠️ Limitation

| Platform | XAML compilation | Generated property |
|-|-|:-:|
| WPF | Markup compiler, after the assembly is built | ✅ |
| Avalonia | XamlIl, after the assembly is built | ✅ |
| MAUI (default) | XamlC, after the assembly is built | ✅ |
| MAUI (`MauiXamlInflator` is `SourceGen`) | Source generator | ❌ |
