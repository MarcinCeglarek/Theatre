using System.IO.Abstractions;
using System.Security.Cryptography;
using Akka.Actor;
using Akka.Event;
using Theatre.Common.Messages;

namespace Theatre.Common.Agents
{
    public class FileReader : ReceiveActor
    {
        private readonly IActorRef _databaser;
        private readonly IFileSystem _fileSystem;
        private readonly ILoggingAdapter _logging;

        public FileReader(IActorRef databaser, IFileSystem fileSystem)
        {
            _logging = Context.GetLogger();
            _databaser = databaser;
            _fileSystem = fileSystem;
            Receive<HashFile>(message => ProcessFile(message.FullPath));
        }

        private void ProcessFile(string fullPath)
        {
            _logging.Info("Processing {0}", fullPath);
            if (_fileSystem.File.Exists(fullPath))
            {
                _logging.Info("Reading {0}", fullPath);
                var byteArray = _fileSystem.File.ReadAllBytes(fullPath);

                var fileSize = byteArray.Length;

                using (var md5Hash = MD5.Create())
                {
                    var hash = md5Hash.ComputeHash(byteArray);
                    _logging.Info("Finishing {0}", fullPath);
                    _databaser.Tell(new FileHashed(fullPath, fileSize, hash,
                        _fileSystem.File.GetCreationTimeUtc(fullPath),
                        _fileSystem.File.GetLastWriteTimeUtc(fullPath)));
                }
            }
            else
            {
                _logging.Warning("{0}: File not found", fullPath);
            }
        }
    }
}