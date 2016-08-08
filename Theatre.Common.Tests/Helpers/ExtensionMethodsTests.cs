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
            Assert.AreEqual("a_b", path.ToActorFriendlyName());
        }

        [TestMethod]
        public void EscapesMultipleSlashes()
        {
            var path = "a\\\\\\b\\\\c";
            Assert.AreEqual("a___b__c", path.ToActorFriendlyName());
        }
    }
}