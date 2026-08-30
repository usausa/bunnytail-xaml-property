namespace BunnyTail.XamlProperty;

using Microsoft.CodeAnalysis;

public sealed class GeneratorTest
{
    private const string Source =
        """
        using BunnyTail.XamlProperty;
        using Avalonia;

        namespace Test;

        public partial class TestElement : AvaloniaObject
        {
            [StyledProperty]
            public partial string? Text { get; set; }
        }
        """;

    // ------------------------------------------------------------
    // Basic
    // ------------------------------------------------------------

    [Fact]
    public void PropertyGeneratesFieldAndAccessor()
    {
        // Arrange & Act
        var generated = GeneratorTestHelper.GetGeneratedSource(Source);

        // Assert
        Assert.Contains("public static readonly global::Avalonia.StyledProperty<string?> TextProperty = global::Avalonia.AvaloniaProperty.Register<TestElement, string?>(", generated, StringComparison.Ordinal);
        Assert.Contains("nameof(Text)", generated, StringComparison.Ordinal);
        Assert.Contains("public partial string? Text", generated, StringComparison.Ordinal);
        Assert.Contains("get => GetValue(TextProperty);", generated, StringComparison.Ordinal);
        Assert.Contains("set => SetValue(TextProperty, value);", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void PropertyProducesNoCompilationError()
    {
        // Arrange & Act
        var diagnostics = GeneratorTestHelper.GetDiagnosticsAll(Source);

        // Assert
        Assert.DoesNotContain(diagnostics, static x => x.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void MultiplePropertiesGenerateInOneClass()
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
                public partial string? Text { get; set; }

                [StyledProperty]
                public partial int Number { get; set; }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("TextProperty", generated, StringComparison.Ordinal);
        Assert.Contains("global::Avalonia.StyledProperty<int> NumberProperty", generated, StringComparison.Ordinal);
        Assert.Contains("get => GetValue(NumberProperty);", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void NestedClassGeneratesContainingTypes()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class Outer
            {
                public partial class TestElement : AvaloniaObject
                {
                    [StyledProperty]
                    public partial string? Text { get; set; }
                }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("partial class Outer", generated, StringComparison.Ordinal);
        Assert.Contains("partial class TestElement", generated, StringComparison.Ordinal);
    }

    // ------------------------------------------------------------
    // Default value
    // ------------------------------------------------------------

    [Fact]
    public void DefaultValueIsApplied()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(DefaultValue = "abc")]
                public partial string? Text { get; set; }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("defaultValue: \"abc\"", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void DefaultValueIsCastToPropertyType()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(DefaultValue = 1)]
                public partial double Scale { get; set; }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("defaultValue: (double)1", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void DefaultValueExpressionIsApplied()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(DefaultValueExpression = "global::Test.TestElement.CreateDefault()")]
                public partial string? Text { get; set; }

                public static string CreateDefault() => "abc";
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("defaultValue: global::Test.TestElement.CreateDefault()", generated, StringComparison.Ordinal);
    }

    // ------------------------------------------------------------
    // Option
    // ------------------------------------------------------------

    [Fact]
    public void InheritsIsApplied()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(Inherits = true)]
                public partial string? Text { get; set; }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("inherits: true", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void DefaultBindingModeIsApplied()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;
            using Avalonia.Data;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(DefaultBindingMode = BindingMode.TwoWay)]
                public partial string? Text { get; set; }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("defaultBindingMode: global::Avalonia.Data.BindingMode.TwoWay", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void EnableDataValidationIsApplied()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(EnableDataValidation = true)]
                public partial string? Text { get; set; }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("enableDataValidation: true", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void DefaultValueMemberIsApplied()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                private static readonly string DefaultText = "abc";

                [StyledProperty(DefaultValueMember = nameof(DefaultText))]
                public partial string? Text { get; set; }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("defaultValue: DefaultText", generated, StringComparison.Ordinal);
    }

    // ------------------------------------------------------------
    // Callback
    // ------------------------------------------------------------
    [Fact]
    public void FrameworkCoerceCallbackIsMethodGroup()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(Coerce = nameof(CoerceScale))]
                public partial double Scale { get; set; }

                private static double CoerceScale(AvaloniaObject o, double value) => value;
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("coerce: CoerceScale", generated, StringComparison.Ordinal);
        Assert.DoesNotContain("static (o, value)", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void BaseTypeCallbackIsResolved()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public class BaseElement : AvaloniaObject
            {
                protected double CoerceScale(double value) => value;
            }

            public partial class TestElement : BaseElement
            {
                [StyledProperty(Coerce = nameof(CoerceScale))]
                public partial double Scale { get; set; }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("coerce: static (o, value) => ((TestElement)o).CoerceScale(value)", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void CoerceCallbackIsApplied()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(Coerce = nameof(CoerceScale))]
                public partial double Scale { get; set; }

                private double CoerceScale(double value) => value;
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("coerce: static (o, value) => ((TestElement)o).CoerceScale(value)", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void StaticCoerceCallbackIsApplied()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(Coerce = nameof(CoerceScale))]
                public partial double Scale { get; set; }

                private static double CoerceScale(double value) => value;
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("coerce: static (o, value) => CoerceScale(value)", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void ValidateCallbackIsApplied()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(Validate = nameof(ValidateScale))]
                public partial double Scale { get; set; }

                private static bool ValidateScale(double value) => true;
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("validate: ValidateScale", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void AllOptionsProduceNoCompilationError()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;
            using Avalonia.Data;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(DefaultValue = 0d, Inherits = true, DefaultBindingMode = BindingMode.TwoWay, EnableDataValidation = true, Coerce = nameof(CoerceScale), Validate = nameof(ValidateScale))]
                public partial double Scale { get; set; }

                private double CoerceScale(double value) => value;

                private static bool ValidateScale(double value) => true;
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnosticsAll(source);

        // Assert
        Assert.DoesNotContain(diagnostics, static x => x.Severity == DiagnosticSeverity.Error);
    }
}
