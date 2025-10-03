using System;
using System.Linq;

using BrightSword.Squid;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.BrightSword.Squid
{
    [TestClass]
    public class AnonymousInterfaceImplementationTests
    {
        [TestMethod]
        public void Test_DynamicInstanceIsInitializedWithDynamicSuppliedValue()
        {
            var source = new
                         {
                             Name = "Test",
                             Age = 100,
                             Junk = false,
                         };

            var instance = Dynamic<IFoo>.NewInstance(source);
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance,
                                    typeof (IFoo));
            Assert.AreEqual(source.Name,
                            instance.Name);
            Assert.AreEqual(source.Age,
                            instance.Age);
        }

        [TestMethod]
        public void Test_DynamicInstanceIsInitializedWithStaticSuppliedValue()
        {
            var source = new Foo
                         {
                             Name = "Test",
                             Age = 100,
                         };

            var instance = Dynamic<IFoo>.NewInstance(source);
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance,
                                    typeof (IFoo));
            Assert.AreEqual(source.Name,
                            instance.Name);
            Assert.AreEqual(source.Age,
                            instance.Age);
        }

        [TestMethod]
        public void Test_DynamicInstanceCanBeCalledFromLinq()
        {
            var instances = from i in Enumerable.Range(0,
                                                       100)
                            select Dynamic<IFoo>.NewInstance(new
                                                             {
                                                                 Name = String.Format("Person{0}",
                                                                                      i),
                                                                 Age = i
                                                             });

            Assert.IsTrue(instances.Select((_item,
                                            _index) =>
                                           {
                                               Assert.IsNotNull(_item);
                                               Assert.IsInstanceOfType(_item,
                                                                       typeof (IFoo));
                                               Assert.AreEqual(String.Format("Person{0}",
                                                                             _index),
                                                               _item.Name);
                                               Assert.AreEqual(_index,
                                                               _item.Age);
                                               return true;
                                           })
                                   .All(_ => _));
        }

        private class Foo : IFoo
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        public interface IFoo
        {
            string Name { get; set; }
            int Age { get; set; }
        }
    }
}