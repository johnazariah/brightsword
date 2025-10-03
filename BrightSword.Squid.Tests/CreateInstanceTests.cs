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
    [TestClass]
    public class CreateInstanceTests
    {
        [ClassCleanup]
        public static void Cleanup()
        {
            Debugger.Break();
        }

        [TestMethod]
    public void TestNonPrimitiveClassType()
        {
            var actual = new BasicDataTransferObjectTypeCreator<Exception>().CreateInstance();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof (Exception));
        }

        [TestMethod]
    public void TestInterfaceWithGenericMethods()
        {
            var actual = new BasicDataTransferObjectTypeCreator<IInterfaceWithGenericMethods>().CreateInstance();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof (IInterfaceWithGenericMethods));
        }

        [TestMethod]
    public void TestNonGenericInterfaceType()
        {
            var actual = new BasicDataTransferObjectTypeCreator<INonGenericInterfaceWithNonGenericProperties>().CreateInstance();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof (INonGenericInterfaceWithNonGenericProperties));
        }

        [TestMethod]
    public void TestDynamicIsObject()
        {
            var actual = new BasicDataTransferObjectTypeCreator<IInterfaceExtendingAnother>().CreateInstance();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof (IInterfaceExtendingAnother));

            Assert.AreEqual(actual.GetType()
                                  .BaseType,
                            typeof (Object));
        }

        [TestMethod]
    public void TestGetOnlyMappedPropertyShouldNotThrow()
        {
            var actual = new BasicDataTransferObjectTypeCreator<INonGenericInterfaceWithGenericProperties>().CreateInstance();
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof (INonGenericInterfaceWithGenericProperties));

            Assert.IsNotNull(actual.Things);

            actual.Things.Add(new BasicDataTransferObjectTypeCreator<INonGenericInterfaceWithNonGenericProperties>().CreateInstance());
        }

        [TestMethod]
    public void TestHiddenProperty()
        {
            var actual = new BasicDataTransferObjectTypeCreator<IDerivedWithHiddenProperty>().CreateInstance();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof (IDerivedWithHiddenProperty));
        }

        [TestMethod]
    public void TestNewHiddenProperty()
        {
            var actual = new BasicDataTransferObjectTypeCreator<IDerivedWithNewHiddenProperty>().CreateInstance();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof (IDerivedWithNewHiddenProperty));
        }

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

        [TestMethod]
    public void TestInterfaceWithAVoidMethodNoArgs()
        {
            TestInterfaceWithMethod<IInterfaceWithAVoidMethodNoArgs>(_ => _.VoidMethodNoArgs());
        }

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

            TestInterfaceWithMethod<IInterfaceHidingAMethod>(_ => ((IInterfaceExtendingAnother) _).IntMethodWithArgs(1,
                                                                                                                     2));
        }

        [TestMethod]
    public void TestInterfaceWithRemoteTypeProperty()
        {
            var actual = new BasicDataTransferObjectTypeCreator<IInterfaceWithRemoteTypeProperty>().CreateInstance();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof (IInterfaceWithRemoteTypeProperty));
        }

        [TestMethod]
    public void TestInterfaceWithEvent()
        {
            var instance = new BasicDataTransferObjectTypeCreator<IInterfaceWithEvent>().CreateInstance();
            Assert.IsNotNull(instance);

            Action<IInterfaceWithEvent> action = _ =>
                                                 {
                                                     _.Foo += (sender,
                                                               args) => { };
                                                 };

            action(instance);
        }

        private static void TestInterfaceWithMethod<T>(Action<T> action) where T : class
        {
            var actual = new BasicDataTransferObjectTypeCreator<T>().CreateInstance();
            Assert.IsNotNull(actual);

            Action call = () => action(actual);
            call.ExpectException<NotImplementedException>();
        }

        [TestMethod]
    public void TestMultipleInheritanceWithCollisionWithSameType()
        {
            var instance = new BasicDataTransferObjectTypeCreator<IInterfaceWithMultipleInheritanceAndPropertyCollisions>().CreateInstance();

            Assert.IsNotNull(instance);

            Assert.IsInstanceOfType(instance,
                                    typeof (IInterfaceLeftBase));
            Assert.IsInstanceOfType(instance,
                                    typeof (IInterfaceRightBase));

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

        [TestMethod]
    public void TestMultipleInheritanceWithCollisionWithDifferentType()
        {
            var instance = new BasicDataTransferObjectTypeCreator<IInterfaceWithMultipleInheritanceAndPropertyCollisions>().CreateInstance();

            Assert.IsNotNull(instance);

            Assert.IsInstanceOfType(instance,
                                    typeof (IInterfaceLeftBase));
            Assert.IsInstanceOfType(instance,
                                    typeof (IInterfaceRightBase));

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

        [TestMethod]
    public void TestFacetsAreInjectedProperly()
        {
            var actual = new BasicDataTransferObjectTypeCreator<INonGenericInterfaceWithNonGenericProperties>().CreateInstance();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof (INonGenericInterfaceWithNonGenericProperties));
        }
    }
}