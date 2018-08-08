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
            this.Log.Saving(id);
            var file = this.GetFileInfo(id);
            this.Store.WriteAllText(file.FullName, message);
            this.Cache.AddOrUpdate(id, message);
            this.Log.Saved(id);
        }

        public Maybe<string> Read(int id)
        {
            this.Log.Reading(id);
            var file = this.GetFileInfo(id);
            if (!file.Exists)
            {
                this.Log.DidNotFind(id);
                return new Maybe<string>();
            }
            var message = this.Cache.GetOrAdd(
                id, _ => this.Store.ReadAllText(file.FullName));
            this.Log.Returning(id);
            return new Maybe<string>(message);
        }

        public FileInfo GetFileInfo(int id)
        {
            return this.Store.GetFileInfo(
                id, this.WorkingDirectory.FullName);
        }

        protected virtual FileStore Store
        {
            get { return this.fileStore; }
        }

        protected virtual StoreCache Cache
        {
            get { return this.cache; }
        }

        protected virtual StoreLogger Log
        {
            get { return this.log; }
        }
    }
}
