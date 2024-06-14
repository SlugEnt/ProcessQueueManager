namespace SlugEnt.ProcessQueueManager
{
    /// <summary>
    /// The status' the engine can be in.
    /// </summary>
    public enum EnumConcurrentEngineStatus
    {
        /// <summary>
        /// The engine is starting up
        /// </summary>
        Starting,

        /// <summary>
        /// Engine is running and fully capable of accepting jobs and processing tasks
        /// </summary>
        Running,

        /// <summary>
        /// Engine is attempting to stop.  No new tasks or jobs will be accepted.
        /// </summary>
        Stopping,

        /// <summary>
        /// Engine has ceased all processing.
        /// </summary>
        Stopped
    }
}
