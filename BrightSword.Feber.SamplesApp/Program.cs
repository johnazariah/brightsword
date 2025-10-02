using System;
using BrightSword.Feber.Samples;

namespace BrightSword.Feber.SamplesApp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("BrightSword.Feber LazyActionBuilderExample demo");

            // Use a concrete demo type with properties so the sample prints values.
            var sample = new LazyActionBuilderExample<DemoProto, DemoProto>();

            // Accessing Action will trigger lazy compilation once.
            var action = sample.Action;

            Console.WriteLine(action == null ? "Action is null" : "Action compiled successfully");

            var instance = new DemoProto { Name = "Alice", Age = 30 };

            // Invoke the compiled action which will print the properties.
            if (action is null)
            {
                Console.WriteLine("Action is null (unexpected)");
            }
            else
            {
                action(instance);
            }

            Console.WriteLine("Done.");
        }
    }

    public class DemoProto
    {
        // Initialize with empty string to satisfy the non-nullable contract and avoid CS8618.
        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }
    }
}
