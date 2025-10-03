using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace BrightSword.Squid
{
    public static class TypeBuilderExtensions
    {
    public static TypeBuilder AddCustomAttribute<TAttribute>(this TypeBuilder self) where TAttribute : Attribute
        {
            var constructorInfo = typeof (TAttribute).GetConstructor(Type.EmptyTypes);
            Debug.Assert(constructorInfo != null);

            self.SetCustomAttribute(new CustomAttributeBuilder(constructorInfo,
                                                                Array.Empty<object>()));
            return self;
        }

    public static PropertyBuilder AddCustomAttribute<TAttribute>(this PropertyBuilder self) where TAttribute : Attribute
        {
            var constructorInfo = typeof (TAttribute).GetConstructor(Type.EmptyTypes);
            Debug.Assert(constructorInfo != null);

            self.SetCustomAttribute(new CustomAttributeBuilder(constructorInfo,
                                                                Array.Empty<object>()));
            return self;
        }

    public static MethodBuilder AddCustomAttribute<TAttribute>(this MethodBuilder self) where TAttribute : Attribute
        {
            var constructorInfo = typeof (TAttribute).GetConstructor(Type.EmptyTypes);
            Debug.Assert(constructorInfo != null);

            self.SetCustomAttribute(new CustomAttributeBuilder(constructorInfo,
                                                                Array.Empty<object>()));
            return self;
        }

    public static EventBuilder AddCustomAttribute<TAttribute>(this EventBuilder self) where TAttribute : Attribute
        {
            var constructorInfo = typeof (TAttribute).GetConstructor(Type.EmptyTypes);
            Debug.Assert(constructorInfo != null);

            self.SetCustomAttribute(new CustomAttributeBuilder(constructorInfo,
                                                                Array.Empty<object>()));
            return self;
        }

    public static FieldBuilder AddCustomAttribute<TAttribute>(this FieldBuilder self) where TAttribute : Attribute
        {
            var constructorInfo = typeof (TAttribute).GetConstructor(Type.EmptyTypes);
            Debug.Assert(constructorInfo != null);

            self.SetCustomAttribute(new CustomAttributeBuilder(constructorInfo,
                                                                Array.Empty<object>()));
            return self;
        }
    }
}