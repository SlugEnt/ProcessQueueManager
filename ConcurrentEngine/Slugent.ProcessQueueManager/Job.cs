using SlugEnt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slugent.ProcessQueueManager
{
    public class Job
    {

        /// <summary>
        /// Name of the job
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Method used to execute the job's functionality
        /// </summary>
        private Func<bool> JobMethod { get; set; }

        private TimeUnit RunInterval { get; set; }



        public Job (string name, TimeUnit runInterval,Func<bool> jobMethod)
        {        
            Name = name;
            JobMethod = jobMethod;
            RunInterval = runInterval;
        }



        /// <summary>
        /// When the next time this job should be run
        /// </summary>
        public DateTime NextRun {  get; private set; }



    }
}
