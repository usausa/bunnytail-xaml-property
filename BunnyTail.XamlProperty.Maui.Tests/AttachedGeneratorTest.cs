namespace BunnyTail.XamlProperty;

using Microsoft.CodeAnalysis;

public sealed class AttachedGeneratorTest
{
    private const string Source =
        """
        using BunnyTail.XamlProperty;
        using Microsoft.Maui.Controls;

        namespace Test;

        public static partial class Focus
        {
            [AttachedProperty(DefaultValue = false)]
            public static partial bool GetSuppress(BindableObject obj);

            public static partial void SetSuppress(BindableObject obj, bool value);
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
        Assert.Contains("public static readonly global::Microsoft.Maui.Controls.BindableProperty SuppressProperty = global::Microsoft.Maui.Controls.BindableProperty.CreateAttached(", generated, StringComparison.Ordinal);
        Assert.Contains("\"Suppress\"", generated, StringComparison.Ordinal);
        Assert.Contains("static partial class Focus", generated, StringComparison.Ordinal);
        Assert.Contains("public static partial bool GetSuppress(global::Microsoft.Maui.Controls.BindableObject obj) => (bool)obj.GetValue(SuppressProperty);", generated, StringComparison.Ordinal);
        Assert.Contains("public static partial void SetSuppress(global::Microsoft.Maui.Controls.BindableObject obj, bool value) => obj.SetValue(SuppressProperty, value);", generated, StringComparison.Ordinal);
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
            using Microsoft.Maui.Controls;

            namespace Test;

            public static partial class Focus
            {
                [AttachedProperty]
                public static partial bool GetSuppress(BindableObject obj);
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("GetSuppress", generated, StringComparison.Ordinal);
        Assert.DoesNotContain("SetSuppress", generated, StringComparison.Ordinal);
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
            using Microsoft.Maui.Controls;

            namespace Test;

            public static partial class Focus
            {
                [AttachedProperty]
                public static partial void GetSuppress(BindableObject obj);
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
