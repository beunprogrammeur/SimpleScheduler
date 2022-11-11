namespace Scheduler
{
    public static class Extensions
    {
        /// <summary>
        /// Wait for a duration (or the token is canceled)
        /// </summary>
        /// <param name="token">token to wait for (or time, which ever takes longer)</param>
        /// <param name="ms">time to wait in milliseconds</param>
        public static void Wait(this CancellationToken token, int ms)
        {
            token.WaitHandle.WaitOne(ms);
        }

        /// <summary>
        /// Waits untill any of the given handles (or this token) raises a signal
        /// </summary>
        /// <param name="token">token to wait for</param>
        /// <param name="handles">waithandles to wait for</param>
        public static void WaitAny(this CancellationToken token, params WaitHandle[] handles)
        {
            var allHandles = new List<WaitHandle> { token.WaitHandle };
            allHandles.AddRange(handles);
            WaitHandle.WaitAny(allHandles.ToArray());
        }
    }
}
