using BrightSword.Squid.Tests.core;
using BrightSword.Squid.TypeCreators;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BrightSword.Squid.Tests
{
    [TestClass]
    public class FacetInjectionTests
    {
        [TestMethod]
        public void TestFacetsAreInjectedProperly()
        {
            var actual = new FacetInjectingTypeCreator<INonGenericInterfaceWithNonGenericProperties>().CreateInstance();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof(INonGenericInterfaceWithNonGenericProperties));
            Assert.IsInstanceOfType(actual,
                                    typeof(IFacet));
        }

        private sealed class FacetInjectingTypeCreator<T> : BasicDataTransferObjectTypeCreator<T>
                where T : class
        {
            public override IEnumerable<Type> FacetInterfaces
            {
                get { yield return typeof(IFacet); }
            }
        }
    }
}
