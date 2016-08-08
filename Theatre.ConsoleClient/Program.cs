using System;
using Akka.DI.AutoFac;
using Akka.DI.Core;

namespace Theatre.ConsoleClient
{
    #region Usings

    using System.IO.Abstractions;

    using Akka.Actor;

    using Autofac;

    using Theatre.Common.Agents;
    using Theatre.Common.Messages;

    #endregion

    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            var actorSystem = ActorSystem.Create("Theatre");

            builder.RegisterType<FileSystem>().As<IFileSystem>();
            
            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var propsResolver = new AutoFacDependencyResolver(container, actorSystem);
                var reader =
                    actorSystem.ActorOf(
                        actorSystem.DI().Props<DirectoryReader>(), 
                        "RootDirectoryReader");
                reader.Tell(new HashDirectory("C:\\Users\\mceglarek03"));

                actorSystem.AwaitTermination();
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}