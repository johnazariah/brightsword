using System;

using BrightSword.Feber.Samples;
using BrightSword.Squid.TypeCreators;

namespace BrightSword.Squid
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Intentional generic type-specific cached TypeCreator")]
    public static class Dynamic<T>
        where T : class
    {
        private static readonly BasicDataTransferObjectTypeCreator<T> _typeCreator = new();

        public static Type Type => _typeCreator.Type;

        public static T NewInstance(dynamic source = null) => _typeCreator.CreateInstance(source);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Intentional generic type-specific cached TypeCreator")]
    public static class Dynamic<T, TTypeCreator>
        where T : class
        where TTypeCreator : BasicDataTransferObjectTypeCreator<T>, new()
    {
        private static readonly TTypeCreator _typeCreator = new();

        public static Type Type => _typeCreator.Type;

        public static T NewInstance() => _typeCreator.CreateInstance();
    }
}
