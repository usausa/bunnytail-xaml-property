namespace BunnyTail.XamlProperty.Generator;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using BunnyTail.XamlProperty.Generator.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SourceGenerateHelper;

[Generator]
public sealed class StyledPropertyGenerator : IIncrementalGenerator
{
    private const string AttributeName = "BunnyTail.XamlProperty.StyledPropertyAttribute";

    private const string AvaloniaObjectTypeName = "Avalonia.AvaloniaObject";

    private const string StyledPropertyTypeName = "global::Avalonia.StyledProperty";
    private const string AvaloniaPropertyTypeName = "global::Avalonia.AvaloniaProperty";

    private static readonly SymbolDisplayFormat TypeDisplayFormat = SymbolDisplayFormat.FullyQualifiedFormat
        .WithMiscellaneousOptions(SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

    // ------------------------------------------------------------
    // Initialize
    // ------------------------------------------------------------

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var propertyProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                AttributeName,
                static (syntax, _) => IsPropertySyntax(syntax),
                static (context, _) => GetPropertyModel(context))
            .Collect();

        context.RegisterSourceOutput(
            propertyProvider,
            static (context, properties) => ReportDiagnostics(context, properties));

        var typeProvider = propertyProvider.SelectMany(static (properties, _) => SelectTypeModel(properties));

        context.RegisterImplementationSourceOutput(
            typeProvider,
            static (context, type) => Execute(context, type));
    }

    private static ImmutableArray<TypeModel> SelectTypeModel(ImmutableArray<Result<PropertyModel>> properties) =>
        [.. properties
            .SelectValue()
            .GroupBy(static x => new { x.Namespace, x.ClassName, x.ContainingTypes })
            .Select(static x => new TypeModel(
                x.Key.Namespace,
                x.Key.ClassName,
                x.Key.ContainingTypes,
                new EquatableArray<PropertyModel>(x)))];

    // ------------------------------------------------------------
    // Parser
    // ------------------------------------------------------------

    private static bool IsPropertySyntax(SyntaxNode syntax) =>
        syntax is PropertyDeclarationSyntax;

    private static Result<PropertyModel> GetPropertyModel(GeneratorAttributeSyntaxContext context)
    {
        var syntax = (PropertyDeclarationSyntax)context.TargetNode;
        if (context.TargetSymbol is not IPropertySymbol symbol)
        {
            return Results.Errors<PropertyModel>();
        }

        var location = syntax.GetLocation();

        // Validate property definition
        if (symbol.IsStatic)
        {
            return Results.Error<PropertyModel>(new DiagnosticInfo(Diagnostics.StaticPropertyNotSupported, location, symbol.Name));
        }

        if (!symbol.IsPartialDefinition || !syntax.Modifiers.Any(static x => x.IsKind(SyntaxKind.PartialKeyword)))
        {
            return Results.Error<PropertyModel>(new DiagnosticInfo(Diagnostics.InvalidPropertyDefinition, location, symbol.Name));
        }

        if ((symbol.GetMethod is null) || (symbol.SetMethod is null) ||
            symbol.SetMethod.IsInitOnly ||
            (symbol.GetMethod.DeclaredAccessibility != symbol.DeclaredAccessibility) ||
            (symbol.SetMethod.DeclaredAccessibility != symbol.DeclaredAccessibility))
        {
            return Results.Error<PropertyModel>(new DiagnosticInfo(Diagnostics.InvalidPropertyAccessor, location, symbol.Name));
        }

        // Validate containing type
        for (var typeSyntax = syntax.Parent as TypeDeclarationSyntax; typeSyntax is not null; typeSyntax = typeSyntax.Parent as TypeDeclarationSyntax)
        {
            if (!typeSyntax.Modifiers.Any(static x => x.IsKind(SyntaxKind.PartialKeyword)))
            {
                return Results.Error<PropertyModel>(new DiagnosticInfo(Diagnostics.ContainingTypeNotPartial, location, symbol.Name));
            }
        }

        var containingType = symbol.ContainingType;
        for (var type = containingType; type is not null; type = type.ContainingType)
        {
            if (type.IsGenericType)
            {
                return Results.Error<PropertyModel>(new DiagnosticInfo(Diagnostics.GenericTypeNotSupported, location, symbol.Name));
            }
        }

        // The base type can be declared in another partial declaration, such as one generated from XAML,
        // so the check is skipped when the type has no explicit base type
        if (containingType.BaseType is { SpecialType: not SpecialType.System_Object } declaredBaseType)
        {
            var isAvaloniaObject = false;
            for (var baseType = declaredBaseType; baseType is not null; baseType = baseType.BaseType)
            {
                if (baseType.ToDisplayString() == AvaloniaObjectTypeName)
                {
                    isAvaloniaObject = true;
                    break;
                }
            }

            if (!isAvaloniaObject)
            {
                return Results.Error<PropertyModel>(new DiagnosticInfo(Diagnostics.InvalidContainingType, location, symbol.Name));
            }
        }

        // Parse attribute
        var defaultValue = default(TypedConstant?);
        var defaultValueExpression = default(string);
        var defaultValueMember = default(string);
        var defaultBindingMode = default(string);
        var inherits = false;
        var enableDataValidation = false;
        var coerceName = default(string);
        var validateName = default(string);
        foreach (var argument in context.Attributes[0].NamedArguments)
        {
            switch (argument.Key)
            {
                case "DefaultValue":
                    defaultValue = argument.Value;
                    break;
                case "DefaultValueExpression":
                    defaultValueExpression = argument.Value.Value as string;
                    break;
                case "DefaultValueMember":
                    defaultValueMember = argument.Value.Value as string;
                    break;
                case "DefaultBindingMode":
                    defaultBindingMode = argument.Value.ToCSharpExpression();
                    break;
                case "Inherits":
                    inherits = argument.Value.Value is true;
                    break;
                case "EnableDataValidation":
                    enableDataValidation = argument.Value.Value is true;
                    break;
                case "Coerce":
                    coerceName = argument.Value.Value as string;
                    break;
                case "Validate":
                    validateName = argument.Value.Value as string;
                    break;
            }
        }

        // Default value
        var defaultValueCount = (defaultValue.HasValue ? 1 : 0) +
                                (String.IsNullOrEmpty(defaultValueExpression) ? 0 : 1) +
                                (String.IsNullOrEmpty(defaultValueMember) ? 0 : 1);
        if (defaultValueCount > 1)
        {
            return Results.Error<PropertyModel>(new DiagnosticInfo(Diagnostics.DefaultValueConflict, location, symbol.Name));
        }

        var defaultValueLiteral = defaultValueExpression;
        if (defaultValue.HasValue)
        {
            defaultValueLiteral = defaultValue.Value.ToCSharpExpression(symbol.Type);
            if (defaultValueLiteral is null)
            {
                return Results.Error<PropertyModel>(new DiagnosticInfo(Diagnostics.InvalidDefaultValue, location, symbol.Name));
            }
        }
        else if (!String.IsNullOrEmpty(defaultValueMember))
        {
            if (!IsDefaultValueMember(context.SemanticModel.Compilation, containingType, defaultValueMember!, symbol.Type))
            {
                return Results.Error<PropertyModel>(new DiagnosticInfo(Diagnostics.InvalidDefaultValueMember, location, defaultValueMember!));
            }

            defaultValueLiteral = defaultValueMember;
        }

        // Callback
        var coerce = default(CoerceModel);
        if (!String.IsNullOrEmpty(coerceName))
        {
            var (model, error) = ResolveCoerce(context.SemanticModel.Compilation, containingType, coerceName!, symbol.Type, location);
            if (error is not null)
            {
                return Results.Error<PropertyModel>(error);
            }

            coerce = model;
        }

        var validate = default(ValidateModel);
        if (!String.IsNullOrEmpty(validateName))
        {
            var (model, error) = ResolveValidate(context.SemanticModel.Compilation, containingType, validateName!, symbol.Type, location);
            if (error is not null)
            {
                return Results.Error<PropertyModel>(error);
            }

            validate = model;
        }

        // Model
        var ns = String.IsNullOrEmpty(containingType.ContainingNamespace.Name)
            ? string.Empty
            : containingType.ContainingNamespace.ToDisplayString();

        var containingTypes = default(List<ContainingTypeModel>?);
        var containingSymbol = containingType.ContainingType;
        while (containingSymbol is not null)
        {
            containingTypes ??= [];
            containingTypes.Add(new ContainingTypeModel(containingSymbol.GetClassName(), containingSymbol.IsValueType));
            containingSymbol = containingSymbol.ContainingType;
        }

        containingTypes?.Reverse();

        return Results.Success(new PropertyModel(
            ns,
            containingType.GetClassName(),
            new EquatableArray<ContainingTypeModel>(containingTypes ?? []),
            symbol.DeclaredAccessibility,
            symbol.Name,
            symbol.Type.ToDisplayString(TypeDisplayFormat),
            defaultValueLiteral,
            defaultBindingMode,
            inherits,
            enableDataValidation,
            coerce,
            validate));
    }

    private static (CoerceModel? Model, DiagnosticInfo? Error) ResolveCoerce(Compilation compilation, INamedTypeSymbol containingType, string methodName, ITypeSymbol propertyType, Location location)
    {
        var found = false;
        var candidates = new List<CoerceModel>();
        foreach (var method in EnumerateCallbackMethods(compilation, containingType, methodName))
        {
            found = true;

            if (method.IsGenericMethod || !SymbolEqualityComparer.Default.Equals(method.ReturnType, propertyType))
            {
                continue;
            }

            // A method that matches the coerce delegate is used as a method group
            if (method.IsStatic &&
                (method.Parameters.Length == 2) &&
                (method.Parameters[0].Type.ToDisplayString() == AvaloniaObjectTypeName) &&
                SymbolEqualityComparer.Default.Equals(method.Parameters[1].Type, propertyType))
            {
                candidates.Add(new CoerceModel(methodName, true, true));
                continue;
            }

            if ((method.Parameters.Length != 1) ||
                !SymbolEqualityComparer.Default.Equals(method.Parameters[0].Type, propertyType))
            {
                continue;
            }

            candidates.Add(new CoerceModel(methodName, false, method.IsStatic));
        }

        if (candidates.Count == 1)
        {
            return (candidates[0], null);
        }

        return found
            ? (null, new DiagnosticInfo(Diagnostics.InvalidCallbackMethod, location, methodName))
            : (null, new DiagnosticInfo(Diagnostics.CallbackMethodNotFound, location, methodName));
    }

    private static (ValidateModel? Model, DiagnosticInfo? Error) ResolveValidate(Compilation compilation, INamedTypeSymbol containingType, string methodName, ITypeSymbol propertyType, Location location)
    {
        var found = false;
        var candidates = new List<ValidateModel>();
        foreach (var method in EnumerateCallbackMethods(compilation, containingType, methodName))
        {
            found = true;

            // Avalonia validate is Func<TValue, bool> and does not receive the instance
            if (!method.IsStatic || method.IsGenericMethod ||
                (method.Parameters.Length != 1) ||
                !SymbolEqualityComparer.Default.Equals(method.Parameters[0].Type, propertyType) ||
                (method.ReturnType.SpecialType != SpecialType.System_Boolean))
            {
                continue;
            }

            candidates.Add(new ValidateModel(methodName));
        }

        if (candidates.Count == 1)
        {
            return (candidates[0], null);
        }

        return found
            ? (null, new DiagnosticInfo(Diagnostics.InvalidCallbackMethod, location, methodName))
            : (null, new DiagnosticInfo(Diagnostics.CallbackMethodNotFound, location, methodName));
    }

    private static IEnumerable<IMethodSymbol> EnumerateCallbackMethods(Compilation compilation, INamedTypeSymbol containingType, string methodName)
    {
        for (var type = containingType; type is not null; type = type.BaseType)
        {
            var declared = false;
            foreach (var method in type.GetMembers(methodName).OfType<IMethodSymbol>())
            {
                if (!compilation.IsSymbolAccessibleWithin(method, containingType))
                {
                    continue;
                }

                declared = true;
                yield return method;
            }

            if (declared)
            {
                yield break;
            }
        }
    }

    // A default value member is a static field or property of the property type
    private static bool IsDefaultValueMember(Compilation compilation, INamedTypeSymbol containingType, string memberName, ITypeSymbol propertyType)
    {
        for (var type = containingType; type is not null; type = type.BaseType)
        {
            var declared = false;
            foreach (var member in type.GetMembers(memberName))
            {
                if (!compilation.IsSymbolAccessibleWithin(member, containingType))
                {
                    continue;
                }

                declared = true;

                var memberType = member switch
                {
                    IFieldSymbol { IsStatic: true } field => field.Type,
                    IPropertySymbol { IsStatic: true, GetMethod: not null } property => property.Type,
                    _ => null
                };
                if ((memberType is not null) && SymbolEqualityComparer.Default.Equals(memberType, propertyType))
                {
                    return true;
                }
            }

            if (declared)
            {
                return false;
            }
        }

        return false;
    }

    // ------------------------------------------------------------
    // Generator
    // ------------------------------------------------------------

    private static void ReportDiagnostics(SourceProductionContext context, ImmutableArray<Result<PropertyModel>> properties)
    {
        foreach (var info in properties.SelectError())
        {
            context.ReportDiagnostic(info);
        }
    }

    private static void Execute(SourceProductionContext context, TypeModel type)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        var builder = new SourceBuilder();
        BuildSource(builder, type);

        context.AddSource(
            HintNameBuilder.Build(type.Namespace, [.. type.ContainingTypes.Select(static x => x.ClassName), type.ClassName]),
            builder);
    }

    private static void BuildSource(SourceBuilder builder, TypeModel type)
    {
        var ns = type.Namespace;
        var containingTypes = type.ContainingTypes;
        var className = type.ClassName;

        builder.AutoGenerated();
        builder.EnableNullable();
        builder.NewLine();

        // namespace
        if (!String.IsNullOrEmpty(ns))
        {
            builder.Namespace(ns);
            builder.NewLine();
        }

        // containing types
        foreach (var containingType in containingTypes)
        {
            builder
                .Indent()
                .Append("partial ")
                .Append(containingType.IsValueType ? "struct " : "class ")
                .Append(containingType.ClassName)
                .NewLine();
            builder.BeginScope();
        }

        // class
        builder
            .Indent()
            .Append("partial class ")
            .Append(className)
            .NewLine();
        builder.BeginScope();

        var first = true;
        foreach (var property in type.Properties)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                builder.NewLine();
            }

            BuildProperty(builder, className, property);
        }

        builder.EndScope();

        // end containing types
        for (var i = 0; i < containingTypes.Count; i++)
        {
            builder.EndScope();
        }
    }

    private static void BuildProperty(SourceBuilder builder, string className, PropertyModel property)
    {
        var accessibility = property.PropertyAccessibility.ToText();

        // field
        builder
            .Indent()
            .Append(accessibility)
            .Append(" static readonly ")
            .Append(StyledPropertyTypeName)
            .Append("<")
            .Append(property.PropertyType)
            .Append("> ")
            .Append(property.PropertyName)
            .Append("Property = ")
            .Append(AvaloniaPropertyTypeName)
            .Append(".Register<")
            .Append(className)
            .Append(", ")
            .Append(property.PropertyType)
            .Append(">(")
            .NewLine();
        builder.Indent().Append("    nameof(").Append(property.PropertyName).Append(")");

        foreach (var argument in MakeOptionArguments(className, property))
        {
            builder.Append(",").NewLine();
            builder.Indent().Append("    ").Append(argument);
        }

        builder.Append(");").NewLine();
        builder.NewLine();

        // property
        builder
            .Indent()
            .Append(accessibility)
            .Append(" partial ")
            .Append(property.PropertyType)
            .Append(" ")
            .Append(property.PropertyName)
            .NewLine();
        builder.BeginScope();
        builder.Indent().Append("get => GetValue(").Append(property.PropertyName).Append("Property);").NewLine();
        builder.Indent().Append("set => SetValue(").Append(property.PropertyName).Append("Property, value);").NewLine();
        builder.EndScope();
    }

    private static List<string> MakeOptionArguments(string className, PropertyModel property)
    {
        var arguments = new List<string>();

        if (property.DefaultValue is not null)
        {
            arguments.Add($"defaultValue: {property.DefaultValue}");
        }

        if (property.Inherits)
        {
            arguments.Add("inherits: true");
        }

        if (property.DefaultBindingMode is not null)
        {
            arguments.Add($"defaultBindingMode: {property.DefaultBindingMode}");
        }

        if (property.Validate is not null)
        {
            arguments.Add($"validate: {property.Validate.MethodName}");
        }

        if (property.Coerce is not null)
        {
            arguments.Add($"coerce: {MakeCoerceCallback(className, property.Coerce)}");
        }

        if (property.EnableDataValidation)
        {
            arguments.Add("enableDataValidation: true");
        }

        return arguments;
    }

    private static string MakeCoerceCallback(string className, CoerceModel coerce)
    {
        if (coerce.IsMethodGroup)
        {
            return coerce.MethodName;
        }

        return coerce.IsStatic
            ? $"static (o, value) => {coerce.MethodName}(value)"
            : $"static (o, value) => (({className})o).{coerce.MethodName}(value)";
    }
}
