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
            using Avalonia;

            namespace Test;

            public partial class BaseElement : AvaloniaObject
            {
                [StyledProperty]
                public virtual partial string? Title { get; set; }

                public int Tag { get; set; }
            }

            public partial class TestElement : BaseElement
            {
                [StyledProperty]
                partial string? Caption { get; set; }

                [StyledProperty]
                public sealed override partial string? Title { get; set; }

                [StyledProperty]
                public required partial string Header { get; set; }

                [StyledProperty]
                public new partial int Tag { get; set; }

                [StyledProperty]
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
            using Avalonia;

            namespace Test;

            public static partial class Attached
            {
                [AttachedProperty]
                public static partial int GetCount(this AvaloniaObject element);

                static partial void SetCount(this AvaloniaObject element, int value);

                [AttachedProperty]
                public static partial string? GetLabel(AvaloniaObject target);

                public static partial void SetLabel(AvaloniaObject target, string? value);
            }

            public partial struct AttachedHost
            {
                [AttachedProperty]
                public static partial int GetValue(AvaloniaObject obj);
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
            using Avalonia;

            namespace Test;

            public static partial class Attached
            {
                [AttachedProperty]
                public static partial int GetCount(AvaloniaObject obj);

                static partial void SetCount(AvaloniaObject obj, string value);
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
                [StyledProperty]
                public partial int Value { get; set; }
            }

            public partial record RecordHost
            {
                [StyledProperty]
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
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(DefaultValue = 1.5)]
                public partial int Truncated { get; set; }

                [StyledProperty(DefaultValue = null)]
                public partial int NullForValue { get; set; }

                [StyledProperty(DefaultValue = 1)]
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
            using Avalonia;

            namespace Test;

            public class Shape;

            public sealed class Circle : Shape;

            public partial class TestElement : AvaloniaObject
            {
                public static readonly Circle DefaultShape = new();

                public const int DefaultCount = 1;

                [StyledProperty(DefaultValueMember = nameof(DefaultShape))]
                public partial Shape? Shape { get; set; }

                [StyledProperty(DefaultValueMember = nameof(DefaultCount))]
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
            using Avalonia;

            namespace Test;

            public partial class BaseElement : AvaloniaObject
            {
                protected int Adjust(int value) => value;
            }

            public partial class TestElement : BaseElement
            {
                public new Func<int, int> Adjust = static x => x;

                [StyledProperty(Coerce = nameof(Adjust))]
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
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(Coerce = nameof(Adjust))]
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
        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal("BTXP0001", diagnostic.Id);
        Assert.True(diagnostic.Location.IsInSource);
        Assert.Equal("Text", diagnostic.Location.SourceTree.GetText(TestContext.Current.CancellationToken).ToString(diagnostic.Location.SourceSpan));
    }

    [Fact]
    public void ErrorsCannotBeSuppressed()
    {
        // Arrange
        var descriptors = typeof(StyledPropertyGenerator).Assembly.GetType("BunnyTail.XamlProperty.Generator.Diagnostics", throwOnError: true)!
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
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(Coerce = "OnMissing")]
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
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(DefaultValue = "text")]
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
                using Avalonia;
                using Test.Defaults;
                using static Test.Defaults.Values;
                using D = Test.Defaults.Values;

                public partial class TestElement : AvaloniaObject
                {
                    [StyledProperty(DefaultValueExpression = "Values.Answer")]
                    public partial int First { get; set; }

                    [StyledProperty(DefaultValueExpression = "Answer")]
                    public partial int Second { get; set; }

                    [StyledProperty(DefaultValueExpression = "D.Answer")]
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
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(DefaultValueExpression = "\"text\"")]
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
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                public static readonly string LevelProperty = "x";

                [StyledProperty]
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
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty]
                public partial int Level { get; set; }

                [AttachedProperty]
                public static partial int GetLevel(AvaloniaObject obj);
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
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(Coerce = nameof(value))]
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
            using Avalonia;

            namespace Test;

            public class BaseElement : AvaloniaObject
            {
                protected static int CoerceValue(int value) => value;
            }

            public partial class TestElement : BaseElement
            {
                private static string CoerceValue(string reason) => reason;

                [StyledProperty(Coerce = nameof(CoerceValue))]
                public partial int Value { get; set; }
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
            using Avalonia;

            namespace Test;

            file partial class TestElement : AvaloniaObject
            {
                [StyledProperty]
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
            using Avalonia;

            namespace Test;

            public static partial class Focus
            {
                [AttachedProperty]
                public static partial bool GetSuppress(AvaloniaObject obj);

                public static partial void SetSuppress(AvaloniaObject obj, int value);
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
            using Avalonia;

            namespace Test;

            public static partial class Refs
            {
                [AttachedProperty]
                public static partial ref int GetLevel(AvaloniaObject obj);
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
            using Avalonia;

            namespace Test;

            public static partial class Labels
            {
                [AttachedProperty]
                public static partial string GetText(AvaloniaObject? obj);

                public static partial void SetText(AvaloniaObject? obj, string? value);
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
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(Coerce = nameof(CoerceValue))]
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
    public void NullableAnnotationsOfCallbacksCompileWithoutWarning()
    {
        // Arrange
        const string source =
            """
            using BunnyTail.XamlProperty;
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty(Coerce = nameof(CoerceText), Validate = nameof(IsValidText))]
                public partial string? Text { get; set; }

                private static string CoerceText(string value) => value.Trim();

                private static bool IsValidText(string value) => value.Length < 10;

                [StyledProperty(DefaultValue = "", Coerce = nameof(CoerceName))]
                public partial string Name { get; set; }

                private static string? CoerceName(string? value) => value;

                [StyledProperty(Coerce = nameof(CoerceTitle))]
                public partial string? Title { get; set; }

                private static string CoerceTitle(AvaloniaObject o, string value) => value;

                [StyledProperty(DefaultValue = "")]
                [System.Diagnostics.CodeAnalysis.AllowNull]
                public partial string Label { get; set; }
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
            using Avalonia;

            namespace Test;

            public partial class TestElement : AvaloniaObject
            {
                [StyledProperty]
                public partial int Value { get; set; }
            }

            public partial class Testelement : AvaloniaObject
            {
                [StyledProperty]
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
