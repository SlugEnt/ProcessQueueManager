using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using SlugEnt;
using Console = Colorful.Console;
using Slugent.ProcessQueueManager;

namespace Sample
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("Hello World!");


            ConcurrentEngine concurrentEngine = new ConcurrentEngine();


			// Create list of Master Tasks


			// Add all the periodic Tasks that need to be done.

			// Job - Check for chores
            DayTimeInterval dayTimeIntervalChores = new DayTimeInterval("4am", "8pm");
            TimeUnit checkIntervalChores = new TimeUnit("1m");

            PeriodicJob jobChoress = new PeriodicJob("Do Chores", DoChores, dayTimeIntervalChores, checkIntervalChores, concurrentEngine.AddTask);
            

			string [] taskIds = new [] {"Take Out Garbage", "Wash Dishes", "Clean Car", "Wash Laundry", "Dry Laundry", "Fold Laundry", "Make Dinner"};


			// Periodic Tasks
            DayTimeInterval dayTimeInterval = new DayTimeInterval("3am", "10pm");
            TimeUnit checkInterval = new TimeUnit("1m");
            PeriodicJob jobA = new PeriodicJob("JobA",JobAMethod,dayTimeInterval,checkInterval,concurrentEngine.AddTask);



			// Load tasks into Task Table
			List<ProcessingTask> masterTasks = new List<ProcessingTask>();
			int i = 0;
			masterTasks.Add(ProcessingTask.CreateReferenceTask(taskIds[i],i++,EnumProcessingTaskSpeed.Fast,TaskTakeOutGarbage));
			masterTasks.Add(ProcessingTask.CreateReferenceTask(taskIds[i], i++, EnumProcessingTaskSpeed.Slow, TaskWashDishes));
			masterTasks.Add(ProcessingTask.CreateReferenceTask(taskIds[i], i++, EnumProcessingTaskSpeed.Moderate, TaskWashCar));
			masterTasks.Add(ProcessingTask.CreateReferenceTask(taskIds[i], i++, EnumProcessingTaskSpeed.Fast, TaskLaundryWashed));
			masterTasks.Add(ProcessingTask.CreateReferenceTask(taskIds[i], i++, EnumProcessingTaskSpeed.Slow, TaskLaundryDried));
			masterTasks.Add(ProcessingTask.CreateReferenceTask(taskIds[i], i++, EnumProcessingTaskSpeed.Slow, TaskLaundryFolder));
			masterTasks.Add(ProcessingTask.CreateReferenceTask(taskIds[i], i++, EnumProcessingTaskSpeed.Moderate, TaskDinner));


			// Start the Queue Engine for short fast processes
			QueueManager fastQueue = new QueueManager("Fast",7);
			

			string name = "Jerry Seinfeld";
			//while ( true ) {
				for ( i = 0; i < 10; i++ ) {
					foreach ( ProcessingTask processingTask in masterTasks ) {
						ProcessingTask t = processingTask.CloneTask(name);
						fastQueue.AddTask(t);

						//t.Execute();
					}
				}

				Console.WriteLine("Items in Queue: {0}", fastQueue.QueueCount,Color.DarkCyan);
				_ = Task.Run(() => fastQueue.Start());
			while (fastQueue.HasItemsInQueue) {
					Console.WriteLine("Main Thread sleeping - still items in fast queue",Color.Yellow);
					Thread.Sleep(2000);	
				}

			int j = 0;
		}



        public static bool DoChores (Action<ProcessingTask> addTaskMethod) {
            return true;
        }


        public static bool JobAMethod (Action<ProcessingTask> addTaskMethod) {
            int i = 22;
            return true;
        }


		public static bool TaskWashCar(object a)
		{
			Thread.Sleep(1500);
			int sleep = RandomSleep(15);
			Console.WriteLine("Car Washed by {0} - slept: {1}", a.ToString(), sleep);
			return true;
		}


		public static bool TaskTakeOutGarbage(object a)
		{
			int sleep = RandomSleep(3);
			Console.WriteLine("Garbage taken to curb by {0}  - slept: {1}", a.ToString(), sleep);
			return true;
		}



		public static bool TaskWashDishes (object a)
		{
			int sleep = RandomSleep(3);
			Console.WriteLine("Dishes Washed.  Wife happy - Good Job {0} - slept: {1}", a.ToString(), sleep);
			return true;
		}


		public static bool TaskLaundryWashed(object a)
		{
			throw new ApplicationException();
			int sleep = RandomSleep(1);
			Console.WriteLine("Laundry in washer - {0} slept: {1}" ,a.ToString(), sleep);
			return true;
		}


		public static bool TaskLaundryDried(object a)
		{
			int sleep = RandomSleep(1);
			Console.WriteLine("Laundry in dryer {0}  - slept: {1}",a.ToString(), sleep);
			return true;
		}


		public static bool TaskLaundryFolder(object a)
		{
			int sleep = RandomSleep(6);
			Console.WriteLine("Laundry has been folded {0} - slept: {1}",a.ToString(), sleep);
			return true;
		}


		public static bool TaskDinner(object a)
		{
			int sleep = RandomSleep(4);
			Console.WriteLine("Dinner has been served {0} - slept: {1}", a.ToString(),sleep);
			return true;
		}


		public static int RandomSleep (int factor) {
			Random random = new Random(); 


			int sleepTime = random.Next(1000 * factor);
			Thread.Sleep(sleepTime);
			return sleepTime;
		}



        private static void ConfigureServices(IServiceCollection services) {
            services.AddLogging(configure => configure.AddSerilog());



        }
	}
}
