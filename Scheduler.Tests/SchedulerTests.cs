using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Scheduler.Tests
{
    [TestClass]
    public class SchedulerTests
    {
        [TestMethod, Timeout(250)]
        public void ScheduledTaskIsExecutedOnlyAfterSchedulerIsStartedTest()
        {
            using var mre = new ManualResetEvent(false);
            using var scheduler = new Scheduler();
            scheduler.Schedule(token => mre.Set());

            bool signaled = mre.WaitOne(50);
            Assert.IsFalse(signaled);

            scheduler.Start();
            signaled = mre.WaitOne();
            Assert.IsTrue(signaled);
        }

        [TestMethod, Timeout(100)]
        public void SchedulerPriorityIsTakenIntoAccountTest()
        {
            var results = new List<string>();
            using var mre = new ManualResetEvent(false);
            using var scheduler = new Scheduler();
            scheduler.Schedule(token => results.Add("low_prio"), Priority.Low);
            scheduler.Schedule(token => results.Add("high_prio"), Priority.High);
            scheduler.Schedule(token => mre.Set());
            scheduler.Start();
            mre.WaitOne(50);

            Assert.AreEqual("high_prio", results.First());
            Assert.AreEqual("low_prio", results.Last());
        }

        [TestMethod, Timeout(50)]
        public void TaskIsCancelableTest()
        {
            using var scheduler = new Scheduler();
            scheduler.Schedule(token => token.Wait(1000));
            scheduler.Start();
            scheduler.Stop(true);
        }

        [TestMethod,Timeout(100)]
        [DataRow(true, 0)]
        [DataRow(false, 3)]
        public void ForceCancelAndGracefulCancelWorkAccordinglyTest(bool force, int expectedCount)
        {
            int counter = 0;
            using var scheduler = new Scheduler();
            scheduler.Schedule(token => token.Wait(50));
            scheduler.Schedule(token => counter++);
            scheduler.Schedule(token => counter++);
            scheduler.Schedule(token => counter++);
            scheduler.Start();
            scheduler.Stop(force);

            Assert.AreEqual(expectedCount, counter);
        }

        [TestMethod]
        public void RestartingSchedulerTest()
        {
            int counter = 0;
            using var scheduler = new Scheduler();
            scheduler.Schedule(token => counter++);

            bool stoppedBefore = false;
            scheduler.Stopped += (sender, args) => { 
                if(!stoppedBefore)
                {
                    stoppedBefore = true;
                    return;
                }

                Assert.AreEqual(2, counter);
            };
            
            scheduler.Start();
            scheduler.Start();
        }
    }
}