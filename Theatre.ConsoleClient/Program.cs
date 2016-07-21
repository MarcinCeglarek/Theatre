using System.IO.Abstractions;
using Akka.Actor;
using Autofac;
using Theatre.Common.Agents;
using Theatre.Common.Messages;

namespace Theatre.ConsoleClient
{
    internal class Program
    {
        private static IContainer Container { get; set; }

        private static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<FileSystem>().As<IFileSystem>();

            Container = builder.Build();

            using (var scope = Container.BeginLifetimeScope())
            {
                var actorSystem = ActorSystem.Create("Theatre");

                var databaser = actorSystem.ActorOf(Props.Create(() => new Databaser()), "Databaser");
                var reader = actorSystem.ActorOf(Props.Create(() => new FileReader(databaser, scope.Resolve<IFileSystem>())), "Reader");

                reader.Tell(new HashFile("c:\\DBAR_Ver.txt"));
                actorSystem.AwaitTermination();
            }
        }
    }
}