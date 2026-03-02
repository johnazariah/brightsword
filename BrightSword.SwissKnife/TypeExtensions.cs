namespace BrightSword.SwissKnife
{
    /// <summary>
    /// Provides extension methods for <see cref="Type"/> to simplify reflection and type-name helpers.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Friendly printable name similar to the original project's intent.
        /// </summary>
        /// <param name="this">The <see cref="Type"/> to get the printable name for.</param>
        public static string PrintableName(this Type @this)
        {
            ArgumentNullException.ThrowIfNull(@this);

            if (!@this.IsGenericType)
            {
                return @this.Name;
            }

            var genericTypeDefinition = @this.GetGenericTypeDefinition();
            var baseName = genericTypeDefinition.Name;
            var tickIndex = baseName.IndexOf('`');
            if (tickIndex > 0)
            {
                baseName = baseName[..tickIndex];
            }

            var args = string.Join(", ", @this.GetGenericArguments().Select(a => a.PrintableName()));
            return $"{baseName}<{args}>";
        }

        /// <summary>
        /// Printable name with optional prefix and suffix wrapping.
        /// </summary>
        public static string PrintableName(this Type @this, string prefix, string suffix)
        {
            ArgumentNullException.ThrowIfNull(@this);

            if (!@this.IsGenericType)
            {
                return $"{prefix}{@this.Name}{suffix}";
            }

            var genericTypeDefinition = @this.GetGenericTypeDefinition();
            var baseName = genericTypeDefinition.Name;
            var tickIndex = baseName.IndexOf('`');
            if (tickIndex > 0)
            {
                baseName = baseName[..tickIndex];
            }

            var args = string.Join(", ", @this.GetGenericArguments().Select(a => a.PrintableName()));
            return $"{prefix}{baseName}<{args}>{suffix}";
        }

        /// <summary>
        /// Printable name with optional prefix, suffix, and a custom name transformer for generic arguments.
        /// </summary>
        public static string PrintableName(this Type @this, string prefix, string suffix, Func<Type, string> nameTransformer)
        {
            ArgumentNullException.ThrowIfNull(@this);

            if (!@this.IsGenericType)
            {
                return $"{prefix}{@this.Name}{suffix}";
            }

            var genericTypeDefinition = @this.GetGenericTypeDefinition();
            var baseName = genericTypeDefinition.Name;
            var tickIndex = baseName.IndexOf('`');
            if (tickIndex > 0)
            {
                baseName = baseName[..tickIndex];
            }

            var args = string.Join(", ", @this.GetGenericArguments().Select(nameTransformer));
            return $"{prefix}{baseName}<{args}>{suffix}";
        }

        /// <summary>
        /// Backwards-compatible Name() extension used by tests and older code.
        /// </summary>
        /// <param name="this">The <see cref="Type"/> to get the name for.</param>
        public static string Name(this Type @this) => PrintableName(@this);

        /// <summary>
        /// Heuristic used by the Squid project to convert interface type names to a concrete class-like name.
        /// Example: IMyInterface -> MyInterface; IList&lt;T&gt; -> List&lt;T&gt;.
        /// </summary>
        /// <param name="this">The <see cref="Type"/> to rename.</param>
        public static string RenameToConcreteType(this Type @this)
        {
            ArgumentNullException.ThrowIfNull(@this);

            // If it's an interface and starts with 'I' followed by uppercase letter, trim the leading 'I'.
            var name = @this.IsGenericType ? @this.PrintableName() : @this.Name;
            return @this.IsInterface && name.Length >= 2 && name[0] == 'I' && char.IsUpper(name[1]) ? name[1..] : name;
        }
    }
}
