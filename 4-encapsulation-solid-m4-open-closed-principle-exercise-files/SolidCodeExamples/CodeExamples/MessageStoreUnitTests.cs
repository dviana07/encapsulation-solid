using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture.Xunit;
using Serilog;
using Serilog.Events;
using Xunit;
using Xunit.Extensions;

namespace Ploeh.Samples.Encapsulation.CodeExamples
{
    public class MessageStoreUnitTests
    {
        [Theory, AutoData]
        public void ReadReturnsMessage(string expected)
        {
            var fileStore = new MessageStore(new DirectoryInfo(Environment.CurrentDirectory));
            fileStore.Save(44, expected);

            var actual = fileStore.Read(44);

            Assert.Equal(expected, actual.Single());
        }

        [Theory, AutoData]
        public void GetFileFileReturnsCorrectResult(int id)
        {
            var fileStore = new MessageStore(new DirectoryInfo(Environment.CurrentDirectory));

            var actual = fileStore.GetFileInfo(id).FullName;

            var expected =
                Path.Combine(fileStore.WorkingDirectory.FullName, id + ".txt");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ConstructWithNullDirectoryThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new MessageStore(null));
        }

        [Theory, AutoData]
        public void ConstructWithInvalidDirectoryThrows(string invalidDirectory)
        {
            Assert.False(Directory.Exists(invalidDirectory));
            Assert.Throws<ArgumentException>(
                () => new MessageStore(new DirectoryInfo(invalidDirectory)));
        }

        [Theory, AutoData]
        public void ReadUsageExample(string expected)
        {
            var fileStore = new MessageStore(new DirectoryInfo(Environment.CurrentDirectory));
            fileStore.Save(49, expected);

            var message = fileStore.Read(49).DefaultIfEmpty("").Single();

            Assert.Equal(expected, message);
        }

        [Theory, AutoData]
        public void ReadExistingFileReturnsTrue(string expected)
        {
            var fileStore = new MessageStore(new DirectoryInfo(Environment.CurrentDirectory));
            fileStore.Save(50, expected);

            var actual = fileStore.Read(50);

            Assert.True(actual.Any());
            Assert.Equal(expected, actual.Single());
        }

        [Theory, AutoData]
        public void ReadNonExistingFileReturnsFalse(string expected)
        {
            var fileStore = new MessageStore(new DirectoryInfo(Environment.CurrentDirectory));

            var actual = fileStore.Read(51);

            Assert.False(actual.Any());
        }

        [Theory, AutoData]
        public void SaveLogsInformation(string message)
        {
            var spy = new SpySink();
            Log.Logger = new LoggerConfiguration().WriteTo.Sink(spy).CreateLogger();
            var fileStore = new MessageStore(new DirectoryInfo(Environment.CurrentDirectory));

            fileStore.Save(52, message);

            Assert.True(spy.Events
                .SelectMany(le => le.Properties)
                .Where(kvp => kvp.Key == "id")
                .Select(kvp => kvp.Value)
                .OfType<ScalarValue>()
                .Any(sv => sv.Value.Equals(52)));
        }

        [Theory, AutoData]
        public void ReadExistingMessageLogsCorrectDebugInformation(string message)
        {
            var fileStore = new MessageStore(new DirectoryInfo(Environment.CurrentDirectory));
            fileStore.Save(53, message);
            var spy = new SpySink();
            Log.Logger = new LoggerConfiguration().WriteTo.Sink(spy).MinimumLevel.Debug().CreateLogger();

            fileStore.Read(53);

            Assert.True(spy.Events
                .Where(le => le.MessageTemplate.Text == "Reading message {id}.")
                .SelectMany(le => le.Properties)
                .Where(kvp => kvp.Key == "id")
                .Select(kvp => kvp.Value)
                .OfType<ScalarValue>()
                .Any(sv => sv.Value.Equals(53)));
            Assert.True(spy.Events
                .Where(le => le.MessageTemplate.Text == "Returning message {id}.")
                .SelectMany(le => le.Properties)
                .Where(kvp => kvp.Key == "id")
                .Select(kvp => kvp.Value)
                .OfType<ScalarValue>()
                .Any(sv => sv.Value.Equals(53)));
        }

        [Fact]
        public void ReadNonExistingMessageLogsCorrectDebugInformation()
        {
            var spy = new SpySink();
            Log.Logger = new LoggerConfiguration().WriteTo.Sink(spy).MinimumLevel.Debug().CreateLogger();
            var fileStore = new MessageStore(new DirectoryInfo(Environment.CurrentDirectory));

            fileStore.Read(54);

            Assert.True(spy.Events
                .Where(le => le.MessageTemplate.Text == "No message {id} found.")
                .SelectMany(le => le.Properties)
                .Where(kvp => kvp.Key == "id")
                .Select(kvp => kvp.Value)
                .OfType<ScalarValue>()
                .Any(sv => sv.Value.Equals(54)));
        }

        [Theory, AutoData]
        public void ReadReadsFromCache(
            string shouldBeCached,
            string backDoorMessage)
        {
            var fileStore = new MessageStore(new DirectoryInfo(Environment.CurrentDirectory));
            fileStore.Save(55, shouldBeCached);
            fileStore.Read(55);
            var file = fileStore.GetFileInfo(55);
            File.WriteAllText(file.FullName, backDoorMessage);

            var actual = fileStore.Read(55);

            Assert.Equal(shouldBeCached, actual.Single());
        }

        [Theory, AutoData]
        public void SaveShouldInvalidateCache(string message1, string expected)
        {
            var fileStore = new MessageStore(new DirectoryInfo(Environment.CurrentDirectory));
            fileStore.Save(56, message1);
            fileStore.Read(56);
            fileStore.Save(56, expected);

            var actual = fileStore.Read(56);

            Assert.Equal(expected, actual.Single());
        }
    }
}
