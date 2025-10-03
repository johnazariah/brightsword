using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using BrightSword.Squid;
using BrightSword.Squid.TypeCreators;
using BrightSword.SwissKnife;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Tests.BrightSword.Squid.core;

namespace Tests.BrightSword.Squid
{
    [TestClass]
    public class AddCustomAttributeTests
    {
        [TestMethod]
    public void TestAttributesOnTypeAreSet()
        {
            var type = new AttributeDecorator<IComprehensive>().Type;
            Assert.IsNotNull(type.GetCustomAttribute<TestAttribute>());
        }

        [TestMethod]
    public void TestAttributesOnPropertiesAreSet()
        {
            var type = new AttributeDecorator<IComprehensive>().Type;
            foreach (var member in type.GetAllProperties())
            {
                Assert.IsNotNull(member.GetCustomAttribute<TestAttribute>(),
                                 member.Name);
            }
        }

        [TestMethod]
    public void TestAttributesOnMethodsAreSet()
        {
            var type = new AttributeDecorator<IComprehensive>().Type;
            foreach (var member in type.GetAllMethods()
                                       .Where(_ => !(_.IsSpecialName || _.IsHideBySig)))
            {
                Assert.IsNotNull(member.GetCustomAttribute<TestAttribute>(),
                                 member.Name);
            }
        }

        [TestMethod]
    public void TestAttributesOnEventsAreSet()
        {
            var type = new AttributeDecorator<IComprehensive>().Type;
            foreach (var member in type.GetAllEvents())
            {
                Assert.IsNotNull(member.GetCustomAttribute<TestAttribute>(),
                                 member.Name);
            }
        }

    private sealed class AttributeDecorator<T> : BasicDataTransferObjectTypeCreator<T>
            where T : class
        {
            protected override IEnumerable<Func<TypeBuilder, TypeBuilder>> CustomClassOperations
            {
                get { yield return _ => _.AddCustomAttribute<TestAttribute>(); }
            }

            protected override IEnumerable<Func<PropertyBuilder, PropertyInfo, PropertyBuilder>> PropertyOperations
            {
                get
                {
                    yield return (_propertyBuilder,
                                  _propertyInfo) => _propertyBuilder.AddCustomAttribute<TestAttribute>();
                }
            }

            protected override IEnumerable<Func<MethodBuilder, MethodInfo, MethodBuilder>> MethodOperations
            {
                get
                {
                    yield return (_methodBuilder,
                                  _methodInfo) => _methodBuilder.AddCustomAttribute<TestAttribute>();
                }
            }

            protected override IEnumerable<Func<EventBuilder, EventInfo, EventBuilder>> EventOperations
            {
                get
                {
                    yield return (_eventBuilder,
                                  _eventInfo) => _eventBuilder.AddCustomAttribute<TestAttribute>();
                }
            }

            protected override IEnumerable<Func<FieldBuilder, FieldInfo, FieldBuilder>> EventFieldOperations
            {
                get
                {
                    yield return (_fieldBuilder,
                                  _fieldInfo) => _fieldBuilder.AddCustomAttribute<TestAttribute>();
                }
            }
        }

    [AttributeUsage(AttributeTargets.All)]
    public class TestAttribute : Attribute {}
    }
}