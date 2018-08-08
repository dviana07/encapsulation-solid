using System;
namespace Ploeh.Samples.Encapsulation.CodeExamples
{
    public interface IStoreCache
    {
        void Save(int id, string message);

        Maybe<string> GetOrAdd(int id, Func<int, Maybe<string>> messageFactory);
    }
}
