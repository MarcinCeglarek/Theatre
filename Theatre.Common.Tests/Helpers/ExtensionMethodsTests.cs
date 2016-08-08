namespace Theatre.Common.Tests.Helpers
{
    #region Usings

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Theatre.Common.Helpers;

    #endregion

    [TestClass]
    public class ExtensionMethodsTests
    {
        [TestMethod]
        public void EscapesSlash()
        {
            var path = "a\\b";
            Assert.AreEqual("a!b", path.ToActorFriendlyName());
        }

        [TestMethod]
        public void EscapesMultipleSlashes()
        {
            var path = "a\\\\\\b\\\\c";
            Assert.AreEqual("a!!!b!!c", path.ToActorFriendlyName());
        }

        [TestMethod]
        public void EscapeSpace()
        {
            var path = "a b";
            Assert.AreEqual("a*b", path.ToActorFriendlyName());
        }

        [TestMethod]
        public void EscepeMultipleSpaces()
        {
            var path = "a   b  c";
            Assert.AreEqual("a***b**c", path.ToActorFriendlyName());
        }
    }
}