using System;
using System.Collections.Generic;

namespace Tests.BrightSword.Squid.core
{
    public interface IProjection
    {
        string Junk { get; set; }
        int Id { get; }
        IFoo Foo { get; }
    }

    public interface IFoo
    {
        string Name { get; set; }
    }


    public interface IFilbert
    {
        string Name { get; set; }
        int Id { get; set; }
    }
    public class Foo : IFoo
    {
        public string Name { get; set; }
    }

    public interface IBase
    {
        string HiddenProperty { get; }
    }

    public interface IFacet {}

    public interface IDerivedWithNewHiddenProperty : IBase
    {
        new double HiddenProperty { get; set; }
    }

    public interface IDerivedWithHiddenProperty : IBase
    {
#pragma warning disable 108,114
        double HiddenProperty { get; set; }
#pragma warning restore 108,114
    }

    public interface INonGenericInterfaceWithNonGenericProperties
    {
        int Foo { get; set; }

        String Name { get; set; }
        Decimal Amount { get; set; }
    }

    public interface IInterfaceWithGenericMethods
    {
        void GenericParameter<T>();
        void GenericParameterConstrainedToReference<T>() where T : class;
        void GenericParameterConstrainedToReferenceAndNew<T>() where T : class, new();
        void GenericParameterConstrainedToReferenceAndNewAndInterface<T>() where T : class, IBase, new();
        void GenericParameterConstrainedToNewAndBaseClass<T>() where T : Exception, new();
        void GenericParameterConstrainedToNewAndBaseClassAndInterface<T>() where T : Random, IInterfaceWithEvent, new();
        void GenericParameterConstrainedToBaseClass<T>() where T : Random;
        void GenericParameterConstrainedToBaseClassAndInterface<T>() where T : Random, IInterfaceWithEvent;
        void GenericParameterConstrainedToBaseClassAndTwoInterfaces<T>() where T : Random, IBase, IInterfaceWithEvent;
    }

    public interface INonGenericInterfaceWithGenericProperties
    {
        String Name { get; set; }
        ICollection<INonGenericInterfaceWithNonGenericProperties> Things { get; }
    }

    public interface IInterfaceWithReadonlyScalarProperty
    {
        Decimal AmountValue { get; }
    }

    public interface IInterfaceWithAVoidMethodNoArgs
    {
        void VoidMethodNoArgs();
    }

    public interface IInterfaceWithAnIntMethodWithArgs
    {
        int IntMethodWithArgs(int a,
                              int b);
    }

    public interface IInterfaceExtendingAnother : IInterfaceWithAnIntMethodWithArgs
    {
        int IntMethodWithArgs(int a,
                              int b,
                              int c);
    }

    public interface IInterfaceHidingAMethod : IInterfaceExtendingAnother
    {
        new double IntMethodWithArgs(int a,
                                     int b);
    }

    public interface IInterfaceWithEvent
    {
        event EventHandler Foo;
    }

    public interface IInterfaceWithACharMethodWithParamsArg
    {
        char ParamsMethod(int a,
                          int b,
                          params string[] foo);
    }

    public interface IInterfaceWithAGuidMethodWithRefArg
    {
        Guid MethodWithRefParameters(ref string a);
    }

    public interface IInterfaceWithAnArrayMethodWithOutArg
    {
        char[] ArrayMethodWithOutParameters(out string a);
    }

    public interface IInterfaceLeftBase
    {
        double CollisionWithSameType { get; set; }
        double CollisionWithDifferentType { get; set; }
    }

    public interface IInterfaceRightBase
    {
        double CollisionWithSameType { get; set; }
        string CollisionWithDifferentType { get; set; }
    }

    public interface IComprehensive
    {
        IFoo Foo { get; }
        IEnumerable<int> IntegerList { get; }
        ISet<decimal> DecimalSet { get; }
        IDictionary<int, string> StringDictionary { get; }
        string Property { get; set; }

        string Method();

        event EventHandler RaiseSomeEvent;
    }

// ReSharper disable PossibleInterfaceMemberAmbiguity
    public interface IInterfaceWithMultipleInheritanceAndPropertyCollisions : IInterfaceLeftBase,
                                                                              IInterfaceRightBase {}

// ReSharper restore PossibleInterfaceMemberAmbiguity
}