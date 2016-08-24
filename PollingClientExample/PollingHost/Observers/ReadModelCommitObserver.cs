namespace PollingHost.Observers
{
    using System;

    using NEventStore;
    using NEventStore.Dispatcher;

    public class ReadModelCommitObserver : IObserver<ICommit>
    {
        private readonly IDispatchCommits dispatcher;

        public ReadModelCommitObserver(IDispatchCommits dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public void OnCompleted()
        {
            Console.WriteLine("commit observation completed.");
        }

        public void OnError(Exception error)
        {
            Console.WriteLine("Exception from ReadModelCommitObserver: {0}", error.Message);
            throw error;
        }

        public void OnNext(ICommit commit)
        {
            this.dispatcher.Dispatch(commit);
        }
    }
}