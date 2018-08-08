using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Ploeh.Samples.Encapsulation.CodeExamples
{
    public class LogSavingStoreWriter : IStoreWriter
    {
        public void Save(int id, string message)
        {
            Log.Information("Saving message {id}.", id);
        }
    }
}
