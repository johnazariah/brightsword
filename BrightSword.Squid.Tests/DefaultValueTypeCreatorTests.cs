using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using BrightSword.Squid.TypeCreators;
using BrightSword.SwissKnife;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.BrightSword.Squid
{
    [TestClass]
    public class DefaultValueTypeCreatorTests
    {
        private static readonly int[] _sampleItems = [1, 2, 3];
        [TestMethod]
        public void TestPropertyWithDefaultValueIsNoticed()
        {
            var creator = new DefaultValueTypeCreator<ITest>();
            var instance = creator.CreateInstance();

            Assert.AreEqual(2,
                            creator._propertiesWithDefaultValues.Count);
            Assert.AreEqual(42,
                            creator._propertiesWithDefaultValues.First(_ => _.Key.Name == "MeaningOfLife")
                                   .Value);
        }

        [TestMethod]
        public void TestDefaultValueIsSetWhenSpecified()
        {
            var instance = new BasicDataTransferObjectTypeCreator<IBar>().CreateInstance();

            Assert.AreEqual(100,
                            instance.Id);
            Assert.AreEqual(0,
                            instance.Number);
            Assert.AreEqual(55.25m,
                            instance.Amount);
            Assert.AreEqual("Name",
                            instance.Name);

            CollectionAssert.AreEqual(_sampleItems, instance.Items);

            Assert.AreEqual(FieldAttributes.InitOnly,
                            instance.FieldAttributes);
        }

        [TestMethod]
        public void TestDefaultValueIsSetOnObjectCreatedWithDefaultConstructorWhenSpecified()
        {
            var instance = (IBar)Activator.CreateInstance(new BasicDataTransferObjectTypeCreator<IBar>().Type);

            Assert.AreEqual(100,
                            instance.Id);
            Assert.AreEqual(0,
                            instance.Number);
            Assert.AreEqual(55.25m,
                            instance.Amount);
            Assert.AreEqual("Name",
                            instance.Name);

            CollectionAssert.AreEqual(_sampleItems, instance.Items);

            Assert.AreEqual(FieldAttributes.InitOnly,
                            instance.FieldAttributes);
        }
        public enum PayrollEmploymentStatus
        {
            All = -2,
            FullTime = 0,

        }

        public interface IPayRunFilter
        {
            [DefaultValue(PayrollEmploymentStatus.All)]
            PayrollEmploymentStatus EmploymentStatusAll { get; }

            [DefaultValue(PayrollEmploymentStatus.FullTime)]
            PayrollEmploymentStatus EmploymentStatusFullTime { get; }

        }

        private sealed class PayRunFilter : IPayRunFilter
        {
            public PayrollEmploymentStatus EmploymentStatusAll { get; } = PayrollEmploymentStatus.All;

            public PayrollEmploymentStatus EmploymentStatusFullTime { get; } = PayrollEmploymentStatus.FullTime;
        }

        [TestMethod]
        public void TestCreateInstanceWithDefaultEnumValueMinusOneShouldCreate()
        {
            var instance = new BasicDataTransferObjectTypeCreator<IPayRunFilter>().CreateInstance();
            Assert.IsNotNull(instance);
        }

        private sealed class DefaultValueTypeCreator<T> : BasicDataTransferObjectTypeCreator<T>
                where T : class
        {
            public readonly ConcurrentDictionary<PropertyInfo, object> _propertiesWithDefaultValues = new();

            protected override TypeBuilder AddProperty(TypeBuilder typeBuilder,
                                                       PropertyInfo propertyInfo)
            {
                if (propertyInfo.GetCustomAttribute<DefaultValueAttribute>() != null)
                {
                    _propertiesWithDefaultValues[propertyInfo] = propertyInfo.GetCustomAttributeValue<DefaultValueAttribute, object>(_ => _.Value);
                }

                return base.AddProperty(typeBuilder,
                                        propertyInfo);
            }
        }

        public interface IBar
        {
            [DefaultValue(100)]
            int Id { get; }

            [DefaultValue(typeof(decimal), "55.25")]
            decimal Amount { get; set; }

            int Number { get; set; }

            [DefaultValue("Name")]
            string Name { get; set; }

            [DefaultValue(FieldAttributes.InitOnly)]
            FieldAttributes FieldAttributes { get; set; }

            [DefaultValue(new[]
                          {
                              1,
                              2,
                              3
                          })]
            int[] Items { get; set; }

            event EventHandler Changed;
        }

        public interface ITest
        {
            [DefaultValue(42)]
            int MeaningOfLife { get; set; }

            [DefaultValue("Hello World")]
            string Greeting { get; set; }
        }
    }
}
