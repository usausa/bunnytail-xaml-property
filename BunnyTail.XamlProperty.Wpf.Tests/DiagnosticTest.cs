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
            using System.Windows;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                [DependencyProperty]
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
            using System.Windows;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                [DependencyProperty]
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
            using System.Windows;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                [DependencyProperty]
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
            using System.Windows;

            namespace Test;

            public class TestElement : DependencyObject
            {
                [DependencyProperty]
                public partial string? Text { get; set; }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        Assert.Contains(diagnostics, static x => x.Id == "BTXP0004");
    }

    [Fact]
    public void Btxp0005NotDependencyObjectEmitsDiagnostic()
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
                [DependencyProperty]
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
                [DependencyProperty]
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
            using System.Windows;

            namespace Test;

            public partial class TestElement<T> : DependencyObject
            {
                [DependencyProperty]
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
            using System.Windows;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                [DependencyProperty(DefaultValue = "abc", DefaultValueExpression = "\"abc\"")]
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
            using System.Windows;

            namespace Test;

            public class BaseElement : DependencyObject
            {
                private void OnChanged()
                {
                }
            }

            public partial class TestElement : BaseElement
            {
                [DependencyProperty(PropertyChanged = nameof(BaseElement.OnChanged))]
                public partial string? Text { get; set; }
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
            using System.Windows;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                [DependencyProperty(PropertyChanged = "OnTextChanged")]
                public partial string? Text { get; set; }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        Assert.Contains(diagnostics, static x => x.Id == "BTXP0008");
    }

    [Fact]
    public void Btxp0009InvalidCallbackSignatureEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using System.Windows;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                [DependencyProperty(PropertyChanged = nameof(OnTextChanged))]
                public partial string? Text { get; set; }

                private void OnTextChanged(int value)
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
            using System.Windows;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                [DependencyProperty(Validate = nameof(ValidateText))]
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
            using System.Windows;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                [DependencyProperty(DefaultValue = new int[] { 1, 2 })]
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
            using System.Windows;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                private static readonly int DefaultText = 1;

                [DependencyProperty(DefaultValueMember = nameof(DefaultText))]
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
            using System.Windows;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                [DependencyProperty(DefaultValue = 0d, Options = FrameworkPropertyMetadataOptions.AffectsRender, PropertyChanged = nameof(OnScaleChanged), Coerce = nameof(CoerceScale), Validate = nameof(ValidateScale))]
                public partial double Scale { get; set; }

                private void OnScaleChanged(double oldValue, double newValue)
                {
                }

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
