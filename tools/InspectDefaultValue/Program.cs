using System;
using System.ComponentModel;
using System.Reflection;

namespace InspectDefaultValue
{
    class Program
    {
        static void Main()
        {
            var iFace = typeof(Tests.BrightSword.Squid.SetDefaultValueTests.IInterfaceWithNonSupportedDefaultValue);
            var prop = iFace.GetProperty("SomeProperty");
            var attr = prop.GetCustomAttribute<DefaultValueAttribute>();
            Console.WriteLine($"Attribute Instance Type: {attr.GetType().FullName}");
            Console.WriteLine($"attr.Value (public): {attr.Value ?? "<null>"}");

            Console.WriteLine("-- CustomAttributeData --");
            var cad = CustomAttributeData.GetCustomAttributes(prop);
            foreach (var data in cad)
            {
                Console.WriteLine($"Attribute: {data.AttributeType.FullName}");
                foreach (var arg in data.ConstructorArguments)
                {
                    Console.WriteLine($"  Ctor arg: Type={arg.ArgumentType.FullName}, Value={arg.Value}");
                }
                foreach (var na in data.NamedArguments)
                {
                    Console.WriteLine($"  Named arg: {na.MemberName} = {na.TypedValue.Value}");
                }
            }
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}
