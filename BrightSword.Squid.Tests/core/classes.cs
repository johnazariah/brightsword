using BrightSword.Squid.Test;

namespace BrightSword.Squid.Tests.core
{
    internal sealed class NonGenericInterfaceWithGenericProperties : ChangeTrackedVersionedEntity,
                                                              INonGenericInterfaceWithGenericProperties
    {
        public NonGenericInterfaceWithGenericProperties()
        {
            Things = [];
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
