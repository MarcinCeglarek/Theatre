﻿namespace Theatre.ConsoleClient
{
    #region Usings

    using System;
    using System.IO.Abstractions;
    using System.Threading;

    using Akka.Actor;
    using Akka.DI.AutoFac;
    using Akka.DI.Core;

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
            builder.RegisterType<DirectoryReader>().As<DirectoryReader>();
            builder.RegisterType<FileReader>().As<FileReader>();

            var container = builder.Build();

            var windowThread = new Thread(ShowWindow);
            windowThread.Start();

            using (var scope = container.BeginLifetimeScope())
            {
                var propsResolver = new AutoFacDependencyResolver(container, actorSystem);
                var reader = actorSystem.ActorOf(actorSystem.DI().Props<DirectoryReader>(), "RootDirectoryReader");
                reader.Tell(new HashDirectory("C:\\Temp"));

                actorSystem.AwaitTermination();
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static void ShowWindow()
        {
            var window = new MainWindow();
            window.ShowDialog();
        }
    }
}