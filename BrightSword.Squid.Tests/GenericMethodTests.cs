using System.Reflection;

using BrightSword.Squid;
using BrightSword.Squid.Tests.core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BrightSword.Squid.Tests
{
    [TestClass]
    public class GenericMethodTests
    {
        private static void TestConstraints<T>(string methodName,
                                               GenericParameterAttributes expectedAttributes,
                                               Type expectedBaseClass = null,
                                               params Type[] expectedImplementedInterfaces) where T : class
        {
            var instanceType = Dynamic<T, PropertyChangingNotificationSinkTypeCreator<T>>.Type;
            var methodInfo = instanceType.GetMethod(methodName);

            Assert.IsNotNull(methodInfo);
            Assert.IsTrue(methodInfo.IsGenericMethod);

            var genericArgument = methodInfo.GetGenericArguments()
                                            .SingleOrDefault();
            Assert.IsNotNull(genericArgument);

            var genericParameterAttributes = genericArgument.GenericParameterAttributes;

            Assert.AreEqual(expectedAttributes,
                            genericParameterAttributes);

            var actualConstraints = genericArgument.GetGenericParameterConstraints();

            if (expectedBaseClass != null)
            {
                var actualBaseClass = actualConstraints.SingleOrDefault(_ => _.IsClass);

                Assert.IsNotNull(actualBaseClass);
            }

            CollectionAssert.AreEqual(expectedImplementedInterfaces,
                                      actualConstraints.Where(_ => _.IsInterface)
                                                       .ToList());
        }

        [TestMethod]
        public void TestNoConstraints()
        {
            TestConstraints<IInterfaceWithGenericMethods>("GenericParameter",
                                                          GenericParameterAttributes.None);
        }

        [TestMethod]
        public void TestGenericParameterConstrainedToReference()
        {
            TestConstraints<IInterfaceWithGenericMethods>("GenericParameterConstrainedToReference",
                                                          GenericParameterAttributes.ReferenceTypeConstraint);
        }

        [TestMethod]
        public void TestGenericParameterConstrainedToReferenceAndNew()
        {
            TestConstraints<IInterfaceWithGenericMethods>("GenericParameterConstrainedToReferenceAndNew",
                                                          GenericParameterAttributes.ReferenceTypeConstraint | GenericParameterAttributes.DefaultConstructorConstraint);
        }

        [TestMethod]
        public void GenericParameterConstrainedToReferenceAndNewAndInterface()
        {
            TestConstraints<IInterfaceWithGenericMethods>("GenericParameterConstrainedToReferenceAndNewAndInterface",
                                                          GenericParameterAttributes.ReferenceTypeConstraint | GenericParameterAttributes.DefaultConstructorConstraint,
                                                          null,
                                                          typeof(IBase));
        }

        [TestMethod]
        public void TestGenericParameterConstrainedToNewAndBaseClass()
        {
            TestConstraints<IInterfaceWithGenericMethods>("GenericParameterConstrainedToNewAndBaseClass",
                                                          GenericParameterAttributes.DefaultConstructorConstraint,
                                                          typeof(Exception));
        }

        [TestMethod]
        public void TestGenericParameterConstrainedToNewAndBaseClassAndInterface()
        {
            TestConstraints<IInterfaceWithGenericMethods>("GenericParameterConstrainedToNewAndBaseClassAndInterface",
                                                          GenericParameterAttributes.DefaultConstructorConstraint,
                                                          typeof(Random),
                                                          typeof(IInterfaceWithEvent));
        }

        [TestMethod]
        public void TestGenericParameterConstrainedToBaseClass()
        {
            TestConstraints<IInterfaceWithGenericMethods>("GenericParameterConstrainedToBaseClass",
                                                          GenericParameterAttributes.None,
                                                          typeof(Random));
        }

        [TestMethod]
        public void TestGenericParameterConstrainedToBaseClassAndTwoInterfaces()
        {
            TestConstraints<IInterfaceWithGenericMethods>("GenericParameterConstrainedToBaseClassAndTwoInterfaces",
                                                          GenericParameterAttributes.None,
                                                          typeof(Random),
                                                          typeof(IBase),
                                                          typeof(IInterfaceWithEvent));
        }
    }
}
