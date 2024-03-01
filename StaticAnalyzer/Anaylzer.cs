using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StaticAnalyzer
{
    public class Anaylzer
    {
        public static async Task<AnalyzerResult[]> Analyze(string solutionPath, string msBuilderPath)
        {

            const string methodName = "Start";
            const string className = "PipeLineComposer";

            List<AnalyzerResult> results = new List<AnalyzerResult>();

            if (!MSBuildLocator.IsRegistered)
            {

                MSBuildLocator.RegisterMSBuildPath(msBuilderPath);
            }


            var workspace = MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(solutionPath);

            ISymbol methodSymbolType = null;

            foreach (var project in solution.Projects)
            {
                var compilation = await project.GetCompilationAsync();

                foreach (var document in project.Documents)
                {
                    var tree = await document.GetSyntaxTreeAsync();
                    var pipeLineComposerClass = tree.GetRoot().DescendantNodes()
                                                             .OfType<ClassDeclarationSyntax>()
                                                             .Where(x => x.Identifier.Text == className)
                                                             .FirstOrDefault();
                    if (pipeLineComposerClass is null)
                        continue;

                    var syntaxRoot = await document.GetSyntaxRootAsync();

                    var semanticModel = await document.GetSemanticModelAsync();
                    var startMethod = syntaxRoot
                        .DescendantNodes()
                        .OfType<MethodDeclarationSyntax>()
                        .First(x => x.Identifier.Text == methodName);

                    methodSymbolType = semanticModel.GetDeclaredSymbol(startMethod);
                    var referencesType0 = await SymbolFinder.FindReferencesAsync(methodSymbolType, solution);

                    foreach (var reference in referencesType0)
                    {
                        foreach (var location in reference.Locations.DistinctBy(x=>x.Document.FilePath))
                        {

                            var methodsInDocument = reference.Locations.First().Location.SourceTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>();
                            var model = await location.Document.GetSemanticModelAsync();
                            foreach (var method in methodsInDocument)
                            {
                                var pipelineMethods = method.DescendantNodes()
                                       .OfType<InvocationExpressionSyntax>()
                                       .Where(x =>
                                       {
                                           var expr = x.Expression;


                                           if (expr is MemberAccessExpressionSyntax memberAccessExpressionSyntax)
                                           {
                                               bool match = memberAccessExpressionSyntax.Name.Identifier.Text == methodName;
                                               if (!match)
                                                   match = memberAccessExpressionSyntax.Name.Identifier.Text == "Handle";
                                               if (!match)
                                                   match = memberAccessExpressionSyntax.Name.Identifier.Text == "End";

                                               return match;
                                           }
                                           return false;
                                       });

                                Debug.WriteLine(location.Document.FilePath.ToString());
                                Debug.WriteLine(method.Identifier.ValueText);
                                pipelineMethods = pipelineMethods.Reverse();
                                foreach (var item in pipelineMethods)
                                {
                                    Debug.WriteLine(item.ArgumentList.Arguments.FirstOrDefault());
                                }

                                results.Add(new()
                                {
                                    Project = location.Document.Project.Name,
                                    Class = method.Parent is ClassDeclarationSyntax ? ((ClassDeclarationSyntax)method.Parent).Identifier.ValueText : "",
                                    DocumentPath = location.Document.FilePath,
                                    MethodName = method.Identifier.ValueText,

                                    UseCases = pipelineMethods.Select((x, i) => new UseCaseResult
                                    {
                                        Name = x.ArgumentList.Arguments.FirstOrDefault().Expression.ToString().Trim('"'),
                                        IsToggledOff = x.ArgumentList.Arguments.Count < 3 ? false : Convert.ToBoolean(x.ArgumentList.Arguments[1].Expression.ToString()),
                                        Type = (i == 0 ? "Feature" : (i == (pipelineMethods.Count() - 1)) ? "End" : "UseCase")
                                    }).ToArray(),
                                });

                            }

                        }



                    }
                }


            }

            return results.ToArray();
        }
    }


}
