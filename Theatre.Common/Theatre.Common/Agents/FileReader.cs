using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Akka.Actor;
using Theatre.Common.Messages;

namespace Theatre.Common.Agents
{
    public class FileReader : ReceiveActor
    {
        private IActorRef _databaser;

        public FileReader(IActorRef databaser)
        {
            this._databaser = databaser;
            ReceiveAsync<HashFile>(message => ProcessFile(message.FullPath));
        }

        private async Task ProcessFile(string fullPath)
        {
            if (File.Exists(fullPath))
            {
                var byteArray = File.ReadAllBytes(fullPath);

                using (var md5Hash = MD5.Create())
                {
                    var hash = md5Hash.ComputeHash(byteArray);
                    _databaser.Tell(new FileHashed(hash, File.GetCreationTimeUtc(fullPath), File.GetLastWriteTimeUtc(fullPath), fullPath));
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

    }
}