using System;
using System.Diagnostics;

using BrightSword.Squid;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using Tests.BrightSword.Squid.core;

namespace Tests.BrightSword.Squid
{
    [TestClass]
    public class TimingTests
    {
        [TestMethod]
        public void Given_AnInterfaceType_Then_CreationOfObjectsShouldBeFasterWithSquid()
        {
            const int count = 1000*1000;

// ReSharper disable UnusedVariable
            var foo = Dynamic<INonGenericInterfaceWithGenericProperties, PropertyChangingNotificationSinkTypeCreator<INonGenericInterfaceWithGenericProperties>>.NewInstance();
            var bar = Dynamic<INonGenericInterfaceWithNonGenericProperties, PropertyChangingNotificationSinkTypeCreator<INonGenericInterfaceWithNonGenericProperties>>.NewInstance();
// ReSharper restore UnusedVariable

            var sw = new Stopwatch();

            //Trace.WriteLine("Rhino.GenerateStub<T> implementation takes 20+ s for 1000x1000 objects");

            Trace.WriteLine(String.Format("Lambda: {0} items created in {1} ms ",
                                          count,
                                          GetTimeForLambda(sw,
                                                           count)));

            Trace.WriteLine(String.Format("New: {0} items created in {1} ms ",
                                          count,
                                          GetTimeForNew(sw,
                                                        count)));

            Trace.WriteLine(String.Format("Squid: {0} items created in {1} ms ",
                                          count,
                                          GetTimeForSquid(sw,
                                                          count)));

            Trace.WriteLine(String.Format("Activator: {0} items created in {1} ms ",
                                          count,
                                          GetTimeForActivator(sw,
                                                              count)));

            Trace.WriteLine(String.Format("Default CTOR: {0} items created in {1} ms ",
                                          count,
                                          GetTimeForDefaultCtorInvoke(sw,
                                                                      count)));

            Trace.WriteLine(String.Format("Rhino: {0} items created in {1} ms ",
                                          count,
                                          GetTimeForRhino(sw,
                                                          count)));
        }

        private static long GetTimeForRhino(Stopwatch sw,
                                            int count)
        {
            sw.Reset();
            sw.Start();
            for (var i = 0;
                 i < count;
                 i++)
            {
                var _obj = new Mock<INonGenericInterfaceWithGenericProperties>().Object;
            }

            sw.Stop();

            return sw.ElapsedMilliseconds;
        }

        private static long GetTimeForSquid(Stopwatch sw,
                                            int count)
        {
#pragma warning disable 219
            var foo = Dynamic<INonGenericInterfaceWithGenericProperties, PropertyChangingNotificationSinkTypeCreator<INonGenericInterfaceWithGenericProperties>>.NewInstance();
#pragma warning restore 219

            sw.Reset();
            sw.Start();
// ReSharper disable RedundantAssignment
            for (var i = 0;
                 i < count;
                 i++)
            {
                foo = Dynamic<INonGenericInterfaceWithGenericProperties, PropertyChangingNotificationSinkTypeCreator<INonGenericInterfaceWithGenericProperties>>.NewInstance();
            }
// ReSharper restore RedundantAssignment

            sw.Stop();

            return sw.ElapsedMilliseconds;
        }

        private static long GetTimeForDefaultCtorInvoke(Stopwatch sw,
                                                        int count)
        {
            var defaultCtorNonGenericInterfaceWithGenericProperties = typeof (NonGenericInterfaceWithGenericProperties).GetConstructor(Type.EmptyTypes);
            var defaultCtorNonGenericInterfaceWithNonGenericProperties = typeof (NonGenericInterfaceWithNonGenericProperties).GetConstructor(Type.EmptyTypes);
            Debug.Assert(defaultCtorNonGenericInterfaceWithGenericProperties != null,
                         "defaultCtorNonGenericInterfaceWithGenericProperties != null");
            Debug.Assert(defaultCtorNonGenericInterfaceWithNonGenericProperties != null,
                         "defaultCtorNonGenericInterfaceWithNonGenericProperties != null");

            sw.Reset();
            sw.Start();
            for (var i = 0;
                 i < count;
                 i++)
            {
                defaultCtorNonGenericInterfaceWithGenericProperties.Invoke(null);
            }

            sw.Stop();

            return sw.ElapsedMilliseconds;
        }

        private static long GetTimeForLambda(Stopwatch sw,
                                             int count)
        {
            Func<NonGenericInterfaceWithGenericProperties> nonGenericInterfaceWithGenericPropertiesFunc = () => new NonGenericInterfaceWithGenericProperties();

            sw.Reset();
            sw.Start();
            for (var i = 0;
                 i < count;
                 i++)
            {
                nonGenericInterfaceWithGenericPropertiesFunc();
            }

            sw.Stop();

            return sw.ElapsedMilliseconds;
        }

        private static long GetTimeForActivator(Stopwatch sw,
                                                int count)
        {
            sw.Reset();
            sw.Start();
            for (var i = 0;
                 i < count;
                 i++)
            {
                Activator.CreateInstance(typeof (NonGenericInterfaceWithGenericProperties));
            }

            sw.Stop();

            return sw.ElapsedMilliseconds;
        }

        private static long GetTimeForNew(Stopwatch sw,
                                          int count)
        {
            sw.Reset();
            sw.Start();
            for (var i = 0;
                 i < count;
                 i++)
            {
// ReSharper disable ObjectCreationAsStatement
                new NonGenericInterfaceWithGenericProperties();
// ReSharper restore ObjectCreationAsStatement
            }

            sw.Stop();

            return sw.ElapsedMilliseconds;
        }
    }
}