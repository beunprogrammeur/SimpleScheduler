namespace Scheduler
{
    public class BackgroundTask : IDisposable
    {
        private Action<CancellationToken> _method;
        private string _name;
        private Thread? _thread;
        private CancellationTokenSource? _tokenSource;

        public BackgroundTask(Action<CancellationToken> method, string name = $"{nameof(Scheduler)}.{nameof(BackgroundTask)}")
        {
            _method = method;
            _name = name;
        }

        public void Dispose()
        {
            Stop();
        }

        public bool Start(ThreadPriority priority = ThreadPriority.Normal)
        {
            if (_thread?.IsAlive ?? false) return false;
            _tokenSource = new CancellationTokenSource();    
            _thread = new Thread(() => _method(_tokenSource.Token)) { IsBackground = true, Priority = priority, Name = _name };
            _thread.Start();
            return true;
        }

        public void Stop()
        {
            _tokenSource?.Cancel();
            _thread?.Join();
            _tokenSource?.Dispose();

            _tokenSource = null;
            _thread = null;
        }
    }
}
