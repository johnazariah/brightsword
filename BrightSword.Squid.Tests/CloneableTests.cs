using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using BrightSword.Crucible;
using BrightSword.Squid.TypeCreators;

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.BrightSword.Squid
{
    /// <summary>
    /// Tests that the clone behaviour is composed correctly into generated types
    /// when <see cref="ICloneable"/> is present as a facet or when the primary
    /// interface extends <see cref="ICloneable"/>.
    ///
    /// The tests validate that the emitted type implements <see cref="ICloneable"/>,
    /// that the runtime Clone method exists when expected, and that the returned clone
    /// is a deep copy (children and value equality) and is annotated with <see cref="SerializableAttribute"/>,    /// matching legacy expectations for serialization compatibility.
    /// </summary>
    [TestClass]
    public class CloneableTests
    {
        [ClassCleanup]
        public static void Cleanup() => Debugger.Break();

        /// <summary>
        /// If the interface hierarchy includes <see cref="ICloneable"/>, the generated
        /// type should expose a working Clone method that returns a deep clone.
        /// </summary>
        [TestMethod]
        public void TestICloneableImplementation()
        {
            CloneableIsSupported(new BasicDataTransferObjectTypeCreator<IFoo>
            {
                AssemblyName = "Dynamic.Cloneable.IFoo",
                SaveAssemblyToDisk = true
            });
        }

        /// <summary>
        /// When the primary interface does not include <see cref="ICloneable"/>, there
        /// should be no Clone method available on the dynamic object (dynamic binding should fail).
        /// </summary>
        [TestMethod]
        public void TestNonCloneableInterfaceDoesNotHaveCloneMethod()
        {
            dynamic instance = new BasicDataTransferObjectTypeCreator<INonCloneable>().CreateInstance();

            _ = ExceptionHelper.ExpectException<RuntimeBinderException>(() => instance.Clone());
        }

        /// <summary>
        /// If the type creator is configured to inject <see cref="ICloneable"/> as a facet
        /// into the generated type, the resulting instance should present both the primary
        /// interface and the cloneable facet and the clone should preserve values.
        /// </summary>
        [TestMethod]
        public void TestImplementationSupportsCloneableIfTypeCreatorSpecifiesICloneableFacet()
        {
            var typeCreator = new BasicDataTransferObjectTypeCreator<INonCloneable>
            {
                AssemblyName = "Dynamic.Cloneable.ICloneable",

                FacetInterfaces =
                    [
                        typeof (ICloneable)
                    ],

                SaveAssemblyToDisk = true
            };

            var root = typeCreator.CreateInstance();
            Assert.IsNotNull(root);
            Assert.IsInstanceOfType(root, typeof(INonCloneable)); // from the interface definition
            Assert.IsInstanceOfType(root, typeof(ICloneable)); // from the facet interfaces

            root.FooBar = 99;
            var cloned = (INonCloneable)((dynamic)root).Clone();

            Assert.IsNotNull(cloned.GetType().GetCustomAttribute<SerializableAttribute>());

            Assert.AreEqual(cloned.FooBar,
                            root.FooBar);
        }

        private static void CloneableIsSupported(BasicDataTransferObjectTypeCreator<IFoo> typeCreator)
        {
            var root = typeCreator.CreateInstance();
            var child = typeCreator.CreateInstance();

            Assert.IsNotNull(root);
            Assert.IsInstanceOfType(root,
                                    typeof(IFoo));
            Assert.IsInstanceOfType(root,
                                    typeof(ICloneable));

            root.Value = 42;
            child.Value = 21;

            ((List<IFoo>)root.Children).Add(child);

            var cloned = (IFoo)root.Clone();

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
