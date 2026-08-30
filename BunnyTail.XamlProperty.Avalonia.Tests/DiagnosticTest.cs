namespace BunnyTail.XamlProperty;

public sealed class DiagnosticTest
{
    // ------------------------------------------------------------
    // Property definition
    // ------------------------------------------------------------

    [Fact]
    public void Btxp0001NotPartialEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty]
                public string? Text { get; set; }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        Assert.Contains(diagnostics, static x => x.Id == "BTXP0001");
    }

    [Fact]
    public void Btxp0002StaticPropertyEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty]
                public static partial string? Text { get; set; }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        Assert.Contains(diagnostics, static x => x.Id == "BTXP0002");
    }

    [Fact]
    public void Btxp0003AccessorModifierEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty]
                public partial string? Text { get; private set; }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        Assert.Contains(diagnostics, static x => x.Id == "BTXP0003");
    }

    // ------------------------------------------------------------
    // Containing type
    // ------------------------------------------------------------

    [Fact]
    public void Btxp0004ContainingTypeNotPartialEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public class TestElement : AvaloniaObject
            {
                [StyledProperty]
                public partial string? Text { get; set; }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        Assert.Contains(diagnostics, static x => x.Id == "BTXP0004");
    }

    [Fact]
    public void Btxp0005NotAvaloniaObjectEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;

            namespace Test;

            public class OtherBase
            {
            }

            public partial class TestElement : OtherBase
            {
                [StyledProperty]
                public partial string? Text { get; set; }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        Assert.Contains(diagnostics, static x => x.Id == "BTXP0005");
    }

    [Fact]
    public void UnknownBaseTypeEmitsNoDiagnostic()
    {
        // Arrange
        // The base type may be declared in another partial declaration that is generated from XAML
        const string source =
            """
            using BunnyTail.XamlProperty;

            namespace Test;

            public partial class TestElement
            {
                [StyledProperty]
                public partial string? Text { get; set; }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnosticsWithoutVerify(source);

        // Assert
        Assert.DoesNotContain(diagnostics, static x => x.Id == "BTXP0005");
    }

    [Fact]
    public void Btxp0006GenericTypeEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement<T> : AvaloniaObject
            {
                [StyledProperty]
                public partial string? Text { get; set; }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        Assert.Contains(diagnostics, static x => x.Id == "BTXP0006");
    }

    // ------------------------------------------------------------
    // Attribute argument
    // ------------------------------------------------------------

    [Fact]
    public void Btxp0007DefaultValueConflictEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(DefaultValue = "abc", DefaultValueExpression = "\"abc\"")]
                public partial string? Text { get; set; }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        Assert.Contains(diagnostics, static x => x.Id == "BTXP0007");
    }

    [Fact]
    public void Btxp0008InaccessibleBaseCallbackEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public class BaseElement : AvaloniaObject
            {
                private double CoerceScale(double value) => value;
            }

            public partial class TestElement : BaseElement
            {
                [StyledProperty(Coerce = "CoerceScale")]
                public partial double Scale { get; set; }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        Assert.Contains(diagnostics, static x => x.Id == "BTXP0008");
    }

    [Fact]
    public void Btxp0008CallbackNotFoundEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(Coerce = "CoerceText")]
                public partial string? Text { get; set; }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        Assert.Contains(diagnostics, static x => x.Id == "BTXP0008");
    }

    [Fact]
    public void Btxp0009InvalidCoerceSignatureEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(Coerce = nameof(CoerceText))]
                public partial string? Text { get; set; }

                private void CoerceText(int value)
                {
                }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        Assert.Contains(diagnostics, static x => x.Id == "BTXP0009");
    }

    [Fact]
    public void Btxp0009NonStaticValidateEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(Validate = nameof(ValidateText))]
                public partial string? Text { get; set; }

                private bool ValidateText(string? value) => true;
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        Assert.Contains(diagnostics, static x => x.Id == "BTXP0009");
    }

    [Fact]
    public void Btxp0010InvalidDefaultValueEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(DefaultValue = new int[] { 1, 2 })]
                public partial int[]? Values { get; set; }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        Assert.Contains(diagnostics, static x => x.Id == "BTXP0010");
    }

    [Fact]
    public void Btxp0011InvalidDefaultValueMemberEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                private static readonly int DefaultText = 1;

                [StyledProperty(DefaultValueMember = nameof(DefaultText))]
                public partial string? Text { get; set; }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnosticsWithoutVerify(source);

        // Assert
        Assert.Contains(diagnostics, static x => x.Id == "BTXP0011");
    }

    // ------------------------------------------------------------
    // Valid
    // ------------------------------------------------------------

    [Fact]
    public void ValidDefinitionEmitsNoDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(DefaultValue = 0d, Inherits = true, Coerce = nameof(CoerceScale), Validate = nameof(ValidateScale))]
                public partial double Scale { get; set; }

                private double CoerceScale(double value) => value;

                private static bool ValidateScale(double value) => true;
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        Assert.Empty(diagnostics);
    }
}
