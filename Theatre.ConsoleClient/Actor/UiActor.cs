namespace Theatre.ConsoleClient.Actor
{
    #region

    using System;

    using Akka.Actor;

    using Theatre.Common.Helpers;

    #endregion

    public class UiActor : ReceiveActor
    {
        public UiActor()
        {
            if (this.RootPath == null)
            {
                this.Become(this.Initializing);
            }
            else
            {
                this.Become(this.Working);
            }
        }

        private string RootPath { get; set; }

        private MainWindow MainWindow { get; set; }

        private void Initializing()
        {
            this.Receive<InitializationMessage>(message => this.ProcessInitialization(message));
        }

        private void Working()
        {
            this.Receive<OpenDirectory>(message => this.HandleOpenDirectory(message));
            this.Receive<CloseDirectory>(message => this.HandleCloseDirectory(message));
            this.Receive<ReturnDetails>(message => this.HandleReturnDetails(message));
        }

        private void ProcessInitialization(InitializationMessage message)
        {
            this.MainWindow = message.MainWindow;
            this.RootPath = message.RootPath;

            this.MainWindow.CreateRootNode(this.RootPath);
            this.Become(this.Working);
        }

        private void HandleCloseDirectory(CloseDirectory message)
        {
            throw new NotImplementedException();
        }

        private void HandleOpenDirectory(OpenDirectory message)
        {
            var path = message.Path.Remove(0, this.RootPath.Length);
            path = path.ToActorFriendlyName();
            var actors = Context.ActorSelection(path);
            actors.Tell(new GiveDetails());
        }

        private void HandleReturnDetails(ReturnDetails message)
        {
            throw new NotImplementedException();
        }
    }

    public class GiveDetails
    {
    }

    public class ReturnDetails
    {
    }

    public class InitializationMessage
    {
        public InitializationMessage(string path, MainWindow mainWindow)
        {
            this.RootPath = path;
            this.MainWindow = mainWindow;
        }

        public MainWindow MainWindow { get; }
        public string RootPath { get; }
    }

    public class CloseDirectory
    {
        public CloseDirectory(string path)
        {
            this.Path = path;
        }

        public string Path { get; }
    }

    public class OpenDirectory
    {
        public OpenDirectory(string path)
        {
            this.Path = path;
        }

        public string Path { get; }
    }
}