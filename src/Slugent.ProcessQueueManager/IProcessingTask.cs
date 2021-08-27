using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlugEnt.ProcessQueueManager
{
	public interface IProcessingTask
	
	{
		public string Name { get; }
		public int TaskTypeId { get; }
		public EnumProcessingTaskSpeed TaskSpeed { get; }
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

		public bool Execute ();
	}
}
