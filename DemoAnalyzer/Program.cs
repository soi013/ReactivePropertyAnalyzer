using Reactive.Bindings;
using System;
using System.Text.RegularExpressions;

namespace DemoAnalyzer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            Console.WriteLine(args);

            var someViewModel = new SomeViewModel();
        }
    }

    public class SomeViewModel
    {
        public ReactiveProperty<string> RName2 { get; } = new ReactiveProperty<string>(string.Empty);
        public ReactiveProperty<string?> RName3 { get; } = new ReactiveProperty<string?>(default(string));
        public ReactiveProperty<int> RName4 { get; } = new();
        public ReactiveProperty<int> RName5 { get; } = new ReactiveProperty<int>();
        public ReactiveProperty<string> RNameNull1 { get; } = new();
        public ReactiveProperty<string> RNameNull2 { get; } = new ReactiveProperty<string>();

        public void SetNullValue()
        {
            RNameNull1.Value = null;
            RName2.Value = null;
            RName3.Value = null;
        }

        public int GetValue()
        {
            int l1 = RNameNull1.Value.Length;
            int l2 = RName2.Value.Length;
            int l3 = RName3.Value.Length;
            return l1 + l2 + l3;
        }

        public string GetObjValue()
        {
            object o1 = ((IReactiveProperty)RNameNull1).Value;
            object o2 = ((IReactiveProperty)RName2).Value;
            object o3 = ((IReactiveProperty)RName3).Value;
            return o1.ToString();
        }
    }
}