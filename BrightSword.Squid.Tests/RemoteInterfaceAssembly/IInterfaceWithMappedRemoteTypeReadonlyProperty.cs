using RemotePropertyInterfaceAssembly;

namespace RemoteInterfaceAssembly
{
    public interface IInterfaceWithMappedRemoteTypeReadonlyProperty
    {
        IFoo RemoteProperty { get; }
    }
}
