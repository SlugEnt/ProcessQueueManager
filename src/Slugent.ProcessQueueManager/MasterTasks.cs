using System;
using System.Collections.Generic;

namespace SlugEnt.ProcessQueueManager
{
    /// <summary>
    /// This is a Dictionary of ProcessingTasks (Reference type) with special Add and Clone methods to make adding tasks much simpler.
    /// </summary>
    public class MasterTasks : Dictionary<int,ProcessingTask> {
        public MasterTasks () : base() {}


        /// <summary>
        /// Adds a new ProcessingTask to the Dictionary.
        /// </summary>
        /// <typeparam name="T">The Enum Id of the task</typeparam>
        /// <param name="id">The enum value of the task</param>
        /// <param name="speed">Whether this task is fast, moderate or long running</param>
        /// <param name="methodToRun">The method that should be called when this task needs to run</param>
        public void Add<T> (T id, EnumProcessingTaskSpeed speed, Func<Object, bool> methodToRun) where T : Enum {

            int keyId = Convert.ToInt32(id);

            ProcessingTask processingTask = ProcessingTask.CreateReferenceTask(
                Enum.GetName(typeof(T), id),
                keyId,
                speed,
                methodToRun
            );

            base.Add(keyId,processingTask);
        }


        /// <summary>
        /// Clones the provided ProcessingTask that is stored in the dictionary
        /// </summary>
        /// <typeparam name="T">The enum class </typeparam>
        /// <param name="id">The enum value to retrieve</param>
        /// <param name="payload">An optional payload object that should be sent to the task when it is run</param>
        /// <returns></returns>
        public ProcessingTask Clone<T> (T id, Object payload) where T : Enum {
            int keyId = Convert.ToInt32(id);

            if ( !base.TryGetValue(keyId, out ProcessingTask masterTask) ) {
                throw new ArgumentException("The Master Task ID [ " +
                                            keyId +
                                            " ] with name: [ " +
                                            id.ToString() +
                                            " ] was not found in the MasterTasks Dictionary.");
            }

            ProcessingTask newTask = masterTask.CloneTask(payload);
            return newTask;
        }
    }
}
