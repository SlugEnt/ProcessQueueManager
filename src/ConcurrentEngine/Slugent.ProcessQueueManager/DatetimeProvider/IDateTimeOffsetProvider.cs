using System;

namespace ProcessQueueManager.DatetimeProvider;

public interface IDateTimeOffsetProvider
{
    DateTimeOffset Now { get; }
    DateTimeOffset UtcNow { get; }
}