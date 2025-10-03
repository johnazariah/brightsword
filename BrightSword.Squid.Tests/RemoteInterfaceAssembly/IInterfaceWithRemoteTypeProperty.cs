using RemoteBaseInterfaceAssembly;

using RemotePropertyInterfaceAssembly;

namespace RemoteInterfaceAssembly
{
    public interface IInterfaceWithRemoteTypeProperty : IInterfaceWithRemoteTypePropertyBase
    {
        IFoo RemoteProperty { get; }
    }
}
