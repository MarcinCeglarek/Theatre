namespace Theatre.Common.Actors
{
    #region Usings

    using System.IO.Abstractions;
    using System.Security.Cryptography;

    using Akka.Actor;
    using Akka.Event;

    using Theatre.Common.Messages;

    #endregion

    public class FileReader : ReceiveActor
    {
        private readonly IFileSystem fileSystem;

        private readonly ILoggingAdapter logging;

        public FileReader(IFileSystem fileSystem)
        {
            this.logging = Context.GetLogger();
            this.fileSystem = fileSystem;
            this.Receive<HashFile>(message => this.ProcessFile(message.FullPath));
        }

        private void ProcessFile(string fullPath)
        {
            this.logging.Info("Processing {0}", fullPath);
            if (this.fileSystem.File.Exists(fullPath))
            {
                this.logging.Debug("Reading {0}", fullPath);
                var byteArray = this.fileSystem.File.ReadAllBytes(fullPath);

                var fileSize = byteArray.Length;

                using (var md5Hash = MD5.Create())
                {
                    var hash = md5Hash.ComputeHash(byteArray);
                    this.logging.Info("Finishing {0}", fullPath);
                    Context.Parent.Tell(
                        new FileHashed(
                            fullPath, 
                            fileSize, 
                            hash, 
                            this.fileSystem.File.GetCreationTimeUtc(fullPath), 
                            this.fileSystem.File.GetLastWriteTimeUtc(fullPath)));
                }
            }
            else
            {
                this.logging.Warning("{0}: File not found", fullPath);
            }
        }
    }
}