using System;
using System.Diagnostics;
using System.Linq;

using BrightSword.Crucible;
using BrightSword.Squid.TypeCreators;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RemoteInterfaceAssembly;

using Tests.BrightSword.Squid.core;

namespace Tests.BrightSword.Squid
{
    /// <summary>
    /// Tests validating that the runtime type creator can generate concrete instances
    /// for a variety of input types (interfaces and classes) and that the generated
    /// objects meet the expected shape and behavior.
    ///
    /// These tests focus on creation semantics (not behavior of individual members)
    /// and assert that generated objects implement required interfaces, expose
    /// properties and methods, and correctly handle inheritance, hiding and generic
    /// member shapes.
    /// </summary>
    [TestClass]
    public class CreateInstanceTests
    {
        [ClassCleanup]
        public static void Cleanup() => Debugger.Break();

        /// <summary>
        /// Creating an instance for a concrete non-primitive class type should
        /// return a real instance of that class.
        /// </summary>
        [TestMethod]
        public void TestNonPrimitiveClassType()
        {
            var actual = new BasicDataTransferObjectTypeCreator<Exception>().CreateInstance();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof(Exception));
        }

        /// <summary>
        /// Ensure generator can create an instance for an interface that declares generic methods.
        /// The test verifies that an object is produced and that it exposes the interface type.
        /// </summary>
        [TestMethod]
        public void TestInterfaceWithGenericMethods()
        {
            var actual = new BasicDataTransferObjectTypeCreator<IInterfaceWithGenericMethods>().CreateInstance();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof(IInterfaceWithGenericMethods));
        }

        /// <summary>
        /// Ensure generator can create an instance for a simple non-generic interface
        /// and that the returned object is assignable to the interface.
        /// </summary>
        [TestMethod]
        public void TestNonGenericInterfaceType()
        {
            var actual = new BasicDataTransferObjectTypeCreator<INonGenericInterfaceWithNonGenericProperties>().CreateInstance();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof(INonGenericInterfaceWithNonGenericProperties));
        }

        /// <summary>
        /// When an interface extends another interface, ensure the generated dynamic
        /// implementation has a base type that is object (dynamic proxy semantics) rather
        /// than attempting to inherit from the interface type itself.
        /// </summary>
        [TestMethod]
        public void TestDynamicIsObject()
        {
            var actual = new BasicDataTransferObjectTypeCreator<IInterfaceExtendingAnother>().CreateInstance();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof(IInterfaceExtendingAnother));

            Assert.AreEqual(actual.GetType()
                                  .BaseType,
                            typeof(object));
        }

        /// <summary>
        /// Validate that get-only properties which are backed by mapped collection types
        /// are initialized correctly and are usable (i.e., adding an item does not throw).
        /// This tests default construction of mapped collection properties.
        /// </summary>
        [TestMethod]
        public void TestGetOnlyMappedPropertyShouldNotThrow()
        {
            var actual = new BasicDataTransferObjectTypeCreator<INonGenericInterfaceWithGenericProperties>().CreateInstance();
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof(INonGenericInterfaceWithGenericProperties));

            Assert.IsNotNull(actual.Things);

            actual.Things.Add(new BasicDataTransferObjectTypeCreator<INonGenericInterfaceWithNonGenericProperties>().CreateInstance());
        }

        /// <summary>
        /// Ensure interface members that hide base members still result in a generated
        /// object that can be assigned to the derived interface type.
        /// </summary>
        [TestMethod]
        public void TestHiddenProperty()
        {
            var actual = new BasicDataTransferObjectTypeCreator<IDerivedWithHiddenProperty>().CreateInstance();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof(IDerivedWithHiddenProperty));
        }

        /// <summary>
        /// Tests that a 'new' hidden property is correctly handled in generated types.
        /// </summary>
        [TestMethod]
        public void TestNewHiddenProperty()
        {
            var actual = new BasicDataTransferObjectTypeCreator<IDerivedWithNewHiddenProperty>().CreateInstance();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof(IDerivedWithNewHiddenProperty));
        }

        /// <summary>
        /// Verify that generated object implements simple property getters/setters for a
        /// non-generic interface and that values round-trip through the generated properties.
        /// </summary>
        [TestMethod]
        public void TestNonGenericInterfaceTypeImplementingAllProperties()
        {
            var actual = new BasicDataTransferObjectTypeCreator<INonGenericInterfaceWithNonGenericProperties>().CreateInstance();
            actual.Name = "I Love Pi";
            actual.Amount = 3.14M;

            Assert.AreEqual(3.14M,
                            actual.Amount);
            Assert.AreEqual("I Love Pi",
                            actual.Name);
        }

        /// <summary>
        /// Verify that generated object implements generic collection properties and
        /// that items can be added and retrieved as expected.
        /// </summary>
        [TestMethod]
        public void TestInterfaceTypeWithGenericPropertiesImplementingAllProperties()
        {
            var actual = new BasicDataTransferObjectTypeCreator<INonGenericInterfaceWithGenericProperties>().CreateInstance();
            actual.Name = "I Love Pi";

            Assert.AreEqual("I Love Pi",
                            actual.Name);

            Assert.IsNotNull(actual.Things);

            var item = new BasicDataTransferObjectTypeCreator<INonGenericInterfaceWithNonGenericProperties>().CreateInstance();

            actual.Things.Add(item);
            Assert.IsNotNull(actual.Things.Single());
        }

        /// <summary>
        /// Helper wrapper used to verify that calling non-implemented methods on a generated
        /// instance throws <see cref="NotImplementedException"/> as expected.
        /// </summary>
        [TestMethod]
        public void TestInterfaceWithAVoidMethodNoArgs() => TestInterfaceWithMethod<IInterfaceWithAVoidMethodNoArgs>(_ => _.VoidMethodNoArgs());

        [TestMethod]
        public void TestInterfaceWithAnIntMethodWithArgs()
        {
            TestInterfaceWithMethod<IInterfaceWithAnIntMethodWithArgs>(_ => _.IntMethodWithArgs(1,
                                                                                                1));
        }

        [TestMethod]
        public void TestInterfaceWithACharMethodWithParamsArg()
        {
            TestInterfaceWithMethod<IInterfaceWithACharMethodWithParamsArg>(_ => _.ParamsMethod(1,
                                                                                                2,
                                                                                                "Hello",
                                                                                                "World"));
        }

        [TestMethod]
        public void TestInterfaceWithAGuidMethodWithRefArg()
        {
            var arg = "Hello World";
            TestInterfaceWithMethod<IInterfaceWithAGuidMethodWithRefArg>(_ => _.MethodWithRefParameters(ref arg));
        }

        [TestMethod]
        public void TestInterfaceWithAnArrayMethodWithOutArg()
        {
            string arg;
            TestInterfaceWithMethod<IInterfaceWithAnArrayMethodWithOutArg>(_ => _.ArrayMethodWithOutParameters(out arg));
        }

        [TestMethod]
        public void TestInterfaceExtendingAnother()
        {
            TestInterfaceWithMethod<IInterfaceExtendingAnother>(_ => _.IntMethodWithArgs(1,
                                                                                         2));

            TestInterfaceWithMethod<IInterfaceExtendingAnother>(_ => _.IntMethodWithArgs(1,
                                                                                         2,
                                                                                         3));
        }

        [TestMethod]
        public void TestInterfaceHidingAMethod()
        {
            TestInterfaceWithMethod<IInterfaceHidingAMethod>(_ => _.IntMethodWithArgs(1,
                                                                                      2));

            TestInterfaceWithMethod<IInterfaceHidingAMethod>(_ => ((IInterfaceExtendingAnother)_).IntMethodWithArgs(1,
                                                                                                                     2));
        }

        [TestMethod]
        public void TestInterfaceWithRemoteTypeProperty()
        {
            var actual = new BasicDataTransferObjectTypeCreator<IInterfaceWithRemoteTypeProperty>().CreateInstance();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof(IInterfaceWithRemoteTypeProperty));
        }

        /// <summary>
        /// Verify that events can be attached to a generated instance without throwing and
        /// that the generated event field supports standard subscription semantics.
        /// </summary>
        [TestMethod]
        public void TestInterfaceWithEvent()
        {
            var instance = new BasicDataTransferObjectTypeCreator<IInterfaceWithEvent>().CreateInstance();
            Assert.IsNotNull(instance);

            Action<IInterfaceWithEvent> action = _ =>
                                                 {
                                                     _.Foo += (sender,
                                                               args) =>
                                                     { };
                                                 };

            action(instance);
        }

        private static void TestInterfaceWithMethod<T>(Action<T> action) where T : class
        {
            var actual = new BasicDataTransferObjectTypeCreator<T>().CreateInstance();
            Assert.IsNotNull(actual);

            Action call = () => action(actual);
            _ = call.ExpectException<NotImplementedException>();
        }

        /// <summary>
        /// Test that multiple inheritance with property collisions where both sides use
        /// the same property type are handled by the generator such that each interface
        /// view has its own independent storage.
        /// </summary>
        [TestMethod]
        public void TestMultipleInheritanceWithCollisionWithSameType()
        {
            var instance = new BasicDataTransferObjectTypeCreator<IInterfaceWithMultipleInheritanceAndPropertyCollisions>().CreateInstance();

            Assert.IsNotNull(instance);

            Assert.IsInstanceOfType(instance,
                                    typeof(IInterfaceLeftBase));
            Assert.IsInstanceOfType(instance,
                                    typeof(IInterfaceRightBase));

            var left = instance as IInterfaceLeftBase;
            left.CollisionWithSameType = 12;
            Assert.AreEqual(12,
                            left.CollisionWithSameType);

            var right = instance as IInterfaceRightBase;
            right.CollisionWithSameType = 25;
            Assert.AreEqual(12,
                            left.CollisionWithSameType);
            Assert.AreEqual(25,
                            right.CollisionWithSameType);
        }

        /// <summary>
        /// Test that multiple inheritance with property collisions where the colliding
        /// properties have different types still keeps the values isolated per-interface.
        /// </summary>
        [TestMethod]
        public void TestMultipleInheritanceWithCollisionWithDifferentType()
        {
            var instance = new BasicDataTransferObjectTypeCreator<IInterfaceWithMultipleInheritanceAndPropertyCollisions>().CreateInstance();

            Assert.IsNotNull(instance);

            Assert.IsInstanceOfType(instance,
                                    typeof(IInterfaceLeftBase));
            Assert.IsInstanceOfType(instance,
                                    typeof(IInterfaceRightBase));

            var left = instance as IInterfaceLeftBase;
            left.CollisionWithDifferentType = 12;
            Assert.AreEqual(12,
                            left.CollisionWithDifferentType);

            var right = instance as IInterfaceRightBase;
            right.CollisionWithDifferentType = "TwentyFive";
            Assert.AreEqual(12,
                            left.CollisionWithDifferentType);
            Assert.AreEqual("TwentyFive",
                            right.CollisionWithDifferentType);
        }

        /// <summary>
        /// Ensures that facet interfaces (additional interfaces injected into the generated type)
        /// are correctly added to the generated type and that it remains assignable to the primary
        /// interface under test.
        /// </summary>
        [TestMethod]
        public void TestFacetsAreInjectedProperly()
        {
            var actual = new BasicDataTransferObjectTypeCreator<INonGenericInterfaceWithNonGenericProperties>().CreateInstance();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof(INonGenericInterfaceWithNonGenericProperties));
        }
    }
}
