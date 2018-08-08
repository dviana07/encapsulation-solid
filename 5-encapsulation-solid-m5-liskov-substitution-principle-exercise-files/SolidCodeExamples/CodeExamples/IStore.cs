using System;
using System.IO;

namespace Ploeh.Samples.Encapsulation.CodeExamples
{
    public interface IStore
    {
        void WriteAllText(int id, string message);

        Maybe<string> ReadAllText(int id);

        FileInfo GetFileInfo(int id);
    }
}
