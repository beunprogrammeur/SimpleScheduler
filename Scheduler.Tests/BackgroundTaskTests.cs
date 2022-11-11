using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Scheduler.Tests
{
    [TestClass]
    public class BackgroundTaskTests
    {
        [TestMethod, Timeout(50)]
        public void CanRunBackgroundTaskTest()
        {
            using var mre = new ManualResetEvent(false);
            using var task = new BackgroundTask(token => {
                mre.Set();
            });

            task.Start();
            mre.WaitOne();
        }

        [TestMethod, Timeout(50)]
        public void CanCancelBackgroundTaskTest()
        {
            // cancels on dispose
            using var task = new BackgroundTask(token =>
            {
                token.Wait(1000);
            });

            task.Start();
        }

        [TestMethod]
        public void StartingTaskThatIsAlreadyStartedDoesntInterfereTest()
        {
            using var task = new BackgroundTask(token => token.Wait(1000));
            var first  = task.Start();
            var second = task.Start();

            Assert.IsTrue(first);
            Assert.IsFalse(second);
        }

        [TestMethod]
        public void StoppingStoppedTaskDoesntThrowTest()
        {
            using var task = new BackgroundTask(token => token.Wait(1000));
            task.Start();
            task.Stop();
            task.Stop();
        }
    }
}