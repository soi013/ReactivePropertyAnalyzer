using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace CodeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReactivePropertyCreationAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "ReactivePropertyCreation";

        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new(
            id: DiagnosticId,
            title: "ReactivePropertyCreation",
            messageFormat: "ReactiveProperty avoid null initialize",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Set initial value.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            //for `new ReactiveProperty()`
            context.RegisterSyntaxNodeAction(AnalyzeObjectCreationSyntax, SyntaxKind.ObjectCreationExpression);

            //for `new()`
            context.RegisterSyntaxNodeAction(AnalyzeImplicitObjectCreationSyntax, SyntaxKind.ImplicitObjectCreationExpression);

            //TODO for  new ReactiveProperty<string>(mode: ReactivePropertyMode.Default);
        }

        private void AnalyzeObjectCreationSyntax(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not ObjectCreationExpressionSyntax invocationExpr)
                return;

            if (invocationExpr.Type is not GenericNameSyntax geneSynatx)
                return;

            if (geneSynatx.Identifier.ValueText is not nameof(ReactiveProperty) or nameof(ReadOnlyReactivePropertySlim))
                return;

            var typeGenerigArg = geneSynatx.TypeArgumentList.Arguments.FirstOrDefault();

            if (typeGenerigArg == null)
                return;

            var symbolGeneric = context.SemanticModel.GetSymbolInfo(typeGenerigArg).Symbol;

            if ((symbolGeneric as INamedTypeSymbol)?.IsValueType != false)
                return;

            var arguments = invocationExpr.ArgumentList?.Arguments;

            if (arguments?.Count != 0)
                return;

            var diagnostic = Diagnostic.Create(Rule, invocationExpr.GetLocation(), invocationExpr.ToString());
            context.ReportDiagnostic(diagnostic);
        }

        private void AnalyzeImplicitObjectCreationSyntax(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not ImplicitObjectCreationExpressionSyntax invocationExpr)
                return;

            var typeSymbol = context.SemanticModel.GetTypeInfo(invocationExpr, context.CancellationToken).Type;

            if (typeSymbol?.Name is not nameof(ReactiveProperty) or nameof(ReadOnlyReactivePropertySlim))
                return;

            var symbolGeneric = (typeSymbol as INamedTypeSymbol)?.TypeArguments.FirstOrDefault();

            if (symbolGeneric?.IsValueType != false)
                return;

            var arguments = invocationExpr.ArgumentList?.Arguments;
            if (arguments?.Count != 0)
                return;

            var diagnostic = Diagnostic.Create(Rule, invocationExpr.GetLocation(), invocationExpr.ToString());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
