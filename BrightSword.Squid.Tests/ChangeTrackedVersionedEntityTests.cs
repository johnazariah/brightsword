using BrightSword.Squid;
using BrightSword.Squid.API;
using BrightSword.Squid.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Tests.BrightSword.Squid.core;

namespace Tests.BrightSword.Squid
{
    [TestClass]
    public class ChangeTrackedPropertyTypeTests
    {
        [TestMethod]
    public void GivenAPropertyChangesOnAnObjectThenTheChangedPropertiesDictionaryShouldRegisterThatChange()
        {
            var creator = new PropertyChangingNotificationSinkTypeCreator<INonGenericInterfaceWithNonGenericProperties>();
            var instance = creator.CreateInstance();

            Assert.IsInstanceOfType(instance,
                                    typeof (INotifyPropertyChanging));
            Assert.IsInstanceOfType(instance,
                                    typeof (PropertyChangingNotificationSink));

            var sink = (PropertyChangingNotificationSink) instance;

            Assert.IsFalse(sink.IsChanged);
            instance.Amount = 3.14M;
            Assert.IsTrue(sink.IsChanged);

            Assert.IsTrue(sink.IsPropertyChanged("Amount"));
            Assert.AreEqual(3.14M,
                            sink.GetOriginalValue("Amount"));
            instance.Amount = 4.26M;
            Assert.AreEqual(4.26M,
                            instance.Amount);
            Assert.AreEqual(3.14M,
                            sink.GetOriginalValue("Amount"));
        }

        [TestMethod]
    public void TestDynamicChangeTrackedPropertyTypeIsPropertyChangingNotificationSink()
        {
            var actual = Dynamic<IInterfaceExtendingAnother, PropertyChangingNotificationSinkTypeCreator<IInterfaceExtendingAnother>>.NewInstance();

            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual,
                                    typeof (IInterfaceExtendingAnother));

            Assert.AreEqual("Types.IInterfaceExtendingAnother.ChangeTrackedAttributed",
                            actual.GetType()
                                  .FullName);

            Assert.AreEqual(actual.GetType()
                                  .BaseType,
                            typeof (PropertyChangingNotificationSink));
        }

        //[TestMethod]
        //public void Test_ChangeTrackedObjectHasDataContractAndDataMemberAttributes()
        //{
        //    var actual = Dynamic<IInterfaceExtendingAnother, AttributedDTOTypeCreator<IInterfaceExtendingAnother>>.NewInstance();

        //    Assert.IsNotNull(actual);

        //    Assert.IsNotNull(actual.GetType()
        //                           .GetCustomAttributes(typeof (DataContractAttribute),
        //                                                false)
        //                           .Single());

        //    Assert.IsTrue(actual.GetType()
        //                        .GetProperties()
        //                        .Select(_propertyInfo => _propertyInfo.GetCustomAttributes(typeof (DataMemberAttribute),
        //                                                                                   false))
        //                        .All(_ => _ != null));
        //}
    }
}