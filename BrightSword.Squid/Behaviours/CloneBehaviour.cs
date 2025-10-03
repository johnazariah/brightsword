using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace BrightSword.Squid.Behaviours
{
    public class CloneBehaviour : IBehaviour
    {
        public virtual IEnumerable<Func<TypeBuilder, TypeBuilder>> Operations
        {
            get
            {
                yield return _ => _.AddCustomAttribute<SerializableAttribute>();
                yield return AddCloneMethod;
            }
        }

        public virtual TypeBuilder AddCloneMethod(TypeBuilder typeBuilder)
        {
            // Emit a simple clone method that calls back into a static helper which uses System.Text.Json
            // for deep cloning. This avoids relying on BinaryFormatter which is unavailable on recent runtimes.
            var method = typeBuilder.DefineMethod("Clone",
                                                  MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot,
                                                  typeof(object),
                                                  Type.EmptyTypes);

            var gen = method.GetILGenerator();

            // Load 'this' onto the stack and call the static fallback helper
            var helper = typeof(CloneBehaviour).GetMethod(nameof(FallbackClone), BindingFlags.Public | BindingFlags.Static);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, helper);
            gen.Emit(OpCodes.Ret);

            return typeBuilder;
        }

        // Public helper used by emitted Clone methods when BinaryFormatter is not available.
        // Performs a reflection-based deep clone handling arrays, lists and dictionaries.
        public static object FallbackClone(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var visited = new Dictionary<object, object>(new ReferenceEqualityComparer());
            return InternalClone(obj, visited);
        }

        private static object InternalClone(object obj, Dictionary<object, object> visited)
        {
            if (obj == null) return null;

            var type = obj.GetType();

            // Immutable or primitive types
            if (type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal))
            {
                return obj;
            }

            if (visited.TryGetValue(obj, out var existing))
            {
                return existing;
            }

            // Arrays
            if (type.IsArray)
            {
                var arr = (Array)obj;
                var elementType = type.GetElementType() ?? typeof(object);
                var clone = Array.CreateInstance(elementType, arr.Length);
                visited[obj] = clone;
                for (int i = 0; i < arr.Length; i++)
                {
                    clone.SetValue(InternalClone(arr.GetValue(i), visited), i);
                }

                return clone;
            }

            // IDictionary
            if (typeof(System.Collections.IDictionary).IsAssignableFrom(type))
            {
                var dict = (System.Collections.IDictionary)obj;
                object clone;
                try
                {
                    clone = Activator.CreateInstance(type) ?? new System.Collections.Hashtable();
                }
                catch
                {
                    clone = new System.Collections.Hashtable();
                }

                visited[obj] = clone;
                var cloneDict = (System.Collections.IDictionary)clone;
                foreach (var key in dict.Keys)
                {
                    var k = InternalClone(key, visited);
                    var v = InternalClone(dict[key], visited);
                    cloneDict[k] = v;
                }

                return cloneDict;
            }

            // IEnumerable (IList)
            if (typeof(System.Collections.IList).IsAssignableFrom(type))
            {
                var list = (System.Collections.IList)obj;
                object clone;
                try
                {
                    clone = Activator.CreateInstance(type) ?? new System.Collections.ArrayList();
                }
                catch
                {
                    clone = new System.Collections.ArrayList();
                }

                visited[obj] = clone;
                var cloneList = (System.Collections.IList)clone;
                foreach (var item in list)
                {
                    cloneList.Add(InternalClone(item, visited));
                }

                return cloneList;
            }

            // Fallback: create instance and copy fields
            object result = null;
            // First try a normal public constructor
            try
            {
                result = Activator.CreateInstance(type);
            }
            catch
            {
                // If that fails, try to invoke a non-public constructor (safer than uninitialized objects)
                try
                {
                    result = Activator.CreateInstance(type, nonPublic: true);
                }
                catch
                {
                    // As a last resort, create an uninitialized object. FormatterServices is marked obsolete for
                    // formatter-based serialization scenarios, but GetUninitializedObject is still the pragmatic
                    // fallback for types with no accessible constructors. Suppress the obsoletion warning locally.
                    #pragma warning disable SYSLIB0050
                    result = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
                    #pragma warning restore SYSLIB0050
                }
            }

            if (result == null)
            {
                throw new InvalidOperationException($"Unable to create instance of type {type.FullName} for cloning.");
            }

            visited[obj] = result;

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                var val = field.GetValue(obj);
                field.SetValue(result, InternalClone(val, visited));
            }

            return result;
        }

        private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            public new bool Equals(object x, object y) => ReferenceEquals(x, y);
            public int GetHashCode(object obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
    }
}