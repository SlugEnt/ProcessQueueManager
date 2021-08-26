using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Test_ConcurrentEngine")]

namespace SlugEnt.ProcessQueueManager
{


    public class PeriodicJob
    {
        private static int _id;
        private long _runCount = 0;
        private ILogger<PeriodicJob> _logger;


        /// <summary>
        /// Name of the Job.
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Unique ID that identifies this Periodic Job.
        /// </summary>
        public int Id { get {  return _id; }  }


        /// <summary>
        /// Number of times this job has been run
        /// </summary>
        public long RunCount {  get {  return _runCount;} }


        /// <summary>
        /// The time period in a given day that this task is allowed to run.
        /// </summary>
        public DayTimeInterval AllowedInterval { get; set; } = new DayTimeInterval("12 am","11:59:59 pm");


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
        private Func<Action<ProcessingTask>,bool> MethodToRun { get; set; }


        /// <summary>
        /// External method used to add a processing task to be run.
        /// </summary>
        private Action<ProcessingTask> AddTask { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="methodToRun"></param>
        /// <param name="addTaskMethod"></param>
        /// <param name="logger"></param>
        public PeriodicJob (string name, Func<Action<ProcessingTask>,bool> methodToRun, Action<ProcessingTask> addTaskMethod,ILogger<PeriodicJob> logger = null)
        {
            _id++;
            Name = name;
            MethodToRun = methodToRun;
            AddTask = addTaskMethod;
            _logger = logger;
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Job Name</param>
        /// <param name="methodToRun">The method that should be run</param>
        /// <param name="allowedInterval">The DayTimeInterval that the job is allowed to run within</param>
        /// <param name="checkInterval">How often the job should be run.  So the time between one run ending and the next starting.</param>
        public PeriodicJob(string name, Func<Action<ProcessingTask>,bool> methodToRun, DayTimeInterval allowedInterval, TimeUnit checkInterval, Action<ProcessingTask> addTaskMethod, ILogger<PeriodicJob> logger = null) :this(name, methodToRun, addTaskMethod,logger)
        {
            AllowedInterval = allowedInterval;
            CheckInterval = checkInterval;
        }


        /// <summary>
        /// Determines if this job is allowed to run now, because it is within the run window.
        /// </summary>
        /// <returns></returns>
        public bool IsTimeToRun ()
        {
            DateTimeOffset current = DateTimeOffset.Now;
            if ( !(current > NextRunTime) ) return false;

            if (AllowedInterval.IsInInterval(current))
                return true;
            return false;
        }



        public void ForceRun () { }


        /// <summary>
        /// Determines what the next run time will be.
        /// </summary>
        protected internal void SetNextRunTime () {
            DateTimeOffset nextTime;

            // If called and we have never run then set runtime to now.
            if ( _runCount == 0 )
                nextTime = DateTimeOffset.Now;
            else 
                nextTime = DateTime.Now.AddMilliseconds(CheckInterval.InMilliSecondsLong);


            if (AllowedInterval.IsInInterval(nextTime)) {
                NextRunTime = nextTime;
                return;
            }

            // Set next run time to start time of the interval
            NextRunTime = AllowedInterval.GetNextIntervalStartDateTimeOffset();
        }


        /// <summary>
        /// Runs the method MethodToRun.  All errors are swallowed within this routine.
        /// </summary>
        async protected internal void Execute () {
            _runCount++;
            //_logger.LogInformation("Starting - " + Name);
            try {
                bool success = MethodToRun(AddTask);
            }
            catch ( Exception e ) {

            }

        }
    }
}
