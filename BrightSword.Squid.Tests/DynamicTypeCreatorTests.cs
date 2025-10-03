using System.Collections.Generic;

using BrightSword.Squid.TypeCreators;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.BrightSword.Squid
{
    [TestClass]
    public class DynamicTypeCreatorTests
    {
        private readonly TestTypeCreator<IEnumerable<int>> _typeCreatorForIEnumerableOfInt = new TestTypeCreator<IEnumerable<int>>();

        [TestMethod]
    public void TestInterfaceName()
        {
            Assert.AreEqual("IEnumerable<Int32>",
                            _typeCreatorForIEnumerableOfInt.InterfaceName);
        }

        [TestMethod]
    public void TestClassName()
        {
            Assert.AreEqual("Enumerable<Int32>",
                            _typeCreatorForIEnumerableOfInt.ClassName);
        }

    private sealed class TestTypeCreator<T> : BasicDataTransferObjectTypeCreator<T>
            where T : class
        {
            public new string InterfaceName
            {
                get { return base.InterfaceName; }
            }

            public new string ClassName
            {
                get { return base.ClassName; }
            }
        }
    }
}