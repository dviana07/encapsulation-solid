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
        private readonly IStoreLogger log;
        private readonly IStore store;
        private readonly IFileLocator fileLocator;

        public MessageStore(DirectoryInfo workingDirectory)
        {
            this.WorkingDirectory = workingDirectory;
            this.cache = new StoreCache();
            this.log = new StoreLogger();
            var fileStore = new FileStore(workingDirectory);
            this.store = fileStore;
            this.fileLocator = fileStore;
        }

        public DirectoryInfo WorkingDirectory { get; private set; }

        public void Save(int id, string message)
        {
            this.Log.Saving(id, message);
            this.Store.Save(id, message);
            this.Cache.Save(id, message);
            this.Log.Saved(id, message);
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
            return this.FileLocator.GetFileInfo(id);
        }

        protected virtual IStore Store
        {
            get { return this.store; }
        }

        protected virtual IStoreCache Cache
        {
            get { return this.cache; }
        }

        protected virtual IStoreLogger Log
        {
            get { return this.log; }
        }

        protected virtual IFileLocator FileLocator
        {
            get { return this.fileLocator; }
        }
    }
}
