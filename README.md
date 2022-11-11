# SimpleScheduler
A simple scheduler for C#, similar to the WPF Dispatcher.

This scheduler has support for prioritized tasks.

It also adds 2 Extension methods to the CancellationToken, so that token can be used for delays (which can be canceled by stopping the scheduler)


`token.Wait(time_in_milliseconds)`

Waits for `token` and any amount of other handles, like `ManualResetEvent` and `AutoResetEvent`.
`token.WaitAny(other_wait_handle, optional_second_handle, ...)`

Examples:

```cs

public void MyTask(CancellationToken token)
{
    // do intense work

    if(token.IsCancellationRequested) return;

    // do some follow-up work
}

// ---
var scheduler = new Scheduler();
scheduler.Start();

// ---

public void ButtonClick()
{
    scheduler.Schedule(MyTask, Priority.BelowNormal);
}
```

```cs 
var scheduler = new Scheduler();
scheduler.Schedule(token => token.Wait(200));
scheduler.Schedule(() => Console.Writeline("after 200ms"));

scheduler.Start();
// tasks can be scheduled while the scheduler isn't running.
// they will be picked up as soon as Start() is called.
```

```cs
var scheduler = new Scheduler();
scheduler.Start();

scheduler.Schedule(token => token.Wait(10 * 1000));

// waits for all scheduled tasks (from before Stop was called) to be finished before killing the scheduler thread.
scheduler.Stop();

// alternatively: cancels the token, waits for the current task to finish but does not start any other tasks in the queue.
scheduler.Stop(true);
```

```cs
var mre  = new ManualResetEvent(false);
var mre2 = new ManualResetEvent(false);
// ---
var scheduler = new Scheduler();
scheduler.Start();

// waits for the token, mre OR mre2 to be signaled.
scheduler.Schedule(token => token.WaitAny(mre, mre2));
```

```cs

var scheduler = new Scheduler();
scheduler.Schedule(() => Console.WriteLine("ran last"), Priority.Lowest);
scheduler.Schedule(() => Console.WriteLine("ran second"))
scheduler.Schedule(() => Console.WriteLine("ran first"), Priority.Highest);

scheduler.Start();
```