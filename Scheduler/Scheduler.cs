namespace Scheduler
{
    public enum Priority
    {
        Highest,
        High,
        AboveNormal,
        Normal,
        BelowNormal,
        Low,
        Lowest
    }
    public class Scheduler : IDisposable
    {
        private string _name;

        /// <summary>
        /// Creates a simple scheduler.
        /// </summary>
        /// <param name="name">name of the background thread (useful for debugging)</param>
        public Scheduler(string name = $"{nameof(Scheduler)}.{nameof(WorkerThread)}")
        {
            _name = name;
        }

        public void Dispose()
        {
            Stop();
        }

        public bool Start()
        {
            throw new NotImplementedException();
        }

        public bool Stop()
        {
            throw new NotImplementedException();
        }

        public bool Schedule(Action method, Priority priority = Priority.Normal) => Schedule((token) => method(), priority);

        public bool Schedule(Action<CancellationToken> method, Priority priority = Priority.Normal)
        {
            throw new NotImplementedException();
        }

        private void WorkerThread(CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}