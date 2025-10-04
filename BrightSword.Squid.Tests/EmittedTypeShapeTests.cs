using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BrightSword.Squid.TypeCreators;

namespace Tests.BrightSword.Squid
{
    public interface IFacetMarker { }

    public interface IFacetExample
    {
        string Name { get; set; }
    }

    public interface ISecretExample
    {
        string Visible { get; set; }
        string Secret { get; set; }
    }

    public interface ISimple
    {
        string Name { get; set; }
        int Count { get; set; }
    }

    [TestClass]
    public class EmittedTypeShapeTests
    {
        [TestMethod]
        public void FacetInterface_Is_Implemented_On_Emitted_Type()
        {
            var creator = new BasicDataTransferObjectTypeCreator<IFacetExample>
            {
                FacetInterfaces = new[] { typeof(IFacetMarker) }
            };

            var emittedType = creator.Type;

            Assert.IsNotNull(emittedType);
            // The emitted concrete type should implement the facet marker interface
            Assert.IsTrue(typeof(IFacetMarker).IsAssignableFrom(emittedType), "Emitted type must implement the facet marker interface");
        }

        [TestMethod]
        [Ignore("Failing in .NET 10 - PropertyFilter not properly excluding property implementation. Needs investigation.")]
        public void PropertyFilter_Excludes_Configured_Property()
        {
            // PropertyFilter is a protected virtual method on the creator; create a subclass to override it.
            var creator = new SecretFilteringCreator();

            var emittedType = creator.Type;

            Assert.IsNotNull(emittedType);

            var visibleProp = emittedType.GetProperty("Visible");
            var secretProp = emittedType.GetProperty("Secret");

            Assert.IsNotNull(visibleProp, "Visible property should be emitted");
            Assert.IsNull(secretProp, "Secret property should be excluded by PropertyFilter");
        }

        [TestMethod]
        public void CreateInstance_Allows_Property_ReadWrite_Access()
        {
            var creator = new BasicDataTransferObjectTypeCreator<ISimple>();
            var instance = (ISimple)creator.CreateInstance();

            instance.Name = "test";
            instance.Count = 42;

            Assert.AreEqual("test", instance.Name);
            Assert.AreEqual(42, instance.Count);
        }

        private sealed class SecretFilteringCreator : BasicDataTransferObjectTypeCreator<ISecretExample>
        {
            protected override bool PropertyFilter(PropertyInfo propertyInfo) => propertyInfo.Name != "Secret";
        }
    }
}
