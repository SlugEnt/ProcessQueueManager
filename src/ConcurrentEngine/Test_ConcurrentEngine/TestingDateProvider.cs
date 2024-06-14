using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProcessQueueManager.DatetimeProvider;

namespace Test_ConcurrentEngine
{
    public class TestingDateProvider : IDateTimeOffsetProvider
    {
        private DateTimeOffset _now;
        private DateTimeOffset _utcNow;


        public TestingDateProvider() { }


        /// <summary>
        /// Gets current date and time
        /// </summary>
        public DateTimeOffset Now
        {
            get => this._now;
            set => this._now = value;
        }


        /// <summary>
        /// Gets current UTC date and time
        /// </summary>
        public DateTimeOffset UtcNow
        {
            get { return this._utcNow; }
            set => this._utcNow = value;
        }


        /// <summary>
        /// The current UTC offset for the object 
        /// </summary>
        public int UTC_Offset { get; set; } = 5;


        /// <summary>
        /// Sets the current date and time to current date at 6am EST
        /// </summary>
        public void Today_6AM()
        {
            DateTime today = DateTime.Today;
            _now = new DateTimeOffset(today.Year,
                                      today.Month,
                                      today.Day,
                                      6,
                                      0,
                                      0,
                                      new TimeSpan(UTC_Offset, 0, 0));
        }


        /// <summary>
        /// Sets date to today and the hour, minute and second to specified time
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        public void Today_SetTime(int hour = 0,
                                  int minute = 0,
                                  int second = 0)
        {
            DateTime today = DateTime.Today;
            _now = new DateTimeOffset(today.Year,
                                      today.Month,
                                      today.Day,
                                      hour,
                                      minute,
                                      second,
                                      new TimeSpan(UTC_Offset, 0, 0));
        }
    }
}