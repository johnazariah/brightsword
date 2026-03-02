namespace BrightSword.SwissKnife.Tests
{
    public interface ISimpleTestInterface
    {
        string Name { get; set; }
        int Value { get; set; }
    }

    public interface IInheritedInterface : ISimpleTestInterface
    {
        string Extra { get; set; }
    }

    public interface IInterfaceWithMethods
    {
        void DoSomething();
        int Add(int a, int b);
    }

    public interface IInterfaceWithEvent
    {
        string Name { get; set; }
        event EventHandler Changed;
    }

    public class SimpleTestClass
    {
        public string Name { get; set; }
    }

    public class TypeMemberDiscoveryTests
    {
        [Fact]
        public void GetAllProperties_Interface_ReturnsOwnProperties()
        {
            var props = typeof(ISimpleTestInterface).GetAllProperties().ToList();
            Assert.Contains(props, p => p.Name == "Name");
            Assert.Contains(props, p => p.Name == "Value");
        }

        [Fact]
        public void GetAllProperties_InheritedInterface_ReturnsAllProperties()
        {
            var props = typeof(IInheritedInterface).GetAllProperties().ToList();
            Assert.Contains(props, p => p.Name == "Name");
            Assert.Contains(props, p => p.Name == "Value");
            Assert.Contains(props, p => p.Name == "Extra");
        }

        [Fact]
        public void GetAllProperties_Class_ReturnsProperties()
        {
            var props = typeof(SimpleTestClass).GetAllProperties().ToList();
            Assert.Contains(props, p => p.Name == "Name");
        }

        [Fact]
        public void GetAllMethods_Interface_ReturnsMethods()
        {
            var methods = typeof(IInterfaceWithMethods).GetAllMethods().ToList();
            Assert.Contains(methods, m => m.Name == "DoSomething");
            Assert.Contains(methods, m => m.Name == "Add");
        }

        [Fact]
        public void GetAllEvents_Interface_ReturnsEvents()
        {
            var events = typeof(IInterfaceWithEvent).GetAllEvents().ToList();
            Assert.Contains(events, e => e.Name == "Changed");
        }

        [Fact]
        public void GetAllProperties_Struct_ReturnsProperties()
        {
            var props = typeof(TestStruct).GetAllProperties().ToList();
            Assert.Contains(props, p => p.Name == "X");
        }

        public struct TestStruct
        {
            public int X { get; set; }
        }
    }
}
