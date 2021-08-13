using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Console = Colorful.Console;

[assembly: InternalsVisibleTo("Test_ConcurrentEngine")]

namespace Slugent.ProcessQueueManager
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

		public Exception Exception { get; private set; }


		/// <summary>
		/// Constructs a new ProcessingTask
		/// </summary>
		/// <param name="name"></param>
		/// <param name="taskTypeId"></param>
		/// <param name="taskSpeed"></param>
		/// <param name="methodToRun"></param>
		/// <param name="payload"></param>
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
		/// <param name="payload"></param>
		/// <returns></returns>
		public ProcessingTask CloneTask (object payload = null) {
			ProcessingTask task = new ProcessingTask(this.Name,this.TaskTypeId,this.TaskSpeed,this.MethodToExecute,payload);
			return task;
		}



		/// <summary>
		/// Creates a reference task, which is a task that operational tasks are cloned from.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="taskTypeId"></param>
		/// <param name="taskSpeed"></param>
		/// <param name="methodToRun"></param>
		/// <param name="payload"></param>
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
