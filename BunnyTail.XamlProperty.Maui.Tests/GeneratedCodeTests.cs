namespace BunnyTail.XamlProperty;

using System.Globalization;
using System.Reflection;

using BunnyTail.XamlProperty.Generator;

using Microsoft.CodeAnalysis;

public sealed class GeneratedCodeTests
{
    // ------------------------------------------------------------
    // Declaration
    // ------------------------------------------------------------

    [Fact]
    public void PropertyDeclarationIsRepeated()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            public partial class BaseElement : BindableObject
            {
                [BindableProperty]
                public virtual partial string? Title { get; set; }

                public int Tag { get; set; }
            }

            public partial class TestElement : BaseElement
            {
                [BindableProperty]
                partial string? Caption { get; set; }

                [BindableProperty]
                public sealed override partial string? Title { get; set; }

                [BindableProperty]
                public required partial string Header { get; set; }

                [BindableProperty]
                public new partial int Tag { get; set; }

                [BindableProperty]
                public partial int @class { get; set; }
            }
            """;

        // Act
        var problems = GeneratorTestHelper.GetProblemIds(source);

        // Assert
        Assert.Empty(problems);
    }

    [Fact]
    public void AttachedAccessorDeclarationIsRepeated()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            public static partial class Attached
            {
                [AttachedProperty]
                public static partial int GetCount(this BindableObject element);

                static partial void SetCount(this BindableObject element, int value);

                [AttachedProperty]
                public static partial string? GetLabel(BindableObject target);

                public static partial void SetLabel(BindableObject target, string? value);
            }

            public partial struct AttachedHost
            {
                [AttachedProperty]
                public static partial int GetValue(BindableObject obj);
            }
            """;

        // Act
        var problems = GeneratorTestHelper.GetProblemIds(source);

        // Assert
        Assert.Empty(problems);
    }

    [Fact]
    public void Btxp0014SetterNotMatchingGetterEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            public static partial class Attached
            {
                [AttachedProperty]
                public static partial int GetCount(BindableObject obj);

                static partial void SetCount(BindableObject obj, string value);
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        Assert.Equal(["BTXP0014"], diagnostics.Select(static x => x.Id));
    }

    [Fact]
    public void Btxp0005StructOrRecordHostEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;

            namespace Test;

            public partial struct StructHost
            {
                [BindableProperty]
                public partial int Value { get; set; }
            }

            public partial record RecordHost
            {
                [BindableProperty]
                public partial int Value { get; set; }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnosticsWithoutVerify(source);

        // Assert
        Assert.Equal(["BTXP0005", "BTXP0005"], diagnostics.Select(static x => x.Id));
    }

    // ------------------------------------------------------------
    // Default value
    // ------------------------------------------------------------

    [Fact]
    public void Btxp0010DefaultValueNotConvertibleEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            public partial class TestElement : BindableObject
            {
                [BindableProperty(DefaultValue = 1.5)]
                public partial int Truncated { get; set; }

                [BindableProperty(DefaultValue = null)]
                public partial int NullForValue { get; set; }

                [BindableProperty(DefaultValue = 1)]
                public partial double Widened { get; set; }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnosticsWithoutVerify(source);

        // Assert
        Assert.Equal(["BTXP0010", "BTXP0010"], diagnostics.Select(static x => x.Id));
    }

    [Fact]
    public void DefaultValueMemberOfDerivedTypeIsAccepted()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            public class Shape;

            public sealed class Circle : Shape;

            public partial class TestElement : BindableObject
            {
                public static readonly Circle DefaultShape = new();

                public const int DefaultCount = 1;

                [BindableProperty(DefaultValueMember = nameof(DefaultShape))]
                public partial Shape? Shape { get; set; }

                [BindableProperty(DefaultValueMember = nameof(DefaultCount))]
                public partial double Count { get; set; }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnosticsWithoutVerify(source);

        // Assert
        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal("BTXP0011", diagnostic.Id);
        Assert.Contains("DefaultCount", diagnostic.GetMessage(CultureInfo.InvariantCulture), StringComparison.Ordinal);
    }

    // ------------------------------------------------------------
    // Callback
    // ------------------------------------------------------------

    [Fact]
    public void Btxp0008CallbackHiddenByDelegateFieldEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using System;

            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            public partial class BaseElement : BindableObject
            {
                protected int Adjust(int value) => value;
            }

            public partial class TestElement : BaseElement
            {
                public new Func<int, int> Adjust = static x => x;

                [BindableProperty(Coerce = nameof(Adjust))]
                public partial int Value { get; set; }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnosticsWithoutVerify(source);

        // Assert
        Assert.Equal(["BTXP0008"], diagnostics.Select(static x => x.Id));
    }

    [Fact]
    public void CallbackWithDifferentTupleNamesIsAccepted()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            public partial class TestElement : BindableObject
            {
                [BindableProperty(Coerce = nameof(Adjust))]
                public partial (int A, int B) Pair { get; set; }

                private (int, int) Adjust((int, int) value) => value;
            }
            """;

        // Act
        var problems = GeneratorTestHelper.GetProblemIds(source);

        // Assert
        Assert.Empty(problems);
    }

    // ------------------------------------------------------------
    // Reporting
    // ------------------------------------------------------------

    [Fact]
    public void DiagnosticIsReportedOnPropertyName()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            public partial class TestElement : BindableObject
            {
                [BindableProperty]
                public string? Text { get; set; }
            }
            """;

        // Act
        var diagnostics = GeneratorTestHelper.GetDiagnostics(source);

        // Assert
        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal("BTXP0001", diagnostic.Id);
        Assert.True(diagnostic.Location.IsInSource);
        Assert.Equal("Text", diagnostic.Location.SourceTree.GetText(TestContext.Current.CancellationToken).ToString(diagnostic.Location.SourceSpan));
    }

    [Fact]
    public void ErrorsCannotBeSuppressed()
    {
        // Arrange
        var descriptors = typeof(BindablePropertyGenerator).Assembly.GetType("BunnyTail.XamlProperty.Generator.Diagnostics", throwOnError: true)!
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(static x => x.PropertyType == typeof(DiagnosticDescriptor))
            .Select(static x => (DiagnosticDescriptor)x.GetValue(null)!)
            .ToList();

        // Assert
        Assert.All(
            descriptors.Where(static x => x.DefaultSeverity == DiagnosticSeverity.Error),
            static x => Assert.Equal([WellKnownDiagnosticTags.NotConfigurable, WellKnownDiagnosticTags.Compiler], x.CustomTags));
    }

    // ------------------------------------------------------------
    // Fallback and lookup
    // ------------------------------------------------------------

    [Fact]
    public void CallbackNotFoundLeavesOnlyTheDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            public partial class TestElement : BindableObject
            {
                [BindableProperty(PropertyChanged = "OnMissing")]
                public partial int Value { get; set; }

                public static object Use() => ValueProperty;
            }
            """;

        // Act
        var problems = GeneratorTestHelper.GetProblemIds(source);

        // Assert
        Assert.Equal(["BTXP0008"], problems);
    }

    [Fact]
    public void InvalidDefaultValueLeavesOnlyTheDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            public partial class TestElement : BindableObject
            {
                [BindableProperty(DefaultValue = "text")]
                public partial int Value { get; set; }
            }
            """;

        // Act
        var problems = GeneratorTestHelper.GetProblemIds(source);

        // Assert
        Assert.Equal(["BTXP0010"], problems);
    }

    [Fact]
    public void DefaultValueExpressionUsesUsingsOfDeclaration()
    {
        // Arrange
        const string source =
            """
            namespace Test.Defaults
            {
                public static class Values
                {
                    public const int Answer = 42;
                }
            }

            namespace Test
            {
                using BunnyTail.XamlProperty;
                using Microsoft.Maui.Controls;
                using Test.Defaults;
                using static Test.Defaults.Values;
                using D = Test.Defaults.Values;

                public partial class TestElement : BindableObject
                {
                    [BindableProperty(DefaultValueExpression = "Values.Answer")]
                    public partial int First { get; set; }

                    [BindableProperty(DefaultValueExpression = "Answer")]
                    public partial int Second { get; set; }

                    [BindableProperty(DefaultValueExpression = "D.Answer")]
                    public partial int Third { get; set; }
                }
            }
            """;

        // Act
        var problems = GeneratorTestHelper.GetProblemIds(source);

        // Assert
        Assert.Empty(problems);
    }

    [Fact]
    public void DefaultValueExpressionOfOtherTypeEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            public partial class TestElement : BindableObject
            {
                [BindableProperty(DefaultValueExpression = "\"text\"")]
                public partial int Value { get; set; }
            }
            """;

        // Act
        var problems = GeneratorTestHelper.GetProblemIds(source);

        // Assert
        Assert.Equal(["BTXP0010"], problems);
    }

    [Fact]
    public void FieldNameTakenByUserMemberEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            public partial class TestElement : BindableObject
            {
                public static readonly string LevelProperty = "x";

                [BindableProperty]
                public partial int Level { get; set; }
            }
            """;

        // Act
        var problems = GeneratorTestHelper.GetProblemIds(source);

        // Assert
        Assert.Equal(["BTXP0015"], problems);
    }

    [Fact]
    public void SameNameOfPropertyAndAttachedPropertyEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            public partial class TestElement : BindableObject
            {
                [BindableProperty]
                public partial int Level { get; set; }

                [AttachedProperty]
                public static partial int GetLevel(BindableObject obj);
            }
            """;

        // Act
        var problems = GeneratorTestHelper.GetProblemIds(source);

        // Assert
        Assert.Equal(["BTXP0015"], problems);
    }

    [Fact]
    public void CallbackNamedLikeLambdaParameterIsCalled()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            public partial class TestElement : BindableObject
            {
                [BindableProperty(Coerce = nameof(value))]
                public partial int Value { get; set; }

            #pragma warning disable IDE1006
                private static int value(int x) => x;
            #pragma warning restore IDE1006
            }
            """;

        // Act
        var problems = GeneratorTestHelper.GetProblemIds(source);

        // Assert
        Assert.Empty(problems);
    }

    [Fact]
    public void OverloadInDerivedTypeDoesNotHideBaseCallback()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            public class BaseElement : BindableObject
            {
                protected void OnValueChanged(int oldValue, int newValue)
                {
                }
            }

            public partial class TestElement : BaseElement
            {
                private void OnValueChanged(string reason)
                {
                }

                [BindableProperty(PropertyChanged = nameof(OnValueChanged))]
                public partial int Value { get; set; }
            }
            """;

        // Act
        var problems = GeneratorTestHelper.GetProblemIds(source);

        // Assert
        Assert.Empty(problems);
    }

    [Fact]
    public void AttachedCallbackInBaseTypeIsResolved()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            public class HostBase
            {
                protected static void OnLevelChanged(BindableObject bindable, object oldValue, object newValue)
                {
                }
            }

            public partial class Host : HostBase
            {
                [AttachedProperty(PropertyChanged = nameof(OnLevelChanged))]
                public static partial int GetLevel(BindableObject obj);
            }
            """;

        // Act
        var problems = GeneratorTestHelper.GetProblemIds(source);

        // Assert
        Assert.Empty(problems);
    }

    [Fact]
    public void FileLocalContainingTypeEmitsDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            file partial class TestElement : BindableObject
            {
                [BindableProperty]
                public partial int Value { get; set; }
            }
            """;

        // Act
        var problems = GeneratorTestHelper.GetProblemIds(source);

        // Assert
        // A file-local type can not get the implementation from another file
        Assert.Equal(["BTXP0004", "CS9248"], problems);
    }

    [Fact]
    public void InvalidSetMethodGetsThrowingImplementation()
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

                public static partial void SetSuppress(BindableObject obj, int value);
            }
            """;

        // Act
        var problems = GeneratorTestHelper.GetProblemIds(source);

        // Assert
        Assert.Equal(["BTXP0014"], problems);
    }

    [Fact]
    public void RefReturnGetterGetsThrowingImplementation()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            public static partial class Refs
            {
                [AttachedProperty]
                public static partial ref int GetLevel(BindableObject obj);
            }
            """;

        // Act
        var problems = GeneratorTestHelper.GetProblemIds(source);

        // Assert
        Assert.Equal(["BTXP0012"], problems);
    }

    [Fact]
    public void NullableAttachedTargetCompilesWithoutWarning()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            public static partial class Labels
            {
                [AttachedProperty]
                public static partial string GetText(BindableObject? obj);

                public static partial void SetText(BindableObject? obj, string? value);
            }
            """;

        // Act
        var problems = GeneratorTestHelper.GetProblemIds(source);

        // Assert
        Assert.Empty(problems);
    }

    [Fact]
    public void ObsoleteCallbackCompilesWithoutWarning()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            public partial class TestElement : BindableObject
            {
                [BindableProperty(Coerce = nameof(CoerceValue))]
                public partial int Value { get; set; }

                [System.Obsolete("old")]
                private static int CoerceValue(int value) => value;
            }
            """;

        // Act
        var problems = GeneratorTestHelper.GetProblemIds(source);

        // Assert
        Assert.Empty(problems);
    }

    [Fact]
    public void TypeNamesDifferingOnlyInCaseEmitDiagnostic()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Microsoft.Maui.Controls;

            namespace Test;

            public partial class TestElement : BindableObject
            {
                [BindableProperty]
                public partial int Value { get; set; }
            }

            public partial class Testelement : BindableObject
            {
                [BindableProperty]
                public partial int Value { get; set; }
            }
            """;

        // Act
        var problems = GeneratorTestHelper.GetProblemIds(source);

        // Assert
        Assert.Contains("BTXP0016", problems);
        Assert.DoesNotContain("CS8785", problems);
    }
}
