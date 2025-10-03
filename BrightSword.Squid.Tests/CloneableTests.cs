using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using BrightSword.Crucible;
using BrightSword.Squid.API;
using BrightSword.Squid.TypeCreators;

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.BrightSword.Squid
{
    [TestClass]
    public class CloneableTests
    {
        [ClassCleanup]
        public static void Cleanup()
        {
            Debugger.Break();
        }

        [TestMethod]
        public void Test_ICloneableImplementation()
        {
            CloneableIsSupported(new BasicDataTransferObjectTypeCreator<IFoo>
                                 {
                                     AssemblyName = "Dynamic.Cloneable.IFoo",
                                     SaveAssemblyToDisk = true
                                 });
        }

        [TestMethod]
        public void Test_NonCloneableInterfaceDoesNotHaveCloneMethod()
        {
            dynamic instance = new BasicDataTransferObjectTypeCreator<INonCloneable>().CreateInstance();

            ExceptionHelper.ExpectException<RuntimeBinderException>(() => instance.Clone());
        }

        [TestMethod]
        public void Test_ImplementationSupportsCloneableIfTypeCreatorSpecifiesICloneableFacet()
        {
            var typeCreator = new BasicDataTransferObjectTypeCreator<INonCloneable>
                              {
                                  AssemblyName = "Dynamic.Cloneable.ICloneable",

                                  FacetInterfaces = new[]
                                                    {
                                                        typeof (ICloneable)
                                                    },

                                  SaveAssemblyToDisk = true
                              };

            var root = typeCreator.CreateInstance();
            Assert.IsNotNull(root);
            Assert.IsInstanceOfType(root,
                                    typeof (INonCloneable)); // from the interface definition
            Assert.IsInstanceOfType(root,
                                    typeof (ICloneable)); // from the facet interfaces

            root.FooBar = 99;
            var cloned = (INonCloneable) ((dynamic) root).Clone();

            Assert.IsNotNull(cloned.GetType()
                                   .GetCustomAttribute<SerializableAttribute>());

            Assert.AreEqual(cloned.FooBar,
                            root.FooBar);
        }

        private static void CloneableIsSupported(ITypeCreator<IFoo> typeCreator)
        {
            var root = typeCreator.CreateInstance();
            var child = typeCreator.CreateInstance();

            Assert.IsNotNull(root);
            Assert.IsInstanceOfType(root,
                                    typeof (IFoo));
            Assert.IsInstanceOfType(root,
                                    typeof (ICloneable));

            root.Value = 42;
            child.Value = 21;

            ((List<IFoo>) root.Children).Add(child);

            var cloned = (IFoo) root.Clone();

            Assert.IsNotNull(cloned.GetType()
                                   .GetCustomAttribute<SerializableAttribute>());

            Assert.AreEqual(cloned.Value,
                            root.Value);

            Assert.AreEqual(cloned.Children.Single()
                                  .Value,
                            root.Children.Single()
                                .Value);
        }

        public interface IBar : ICloneable
        {
            int Value { get; set; }
        }

// ReSharper disable once RedundantExtendsListEntry
        public interface IFoo : IBar,
                                ICloneable
        {
            IEnumerable<IFoo> Children { get; }
        }

        public interface INonCloneable
        {
            int FooBar { get; set; }
        }
    }
}