using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.Encapsulation.CodeExamples
{
    public class MessageStore
    {
        private readonly StoreCache cache;
        private readonly StoreLogger log;
        private readonly FileStore fileStore;

        public MessageStore(DirectoryInfo workingDirectory)
        {
            if (workingDirectory == null)
                throw new ArgumentNullException("workingDirectory");
            if (!workingDirectory.Exists)
                throw new ArgumentException("Boo", "workingDirectory");

            this.WorkingDirectory = workingDirectory;
            this.cache = new StoreCache();
            this.log = new StoreLogger();
            this.fileStore = new FileStore();
        }

        public DirectoryInfo WorkingDirectory { get; private set; }

        public void Save(int id, string message)
        {
            this.log.Saving(id);
            var file = this.GetFileInfo(id);
            this.fileStore.WriteAllText(file.FullName, message);
            this.cache.AddOrUpdate(id, message);
            this.log.Saved(id);
        }

        public Maybe<string> Read(int id)
        {
            this.log.Reading(id);
            var file = this.GetFileInfo(id);
            if (!file.Exists)
            {
                this.log.DidNotFind(id);
                return new Maybe<string>();
            }
            var message = this.cache.GetOrAdd(
                id, _ => this.fileStore.ReadAllText(file.FullName));
            this.log.Returning(id);
            return new Maybe<string>(message);
        }

        public FileInfo GetFileInfo(int id)
        {
            return this.fileStore.GetFileInfo(
                id, this.WorkingDirectory.FullName);
        }
    }
}
