using System;

namespace BrightSword.Squid.API
{
    public interface ITypeCreator<out T>
        where T : class
    {
        Type Type { get; }
        T CreateInstance(dynamic source = null);
    }
}