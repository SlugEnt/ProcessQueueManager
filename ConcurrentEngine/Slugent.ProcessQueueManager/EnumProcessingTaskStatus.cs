using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlugEnt.ProcessQueueManager
{
	public enum EnumProcessingTaskStatus
	{
		Created = 0,
		Scheduled = 50,
		Started = 100,
		Errored = 200,
		Completed = 254
	}
}
