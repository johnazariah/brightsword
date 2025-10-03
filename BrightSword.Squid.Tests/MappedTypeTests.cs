using System;
using System.Collections.Generic;

using BrightSword.Squid.TypeCreators;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RemoteInterfaceAssembly;

using RemotePropertyInterfaceAssembly;

using Tests.BrightSword.Squid.core;

using IFoo = Tests.BrightSword.Squid.core.IFoo;

namespace Tests.BrightSword.Squid
{
    [TestClass]
    public class MappedTypeTests
    {
        [TestMethod]
    public void TestInterfaceWithRemoteTypeProperty()
        {
            var actual = MappedTypeDynamic<IInterfaceWithMappedRemoteTypeReadonlyProperty>.NewInstance();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof (IInterfaceWithMappedRemoteTypeReadonlyProperty));
        }

        [TestMethod]
    public void TestDefaultMappedReadonlyPropertiesWork()
        {
            var instance = new BasicDataTransferObjectTypeCreator<IComprehensive>().CreateInstance();

            Assert.AreEqual(typeof (List<int>),
                            instance.IntegerList.GetType());
            Assert.AreEqual(typeof (HashSet<decimal>),
                            instance.DecimalSet.GetType());
            Assert.AreEqual(typeof (Dictionary<int, string>),
                            instance.StringDictionary.GetType());
        }

        [TestMethod]
    public void TestInlineMappedReadonlyPropertiesWork()
        {
            var instance = new BasicDataTransferObjectTypeCreator<IComprehensive>((_ => typeof (IFoo).IsAssignableFrom(_)
                                                                                            ? typeof (Foo)
                                                                                            : null)).CreateInstance();

            Assert.AreEqual(typeof (Foo),
                            instance.Foo.GetType());
        }

    internal sealed class LocalBar : IBar
        {
            private const decimal C_VALUE = 25;

            public decimal Value
            {
                get { return C_VALUE; }
            }
        }

        private static class MappedTypeDynamic<T>
            where T : class
        {
            private static readonly BasicDataTransferObjectTypeCreator<T> _simpleTypeDynamicInfo = new BasicDataTransferObjectTypeCreator<T>(MapIFooToFoo,
                                                                                                                                             MapIBarToLocalBar);

            public static T NewInstance()
            {
                return _simpleTypeDynamicInfo.CreateInstance();
            }

            private static Type MapIFooToFoo(Type arg)
            {
                return typeof (IFoo).IsAssignableFrom(arg)
                           ? typeof (Foo)
                           : null;
            }

            private static Type MapIBarToLocalBar(Type arg)
            {
                return typeof (IBar).IsAssignableFrom(arg)
                           ? typeof (LocalBar)
                           : null;
            }
        }
    }
}