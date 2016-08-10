namespace Theatre.ConsoleClient
{
    #region

    using System.IO.Abstractions;

    using Akka.Actor;
    using Akka.DI.AutoFac;
    using Akka.DI.Core;

    using Autofac;

    using Theatre.Common.Actors;
    using Theatre.Common.Messages;

    #endregion

    internal class Program
    {
        private const string Path = "C:\\Temp";

        private static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("Theatre");

            // Registering dependency injection
            var builder = new ContainerBuilder();
            builder.RegisterType<FileSystem>().As<IFileSystem>();
            builder.RegisterType<DirectoryReader>().As<DirectoryReader>();
            builder.RegisterType<FileReader>().As<FileReader>();

            var container = builder.Build();
            using (var scope = container.BeginLifetimeScope())
            {
                // Creating props resolver
                var propsResolver = new AutoFacDependencyResolver(container, actorSystem);

                // Creating main agents
                var reader = actorSystem.ActorOf(actorSystem.DI().Props<DirectoryReader>(), "Root");
                reader.Tell(new ProcessDirectory(Path));

                // Actor system is running. Blocking operation
                actorSystem.AwaitTermination();
            }
        }
    }
}