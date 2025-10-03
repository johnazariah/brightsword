using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;

using BrightSword.Squid;
using BrightSword.Squid.TypeCreators;
using BrightSword.SwissKnife;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using INotifyPropertyChanged = BrightSword.Squid.API.INotifyPropertyChanged;
using INotifyPropertyChanging = BrightSword.Squid.API.INotifyPropertyChanging;
using Tests.BrightSword.Squid.core;

namespace Tests.BrightSword.Squid
{
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
                                             Assert.AreEqual(((IFilbert) sender).Name,
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
                                              Assert.AreEqual(((IFilbert) sender).Name,
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
                                    ((IFilbert) sender).Name);
                }
                    break;

                    case nameof(IFilbert.Id):
                {
                    Assert.AreEqual(86,
                                    ((IFilbert) sender).Id);
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
                                    ((IFilbert) sender).Name);
                }
                    break;

                    case nameof(IFilbert.Id):
                {
                    Assert.AreEqual(42,
                                    ((IFilbert) sender).Id);
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
                                    typeof (INotifyPropertyChanged));
            Assert.IsInstanceOfType(_instance,
                                    typeof (CommonBaseTypeWithPropertyChanged));
            var _base = (CommonBaseTypeWithPropertyChanged) _instance;

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
                                    typeof (INotifyPropertyChanging));
            Assert.IsInstanceOfType(_instance,
                                    typeof (CommonBaseTypeWithPropertyChanging));
            var _base = (CommonBaseTypeWithPropertyChanging) _instance;

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
                                  BaseType = typeof (CommonBaseTypeWithPropertyChangingAndPropertyChanged),
                              };

            var _instance = typeCreator.CreateInstance();
            _instance.Name = "currentName";
            _instance.Id = 42;

            Assert.IsNotNull(_instance);
            Assert.IsInstanceOfType(_instance,
                                    typeof (INotifyPropertyChanging));
            Assert.IsInstanceOfType(_instance,
                                    typeof (INotifyPropertyChanged));
            Assert.IsInstanceOfType(_instance,
                                    typeof (CommonBaseTypeWithPropertyChangingAndPropertyChanged));

            var _base = (CommonBaseTypeWithPropertyChangingAndPropertyChanged) _instance;
            _base.ObjectPropertyChanging += (sender,
                                             args) =>
                                            {
                                                    Assert.AreEqual(args.PropertyName,
                                                                    nameof(IFilbert.Name));
                                                Assert.AreEqual(((IFilbert) sender).Name,
                                                                "currentName");
                                            };

            _base.ObjectPropertyChanged += (sender,
                                            args) =>
                                           {
                                                   Assert.AreEqual(args.PropertyName,
                                                                   nameof(IFilbert.Name));
                                               Assert.AreEqual(((IFilbert) sender).Name,
                                                               "newName");
                                           };

            _instance.Name = "newName";
        }

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
            private int _id;
            private string _name;

            public string Name
            {
                get { return _name; }
                set
                {
                    var currentValue = _name;
                    _name = value;

                    OnPropertyChanged(nameof(IFilbert.Name),
                                      typeof (String),
                                      currentValue,
                                      value);
                }
            }

            public int Id
            {
                get { return _id; }
                set
                {
                    var currentValue = _id;
                    _id = value;
                    OnPropertyChanged(nameof(IFilbert.Id),
                                      typeof (int),
                                      currentValue,
                                      value);
                }
            }
        }

    internal sealed class FilbertWithPropertyChanging : CommonBaseTypeWithPropertyChanging,
                             IFilbert
        {
            private int _id;
            private string _name;

            public string Name
            {
                get { return _name; }
                set
                {
                    OnPropertyChanging(nameof(IFilbert.Name),
                                       typeof (String),
                                       _name,
                                       value);
                    _name = value;
                }
            }

            public int Id
            {
                get { return _id; }
                set
                {
                    OnPropertyChanging(nameof(IFilbert.Id),
                                       typeof (int),
                                       _id,
                                       value);
                    _id = value;
                }
            }
        }

    internal sealed class FilbertWithPropertyChangingAndPropertyChanged : CommonBaseTypeWithPropertyChangingAndPropertyChanged,
                                       IFilbert
        {
            private int _id;
            private string _name;

            public string Name
            {
                get { return _name; }
                set
                {
                    OnPropertyChanging(nameof(IFilbert.Name),
                                       typeof (String),
                                       _name,
                                       value);
                    var currentValue = _name;
                    _name = value;
                    OnPropertyChanged(nameof(IFilbert.Name),
                                      typeof (String),
                                      currentValue,
                                      value);
                }
            }

            public int Id
            {
                get { return _id; }
                set
                {
                    OnPropertyChanging(nameof(IFilbert.Id),
                                       typeof (int),
                                       _id,
                                       value);
                    var currentValue = _id;
                    _id = value;
                    OnPropertyChanged(nameof(IFilbert.Id),
                                      typeof (int),
                                      currentValue,
                                      value);
                }
            }
        }

    private sealed class PropertyChangedTypeCreator<T> : BasicDataTransferObjectTypeCreator<T>
            where T : class
        {
            public override Type BaseType
            {
                get { return typeof (CommonBaseTypeWithPropertyChanged); }
            }
        }

    private sealed class PropertyChangingTypeCreator<T> : BasicDataTransferObjectTypeCreator<T>
            where T : class

        {
            public override Type BaseType
            {
                get { return typeof (CommonBaseTypeWithPropertyChanging); }
            }
        }
    }
}