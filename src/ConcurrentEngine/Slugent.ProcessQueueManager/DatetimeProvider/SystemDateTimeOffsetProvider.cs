using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessQueueManager.DatetimeProvider;

/// <summary>
/// Provides the .Net Core DateTime Standard Methods
/// </summary>
public class SystemDateTimeOffsetProvider : IDateTimeOffsetProvider
{
    public DateTimeOffset UtcNow
    {
        get { return DateTimeOffset.UtcNow; }
    }

    public DateTimeOffset Now
    {
        get { return DateTimeOffset.Now; }
    }
}