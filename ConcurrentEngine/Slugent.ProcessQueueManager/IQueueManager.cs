﻿using System.Threading.Tasks;

namespace SlugEnt.ProcessQueueManager
{
	public interface IQueueManager
	{
		public string Name { get; }


		/// <summary>
		/// The maximum number of tasks that can be run simultaneously in a parallel manner.
		/// </summary>
		public int MaxParallelTasksCount { get; }

		/// <summary>
		/// Maximum number of seconds task can run, before being killed.
		/// </summary>
		public int MaxTaskRunTime { get; }

		public void StopProcessing ();

		public Task Start ();

		public bool AddTask (ProcessingTask task);

	}
}
