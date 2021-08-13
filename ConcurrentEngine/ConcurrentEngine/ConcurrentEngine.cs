using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentEngine
{
	public class ConcurrentEngine
	{
		private ConcurrentQueue<string> stringQueue = new ConcurrentQueue<string>();

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
	}
}
