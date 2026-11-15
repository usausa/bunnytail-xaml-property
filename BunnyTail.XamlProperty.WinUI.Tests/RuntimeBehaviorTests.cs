namespace BunnyTail.XamlProperty;

using Microsoft.UI;
using Microsoft.UI.Xaml;

using Windows.UI;

public sealed class RuntimeBehaviorTests
{
    private static RuntimeElement CreateElement() => new();

    // ------------------------------------------------------------
    // Property
    // ------------------------------------------------------------

    [Fact]
    public void PropertyIsRegistered()
    {
        XamlThread.Run(static () =>
        {
            // Arrange & Act
            var metadata = RuntimeElement.ScaleProperty.GetMetadata(typeof(RuntimeElement));

            // Assert
            Assert.Equal(1d, metadata.DefaultValue);
        });
    }

    [Fact]
    public void ValueRoundTrips()
    {
        XamlThread.Run(static () =>
        {
            // Arrange
            var element = CreateElement();

            // Act
            element.Scale = 5d;

            // Assert
            Assert.Equal(5d, element.Scale);
            Assert.Equal(5d, element.GetValue(RuntimeElement.ScaleProperty));
        });
    }

    // ------------------------------------------------------------
    // Default value
    // ------------------------------------------------------------

    [Fact]
    public void DefaultValueIsApplied()
    {
        XamlThread.Run(static () =>
        {
            // Arrange & Act
            var element = CreateElement();

            // Assert
            Assert.Equal(1d, element.Scale);
        });
    }

    [Fact]
    public void ValueTypeWithoutDefaultValueIsDefaultOfType()
    {
        XamlThread.Run(static () =>
        {
            // Arrange & Act
            var element = CreateElement();

            // Assert
            Assert.Equal(0, element.Count);
        });
    }

    [Fact]
    public void DefaultValueExpressionIsApplied()
    {
        XamlThread.Run(static () =>
        {
            // Arrange & Act
            var element = CreateElement();

            // Assert
            Assert.Equal(Colors.SteelBlue, element.BarColor);
        });
    }

    // ------------------------------------------------------------
    // Callback
    // ------------------------------------------------------------

    [Fact]
    public void PropertyChangedCallbackIsInvoked()
    {
        XamlThread.Run(static () =>
        {
            // Arrange
            var element = CreateElement();

            // Act
            element.Scale = 3d;

            // Assert
            Assert.Equal(1, element.ChangedCount);
            Assert.Equal(1d, element.OldValue);
            Assert.Equal(3d, element.NewValue);
        });
    }

    // ------------------------------------------------------------
    // Attached
    // ------------------------------------------------------------

    [Fact]
    public void AttachedValueRoundTrips()
    {
        XamlThread.Run(static () =>
        {
            // Arrange
            var element = CreateElement();

            // Act
            var before = RuntimeAttached.GetMark(element);
            RuntimeAttached.SetMark(element, true);

            // Assert
            Assert.False(before);
            Assert.True(RuntimeAttached.GetMark(element));
        });
    }
}

internal sealed partial class RuntimeElement : DependencyObject
{
    [DependencyProperty(DefaultValue = 1d, PropertyChanged = nameof(OnScaleChanged))]
    public partial double Scale { get; set; }

    [DependencyProperty]
    public partial int Count { get; set; }

    [DependencyProperty(DefaultValueExpression = "global::Microsoft.UI.Colors.SteelBlue")]
    public partial Color BarColor { get; set; }

    public int ChangedCount { get; private set; }

    public double OldValue { get; private set; }

    public double NewValue { get; private set; }

    private void OnScaleChanged(double oldValue, double newValue)
    {
        ChangedCount++;
        OldValue = oldValue;
        NewValue = newValue;
    }
}

internal static partial class RuntimeAttached
{
    [AttachedProperty]
    public static partial bool GetMark(DependencyObject obj);

    public static partial void SetMark(DependencyObject obj, bool value);
}
