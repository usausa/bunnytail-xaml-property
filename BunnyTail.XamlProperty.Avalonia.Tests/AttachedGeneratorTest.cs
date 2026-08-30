namespace BunnyTail.XamlProperty;

using Microsoft.CodeAnalysis;

public sealed class AttachedGeneratorTest
{
    private const string Source =
        """
        using BunnyTail.XamlProperty;
        using Avalonia;

        namespace Test;

        public static partial class Focus
        {
            [AttachedProperty(DefaultValue = false)]
            public static partial bool GetSuppress(AvaloniaObject obj);

            public static partial void SetSuppress(AvaloniaObject obj, bool value);
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
        Assert.Contains("public static readonly global::Avalonia.AttachedProperty<bool> SuppressProperty = global::Avalonia.AvaloniaProperty.RegisterAttached<global::Avalonia.AvaloniaObject, bool>(", generated, StringComparison.Ordinal);
        Assert.Contains("\"Suppress\"", generated, StringComparison.Ordinal);
        Assert.Contains("static partial class Focus", generated, StringComparison.Ordinal);
        Assert.Contains("public static partial bool GetSuppress(global::Avalonia.AvaloniaObject obj) => obj.GetValue(SuppressProperty);", generated, StringComparison.Ordinal);
        Assert.Contains("public static partial void SetSuppress(global::Avalonia.AvaloniaObject obj, bool value) => obj.SetValue(SuppressProperty, value);", generated, StringComparison.Ordinal);
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
            using Avalonia;

            namespace Test;

            public static partial class Focus
            {
                [AttachedProperty]
                public static partial bool GetSuppress(AvaloniaObject obj);
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
            using Avalonia;

            namespace Test;

            public static partial class Focus
            {
                [AttachedProperty]
                public static partial void GetSuppress(AvaloniaObject obj);
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
