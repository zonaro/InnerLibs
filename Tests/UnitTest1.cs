using InnerLibs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void AdjustBlankSpace()
        {
            Assert.AreEqual("  jubileu  ".AdjustBlankSpaces(), "jubileu");
        }
    }
}