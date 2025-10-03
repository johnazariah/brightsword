using System;
using System.Collections.Generic;
using System.ComponentModel;

using BrightSword.Squid.Test;

using INotifyPropertyChanged = BrightSword.Squid.API.INotifyPropertyChanged;
using INotifyPropertyChanging = BrightSword.Squid.API.INotifyPropertyChanging;

namespace Tests.BrightSword.Squid.core
{
    internal sealed class NonGenericInterfaceWithGenericProperties : ChangeTrackedVersionedEntity,
                                                              INonGenericInterfaceWithGenericProperties
    {
        public NonGenericInterfaceWithGenericProperties()
        {
            Things = new List<INonGenericInterfaceWithNonGenericProperties>();
            //SelfProperty = new NonGenericInterfaceWithGenericProperties();
        }

        public string Name { get; set; }
        public ICollection<INonGenericInterfaceWithNonGenericProperties> Things { get; private set; }
    }

    internal sealed class NonGenericInterfaceWithNonGenericProperties : ChangeTrackedVersionedEntity,
                                                                 INonGenericInterfaceWithNonGenericProperties
    {
        public int Foo { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }
}
