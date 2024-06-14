using NUnit.Framework;
using SlugEnt;
using SlugEnt.ProcessQueueManager;
using System;
using ProcessQueueManager.DatetimeProvider;

namespace Test_ConcurrentEngine
{
    [TestFixture]
    class Test_PeriodicJob
    {
        /// <summary>
        /// Tests that 
        /// </summary>
        [Test]
        public void NextRunTime_InitialStartupWithinInterval()
        {
            // Prep
            // Set an interval based upon current time.
            TestingDateProvider utTimeProvider = new TestingDateProvider();
            utTimeProvider.Today_6AM();
            DateTimeOffset current = utTimeProvider.Now;

            string          priorHour       = current.AddHours(-1).ToString("h tt");
            string          nextHour        = current.AddHours(1).ToString("h tt");
            DayTimeInterval dayTimeInterval = new DayTimeInterval(priorHour, nextHour);

            TimeUnit checkInterval = new TimeUnit("1h");


            // Test 1:
            PeriodicJob jobA = new PeriodicJob("JobA",
                                               JobMethodA_ReturnTrue,
                                               dayTimeInterval,
                                               checkInterval,
                                               null);
            jobA.SetNextRunTime(utTimeProvider);

            // Validate
            Assert.NotZero(jobA.Id, "A10:");

            DateTimeOffset mostRecent = DateTimeOffset.Now;
            Assert.That(jobA.NextRunTime, Is.EqualTo(utTimeProvider.Now), "A100:  JobA appears to have an incorrect future NextRun date.");
        }



        [Test]
        public void NextRunTime_InitialStartupOutsideInterval()
        {
            // Prep
            // Set an interval based upon current time.
            TestingDateProvider utTimeProvider = new TestingDateProvider();
            TestingDateProvider today          = new TestingDateProvider();
            today.Now = new DateTimeOffset(2024,
                                           06,
                                           10,
                                           0,
                                           0,
                                           0,
                                           new TimeSpan(5, 0, 0));

            utTimeProvider.Now = DateTime.Today.AddHours(6);

            DateTimeOffset current             = utTimeProvider.Now;
            DateTimeOffset startDateTimeOffset = current.AddHours(3);


            string startHour        = startDateTimeOffset.ToString("h tt");
            int    startHourNumeric = startDateTimeOffset.Hour;

            string          endHour         = current.AddHours(5).ToString("h tt");
            DayTimeInterval dayTimeInterval = new DayTimeInterval(startHour, endHour);

            TimeUnit checkInterval = new TimeUnit("1h");


            // Test 1:
            PeriodicJob jobA = new PeriodicJob("JobA",
                                               JobMethodA_ReturnTrue,
                                               dayTimeInterval,
                                               checkInterval,
                                               null);
            jobA.SetNextRunTime(utTimeProvider);

            // Validate
            Assert.NotZero(jobA.Id, "A10:");

            DateTimeOffset mostRecent = DateTimeOffset.Now;

            // Get midnight of next day.
            DateTimeOffset midnightNextDay = new DateTimeOffset(current.Year,
                                                                current.Month,
                                                                current.Day,
                                                                0,
                                                                0,
                                                                0,
                                                                current.Offset).AddDays(1);

            Assert.Greater(jobA.NextRunTime, midnightNextDay, "A20:  JobA appears to have an incorrect future NextRun date that is in the past.");
            Assert.AreEqual(startHourNumeric, jobA.NextRunTime.Hour, "A30: Next Run Start hour should be the same as Run interval start hour");
        }



        /// <summary>
        /// Tests the IsTimeToRun method
        /// </summary>
        [Test]
        public void IsTimeToRun_Success()
        {
            // Prep
            // Set an interval based upon current time.
            TestingDateProvider utTimeProvider = new TestingDateProvider();
            utTimeProvider.Today_6AM();
            DateTimeOffset current = utTimeProvider.Now;

            DateTimeOffset startDateTimeOffset = current;

            string          startHour       = startDateTimeOffset.ToString("h tt");
            string          endHour         = current.AddHours(5).ToString("h tt");
            DayTimeInterval dayTimeInterval = new DayTimeInterval(startHour, endHour);
            TimeUnit        checkInterval   = new TimeUnit("1h");


            // Test 1:
            PeriodicJob jobA = new PeriodicJob("JobA",
                                               JobMethodA_ReturnTrue,
                                               dayTimeInterval,
                                               checkInterval,
                                               null);

            utTimeProvider.Now = utTimeProvider.Now.AddHours(1);
            jobA.SetNextRunTime(utTimeProvider);
            utTimeProvider.Now = utTimeProvider.Now.AddHours(1);
            Assert.IsTrue(jobA.IsTimeToRun(utTimeProvider), "A10:");
        }



        /// <summary>
        /// Tests the IsTimeToRun method
        /// </summary>
        [Test]
        [TestCase(-4, -1, Description = "Interval is before current time")]
        [TestCase(4, 6, Description = "Interval is after current time")]
        public void IsTimeToRun_ReturnsFalse(int startHours,
                                             int EndHours)
        {
            // Prep
            // Set an interval based upon current time.
            TestingDateProvider utTimeProvider = new TestingDateProvider();
            utTimeProvider.Now = new DateTimeOffset(2024,
                                                    06,
                                                    11,
                                                    6,
                                                    0,
                                                    0,
                                                    new TimeSpan(5, 0, 0));

            //utTimeProvider.Now = DateTime.Today.AddHours(6);

            DateTimeOffset current = utTimeProvider.Now;

            // Setup a job
            DateTimeOffset  startDateTimeOffset = current.AddHours(3);
            string          startHour           = startDateTimeOffset.ToString("h tt");
            string          endHour             = current.AddHours(5).ToString("h tt");
            DayTimeInterval dayTimeInterval     = new DayTimeInterval(startHour, endHour);

            TimeUnit checkInterval = new TimeUnit("1h");

            // Test 1:
            PeriodicJob jobA = new PeriodicJob("JobA",
                                               JobMethodA_ReturnTrue,
                                               dayTimeInterval,
                                               checkInterval,
                                               null);
            jobA.SetNextRunTime(utTimeProvider);

            Assert.IsFalse(jobA.IsTimeToRun(utTimeProvider), "A10:");
        }



        [Test]
        public void MidnightJobRun()
        {
            TestingDateProvider utTimeProvider = new TestingDateProvider();
            utTimeProvider.Today_SetTime(23, 59, 0);
            DateTimeOffset current = utTimeProvider.Now;

            // Set a job interval
            string          priorHour       = current.AddHours(-1).ToString("h tt");
            string          nextHour        = current.AddHours(1).ToString("h tt");
            DayTimeInterval dayTimeInterval = new DayTimeInterval(priorHour, nextHour);
            TimeUnit        checkInterval   = new TimeUnit("1m");


            // Test 1:
            PeriodicJob jobA = new PeriodicJob("JobA",
                                               JobMethodA_ReturnTrue,
                                               dayTimeInterval,
                                               checkInterval,
                                               null);

            // B
            jobA.SetNextRunTime(utTimeProvider);
            Assert.That(jobA.IsTimeToRun(utTimeProvider), Is.True, "B100:");
            jobA.Execute();
            jobA.SetNextRunTime(utTimeProvider);

            // Now move forward 1 minute and lets test again
            utTimeProvider.Now = utTimeProvider.Now.AddMinutes(1);
            Assert.That(jobA.IsTimeToRun(utTimeProvider), Is.True, "C100:");
            jobA.Execute();
            jobA.SetNextRunTime(utTimeProvider);
        }


        /// <summary>
        /// Sample Method run by some of the jobs
        /// </summary>
        /// <returns></returns>
        private bool JobMethodA_ReturnTrue(Action<ProcessingTask> addTaskMethod)
        {
            int j = 1;
            return true;
        }
    }
}