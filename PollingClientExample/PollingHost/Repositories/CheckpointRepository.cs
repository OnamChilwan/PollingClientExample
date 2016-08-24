namespace PollingHost.Repositories
{
    using System;
    using System.Linq;
    using System.Configuration;

    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;

    using PollingHost.Documents;

    public class CheckpointRepository
    {
        private readonly MongoCollection<Checkpoint> collection;

        public CheckpointRepository()
        {
            var client = new MongoClient(ConfigurationManager.ConnectionStrings["Mongo"].ConnectionString);
            var database = client.GetServer().GetDatabase("PollingClient");
            this.collection = database.GetCollection<Checkpoint>("Checkpoints");
        }

        public void CommitCheckpoint(string checkpoint)
        {
            this.collection.Insert(new Checkpoint { Value = checkpoint, Id = ObjectId.GenerateNewId() });
            Console.WriteLine("Checkpoint token = {0}", checkpoint);
        }

        public string GetCheckpoint()
        {
            var checkpoint = this.collection.Find(Query.Null).LastOrDefault();

            return checkpoint?.Value;
        }
    }
}