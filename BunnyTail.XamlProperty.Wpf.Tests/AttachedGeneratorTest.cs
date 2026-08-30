namespace BunnyTail.XamlProperty;

using Microsoft.CodeAnalysis;

public sealed class AttachedGeneratorTest
{
    private const string Source =
        """
        using BunnyTail.XamlProperty;
        using System.Windows;

        namespace Test;

        public static partial class Focus
        {
            [AttachedProperty(DefaultValue = false)]
            public static partial bool GetSuppress(DependencyObject obj);

            public static partial void SetSuppress(DependencyObject obj, bool value);
        }
        """;

    // ------------------------------------------------------------
    // Basic
    // ------------------------------------------------------------

    [Fact]
    public void AccessorGeneratesFieldAndImplementation()
    {
        // Arrange & Act
        var generated = GeneratorTestHelper.GetGeneratedSource(Source);

        // Assert
        Assert.Contains("public static readonly global::System.Windows.DependencyProperty SuppressProperty = global::System.Windows.DependencyProperty.RegisterAttached(", generated, StringComparison.Ordinal);
        Assert.Contains("\"Suppress\"", generated, StringComparison.Ordinal);
        Assert.Contains("typeof(bool)", generated, StringComparison.Ordinal);
        Assert.Contains("typeof(Focus)", generated, StringComparison.Ordinal);
        Assert.Contains("static partial class Focus", generated, StringComparison.Ordinal);
        Assert.Contains("public static partial bool GetSuppress(global::System.Windows.DependencyObject obj) => (bool)obj.GetValue(SuppressProperty);", generated, StringComparison.Ordinal);
        Assert.Contains("public static partial void SetSuppress(global::System.Windows.DependencyObject obj, bool value) => obj.SetValue(SuppressProperty, value);", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void AccessorProducesNoCompilationError()
    {
        // Arrange & Act
        var diagnostics = GeneratorTestHelper.GetDiagnosticsAll(Source);

        // Assert
        Assert.DoesNotContain(diagnostics, static x => x.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void GetterOnlyIsSupported()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using System.Windows;

            namespace Test;

            public static partial class Focus
            {
                [AttachedProperty]
                public static partial bool GetSuppress(DependencyObject obj);
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("GetSuppress", generated, StringComparison.Ordinal);
        Assert.DoesNotContain("SetSuppress", generated, StringComparison.Ordinal);
    }

    // ------------------------------------------------------------
    // Metadata
    // ------------------------------------------------------------

    [Fact]
    public void OptionsAndCallbackAreApplied()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using System.Windows;

            namespace Test;

            public static partial class Focus
            {
                [AttachedProperty(DefaultValue = 1d, Options = FrameworkPropertyMetadataOptions.AffectsRender, PropertyChanged = nameof(OnSuppressChanged))]
                public static partial double GetSuppress(DependencyObject obj);

                public static partial void SetSuppress(DependencyObject obj, double value);

                private static void OnSuppressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("new global::System.Windows.FrameworkPropertyMetadata(", generated, StringComparison.Ordinal);
        Assert.Contains("global::System.Windows.FrameworkPropertyMetadataOptions.AffectsRender", generated, StringComparison.Ordinal);
        Assert.Contains("OnSuppressChanged", generated, StringComparison.Ordinal);
    }

    // ------------------------------------------------------------
    // Diagnostics
    // ------------------------------------------------------------

    [Fact]
    public void Btxp0012InvalidAccessorEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using System.Windows;

            namespace Test;

            public static partial class Focus
            {
                [AttachedProperty]
                public static partial void GetSuppress(DependencyObject obj);
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        Assert.Contains(diagnostics, static x => x.Id == "BTXP0012");
    }

    [Fact]
    public void Btxp0013InvalidTargetTypeEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;

            namespace Test;

            public static partial class Focus
            {
                [AttachedProperty]
                public static partial bool GetSuppress(string obj);
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnosticsWithoutVerify(source);

        // Assert
        Assert.Contains(diagnostics, static x => x.Id == "BTXP0013");
    }
}
