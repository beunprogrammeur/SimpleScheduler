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

    public enum State
    {
        Inactive,
        Idle,
        Running,
        Stopping,
        ForcefulStopping
    }
    public class Scheduler : IDisposable
    {
        private BackgroundTask _task;
        private PriorityQueue<Action<CancellationToken>, Priority> _queue;
        private object _lock;
        private ManualResetEvent _manualResetEvent;
        private State _state;

        public event EventHandler Stopped;
        
        public State State 
        { 
            get { lock (_lock) { return _state;  } } 
            set { lock (_lock) { _state = value; } }
        }

        /// <summary>
        /// Creates a simple scheduler.
        /// </summary>
        /// <param name="name">name of the background thread (useful for debugging)</param>
        public Scheduler(string name = $"{nameof(Scheduler)}.{nameof(WorkerThread)}")
        {
            _task = new BackgroundTask(WorkerThread, name);
            _queue = new PriorityQueue<Action<CancellationToken>, Priority>();
            _lock = new object();
            _manualResetEvent = new ManualResetEvent(false);
        }

        public void Dispose()
        {
            Stop();
            _task.Dispose();
        }

        public bool Start()
        {
            if (_task.IsAlive) return false;

            _task.Start();
            return true;
        }

        public bool Stop(bool force = false)
        {
            if (!_task.IsAlive) return false;

            if (force) ForcefulStop();
            else       GracefulStop();

            return true;
        }

        private void ForcefulStop()
        {
            State = State.ForcefulStopping;
            _task.Stop();
            Stopped?.Invoke(this, EventArgs.Empty);
        }

        private void GracefulStop()
        {
            State = State.Stopping;
            using var are = new AutoResetEvent(false);
            Schedule(token => are.Set(), Priority.Lowest);
            are.WaitOne();
            ForcefulStop();
        }

        /// <summary>
        /// Schedules a task on the scheduler.
        /// </summary>
        /// <param name="method">the task to schedule</param>
        /// <param name="priority">higher priority tasks will launch before lower priority tasks</param>
        public void Schedule(Action method, Priority priority = Priority.Normal) => Schedule((token) => method(), priority);

        public void Schedule(Action<CancellationToken> method, Priority priority = Priority.Normal)
        {
            lock (_lock)
            {
                _queue.Enqueue(method, priority);
                _manualResetEvent.Set();
            }
        }

        public Action<CancellationToken>? Dequeue()
        {
            lock( _lock)
            {
                if(_queue.TryDequeue(out var method, out var priority))
                {
                    return method;
                }
                _manualResetEvent.Reset();
                return null;
            }
        }

        private void WorkerThread(CancellationToken token)
        {
            State = State.Idle;

            while(!token.IsCancellationRequested)
            {
                Action<CancellationToken>? method = null;
                while((method = Dequeue()) != null)
                {
                    if (State == State.ForcefulStopping) break;
                    if (State != State.Stopping && State != State.ForcefulStopping) State = State.Running;

                    method(token);
                }

                if (State != State.Stopping) State = State.Idle;
                token.WaitAny(_manualResetEvent);
            }
        }
    }
}