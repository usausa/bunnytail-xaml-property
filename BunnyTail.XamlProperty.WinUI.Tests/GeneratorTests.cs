namespace BunnyTail.XamlProperty;

using Microsoft.CodeAnalysis;

public sealed class GeneratorTests
{
    private const string Source =
        """
        using BunnyTail.XamlProperty;
        using Microsoft.UI.Xaml;

        namespace Test;

        public partial class TestElement : DependencyObject
        {
            [DependencyProperty]
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
        Assert.Contains("public static readonly global::Microsoft.UI.Xaml.DependencyProperty TextProperty = global::Microsoft.UI.Xaml.DependencyProperty.Register(", generated, StringComparison.Ordinal);
        Assert.Contains("nameof(Text)", generated, StringComparison.Ordinal);
        Assert.Contains("typeof(string)", generated, StringComparison.Ordinal);
        Assert.Contains("typeof(TestElement),", generated, StringComparison.Ordinal);
        Assert.Contains("new global::Microsoft.UI.Xaml.PropertyMetadata(default(string?)));", generated, StringComparison.Ordinal);
        Assert.Contains("public partial string? Text", generated, StringComparison.Ordinal);
        Assert.Contains("get => (string?)GetValue(TextProperty);", generated, StringComparison.Ordinal);
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
            using Microsoft.UI.Xaml;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                [DependencyProperty]
                public partial string? Text { get; set; }

                [DependencyProperty]
                public partial int Number { get; set; }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("TextProperty", generated, StringComparison.Ordinal);
        Assert.Contains("NumberProperty", generated, StringComparison.Ordinal);
        Assert.Contains("get => (int)GetValue(NumberProperty);", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void ObjectPropertyOmitsCast()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.UI.Xaml;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                [DependencyProperty]
                public partial object? Value { get; set; }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("get => GetValue(ValueProperty);", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void InternalPropertyKeepsAccessibility()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.UI.Xaml;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                [DependencyProperty]
                internal partial string? Text { get; set; }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("internal static readonly global::Microsoft.UI.Xaml.DependencyProperty TextProperty", generated, StringComparison.Ordinal);
        Assert.Contains("internal partial string? Text", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void NestedClassGeneratesContainingTypes()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.UI.Xaml;

            namespace Test;

            public partial class Outer
            {
                public partial class TestElement : DependencyObject
                {
                    [DependencyProperty]
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

    [Fact]
    public void NestedClassInRecordGeneratesRecordContainingType()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.UI.Xaml;

            namespace Test;

            public partial record Outer
            {
                public partial class TestElement : DependencyObject
                {
                    [DependencyProperty]
                    public partial string? Text { get; set; }
                }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("partial record Outer", generated, StringComparison.Ordinal);
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
            using Microsoft.UI.Xaml;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                [DependencyProperty(DefaultValue = "abc")]
                public partial string? Text { get; set; }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("new global::Microsoft.UI.Xaml.PropertyMetadata(\"abc\")", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void DefaultValueIsCastToPropertyType()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.UI.Xaml;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                [DependencyProperty(DefaultValue = 1)]
                public partial double Scale { get; set; }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("new global::Microsoft.UI.Xaml.PropertyMetadata((double)1)", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void ValueTypeWithoutDefaultValueUsesDefaultOfType()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.UI.Xaml;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                [DependencyProperty]
                public partial double Scale { get; set; }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("new global::Microsoft.UI.Xaml.PropertyMetadata(default(double))", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void DefaultValueExpressionIsApplied()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.UI.Xaml;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                [DependencyProperty(DefaultValueExpression = "global::Test.TestElement.CreateDefault()")]
                public partial string? Text { get; set; }

                public static string CreateDefault() => "abc";
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("new global::Microsoft.UI.Xaml.PropertyMetadata(global::Test.TestElement.CreateDefault())", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void DefaultValueMemberIsApplied()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.UI.Xaml;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                private static readonly string DefaultText = "abc";

                [DependencyProperty(DefaultValueMember = nameof(DefaultText))]
                public partial string? Text { get; set; }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("new global::Microsoft.UI.Xaml.PropertyMetadata(DefaultText)", generated, StringComparison.Ordinal);
    }

    // ------------------------------------------------------------
    // Callback
    // ------------------------------------------------------------
    [Fact]
    public void FrameworkPropertyChangedCallbackIsMethodGroup()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.UI.Xaml;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                [DependencyProperty(PropertyChanged = nameof(OnTextChanged))]
                public partial string? Text { get; set; }

                private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                {
                }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("default(string?),", generated, StringComparison.Ordinal);
        Assert.Contains("            OnTextChanged));", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void BaseTypeCallbackIsResolved()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.UI.Xaml;

            namespace Test;

            public class BaseElement : DependencyObject
            {
                protected void OnChanged()
                {
                }
            }

            public partial class TestElement : BaseElement
            {
                [DependencyProperty(PropertyChanged = nameof(OnChanged))]
                public partial string? Text { get; set; }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("static (__d, __e) => ((TestElement)__d).OnChanged()", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void DeclaredCallbackHidesBaseTypeCallback()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.UI.Xaml;

            namespace Test;

            public class BaseElement : DependencyObject
            {
                protected void OnChanged(int value)
                {
                }
            }

            public partial class TestElement : BaseElement
            {
                [DependencyProperty(PropertyChanged = nameof(OnChanged))]
                public partial string? Text { get; set; }

                private new void OnChanged()
                {
                }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("static (__d, __e) => ((TestElement)__d).OnChanged()", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void PropertyChangedCallbackIsApplied()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.UI.Xaml;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                [DependencyProperty(PropertyChanged = nameof(OnTextChanged))]
                public partial string? Text { get; set; }

                private void OnTextChanged(string? oldValue, string? newValue)
                {
                }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("default(string?),", generated, StringComparison.Ordinal);
        Assert.Contains("static (__d, __e) => ((TestElement)__d).OnTextChanged((string?)__e.OldValue, (string?)__e.NewValue)));", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void PropertyChangedNoArgumentCallbackIsApplied()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.UI.Xaml;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                [DependencyProperty(PropertyChanged = nameof(OnTextChanged))]
                public partial string? Text { get; set; }

                private void OnTextChanged()
                {
                }
            }
            """;

        // Act
        var generated = GeneratorTestHelper.GetGeneratedSource(source);

        // Assert
        Assert.Contains("static (__d, __e) => ((TestElement)__d).OnTextChanged()));", generated, StringComparison.Ordinal);
    }

    [Fact]
    public void AllCallbacksProduceNoCompilationError()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.UI.Xaml;

            namespace Test;

            public partial class TestElement : DependencyObject
            {
                [DependencyProperty(DefaultValue = 0d, PropertyChanged = nameof(OnScaleChanged))]
                public partial double Scale { get; set; }

                private void OnScaleChanged(double oldValue, double newValue)
                {
                }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnosticsAll(source);

        // Assert
        Assert.DoesNotContain(diagnostics, static x => x.Severity == DiagnosticSeverity.Error);
    }
}
