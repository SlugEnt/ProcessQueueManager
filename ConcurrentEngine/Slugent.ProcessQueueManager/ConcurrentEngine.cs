using SlugEnt.ProcessQueueManager;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SlugEnt
{
    public class ConcurrentEngine {
        private Thread _loopThread;
		private bool _continueRunning;
        
        private QueueManager _fastQueue;// = new QueueManager("Fast", 7);
        private QueueManager _mediumQueue;
        private QueueManager _slowQueue;


        public ulong TasksAdded { get; private set; }


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

        public int JobCount {
            get { return Jobs.Count; }
        }
        




        public IReadOnlyDictionary<int,PeriodicJob> JobsList {
            get { return Jobs; }
        }


        public byte MaxThreadsFast { get; set; } = 5;
        public byte MaxThreadsSlow { get; set; } = 2;
        public byte MaxThreadsMedium { get; set; } = 3;


		public ConcurrentEngine () {
			 InitializeQueues();
		}


		public void Initialize () {

		}


        private void InitializeQueues () {
            // Create Queue's if they do not exist
            if (_fastQueue == null) _fastQueue = new QueueManager("Fast Job Queue", MaxThreadsFast);
            if (_mediumQueue == null) _mediumQueue = new QueueManager("Medium Job Queue", MaxThreadsMedium);
            if (_slowQueue == null) _slowQueue = new QueueManager("Slow Job Queue", MaxThreadsSlow);
        }


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
        /// Determines if the queue can be shut down.  If it is empty it can be shut down.  It will be set to null and true is returned.
        /// </summary>
        /// <param name="q"></param>
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
        /// <param name="periodicJob"></param>
        public void AddJob (PeriodicJob periodicJob) {
			Jobs.Add(periodicJob.Id,periodicJob);
        }


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
