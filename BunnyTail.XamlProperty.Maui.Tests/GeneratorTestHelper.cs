namespace BunnyTail.XamlProperty;

using System.Collections.Generic;

using BunnyTail.XamlProperty.Generator;

using Microsoft.CodeAnalysis;

using SourceGenerateHelper.Testing;

internal static class GeneratorTestHelper
{
    private static GeneratorTestRunner Runner => GeneratorTestRunner
        .For<BindablePropertyGenerator>()
        .WithReference(typeof(BindablePropertyAttribute).Assembly)
        .WithReference(typeof(BindableObject).Assembly)
        .WithReference(typeof(BindingMode).Assembly)
        .Add(new AttachedPropertyGenerator())
        .WithDiagnosticPrefix("BTXP")
        .VerifyCompiles();

    public static IReadOnlyList<Diagnostic> GetDiagnostics(string source) => Runner.GetDiagnostics(source);

    public static IReadOnlyList<Diagnostic> GetDiagnosticsAll(string source) => Runner.GetDiagnosticsAll(source);

    // Used when the generated code can not compile by design, such as a type with no known base type
    public static IReadOnlyList<Diagnostic> GetDiagnosticsWithoutVerify(string source) =>
        Runner.VerifyCompiles(false).GetDiagnostics(source);

    public static string GetGeneratedSource(string source) => Runner.GetGeneratedSource(source);

    public static IncrementalRunResult RunIncremental(string source, string addedSource) =>
        Runner.WithTracking().RunIncremental(source, addedSource);
}
