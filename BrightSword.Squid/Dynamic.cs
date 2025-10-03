using System;
using BrightSword.Squid.TypeCreators;

namespace BrightSword.Squid
{
    /// <summary>
    /// Convenience static factory for creating runtime-generated instances for a given interface
    /// type <typeparamref name="T"/>. This caches a single <see cref="BasicDataTransferObjectTypeCreator{T}"/>
    /// per closed generic type to avoid repeated type generation and to provide a simple API to
    /// obtain instances.
    /// </summary>
    /// <remarks>
    /// Use <see cref="Dynamic{T}.NewInstance"/> to obtain new instances. The cached type creator
    /// will lazily generate the emitted type when <see cref="Type"/> or <see cref="NewInstance"/>
    /// is first accessed.
    /// </remarks>
    /// <example>
    /// <code>
    /// var x = Dynamic<IMyDto>.NewInstance();
    /// Debug.Assert(x is IMyDto);
    /// </code>
    /// </example>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Intentional generic type-specific cached TypeCreator")]
    public static class Dynamic<T>
        where T : class
    {
        private static readonly BasicDataTransferObjectTypeCreator<T> _typeCreator = new();

        public static Type Type => _typeCreator.Type;

        public static T NewInstance(dynamic source = null) => _typeCreator.CreateInstance(source);
    }

    /// <summary>
    /// Variant of <see cref="Dynamic{T}"/> that allows specifying a custom
    /// <see cref="BasicDataTransferObjectTypeCreator{T}"/> derived type to be used
    /// for generation. This is useful to supply different base types or behaviors.
    /// </summary>
    /// <typeparam name="TTypeCreator">A derived type of <see cref="BasicDataTransferObjectTypeCreator{T}"/> with a public parameterless constructor.</typeparam>
    /// <example>
    /// <code>
    /// var instance = Dynamic<IMyDto, MyCustomTypeCreator>.NewInstance();
    /// </code>
    /// </example>
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
