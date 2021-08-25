using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slugent.ProcessQueueManager
{
    /// <summary>
    /// The status' the engine can be in.
    /// </summary>
    public enum EnumConcurrentEngineStatus
    {
        Starting,
        Running,
        Stopping,
        Stopped
    }
}
