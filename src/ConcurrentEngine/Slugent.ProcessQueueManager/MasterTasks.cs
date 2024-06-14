using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlugEnt.ProcessQueueManager;

namespace SlugEnt.ProcessQueueManager;

/// <summary>
/// A Dictionary of Tasks.  Provides more advanced capabilities to clone and add.
/// </summary>
public class MasterTasks : Dictionary<int, ProcessingTask>
{
    public MasterTasks() : base() { }


    public void Add<T>(T id,
                       EnumProcessingTaskSpeed speed,
                       Func<Object, bool> methodToRun) where T : Enum
    {
        int keyId = Convert.ToInt32(id);

        ProcessingTask processingTask = ProcessingTask.CreateReferenceTask(
                                                                           Enum.GetName(typeof(T), id),
                                                                           keyId,
                                                                           speed,
                                                                           methodToRun
                                                                          );

        base.Add(keyId, processingTask);
    }


    public ProcessingTask Clone<T>(T id,
                                   Object payload) where T : Enum
    {
        int keyId = Convert.ToInt32(id);

        if (!base.TryGetValue(keyId, out ProcessingTask masterTask))
        {
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