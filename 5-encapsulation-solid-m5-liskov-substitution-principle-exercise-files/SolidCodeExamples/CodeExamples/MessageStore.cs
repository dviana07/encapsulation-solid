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
        private readonly IStoreCache cache;
        private readonly StoreLogger log;
        private readonly IStore fileStore;

        public MessageStore(DirectoryInfo workingDirectory)
        {
            this.WorkingDirectory = workingDirectory;
            this.cache = new StoreCache();
            this.log = new StoreLogger();
            this.fileStore = new FileStore(workingDirectory);
        }

        public DirectoryInfo WorkingDirectory { get; private set; }

        public void Save(int id, string message)
        {
            this.Log.Saving(id);
            this.Store.WriteAllText(id, message);
            this.Cache.AddOrUpdate(id, message);
            this.Log.Saved(id);
        }

        public Maybe<string> Read(int id)
        {
            this.Log.Reading(id);
            var message = this.Cache.GetOrAdd(
                id, _ => this.Store.ReadAllText(id));
            if (message.Any())
                this.Log.Returning(id);
            else
                this.Log.DidNotFind(id);
            return message;
        }

        public FileInfo GetFileInfo(int id)
        {
            return this.Store.GetFileInfo(id);
        }

        protected virtual IStore Store
        {
            get { return this.fileStore; }
        }

        protected virtual IStoreCache Cache
        {
            get { return this.cache; }
        }

        protected virtual StoreLogger Log
        {
            get { return this.log; }
        }
    }
}
