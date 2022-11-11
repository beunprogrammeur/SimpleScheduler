namespace Scheduler
{
    public class BackgroundTask : IDisposable
    {

        public BackgroundTask(string name = $"{nameof(Scheduler)}.{nameof(BackgroundTask)}")
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool Start()
        {
            throw new NotImplementedException();
        }

        public bool Stop()
        {
            throw new NotImplementedException();

        }
    }
}
