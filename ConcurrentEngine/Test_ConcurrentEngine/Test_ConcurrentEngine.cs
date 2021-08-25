using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SlugEnt;
using Slugent.ProcessQueueManager;

namespace Test_ConcurrentEngine
{
    [TestFixture]
    public class Test_ConcurrentEngine
    {
        [Test]
        public void AddTaskMethod_Success () {
            ConcurrentEngine engine = new ConcurrentEngine();
            DayTimeInterval dayTimeInterval = new DayTimeInterval("3am", "10pm");
            TimeUnit checkInterval = new TimeUnit("1m");
            PeriodicJob jobA = new PeriodicJob("JobA", JobMethod_A, dayTimeInterval, checkInterval, engine.AddTask);

            //TODO This is where I am at.
        }


        
        /// <summary>
        /// The method run when Periodic Job A is called.
        /// </summary>
        /// <param name="addTaskMethod"></param>
        /// <returns></returns>
        public static bool JobMethod_A(Action<ProcessingTask> addTaskMethod)
        {
            int i = 22;
            return true;
        }
    }
}
