using System;

namespace BrightSword.Squid.API
{
    /// <summary>
    /// Contract for a factory type that can create instances of a generated or concrete type.
    /// Implementations typically emit or cache runtime-generated types for interface-backed DTOs.
    /// </summary>
    /// <typeparam name="T">The public type produced by the creator. Must be a reference type.</typeparam>
    public interface ITypeCreator<out T>
        where T : class
    {
        /// <summary>
        /// Gets the runtime <see cref="Type"/> produced by this creator. For emitted types this
        /// will trigger type generation on first access.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Creates an instance of the type produced by this creator. Optionally maps values from a
        /// dynamic source object into the newly created instance.
        /// </summary>
        /// <param name="source">An optional dynamic object providing values to map into the created instance.</param>
        /// <returns>A new instance of <typeparamref name="T"/>.</returns>
        T CreateInstance(dynamic source = null);
    }
}
