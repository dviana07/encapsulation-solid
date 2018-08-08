using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploeh.Samples.Encapsulation.CodeExamples
{
    public class SqlStore : IStore
    {
        public void WriteAllText(int id, string message)
        {
            // Write to database here
        }

        public Maybe<string> ReadAllText(int id)
        {
            // Read from database here
            return new Maybe<string>();
        }

        public FileInfo GetFileInfo(int id)
        {
            throw new NotImplementedException();
        }
    }
}
