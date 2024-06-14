using NUnit.Framework;
using SlugEnt;
using SlugEnt.ProcessQueueManager;
using System;
using System.Threading;

namespace Test_ConcurrentEngine
{
    [TestFixture]
    public class Test_ConcurrentEngine
    {
        [Test]
        public void AddTaskMethod_Success()
        {
            ConcurrentEngine engine = new ConcurrentEngine();
            engine.SleepTimeMS = 60;
            DayTimeInterval dayTimeInterval = new DayTimeInterval("3am", "10pm");
            TimeUnit        checkInterval   = new TimeUnit("1m");
            PeriodicJob jobA = new PeriodicJob("JobA",
                                               JobMethod_A,
                                               dayTimeInterval,
                                               checkInterval,
                                               engine.AddTask);

            //TODO This is where I am at.
            Assert.AreEqual(0, engine.JobCount, "A10:");
            engine.AddJob(jobA);

            Assert.AreEqual(1, engine.JobCount, "A20:");
            Assert.AreEqual(EnumConcurrentEngineStatus.Stopped, engine.Status, "A30:");

            // Start the engine 
            engine.Start();

            Assert.AreEqual(EnumConcurrentEngineStatus.Running, engine.Status, "A40:");
            Thread.Sleep(500);
            Assert.AreEqual(1, engine.TasksAdded, "A100:");
            Assert.AreEqual(1, engine.TasksCompleted, "A110:");

            engine.Stop();

            Thread.Sleep(2000);
            Assert.AreEqual(EnumConcurrentEngineStatus.Stopped, engine.Status, "A999:");
        }


        [Test]
        public void AddMultipleTaskMethod_Success()
        {
            ConcurrentEngine engine = new ConcurrentEngine();
            engine.SleepTimeMS = 60;
            DayTimeInterval dayTimeInterval = new DayTimeInterval("3am", "10pm");
            TimeUnit        checkInterval   = new TimeUnit("1m");
            PeriodicJob jobA = new PeriodicJob("JobA",
                                               JobMethod_A,
                                               dayTimeInterval,
                                               checkInterval,
                                               engine.AddTask);

            //TODO This is where I am at.
            Assert.AreEqual(0, engine.JobCount, "A10:");
            engine.AddJob(jobA);

            Assert.AreEqual(1, engine.JobCount, "A20:");
            Assert.AreEqual(EnumConcurrentEngineStatus.Stopped, engine.Status, "A30:");

            // Start the engine 
            engine.Start();

            Assert.AreEqual(EnumConcurrentEngineStatus.Running, engine.Status, "A40:");
            Thread.Sleep(500);
            Assert.AreEqual(1, engine.TasksAdded, "A100:");
            Assert.AreEqual(1, engine.TasksCompleted, "A110:");

            engine.Stop();

            Thread.Sleep(2000);
            Assert.AreEqual(EnumConcurrentEngineStatus.Stopped, engine.Status, "A999:");
        }


        [Test]
        public void AddJob()
        {
            ConcurrentEngine engine = new ConcurrentEngine();
            engine.SleepTimeMS = 60;
/*            DayTimeInterval dayTimeInterval = new DayTimeInterval("3am", "10pm");
            TimeUnit        checkInterval   = new TimeUnit("1m");
            PeriodicJob jobA = new PeriodicJob("JobA",
                                               JobMethod_A,
                                               dayTimeInterval,
                                               checkInterval,
                                               engine.AddTask);*/
            engine.AddNewJob("test1",
                             JobMethod_A,
                             "0:00",
                             "23:59",
                             "10s");
        }


        /// <summary>
        /// The method run when Periodic Job A is called.
        /// </summary>
        /// <param name="addTaskMethod"></param>
        /// <returns></returns>
        public bool JobMethod_A(Action<ProcessingTask> addTaskMethod)
        {
            int i = 22;
            ProcessingTask task = new ProcessingTask("TaskA",
                                                     1,
                                                     EnumProcessingTaskSpeed.Fast,
                                                     TaskA);
            addTaskMethod(task);
            return true;
        }


        public bool TaskA(object a)
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("i = {0}", i);
            }

            return true;
        }
    }
}