using System;
using SlugEnt.ProcessQueueManager;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlugEnt.ProcessQueueManager
{
    /// <summary>
    /// An engine that can process jobs which contain one or more tasks that can be scheduled to run in one of 3 different parallel
    /// processing queues (fast, medium and slow)
    /// </summary>
    public class ConcurrentEngine {
        private Thread _loopThread;
		private bool _continueRunning;
        
        private QueueManager _fastQueue;// = new QueueManager("Fast", 7);
        private QueueManager _mediumQueue;
        private QueueManager _slowQueue;


        /// <summary>
        /// The total number of tasks that have been requested to be processed.
        /// </summary>
        public ulong TasksAdded { get; private set; }


        /// <summary>
        /// The total number of tasks that have been completed
        /// </summary>
        public ulong TasksCompleted {
            get {
                ulong total = 0;
                if ( _fastQueue != null ) total += _fastQueue.TasksCompletedCount;
                if (_slowQueue != null) total += _slowQueue.TasksCompletedCount;
                if (_mediumQueue != null) total += _mediumQueue.TasksCompletedCount;
                return total;
            }
        }


        /// <summary>
		/// Amount of time the loop Thread should sleep between runs.  Time is in milliseconds
		/// </summary>
		public int SleepTimeMS { get; set; } = 5000;


		/// <summary>
		///  Status of the Engine
		/// </summary>
        public EnumConcurrentEngineStatus Status { get; private set; } = EnumConcurrentEngineStatus.Stopped;


		private Dictionary<int,PeriodicJob> Jobs = new Dictionary<int,PeriodicJob>();


        /// <summary>
        /// Returns the number of jobs currently in the engine queue
        /// </summary>
        public int JobCount {
            get { return Jobs.Count; }
        }
        

        /// <summary>
        /// Returns a list of all the jobs
        /// </summary>
        public IReadOnlyDictionary<int,PeriodicJob> JobsList {
            get { return Jobs; }
        }


        /// <summary>
        /// Maximum number of tasks that can be run in parallel for the fast or short running tasks queue.
        /// </summary>
        public byte MaxThreadsFast { get; set; } = 5;


        /// <summary>
        /// Maximum number of tasks that can be run in parallel for the slow or long running tasks queue.
        /// </summary>
        public byte MaxThreadsSlow { get; set; } = 2;

        /// <summary>
        /// Maximum number of Tasks that can be run in parallel for the medium processing queue
        /// </summary>
        public byte MaxThreadsMedium { get; set; } = 3;


        /// <summary>
        /// Constructor
        /// </summary>
		public ConcurrentEngine () {
			 InitializeQueues();
		}

        

        /// <summary>
        /// Sets the Queues up
        /// </summary>
        private void InitializeQueues () {
            // Create Queue's if they do not exist
            if (_fastQueue == null) _fastQueue = new QueueManager("Fast Job Queue", MaxThreadsFast);
            if (_mediumQueue == null) _mediumQueue = new QueueManager("Medium Job Queue", MaxThreadsMedium);
            if (_slowQueue == null) _slowQueue = new QueueManager("Slow Job Queue", MaxThreadsSlow);
        }



        /// <summary>
        /// Starts the engine.  It can immediately begin accepting and executing jobs
        /// </summary>
        public void Start () {
            if ( Status != EnumConcurrentEngineStatus.Stopped ) return;
            Status = EnumConcurrentEngineStatus.Starting;

            InitializeQueues();

            _ = Task.Run(() => _fastQueue.Start());
            _ = Task.Run(() => _mediumQueue.Start());
            _ = Task.Run(() => _slowQueue.Start());

            _continueRunning = true;
            _loopThread = new Thread(Execute);
            _loopThread.Start();
			Status = EnumConcurrentEngineStatus.Running;
        }


		/// <summary>
		/// Stops the engine.  It may take some time for all the tasks in the engine to stop, but no new tasks are allowed to be added or started.
		/// </summary>
        public void Stop () {
            // The order of these is very important.
            Status = EnumConcurrentEngineStatus.Stopping;
            _continueRunning = false;
        }


		/// <summary>
		/// This is the main processing thread.  Reads thru all the Jobs during each cycle to determine which ones need to run.
		/// </summary>
		public void Execute () {
            while ( _continueRunning ) {
                foreach ( KeyValuePair<int, PeriodicJob> jobPair in Jobs ) {
                    if ( jobPair.Value.IsTimeToRun() ) {
                        jobPair.Value.Execute();
                        jobPair.Value.SetNextRunTime();
                    }
                }

                Thread.Sleep(SleepTimeMS);
			}

			// We got here because we have been asked to stop
            if (Status == EnumConcurrentEngineStatus.Stopping) {
                while ( true ) {
                    bool allStopped = true;
                    allStopped = !CanStopQueue(_fastQueue) ? false : allStopped;
                    allStopped = !CanStopQueue(_mediumQueue) ? false : allStopped;
                    allStopped = !CanStopQueue(_slowQueue) ? false : allStopped;

                    if ( allStopped ) {
                        Status = EnumConcurrentEngineStatus.Stopped;
                        return;
                    }
                }
            }
        }


        /// <summary>
        /// Determines if the specified queue can be shut down.  If it is empty it can be shut down.  It will be set to null and true is returned.
        /// </summary>
        /// <param name="q">The queue to be checked</param>
        /// <returns></returns>
        private bool CanStopQueue (QueueManager q) {
            if ( q == null ) return true; 

            if (!q.HasItemsInQueue)
            {
                // || _mediumQueue.HasItemsInQueue || _slowQueue.HasItemsInQueue ) {
                q.StopProcessing();
                q = null;
                return true;
            }

            return false;
        }


        /// <summary>
        /// Adds a Job to the Execution Engine
        /// </summary>
        /// <param name="periodicJob">Periodic Job to be added to the engine's queue</param>
        public void AddJob (PeriodicJob periodicJob) {
			Jobs.Add(periodicJob.Id,periodicJob);
        }


        /// <summary>
        /// Creates a job and then adds it to the processing queue
        /// </summary>
        /// <param name="name">Name of the job</param>
        /// <param name="methodToRun">The method from the application that should be run when the job needs to run</param>
        /// <param name="intervalStartTime">A time in a format acceptable for DayTimeInterval, ex:  2pm</param>
        /// <param name="intervalEndTime">A time in a format acceptable for DayTimeInterval, ex: 9am</param>
        /// <param name="checkInterval">A time period in a format acceptable for a TimeUnit, ex: 12m, or 2h or 3s, etc</param>
        public void AddNewJob (string name, Func<Action<ProcessingTask>, bool> methodToRun, string intervalStartTime, string intervalEndTime, string checkInterval) {

            DayTimeInterval dayTimeInterval = new DayTimeInterval(intervalStartTime,intervalEndTime);
            TimeUnit timeCheckInterval = new TimeUnit(checkInterval);
            PeriodicJob job = new PeriodicJob(name, methodToRun, dayTimeInterval,timeCheckInterval,AddTask);
            AddJob(job);
        }


        /// <summary>
        /// Adds a task to be executed to the appropriate queue
        /// </summary>
        /// <param name="processingTask"></param>
        public void AddTask (ProcessingTask processingTask) {
            if ( processingTask.TaskSpeed == EnumProcessingTaskSpeed.Fast ) _fastQueue.AddTask(processingTask);
            else if ( processingTask.TaskSpeed == EnumProcessingTaskSpeed.Moderate )
                _mediumQueue.AddTask(processingTask);
            else
                _slowQueue.AddTask(processingTask);

            TasksAdded++;

            //Tasks.Add(processingTask.Name,processingTask);
        }


	}
}
