using BrightSword.Crucible;
using BrightSword.Squid.Tests.core;
using BrightSword.Squid.TypeCreators;

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BrightSword.Squid.Tests
{
    [TestClass]
    public class InitializationCheckedDynamicTests
    {
        private readonly BasicDataTransferObjectTypeCreator<IProjection> _simpleDTOTypeCreator = new(_ => typeof(IFoo).IsAssignableFrom(_)
                                                                                                                                                              ? typeof(Foo)
                                                                                                                                                              : null);

        [TestMethod]
        public void ShouldBeAbleToSetAndGetUnmappedImmutablePropertyViaDLR()
        {
            dynamic instance = _simpleDTOTypeCreator.CreateInstance();
            instance.Id = 4;
            Assert.AreEqual(4,
                            instance.Id);
        }

        [TestMethod]
        public void ShouldNotBeAbleToSetAndGetMappedImmutablePropertyViaDLR()
        {
            dynamic instance = _simpleDTOTypeCreator.CreateInstance();
            _ = ExceptionHelper.ExpectException<RuntimeBinderException>(() => { instance.Foo = new Foo(); });
        }

        [TestMethod]
        public void GettingAReadonlyPropertyValueBeforeInitializingShouldThrow()
        {
#pragma warning disable 168
            // ReSharper disable UnusedVariable
            var instance = InitializationCheckedDynamic<IProjection>.NewInstance();
            _ = ExceptionHelper.ExpectException<MethodAccessException>(() => { var foo = instance.Id; });
            // ReSharper restore UnusedVariable
#pragma warning restore 168
        }

        private static class InitializationCheckedDynamic<T>
            where T : class
        {
            private static readonly BasicDataTransferObjectTypeCreator<T> typeCreator = new(_ => typeof(IFoo).IsAssignableFrom(_)
                                                                                                                                           ? typeof(Foo)
                                                                                                                                           : null)
            {
                TrackReadonlyPropertyInitialized = true,
            };

            public static T NewInstance() => typeCreator.CreateInstance();
        }
    }
}
