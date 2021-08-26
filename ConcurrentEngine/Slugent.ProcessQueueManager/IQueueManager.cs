using System.Threading.Tasks;

namespace SlugEnt.ProcessQueueManager
{
	/// <summary>
	/// Represents a queue that can process tasks in a multi-threaded manner
	/// </summary>
	public interface IQueueManager
	{
		/// <summary>
		///  Name of the queue
		/// </summary>
		public string Name { get; }


		/// <summary>
		/// The maximum number of tasks that can be run simultaneously in a parallel manner.
		/// </summary>
		public int MaxParallelTasksCount { get; }

		/// <summary>
		/// Maximum number of seconds task can run, before being killed.
		/// </summary>
		public int MaxTaskRunTime { get; }


		/// <summary>
		/// Requests the queue to stop all procesing and accepting any new tasks
		/// </summary>
		public void StopProcessing ();


		/// <summary>
		/// Starts the queue
		/// </summary>
		/// <returns></returns>
		public Task Start ();


		/// <summary>
		/// Adds a task to be processed to the queue
		/// </summary>
		/// <param name="task">ProcessingTask to be added to the queue</param>
		/// <returns></returns>
		public bool AddTask (ProcessingTask task);

	}
}
