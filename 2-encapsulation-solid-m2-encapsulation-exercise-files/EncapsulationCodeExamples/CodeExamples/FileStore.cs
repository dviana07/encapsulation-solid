using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ploeh.Samples.Encapsulation.CodeExamples
{
    public class FileStore
    {
        public FileStore(string workingDirectory)
        {
            if (workingDirectory == null)
                throw new ArgumentNullException("workingDirectory");
            if (!Directory.Exists(workingDirectory))
                throw new ArgumentException("Boo", "workingDirectory");

            this.WorkingDirectory = workingDirectory;
        }

        public string WorkingDirectory { get; private set; }

        public void Save(int id, string message)
        {
            var path = this.GetFileName(id);
            File.WriteAllText(path, message);
        }

        public Maybe<string> Read(int id)
        {
            var path = this.GetFileName(id);
            if (!File.Exists(path))
                return new Maybe<string>();
            var message = File.ReadAllText(path);
            return new Maybe<string>(message);
        }

        public string GetFileName(int id)
        {
            return Path.Combine(this.WorkingDirectory, id + ".txt");
        }
    }
}
