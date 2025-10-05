using System.Reflection.Emit;

namespace BrightSword.Squid.Behaviours
{
    /// <summary>
    /// Represents a behaviour that can be composed into emitted types. Behaviours expose operations
    /// that take a <see cref="TypeBuilder"/> and return a modified <see cref="TypeBuilder"/>.
    /// </summary>
    public interface IBehaviour
    {
        /// <summary>
        /// A sequence of operations that will be applied to a <see cref="TypeBuilder"/> when building a type.
        /// Each operation receives the current <see cref="TypeBuilder"/> and must return the modified builder.
        /// </summary>
        IEnumerable<Func<TypeBuilder, TypeBuilder>> Operations { get; }
    }
}
