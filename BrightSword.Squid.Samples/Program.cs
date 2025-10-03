using System;
using BrightSword.Squid.TypeCreators;

public interface ISampleDto
{
    string Name { get; set; }
    int Value { get; set; }
}

public interface ICloneFacet : ICloneable { }

class Program
{
    static void Main()
    {
        var creator = new BasicDataTransferObjectTypeCreator<ISampleDto>
        {
            // Add the facet interface so the built-in CloneBehaviour (registered for ICloneable)
            // will be applied automatically by the generator.
            FacetInterfaces = new[] { typeof(ICloneFacet) }
        };

        var type = creator.Type;
        Console.WriteLine($"Emitted type: {type.FullName}");

        var instance = (ISampleDto)creator.CreateInstance();
        instance.Name = "sample";
        instance.Value = 123;

        Console.WriteLine($"Name={instance.Name}, Value={instance.Value}");

        if (instance is ICloneable c)
        {
            var copy = (ISampleDto)c.Clone();
            Console.WriteLine($"Copied Name={copy.Name}, Value={copy.Value}");
        }
    }
}
