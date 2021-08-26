using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using Console = Colorful.Console;

[assembly: InternalsVisibleTo("Test_ConcurrentEngine")]

namespace SlugEnt.ProcessQueueManager
{
    /// <summary>
    /// A single process that is to be executed.
    /// </summary>
    public class ProcessingTask : IProcessingTask {
		public static long _uniqueID = 0;

		/// <summary>
		/// Name of this task.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Unique ID of the job. This is automatically set by the constructor
		/// </summary>
		public long Id { get; private set; }


		/// <summary>
		/// Any payload that the task might need
		/// </summary>
		public object Payload { get; set; }


		/// <summary>
		/// ID that identifies this task
		/// </summary>
		public int TaskTypeId { get; private set; }

		/// <summary>
		/// A window of time that estimates how long it might take the task to run.
		/// </summary>
		public EnumProcessingTaskSpeed TaskSpeed { get; private set; }


		/// <summary>
		/// Status of the task at this point in time
		/// </summary>
		public EnumProcessingTaskStatus Status { get; protected set; }


		/// <summary>
		/// The method that should be run when the task is Executed.
		/// </summary>
		public Func<object,bool> MethodToExecute { get; private set; }


		/// <summary>
		/// If true this is a reference task.  Meaning it cannot be run and is here to be cloned from...
		/// </summary>
		internal bool IsReferenceTask { get; set; }

		/// <summary>
		/// When the task started running
		/// </summary>
		public DateTimeOffset ExecutionStart { get; private set; }


		/// <summary>
		/// When the task should be killed if it has not completed by this time.
		/// </summary>
		public DateTimeOffset KillTime { get; set; }


		/// <summary>
		/// The exception that was generated when the task attempted to run.
		/// </summary>
		public Exception Exception { get; private set; }


		/// <summary>
		/// Constructs a new ProcessingTask
		/// </summary>
		/// <param name="name">Descriptive Name of this task</param>
		/// <param name="taskTypeId">Each specific type of task has its own id that identifies it.  This is not the operating Id (Id) which is incremented for each instance of a particular Task Type</param>
		/// <param name="taskSpeed">How fast the task runs</param>
		/// <param name="methodToRun">The method that is run to actually perform the task</param>
		/// <param name="payload">An object that is passed to the methodToRun</param>
		public ProcessingTask (string name, int taskTypeId, EnumProcessingTaskSpeed taskSpeed, Func<object,bool> methodToRun , object payload = null) {
			Id = ++_uniqueID;
			Name = name;
			MethodToExecute = methodToRun;
			Payload = payload;
			TaskSpeed = taskSpeed;
			TaskTypeId = taskTypeId;
			Status = EnumProcessingTaskStatus.Created;
			IsReferenceTask = false;
		}



		/// <summary>
		/// Executes the given task.  Returns true if the task was processed successfully.
		/// </summary>
		/// <returns></returns>
		public bool Execute () {
			if ( IsReferenceTask ) {
				Console.WriteLine("Task [{0}: {1}] is a reference task.  Reference Tasks cannot be executed.", Id, Name,Color.Red);
				return false;
			}
			if ( Status != EnumProcessingTaskStatus.Created ) {
				Console.WriteLine("Task [{0}: {1}] is not in a status to be run.  Its current status is: [{2}]", Id,Name,Status.ToString(), Color.Red);
				return false;
			}

			ExecutionStart = DateTimeOffset.Now;

			try {
				Status = EnumProcessingTaskStatus.Started;
				bool success = MethodToExecute(Payload);
				if ( !success ) {
					Status = EnumProcessingTaskStatus.Errored; 
					Console.WriteLine("Task [{0}: {1}] errored.", Id,Name, Color.DarkRed);
				}
				else {
					Status = EnumProcessingTaskStatus.Completed; 
					Console.WriteLine("Task [{0}: {1}] completed successfully", Id,Name, Color.Green);
				}

				return success;
			}

			catch ( Exception e ) {
                Exception = e;
				if (Payload == null)
					Console.WriteLine("Error executing Task [{0}: {1}] with no payload data.  Error was: [{2}", Id,Name,e.ToString(), Color.Red);
				else 
					Console.WriteLine("Error executing Task [{0}: {1}] with payload of: [{2}]. {3}{4}",Id,Name,Payload.ToString(),Environment.NewLine,e.ToString(), Color.Red);

				// Log error...
			}

			return false;
		}


		/// <summary>
		/// Creates a clone of the current task.  Status of new task is set to created.  
		/// </summary>
		/// <param name="payload">An object that should be stored as the payload for the created task</param>
		/// <returns></returns>
		public ProcessingTask CloneTask (object payload = null) {
			ProcessingTask task = new ProcessingTask(this.Name,this.TaskTypeId,this.TaskSpeed,this.MethodToExecute,payload);
			return task;
		}



		/// <summary>
		/// Creates a reference task, which is a task that operational tasks are cloned from.  A reference task cannot be run.
		/// </summary>
		/// <param name="name">Name of the task</param>
		/// <param name="taskTypeId">Unique id that identifies the type of task</param>
		/// <param name="taskSpeed">How much time this task needs to run</param>
		/// <param name="methodToRun">The method that should be called to perform the actual processing of the task </param>
        /// <returns></returns>
		public static ProcessingTask CreateReferenceTask (string name,
		                                                  int taskTypeId,
											                  EnumProcessingTaskSpeed taskSpeed,
		                                                  Func<object, bool> methodToRun) {
			ProcessingTask task = new ProcessingTask(name,taskTypeId,taskSpeed,methodToRun,null);
			task.IsReferenceTask = true;
			return task;
		}
	}
}
