using System;
using System.ComponentModel;
using BrightSword.Squid.TypeCreators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.BrightSword.Squid.core;
using INotifyPropertyChanged = BrightSword.Squid.API.INotifyPropertyChanged;
using INotifyPropertyChanging = BrightSword.Squid.API.INotifyPropertyChanging;

namespace Tests.BrightSword.Squid
{
    /// <summary>
    /// Tests for property change notification behavior. These validate both
    /// the control scenarios (hand-written base classes raising events) and
    /// generated types that inherit from base types which implement the
    /// notification contracts.
    ///
    /// The tests ensure that OnPropertyChanging is called before mutation and
    /// that OnPropertyChanged is called after mutation with the expected values.
    /// </summary>
    [TestClass]
    public class ChangeNotificationTests
    {
        [TestMethod]
        public void ControlTestFilbertWithPropertyChangedShouldRaisePropertyChangedEvent()
        {
            var obj = new FilbertWithPropertyChanged
            {
                Name = "currentName",
                Id = 42,
            };

            obj.ObjectPropertyChanged += (sender,
                                          args) =>
                                         {
                                             Assert.AreEqual(args.PropertyName,
                                                             nameof(IFilbert.Name));
                                             Assert.AreEqual(((IFilbert)sender).Name,
                                                             "newName");
                                         };

            obj.Name = "newName";
        }

        [TestMethod]
        public void ControlTestFilbertWithPropertyChangingShouldRaisePropertyChangingEvent()
        {
            var obj = new FilbertWithPropertyChanging
            {
                Name = "currentName",
                Id = 42,
            };

            obj.ObjectPropertyChanging += (sender,
                                           args) =>
                                          {
                                              Assert.AreEqual(args.PropertyName,
                                                              nameof(IFilbert.Name));
                                              Assert.AreEqual(((IFilbert)sender).Name,
                                                              "currentName");
                                          };

            obj.Name = "newName";
        }

        [TestMethod]
        public void ControlTestFilbertWithPropertyChangeShouldRaiseBoth()
        {
            var obj = new FilbertWithPropertyChangingAndPropertyChanged
            {
                Name = "currentName",
                Id = 42,
            };

            obj.ObjectPropertyChanging += (sender,
                                           args) => CheckFilbertPropertyChanging(args,
                                                                                 sender);

            obj.ObjectPropertyChanged += (sender,
                                          args) => CheckFilbertPropertyChanged(args,
                                                                               sender);

            obj.Name = "newName";
            obj.Id = 86;
        }

        private static void CheckFilbertPropertyChanged(PropertyChangedEventArgs args,
                                                        object sender)
        {
            switch (args.PropertyName)
            {
                case nameof(IFilbert.Name):
                    {
                        Assert.AreEqual("newName",
                                        ((IFilbert)sender).Name);
                    }
                    break;

                case nameof(IFilbert.Id):
                    {
                        Assert.AreEqual(86,
                                        ((IFilbert)sender).Id);
                    }
                    break;

                default:
                    Assert.Fail("Notification received for unknown property");
                    break;
            }
        }

        private static void CheckFilbertPropertyChanging(PropertyChangingEventArgs args,
                                                         object sender)
        {
            switch (args.PropertyName)
            {
                case nameof(IFilbert.Name):
                    {
                        Assert.AreEqual("currentName",
                                        ((IFilbert)sender).Name);
                    }
                    break;

                case nameof(IFilbert.Id):
                    {
                        Assert.AreEqual(42,
                                        ((IFilbert)sender).Id);
                    }
                    break;

                default:
                    Assert.Fail("Notification received for unknown property");
                    break;
            }
        }

        [TestMethod]
        public void TestFilbertWithPropertyChangedShouldRaisePropertyChangedEvent()
        {
            var _instance = new PropertyChangedTypeCreator<IFilbert>().CreateInstance();
            _instance.Name = "currentName";

            Assert.IsNotNull(_instance);
            Assert.IsInstanceOfType(_instance,
                                    typeof(INotifyPropertyChanged));
            Assert.IsInstanceOfType(_instance,
                                    typeof(CommonBaseTypeWithPropertyChanged));
            var _base = (CommonBaseTypeWithPropertyChanged)_instance;

            _base.ObjectPropertyChanged += (sender,
                                            args) => CheckFilbertPropertyChanged(args,
                                                                                 sender);

            _instance.Name = "newName";
            _instance.Id = 86;
        }

        [TestMethod]
        public void TestFilbertWithPropertyChangingShouldRaisePropertyChangingEvent()
        {
            var _instance = new PropertyChangingTypeCreator<IFilbert>().CreateInstance();
            _instance.Name = "currentName";
            _instance.Id = 42;

            Assert.IsNotNull(_instance);
            Assert.IsInstanceOfType(_instance,
                                    typeof(INotifyPropertyChanging));
            Assert.IsInstanceOfType(_instance,
                                    typeof(CommonBaseTypeWithPropertyChanging));
            var _base = (CommonBaseTypeWithPropertyChanging)_instance;

            _base.ObjectPropertyChanging += (sender,
                                             args) => CheckFilbertPropertyChanging(args,
                                                                                   sender);

            _instance.Name = "newName";
            _instance.Id = 86;
        }

        [TestMethod]
        public void TestFilbertWithPropertyChangeShouldRaiseBoth()
        {
            var typeCreator = new BasicDataTransferObjectTypeCreator<IFilbert>
            {
                InterfaceName = "IFilbert",
                ClassName = "Filbert",
                AssemblyName = "Dynamic.PropertyChangingAndChanged.IFilbert",
                BaseType = typeof(CommonBaseTypeWithPropertyChangingAndPropertyChanged),
            };

            var _instance = typeCreator.CreateInstance();
            _instance.Name = "currentName";
            _instance.Id = 42;

            Assert.IsNotNull(_instance);
            Assert.IsInstanceOfType(_instance,
                                    typeof(INotifyPropertyChanging));
            Assert.IsInstanceOfType(_instance,
                                    typeof(INotifyPropertyChanged));
            Assert.IsInstanceOfType(_instance,
                                    typeof(CommonBaseTypeWithPropertyChangingAndPropertyChanged));

            var _base = (CommonBaseTypeWithPropertyChangingAndPropertyChanged)_instance;
            _base.ObjectPropertyChanging += (sender,
                                             args) =>
                                            {
                                                Assert.AreEqual(args.PropertyName,
                                                                nameof(IFilbert.Name));
                                                Assert.AreEqual(((IFilbert)sender).Name,
                                                                "currentName");
                                            };

            _base.ObjectPropertyChanged += (sender,
                                            args) =>
                                           {
                                               Assert.AreEqual(args.PropertyName,
                                                               nameof(IFilbert.Name));
                                               Assert.AreEqual(((IFilbert)sender).Name,
                                                               "newName");
                                           };

            _instance.Name = "newName";
        }

        /// <summary>
        /// Base class used by control tests to raise PropertyChanged events. The generated
        /// types may derive from classes like this to provide notification semantics.
        /// </summary>
        public class CommonBaseTypeWithPropertyChanged : INotifyPropertyChanged
        {
            public void OnPropertyChanged(string propertyName,
                                          Type propertyType,
                                          object currentValue,
                                          object newValue)
            {
                if (ObjectPropertyChanged != null)
                {
                    var args = new PropertyChangedEventArgs(propertyName);
                    ObjectPropertyChanged(this,
                                          args);
                }
            }

            public event PropertyChangedEventHandler ObjectPropertyChanged;
        }

        /// <summary>
        /// Base class used by control tests to raise PropertyChanging events. The generated
        /// types may derive from classes like this to provide notification semantics.
        /// </summary>
        public class CommonBaseTypeWithPropertyChanging : INotifyPropertyChanging
        {
            public bool OnPropertyChanging(string propertyName,
                                           Type propertyType,
                                           object currentValue,
                                           object newValue)
            {
                if (ObjectPropertyChanging == null)
                {
                    return true;
                }

                var args = new PropertyChangingEventArgs(propertyName);
                ObjectPropertyChanging(this,
                                       args);
                return true;
            }

            public event PropertyChangingEventHandler ObjectPropertyChanging;
        }

        /// <summary>
        /// Base class used in tests which supports both PropertyChanging and PropertyChanged.
        /// This models a common pattern where a generated type derives from a single base
        /// providing both pre- and post-change notifications.
        /// </summary>
        public class CommonBaseTypeWithPropertyChangingAndPropertyChanged : INotifyPropertyChanged,
                                                                            INotifyPropertyChanging
        {
            public void OnPropertyChanged(string propertyName,
                                          Type propertyType,
                                          object currentValue,
                                          object newValue)
            {
                if (ObjectPropertyChanged == null)
                {
                    return;
                }

                var args = new PropertyChangedEventArgs(propertyName);
                ObjectPropertyChanged(this,
                                      args);
            }

            public bool OnPropertyChanging(string propertyName,
                                           Type propertyType,
                                           object currentValue,
                                           object newValue)
            {
                if (ObjectPropertyChanging == null)
                {
                    return true;
                }

                var args = new PropertyChangingEventArgs(propertyName);
                ObjectPropertyChanging(this,
                                       args);
                return true;
            }

            public event PropertyChangedEventHandler ObjectPropertyChanged;
            public event PropertyChangingEventHandler ObjectPropertyChanging;
        }

        internal sealed class FilbertWithPropertyChanged : CommonBaseTypeWithPropertyChanged,
                                IFilbert
        {
            public string Name
            {
                get;
                set
                {
                    var currentValue = field;
                    field = value;

                    OnPropertyChanged(nameof(IFilbert.Name),
                                      typeof(string),
                                      currentValue,
                                      value);
                }
            }

            public int Id
            {
                get;
                set
                {
                    var currentValue = field;
                    field = value;
                    OnPropertyChanged(nameof(IFilbert.Id),
                                      typeof(int),
                                      currentValue,
                                      value);
                }
            }
        }

        internal sealed class FilbertWithPropertyChanging : CommonBaseTypeWithPropertyChanging,
                                 IFilbert
        {
            public string Name
            {
                get;
                set
                {
                    _ = OnPropertyChanging(nameof(IFilbert.Name),
                                       typeof(string),
                                       field,
                                       value);
                    field = value;
                }
            }

            public int Id
            {
                get;
                set
                {
                    _ = OnPropertyChanging(nameof(IFilbert.Id),
                                       typeof(int),
                                       field,
                                       value);
                    field = value;
                }
            }
        }

        internal sealed class FilbertWithPropertyChangingAndPropertyChanged : CommonBaseTypeWithPropertyChangingAndPropertyChanged,
                                           IFilbert
        {
            public string Name
            {
                get;
                set
                {
                    _ = OnPropertyChanging(nameof(IFilbert.Name),
                                       typeof(string),
                                       field,
                                       value);
                    var currentValue = field;
                    field = value;
                    OnPropertyChanged(nameof(IFilbert.Name),
                                      typeof(string),
                                      currentValue,
                                      value);
                }
            }

            public int Id
            {
                get;
                set
                {
                    _ = OnPropertyChanging(nameof(IFilbert.Id),
                                       typeof(int),
                                       field,
                                       value);
                    var currentValue = field;
                    field = value;
                    OnPropertyChanged(nameof(IFilbert.Id),
                                      typeof(int),
                                      currentValue,
                                      value);
                }
            }
        }

        private sealed class PropertyChangedTypeCreator<T> : BasicDataTransferObjectTypeCreator<T>
                where T : class
        {
            public override Type BaseType => typeof(CommonBaseTypeWithPropertyChanged);
        }

        private sealed class PropertyChangingTypeCreator<T> : BasicDataTransferObjectTypeCreator<T>
                where T : class

        {
            public override Type BaseType => typeof(CommonBaseTypeWithPropertyChanging);
        }
    }
}
