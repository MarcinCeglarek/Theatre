using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Theatre.Common.Messages;

namespace Theatre.Common.Agents
{
    public class FileReader : ReceiveActor
    {
        private readonly IActorRef _databaser;
        private readonly ILoggingAdapter _logging;

        public FileReader(IActorRef databaser)
        {
            this._logging = Context.GetLogger();
            this._databaser = databaser;
            ReceiveAsync<HashFile>(message => ProcessFile(message.FullPath));
        }

        private async Task ProcessFile(string fullPath)
        {
            _logging.Info("Processing {0}", fullPath);
            if (File.Exists(fullPath))
            {
                var fileInfo = new FileInfo(fullPath);
                _logging.Info("Reading {0}", fullPath);
                var byteArray = await ReadAllFileAsync(fullPath);

                using (var md5Hash = MD5.Create())
                {
                    var hash = md5Hash.ComputeHash(byteArray);
                    _logging.Info("Finishing {0}", fullPath);
                    _databaser.Tell(new FileHashed(fullPath,  fileInfo.Length, hash, fileInfo.CreationTimeUtc, fileInfo.LastWriteTimeUtc));
                }
            }
            else
            {
                _logging.Warning("{0}: File not found", fullPath);
            }
        }

        static async Task<byte[]> ReadAllFileAsync(string filename)
        {
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                var buff = new byte[file.Length];
                await file.ReadAsync(buff, 0, (int)file.Length);
                return buff;
            }
        }
    }
}