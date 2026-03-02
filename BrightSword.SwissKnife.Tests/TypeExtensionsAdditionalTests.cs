namespace BrightSword.SwissKnife.Tests
{
    public class TypeExtensionsAdditionalTests
    {
        [Fact]
        public void RenameToConcreteType_Interface_RemovesIPrefix()
        {
            var name = typeof(IDisposable).RenameToConcreteType();
            Assert.Equal("Disposable", name);
        }

        [Fact]
        public void RenameToConcreteType_Class_KeepsName()
        {
            var name = typeof(object).RenameToConcreteType();
            Assert.Equal("Object", name);
        }

        [Fact]
        public void RenameToConcreteType_GenericInterface_RemovesIPrefix()
        {
            var name = typeof(IList<int>).RenameToConcreteType();
            Assert.Contains("List", name);
        }

        [Fact]
        public void RenameToConcreteType_GenericTypeDefinition_Interface_RemovesIPrefix()
        {
            var name = typeof(IList<>).RenameToConcreteType();
            Assert.StartsWith("List", name);
        }

        [Fact]
        public void RenameToConcreteType_GenericTypeDefinition_Class_KeepsName()
        {
            var name = typeof(List<>).RenameToConcreteType();
            Assert.StartsWith("List", name);
        }

        [Fact]
        public void PrintableName_NonGenericType_ReturnsName()
        {
            var name = typeof(int).PrintableName();
            Assert.Equal("Int32", name);
        }

        [Fact]
        public void GetAllMethods_Class_ReturnsMethods()
        {
            var methods = typeof(SimpleTestClass).GetAllMethods().ToList();
            Assert.True(methods.Count > 0);
        }

        [Fact]
        public void GetAllEvents_Class_ReturnsEvents()
        {
            var events = typeof(SimpleTestClass).GetAllEvents().ToList();
            Assert.Empty(events);
        }

        [Fact]
        public void GetAllProperties_NullType_ThrowsArgumentNullException()
        {
            Type nullType = null;
            Assert.Throws<ArgumentNullException>(() => nullType.GetAllProperties().ToList());
        }

        [Fact]
        public void GetAllMethods_NullType_ThrowsArgumentNullException()
        {
            Type nullType = null;
            Assert.Throws<ArgumentNullException>(() => nullType.GetAllMethods().ToList());
        }

        [Fact]
        public void GetAllEvents_NullType_ThrowsArgumentNullException()
        {
            Type nullType = null;
            Assert.Throws<ArgumentNullException>(() => nullType.GetAllEvents().ToList());
        }

        [Fact]
        public void PrintableName_NullType_ThrowsArgumentNullException()
        {
            Type nullType = null;
            Assert.Throws<ArgumentNullException>(() => nullType.PrintableName());
        }

        [Fact]
        public void RenameToConcreteType_NullType_ThrowsArgumentNullException()
        {
            Type nullType = null;
            Assert.Throws<ArgumentNullException>(() => nullType.RenameToConcreteType());
        }
    }
}
