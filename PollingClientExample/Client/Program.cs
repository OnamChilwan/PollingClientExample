namespace Client
{
    using System;

    using Messages.Events;

    using NEventStore;
    using NEventStore.Serialization;

    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Enter 'x' to exit application.");

            while (Console.ReadLine() != "x")
            {
                using (var store = WireupEventStore())
                {
                    var streamId = Guid.NewGuid();

                    using (var stream = store.OpenStream(streamId, 0, int.MaxValue))
                    {
                        var @event = new SomeDomainEvent(Guid.NewGuid().ToString("N"));

                        stream.Add(new EventMessage { Body = @event });
                        stream.CommitChanges(Guid.NewGuid());

                        Console.WriteLine("Event committed..");
                    }
                }
            }
        }

        private static IStoreEvents WireupEventStore()
        {
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