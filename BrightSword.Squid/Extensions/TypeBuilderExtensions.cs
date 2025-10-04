using System;
using System.Diagnostics;
using System.Reflection.Emit;

namespace BrightSword.Squid
{
    public static class TypeBuilderExtensions
    {
        private static CustomAttributeBuilder CreateEmptyCustomAttributeBuilder<TAttribute>() where TAttribute : Attribute
        {
            var constructorInfo = typeof(TAttribute).GetConstructor(Type.EmptyTypes);
            Debug.Assert(constructorInfo != null);
            return new CustomAttributeBuilder(constructorInfo, []);
        }

        public static TypeBuilder AddCustomAttribute<TAttribute>(this TypeBuilder self) where TAttribute : Attribute
        {
            ArgumentNullException.ThrowIfNull(self);
            self.SetCustomAttribute(CreateEmptyCustomAttributeBuilder<TAttribute>());
            return self;
        }

        public static PropertyBuilder AddCustomAttribute<TAttribute>(this PropertyBuilder self) where TAttribute : Attribute
        {
            ArgumentNullException.ThrowIfNull(self);
            self.SetCustomAttribute(CreateEmptyCustomAttributeBuilder<TAttribute>());
            return self;
        }

        public static MethodBuilder AddCustomAttribute<TAttribute>(this MethodBuilder self) where TAttribute : Attribute
        {
            ArgumentNullException.ThrowIfNull(self);
            self.SetCustomAttribute(CreateEmptyCustomAttributeBuilder<TAttribute>());
            return self;
        }

        public static EventBuilder AddCustomAttribute<TAttribute>(this EventBuilder self) where TAttribute : Attribute
        {
            ArgumentNullException.ThrowIfNull(self);
            self.SetCustomAttribute(CreateEmptyCustomAttributeBuilder<TAttribute>());
            return self;
        }

        public static FieldBuilder AddCustomAttribute<TAttribute>(this FieldBuilder self) where TAttribute : Attribute
        {
            ArgumentNullException.ThrowIfNull(self);
            self.SetCustomAttribute(CreateEmptyCustomAttributeBuilder<TAttribute>());
            return self;
        }
    }
}
