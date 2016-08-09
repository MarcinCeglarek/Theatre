#region

using System.IO.Abstractions;
using System.Threading;
using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using Theatre.Common.Actors;
using Theatre.Common.Messages;
using Theatre.ConsoleClient.Actor;

#endregion

namespace Theatre.ConsoleClient
{
    internal class Program
    {
        private const string Path = "C:\\Temp";

        private static readonly MainWindow MainWindow = new MainWindow();

        private static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("Theatre");

            // Registering dependency injection
            var builder = new ContainerBuilder();
            builder.RegisterType<FileSystem>().As<IFileSystem>();
            builder.RegisterType<DirectoryReader>().As<DirectoryReader>();
            builder.RegisterType<FileReader>().As<FileReader>();
            builder.RegisterType<UiActor>().As<UiActor>();

            // Opening window in another thread
            new Thread(ShowDialog).Start();

            var container = builder.Build();
            using (var scope = container.BeginLifetimeScope())
            {
                // Creating props resolver
                var propsResolver = new AutoFacDependencyResolver(container, actorSystem);

                // Creating main agents
                var uiActor = actorSystem.ActorOf(actorSystem.DI().Props<UiActor>(), "UiActor");
                var reader = actorSystem.ActorOf(actorSystem.DI().Props<DirectoryReader>(), "RootDirectoryReader");
                MainWindow.UiActor = uiActor;
                reader.Tell(new HashDirectory(Path));
                uiActor.Tell(new InitializationMessage(Path, MainWindow));

                actorSystem.AwaitTermination();
            }
        }

        private static void ShowDialog()
        {
            MainWindow.ShowDialog();
        }
    }
}