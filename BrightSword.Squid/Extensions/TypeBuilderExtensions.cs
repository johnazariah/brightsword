using System;
using System.Diagnostics;
using System.Reflection.Emit;

namespace BrightSword.Squid
{
    public static class TypeBuilderExtensions
    {
        public static TypeBuilder AddCustomAttribute<TAttribute>(this TypeBuilder _this) where TAttribute : Attribute
        {
            var constructorInfo = typeof (TAttribute).GetConstructor(Type.EmptyTypes);
            Debug.Assert(constructorInfo != null);

            _this.SetCustomAttribute(new CustomAttributeBuilder(constructorInfo,
                                                                new object[0]));
            return _this;
        }

        public static PropertyBuilder AddCustomAttribute<TAttribute>(this PropertyBuilder _this) where TAttribute : Attribute
        {
            var constructorInfo = typeof (TAttribute).GetConstructor(Type.EmptyTypes);
            Debug.Assert(constructorInfo != null);

            _this.SetCustomAttribute(new CustomAttributeBuilder(constructorInfo,
                                                                new object[0]));
            return _this;
        }

        public static MethodBuilder AddCustomAttribute<TAttribute>(this MethodBuilder _this) where TAttribute : Attribute
        {
            var constructorInfo = typeof (TAttribute).GetConstructor(Type.EmptyTypes);
            Debug.Assert(constructorInfo != null);

            _this.SetCustomAttribute(new CustomAttributeBuilder(constructorInfo,
                                                                new object[0]));
            return _this;
        }

        public static EventBuilder AddCustomAttribute<TAttribute>(this EventBuilder _this) where TAttribute : Attribute
        {
            var constructorInfo = typeof (TAttribute).GetConstructor(Type.EmptyTypes);
            Debug.Assert(constructorInfo != null);

            _this.SetCustomAttribute(new CustomAttributeBuilder(constructorInfo,
                                                                new object[0]));
            return _this;
        }

        public static FieldBuilder AddCustomAttribute<TAttribute>(this FieldBuilder _this) where TAttribute : Attribute
        {
            var constructorInfo = typeof (TAttribute).GetConstructor(Type.EmptyTypes);
            Debug.Assert(constructorInfo != null);

            _this.SetCustomAttribute(new CustomAttributeBuilder(constructorInfo,
                                                                new object[0]));
            return _this;
        }
    }
}