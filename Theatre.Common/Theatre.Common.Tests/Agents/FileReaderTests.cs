using System.IO;
using Akka.TestKit.VsTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Theatre.Common.Agents;
using Theatre.Common.Messages;

namespace Theatre.Common.Tests.Agents
{
    [TestClass]
    public class FileReaderTests : TestKit
    {
        [TestMethod]
        public void ThrowsExceptionWhenFileNotFound()
        {
            var target = ActorOfAsTestActorRef<FileReader>();
            target.Tell(new HashFile("non/existent/path"));
            EventFilter.Exception<FileNotFoundException>();
        }
    }
}