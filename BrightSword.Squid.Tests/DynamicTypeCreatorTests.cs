using BrightSword.Squid.TypeCreators;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BrightSword.Squid.Tests
{
    /// <summary>
    /// Unit tests that validate the naming conventions produced by the
    /// BasicDataTransferObjectTypeCreator for interface and class names
    /// when working with generic type parameters.
    /// </summary>
    [TestClass]
    public class DynamicTypeCreatorTests
    {
        private readonly TestTypeCreator<IEnumerable<int>> _typeCreatorForIEnumerableOfInt = new();

        /// <summary>
        /// The creator should expose a printable interface name for diagnostic purposes
        /// that includes the generic argument types.
        /// </summary>
        [TestMethod]
        public void TestInterfaceName()
        {
            Assert.AreEqual("IEnumerable<Int32>",
                            _typeCreatorForIEnumerableOfInt.InterfaceName);
        }

        /// <summary>
        /// The computed class name for the generated concrete type should strip the leading 'I'
        /// and show generic arguments in the same printable format.
        /// </summary>
        [TestMethod]
        public void TestClassName()
        {
            Assert.AreEqual("Enumerable<Int32>",
                            _typeCreatorForIEnumerableOfInt.ClassName);
        }

        private sealed class TestTypeCreator<T> : BasicDataTransferObjectTypeCreator<T>
                where T : class
        {
            public new string InterfaceName => base.InterfaceName;

            public new string ClassName => base.ClassName;
        }
    }
}
