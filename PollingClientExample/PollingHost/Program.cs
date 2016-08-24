namespace PollingHost
{
    using System;

    using Messages.Events;

    using MongoDB.Bson.Serialization;

    using NEventStore;
    using NEventStore.Client;
    using NEventStore.Dispatcher;
    using NEventStore.Serialization;

    using PollingHost.Observers;
    using PollingHost.Repositories;

    class Program
    {
        private static readonly CheckpointRepository CheckpointRepository = new CheckpointRepository();

        private static void Main(string[] args)
        {
            using (var store = WireupEventStore())
            {
                var client = new PollingClient(store.Advanced);

                using (IObserveCommits observeCommits = client.ObserveFrom(CheckpointRepository.GetCheckpoint()))
                {
                    using (observeCommits.Subscribe(new ReadModelCommitObserver(new DelegateMessageDispatcher(DispatchCommit))))
                    {
                        observeCommits.Start();

                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Polling...");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.ReadLine();
                    }
                }
            }
        }

        private static void DispatchCommit(ICommit commit)
        {
            try
            {
                foreach (var @event in commit.Events)
                {
                    Console.WriteLine("Message dispatched");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to dispatch");
            }

            CheckpointRepository.CommitCheckpoint(commit.CheckpointToken);
        }

        private static IStoreEvents WireupEventStore()
        {
            BsonClassMap.RegisterClassMap<SomeDomainEvent>();

            return
                Wireup.Init()
                    .LogToOutputWindow()
                    .UsingMongoPersistence("NEventStore.MongoDB", new DocumentObjectSerializer())
                    .InitializeStorageEngine()
                    .UsingJsonSerialization()
                    .Compress()
                    .Build();
        }
    }
}