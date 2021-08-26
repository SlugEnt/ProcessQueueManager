using NUnit.Framework;
using SlugEnt;
using SlugEnt.ProcessQueueManager;
using System;

namespace Test_ConcurrentEngine
{
    [TestFixture]
    class Test_PeriodicJob
    {
        [Test]
        public void NextRunTime_InitialStartupWithinInterval ()
        {
            // Prep
            // Set an interval based upon current time.
            DateTimeOffset current = DateTime.Now;
            string priorHour = current.AddHours(-1).ToString("h tt"); //current.AddHours(-1).Hour;
            string nextHour = current.AddHours(1).ToString("h tt");
            DayTimeInterval dayTimeInterval= new DayTimeInterval(priorHour,nextHour);

            TimeUnit checkInterval = new TimeUnit("1h");


            // Test 1:
            PeriodicJob jobA = new PeriodicJob("JobA", JobMethodA_ReturnTrue, dayTimeInterval, checkInterval, null);
            jobA.SetNextRunTime();

            // Validate
            Assert.NotZero(jobA.Id, "A10:");

            DateTimeOffset mostRecent = DateTimeOffset.Now;
            Assert.Greater(mostRecent,jobA.NextRunTime,"A20:  JobA appears to have an incorrect future NextRun date.");
            Assert.Greater(jobA.NextRunTime,current,"A30:  JobA appears to have an incorrect future NextRun date that is in the past.");
            
        }


        [Test]
        public void NextRunTime_InitialStartupOutsideInterval()
        {
            // Prep
            // Set an interval based upon current time.
            DateTimeOffset current = DateTime.Now;
            DateTimeOffset startDateTimeOffset = current.AddHours(3);
            string startHour = startDateTimeOffset.ToString("h tt");
            int startHourNumeric = startDateTimeOffset.Hour;

            string endHour = current.AddHours(5).ToString("h tt");
            DayTimeInterval dayTimeInterval = new DayTimeInterval(startHour, endHour);

            TimeUnit checkInterval = new TimeUnit("1h");


            // Test 1:
            PeriodicJob jobA = new PeriodicJob("JobA", JobMethodA_ReturnTrue, dayTimeInterval, checkInterval,null);
            jobA.SetNextRunTime();

            // Validate
            Assert.NotZero(jobA.Id, "A10:");

            DateTimeOffset mostRecent = DateTimeOffset.Now;

            // Get midnight of next day.
            DateTimeOffset midnightNextDay = new DateTimeOffset(current.Year, current.Month, current.Day, 0, 0, 0, current.Offset).AddDays(1);

            Assert.Greater(jobA.NextRunTime, midnightNextDay, "A20:  JobA appears to have an incorrect future NextRun date that is in the past.");
            Assert.AreEqual(startHourNumeric,jobA.NextRunTime.Hour,"A30: Next Run Start hour should be the same as Run interval start hour");
        }


        /// <summary>
        /// Tests the IsTimeToRun method
        /// </summary>
        [Test]
        public void IsTimeToRun_Success () {
            // Prep
            // Set an interval based upon current time.
            DateTimeOffset current = DateTime.Now;
            DateTimeOffset startDateTimeOffset = current;
            string startHour = startDateTimeOffset.ToString("h tt");
            int startHourNumeric = startDateTimeOffset.Hour;

            string endHour = current.AddHours(5).ToString("h tt");
            DayTimeInterval dayTimeInterval = new DayTimeInterval(startHour, endHour);

            TimeUnit checkInterval = new TimeUnit("1h");


            // Test 1:
            PeriodicJob jobA = new PeriodicJob("JobA", JobMethodA_ReturnTrue, dayTimeInterval, checkInterval,null);
            jobA.SetNextRunTime();

            Assert.IsTrue(jobA.IsTimeToRun(),"A10:");
        }




        /// <summary>
        /// Tests the IsTimeToRun method
        /// </summary>
        [Test]
        [TestCase(-4,-1, Description = "Interval is before current time")]
        [TestCase(4, 6,Description = "Interval is after current time")]

        public void IsTimeToRun_ReturnsFalse(int startHours, int EndHours)
        {
            // Prep
            // Set an interval based upon current time.
            DateTimeOffset current = DateTime.Now;
            DateTimeOffset startDateTimeOffset = current.AddHours(3);
            string startHour = startDateTimeOffset.ToString("h tt");
            int startHourNumeric = startDateTimeOffset.Hour;

            string endHour = current.AddHours(5).ToString("h tt");
            DayTimeInterval dayTimeInterval = new DayTimeInterval(startHour, endHour);

            TimeUnit checkInterval = new TimeUnit("1h");


            // Test 1:
            PeriodicJob jobA = new PeriodicJob("JobA", JobMethodA_ReturnTrue, dayTimeInterval, checkInterval,null);
            jobA.SetNextRunTime();

            Assert.IsFalse(jobA.IsTimeToRun(), "A10:");
        }



        /// <summary>
        /// Sample Method run by some of the jobs
        /// </summary>
        /// <returns></returns>
        private bool JobMethodA_ReturnTrue (Action<ProcessingTask> addTaskMethod) {
            int j = 1;
            return true;
        }
    }
}
