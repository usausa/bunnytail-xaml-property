namespace BunnyTail.XamlProperty;

using global::Avalonia;

public sealed class RuntimeBehaviorTest
{
    private static RuntimeElement CreateElement() => new();

    // ------------------------------------------------------------
    // Property
    // ------------------------------------------------------------

    [Fact]
    public void PropertyIsRegistered()
    {
        // Arrange & Act
        var property = RuntimeElement.ScaleProperty;

        // Assert
        Assert.Equal(nameof(RuntimeElement.Scale), property.Name);
        Assert.Equal(typeof(double), property.PropertyType);
        Assert.Equal(typeof(RuntimeElement), property.OwnerType);
    }

    [Fact]
    public void ValueRoundTrips()
    {
        // Arrange
        var element = CreateElement();

        // Act
        element.Scale = 5d;

        // Assert
        Assert.Equal(5d, element.Scale);
        Assert.Equal(5d, element.GetValue(RuntimeElement.ScaleProperty));
    }

    // ------------------------------------------------------------
    // Default value
    // ------------------------------------------------------------

    [Fact]
    public void DefaultValueIsApplied()
    {
        // Arrange & Act
        var element = CreateElement();

        // Assert
        Assert.Equal(1d, element.Scale);
    }

    [Fact]
    public void DefaultValueExpressionIsApplied()
    {
        // Arrange & Act
        var element = CreateElement();

        // Assert
        Assert.Equal("default", element.Title);
    }

    // ------------------------------------------------------------
    // Callback
    // ------------------------------------------------------------

    [Fact]
    public void CoerceCallbackIsApplied()
    {
        // Arrange
        var element = CreateElement();

        // Act
        element.Scale = 100d;

        // Assert
        Assert.Equal(10d, element.Scale);
    }

    [Fact]
    public void ValidateCallbackRejectsInvalidValue()
    {
        // Arrange
        var element = CreateElement();

        // Act & Assert
        element.Label = "abc";
        Assert.Equal("abc", element.Label);
        Assert.Throws<ArgumentException>(() => element.Label = "too long value");
    }
}

internal sealed partial class RuntimeElement : AvaloniaObject
{
    [StyledProperty(DefaultValue = 1d, Coerce = nameof(CoerceScale))]
    public partial double Scale { get; set; }

    [StyledProperty(Validate = nameof(ValidateLabel))]
    public partial string? Label { get; set; }

    [StyledProperty(DefaultValueExpression = "\"default\"")]
    public partial string? Title { get; set; }

    public double MaximumScale { get; set; } = 10d;

    private double CoerceScale(double value) => Math.Clamp(value, 0d, MaximumScale);

    private static bool ValidateLabel(string? value) => value is null || (value.Length <= 5);
}
