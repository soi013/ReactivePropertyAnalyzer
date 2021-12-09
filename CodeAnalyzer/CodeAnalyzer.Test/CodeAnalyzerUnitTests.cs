using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = CodeAnalyzer.Test.CSharpCodeFixVerifier<
    CodeAnalyzer.ReactivePropertyCreationAnalyzer,
    CodeAnalyzer.CodeAnalyzerCodeFixProvider>;

namespace CodeAnalyzer.Test
{
    [TestClass]
    public class CodeAnalyzerUnitTest
    {
        //No diagnostics expected to show up
        [TestMethod]
        public async Task TestMethod1()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task TestMethod5()
        {
            var test = @"
using Reactive.Bindings;
using System;

namespace DemoAnalyzer
{
public class SomeViewModel
{
    public ReactiveProperty<string> RName1 { get; } = new ReactiveProperty<string>();
}
}";
            var expected = VerifyCS.Diagnostic(ReactivePropertyCreationAnalyzer.DiagnosticId);
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task TestMethod6()
        {
            var test = @"
using Reactive.Bindings;
using System;

namespace DemoAnalyzer
{
public class SomeViewModel
{
    public ReactiveProperty<string> RName1 { get; } = [|new()|];
}
}";
            var expected = VerifyCS.Diagnostic(ReactivePropertyCreationAnalyzer.DiagnosticId);
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
