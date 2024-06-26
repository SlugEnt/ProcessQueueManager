﻿using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;
using ProcessQueueManager.DatetimeProvider;

[assembly: InternalsVisibleTo("Test_ConcurrentEngine")]

namespace SlugEnt.ProcessQueueManager
{
    /// <summary>
    /// A job that is run periodically.  A job should be very short lived.  If the job needs to run for more than a second or 2, it should spawn a task to perform its main processing.
    /// </summary>
    public class PeriodicJob
    {
        private static int                  _id;
        private        long                 _runCount = 0;
        private        ILogger<PeriodicJob> _logger;


        /// <summary>
        /// Name of the Job.
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Unique ID that identifies this Periodic Job.
        /// </summary>
        public int Id { get; private set; }


        /// <summary>
        /// Number of times this job has been run
        /// </summary>
        public long RunCount
        {
            get { return _runCount; }
        }


        /// <summary>
        /// The time period in a given day that this task is allowed to run.
        /// </summary>
        public DayTimeInterval AllowedInterval { get; set; } = new DayTimeInterval("12 am", "11:59:59 pm");


        /// <summary>
        /// How often we check to see if this task should run
        /// </summary>
        public TimeUnit CheckInterval { get; set; } = new TimeUnit("1h");


        /// <summary>
        /// The next opportunity for this job to run again.
        /// </summary>
        public DateTimeOffset NextRunTime { get; set; }


        /// <summary>
        /// The method that will be run
        /// </summary>
        private Func<Action<ProcessingTask>, bool> MethodToRun { get; set; }


        /// <summary>
        /// External method used to add a processing task to be run.
        /// </summary>
        private Action<ProcessingTask> AddTask { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Job Name</param>
        /// <param name="methodToRun">The method from the main app that will be used to run this job.  This method should be short running</param>
        /// <param name="addTaskMethod">The method that the job can use to add a task to one of the queues</param>
        /// <param name="logger">The logger</param>
        public PeriodicJob(string name,
                           Func<Action<ProcessingTask>, bool> methodToRun,
                           Action<ProcessingTask> addTaskMethod,
                           ILogger<PeriodicJob> logger = null)
        {
            Id          = ++_id;
            Name        = name;
            MethodToRun = methodToRun;
            AddTask     = addTaskMethod;
            _logger     = logger;
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Job Name</param>
        /// <param name="methodToRun">The method that should be run</param>
        /// <param name="allowedInterval">The DayTimeInterval that the job is allowed to run within</param>
        /// <param name="checkInterval">How often the job should be run.  So the time between one run ending and the next starting.</param>
        public PeriodicJob(string name,
                           Func<Action<ProcessingTask>, bool> methodToRun,
                           DayTimeInterval allowedInterval,
                           TimeUnit checkInterval,
                           Action<ProcessingTask> addTaskMethod,
                           ILogger<PeriodicJob> logger = null) : this(name,
                                                                      methodToRun,
                                                                      addTaskMethod,
                                                                      logger)
        {
            AllowedInterval = allowedInterval;
            CheckInterval   = checkInterval;
        }


        /// <summary>
        /// Determines if this job is allowed to run now, because it is within the run window.
        /// </summary>
        /// <returns></returns>
        public bool IsTimeToRun(IDateTimeOffsetProvider dateTime)
        {
            //DateTimeOffset current = DateTimeOffset.Now;
            DateTimeOffset current = dateTime.Now;
            if (!(current >= NextRunTime))
                return false;

            if (AllowedInterval.IsInInterval(current))
                return true;

            return false;
        }



        /// <summary>
        /// Determines what the next run time will be.
        /// </summary>
        protected internal void SetNextRunTime(IDateTimeOffsetProvider dateTime)
        {
            DateTimeOffset nextTime;

            // If called and we have never run then set runtime to now.
            if (_runCount == 0)
                nextTime = dateTime.Now;
            else
                nextTime = dateTime.Now.AddMilliseconds(CheckInterval.InMilliSecondsLong);


            if (AllowedInterval.IsInInterval(nextTime))
            {
                NextRunTime = nextTime;
                return;
            }

            // Set next run time to start time of the interval
            NextRunTime = AllowedInterval.GetNextIntervalStartDateTimeOffset();
        }



        /// <summary>
        /// Runs the method MethodToRun.  All errors are swallowed within this routine.
        /// </summary>
        async protected internal void Execute()
        {
            _runCount++;

            //_logger.LogInformation("Starting - " + Name);
            try
            {
                bool success = MethodToRun(AddTask);
            }
            catch (Exception e) { }
        }
    }
}