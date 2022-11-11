namespace Scheduler
{
    internal interface IJob
    {
        Action<CancellationToken> Method { get; }
        Priority Priority { get; }
    }

    internal class Job : IJob
    {
        public Action<CancellationToken> Method { get; }
        public Priority Priority { get; }

        public Job(Action<CancellationToken> method, Priority priority)
        {
            Method = method;
            Priority = priority;
        }
    }
}
