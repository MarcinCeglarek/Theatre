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
        public void EscapesParenthesis()
        {
            const string Path = "[{TestName.023}]";
            Assert.AreEqual("W3tUZXN0TmFtZS4wMjN9XQ==", Path.ToActorFriendlyName());
        }

        [TestMethod]
        public void EscapesMultipleSlashes()
        {
            const string Path = "!@#$%^&*()_+-=[]{}\\|/?";
            Assert.AreEqual("IUAjJCVeJiooKV8rLT1bXXt9\\fC8/", Path.ToActorFriendlyName());
        }

        [TestMethod]
        public void EscapeSpace()
        {
            const string Path = "And Space";
            Assert.AreEqual("QW5kIFNwYWNl", Path.ToActorFriendlyName());
        }

        [TestMethod]
        public void EscapeFullDirectory()
        {
            const string Path = "C:\\Some Directory\\ And another\\.{}!And Strange File .3";
            Assert.AreEqual("Qzo=\\U29tZSBEaXJlY3Rvcnk=\\IEFuZCBhbm90aGVy\\Lnt9IUFuZCBTdHJhbmdlIEZpbGUgLjM=", Path.ToActorFriendlyName());
        }
    }
}