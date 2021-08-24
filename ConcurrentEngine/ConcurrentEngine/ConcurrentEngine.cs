using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slugent.ProcessQueueManager;

namespace ConcurrentEngine
{
	public class ConcurrentEngine
	{
		//private ConcurrentQueue<string> stringQueue = new ConcurrentQueue<string>();
		Dictionary<int,PeriodicJob> Jobs = new Dictionary<int,PeriodicJob>();

        public byte MaxThreadsFast { get; set; } = 5;
        public byte MaxThreadsSlow { get; set; } = 2;
        public byte MaxThreadsMedium { get; set; } = 3;


		public ConcurrentEngine () {
			 
		}


		public void Initialize () {

		}


		public void Execute () {
			bool continueProcessing = true;
			
			while ( continueProcessing ) {
				// Check MQ

				// Check DB

				// Check Other...
			}
		}



		/// <summary>
		/// Adds a Job to the Execution Engine
		/// </summary>
		/// <param name="periodicJob"></param>
        public void AddJob (PeriodicJob periodicJob) {
			Jobs.Add(periodicJob.Id,periodicJob);
        }



	}
}
