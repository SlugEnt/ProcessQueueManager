using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlugEnt.ProcessQueueManager
{
	/// <summary>
	/// A Task to be performed.
	/// </summary>
	public interface IProcessingTask
	
	{
		/// <summary>
		/// Name of the Task
		/// </summary>
		public string Name { get; }


		/// <summary>
		/// ID that identifies the type of task
		/// </summary>
		public int TaskTypeId { get; }


		/// <summary>
        /// The expected time it will take for this task to typically run.  This assists with placing it on the appropriate queue
		/// </summary>
		public EnumProcessingTaskSpeed TaskSpeed { get; }


		/// <summary>
		/// The status of the task
		/// </summary>
		public EnumProcessingTaskStatus Status { get; }
		public object Payload { get; }


		/// <summary>
		/// When the task started running.
		/// </summary>
		public DateTimeOffset ExecutionStart { get; }

		/// <summary>
		/// The Time when the task should be killed, because it has run too long
		/// </summary>
		public DateTimeOffset KillTime { get; }


		/// <summary>
		/// Runs the actual task
		/// </summary>
		/// <returns></returns>
		public bool Execute ();
	}
}
