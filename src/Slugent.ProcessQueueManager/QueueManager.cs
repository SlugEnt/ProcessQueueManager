using Amib.Threading;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Console = Colorful.Console;

namespace SlugEnt.ProcessQueueManager
{
	/// <summary>
	/// A processing queue that can run 1..n tasks in parallel.  It will have a maximum run time, after which any task that exceeds will be killed.
	/// </summary>
    public class QueueManager : IQueueManager {
		private int _runningTasksCount = 0;
        private ulong _tasksCompletedCount = 0;
		private bool _stopProcessing = false;
		private SmartThreadPool _threadPool = new SmartThreadPool();
        private IWorkItemsGroup _poolGroup;

		private ConcurrentQueue<ProcessingTask> waitingTasks = new ConcurrentQueue<ProcessingTask>();
		private ConcurrentDictionary<long,ProcessingTask> _runningTasks = new ConcurrentDictionary<long, ProcessingTask>();



		/// <summary>
		/// Name of this Queue Manager
		/// </summary>
		public string Name { get; set; }


		/// <summary>
		/// The maximum number of tasks that can be run simultaneously in a parallel manner.
		/// </summary>
		public int MaxParallelTasksCount { get; private set; } = 2;


		/// <summary>
		/// Maximum amount of seconds a task can run before it will be killed.
		/// </summary>
		public int MaxTaskRunTime { get; set; } = 4;


		/// <summary>
		/// Number of tasks that the queue has completed.
		/// </summary>
		public ulong TasksCompletedCount {
            get { return _tasksCompletedCount;} }


        /// <summary>
        /// Initiates the Queue to stop processing.  It will finish up items in the queue, stops accepting new items.
        /// </summary>
        public void StopProcessing () {
            _stopProcessing = true;
			// TODO - Need to drain the waiting items from the queue.,
        }


		/// <summary>
		/// Returns true if there are items in waiting queue or are currently Running
		/// </summary>
		public bool HasItemsInQueue {
			get {
				if ( waitingTasks.Count == 0 && _runningTasks.Count == 0 ) return false;
				return true;
			}
		}

		/// <summary>
		/// Number of items in the waiting queue
		/// </summary>
		public int QueueCount {
			get { return waitingTasks.Count; }
		}


		/// <summary>
		/// True if the queue is ready to receive tasks
		/// </summary>
		public bool IsReadyToReceiveTasks { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name"></param>
		public QueueManager (string name, int maxParallelTasksCount) { 
			Name = name;
			MaxParallelTasksCount = maxParallelTasksCount;
		}


		/// <summary>
		/// Starts the Queue Manager up.
		/// </summary>
		/// <returns></returns>
		public async Task Start () {
			// Create Smart Group to process Work Items
			WIGStartInfo poolGroupStartInfo = new WIGStartInfo()
            {
				PostExecuteWorkItemCallback = PostExecuteWorkItemCallback
            };
            _poolGroup = _threadPool.CreateWorkItemsGroup(MaxParallelTasksCount,poolGroupStartInfo);
			

			while ( !_stopProcessing ) {
				// Do we have capacity to run more parellel items?
                while ( _runningTasksCount < MaxParallelTasksCount && waitingTasks.Count > 0) {
                    // Are there items waiting to be processed
                    if ( waitingTasks.Count > 0 ) {
                        // Pull item off Queue, add to running tasks and start the process.
                        if ( waitingTasks.TryDequeue(out ProcessingTask processingTask) ) { _ = ExecuteTask(processingTask); }
                    }
                }
                Console.Write(Name[0], Color.DarkOrange);
                Thread.Sleep(50);

			}
		}


		/// <summary>
		/// Adds the given task to the queue of items to be executed.
		/// </summary>
		/// <param name="task"></param>
		//public bool AddTask<P> (ProcessingTask task, P payload ) {
        public bool AddTask(ProcessingTask task)
        {
			//if ( waitingTasks.Count > MaxParallelTasksCount ) return false;
			if ( _stopProcessing ) return false;

			waitingTasks.Enqueue(task);
			return true;
		}


		/// <summary>
		/// Runs a given task in a thread.
		/// </summary>
		/// <param name="processingTask">Task to be run</param>
		/// <returns></returns>
		private async Task ExecuteTask (ProcessingTask processingTask) {
            try {
                Interlocked.Increment(ref _runningTasksCount);
                if ( !_runningTasks.TryAdd(processingTask.Id, processingTask) ) {
                    Console.WriteLine(
                        "Failed to add the task [{0}: {1}] to the internal Dictionary.  This means this task has already been added to the dictionary previously.  This should never happen",
                        processingTask.Name, processingTask.Id);
                    return;
                }


                IWorkItemResult result = _poolGroup.QueueWorkItem(new WorkItemCallback(this.RunTask), processingTask);
                return;

            }

            catch ( Exception e ) { Console.WriteLine("Task threw an error - {0}", e.ToString()); }
            finally {
                Interlocked.Decrement(ref _runningTasksCount);
                Interlocked.Increment(ref _tasksCompletedCount);
			}

            
        }


		/// <summary>
		/// Executes the given task.  
		/// </summary>
		/// <param name="state">The processing task to be run</param>
		/// <returns></returns>
        private object RunTask (object state) {
            ProcessingTask t = (ProcessingTask) state;
            if ( t.Execute() ) return true;

			// It failed.  
            Console.WriteLine("Task [{0}: {1}] failed to execute successfully.", t.Name,t.Id,Color.Red);
			if (t.Exception != null) Console.WriteLine("  --> Task had an exception of: {0}", t.Exception,Color.DarkRed);
            return false;
        }



		/// <summary>
		/// This method is called AFTER the task has completed or been cancelled.  
		/// </summary>
		/// <param name="workItemResult"></param>
        private void PostExecuteWorkItemCallback (IWorkItemResult workItemResult) {
            ProcessingTask processingTask = (ProcessingTask)workItemResult.State;

			// We could handle the exception here, but right now we are handling in the RunTask Item
			//if (!workItemResult.IsCompleted || !(bool) workItemResult.Result)
            //    Console.WriteLine("Task [{0}] did not complete in its allotted time of [{1}] seconds.  It was cancelled.", processingTask.Name, MaxTaskRunTime);
			// Remove task from Running Tasks
			_runningTasks.TryRemove(processingTask.Id, out ProcessingTask removedProcessingTask);
            Interlocked.Decrement(ref _runningTasksCount);
			
        }



	}
}
