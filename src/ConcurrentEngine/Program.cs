using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SlugEnt;
using SlugEnt.ProcessQueueManager;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Console = Colorful.Console;

namespace ConsoleApp
{
    class Program
	{
		//public static List<ProcessingTask> _masterTasks = new List<ProcessingTask>();
        public static MasterTasks _masterTasks = new MasterTasks();


		static async Task Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			Console.WriteLine("Engine New");
			Example_New();
			/*

			*/
		}


        private static void Example_New () {
            ConcurrentEngine concurrentEngine = new ConcurrentEngine();
			
            
            // Load tasks into a Master Task Table.  These are all reference tasks
            int i = 0;
			_masterTasks.Add(EnumTaskIDs.TakeOutGarbage,EnumProcessingTaskSpeed.Fast,Tasks.TaskTakeOutGarbage);
            _masterTasks.Add(EnumTaskIDs.WashDishes, EnumProcessingTaskSpeed.Slow, Tasks.TaskWashDishes);
            _masterTasks.Add(EnumTaskIDs.WashCar, EnumProcessingTaskSpeed.Slow, Tasks.TaskWashCar);
            _masterTasks.Add(EnumTaskIDs.WashLaundry, EnumProcessingTaskSpeed.Slow, Tasks.TaskLaundryWashed);
            _masterTasks.Add(EnumTaskIDs.DryLaundry, EnumProcessingTaskSpeed.Fast, Tasks.TaskLaundryDried);
            _masterTasks.Add(EnumTaskIDs.FoldLaundry, EnumProcessingTaskSpeed.Moderate, Tasks.TaskLaundryFolder);
            _masterTasks.Add(EnumTaskIDs.EatDinner, EnumProcessingTaskSpeed.Moderate, Tasks.TaskDinner);
            _masterTasks.Add(EnumTaskIDs.EatBreakfast, EnumProcessingTaskSpeed.Fast, Tasks.Task_EatBreakfast);


            concurrentEngine.Start();


			// First we define jobs.
			// 2 ways, define the Job and required parameters and manually add to the engine's jobs or in a single call.

			// Method 1:  Job - Check for chores
/*			DayTimeInterval dayTimeIntervalChores = new DayTimeInterval("4am", "8pm");
            TimeUnit checkIntervalChores = new TimeUnit("10s");
            PeriodicJob jobChoress = new PeriodicJob("Do Chores", JobMethod_DoChores, dayTimeIntervalChores, checkIntervalChores, concurrentEngine.AddTask);
            concurrentEngine.AddJob(jobChoress);

            dayTimeIntervalChores = new DayTimeInterval("4am", "8pm");
            checkIntervalChores = new TimeUnit("3s");
            jobChoress = new PeriodicJob("Breakfast", JobMethod_EatBreakfast, dayTimeIntervalChores, checkIntervalChores, concurrentEngine.AddTask);
            concurrentEngine.AddJob(jobChoress);
*/
            // Method 2:  Single call method:
            concurrentEngine.AddNewJob("Eat Breakfast", JobMethod_EatBreakfast, "2am","10pm","3s");

			while (true) {Thread.Sleep(1000);}
			concurrentEngine.Stop();

        }



		private static void Example_Old () {
            // Create list of Master Tasks
			string[] taskIds = new[] { "Take Out Garbage", "Wash Dishes", "Clean Car", "Wash Laundry", "Dry Laundry", "Fold Laundry", "Make Dinner" };

            // Load tasks into Task Table
            List<ProcessingTask> masterTasks = new List<ProcessingTask>();
            int i = 0;
            masterTasks.Add(ProcessingTask.CreateReferenceTask(taskIds[i], i++, EnumProcessingTaskSpeed.Fast, Tasks.TaskTakeOutGarbage));
            masterTasks.Add(ProcessingTask.CreateReferenceTask(taskIds[i], i++, EnumProcessingTaskSpeed.Slow, Tasks.TaskWashDishes));
            masterTasks.Add(ProcessingTask.CreateReferenceTask(taskIds[i], i++, EnumProcessingTaskSpeed.Moderate, Tasks.TaskWashCar));
            masterTasks.Add(ProcessingTask.CreateReferenceTask(taskIds[i], i++, EnumProcessingTaskSpeed.Fast, Tasks.TaskLaundryWashed));
            masterTasks.Add(ProcessingTask.CreateReferenceTask(taskIds[i], i++, EnumProcessingTaskSpeed.Slow, Tasks.TaskLaundryDried));
            masterTasks.Add(ProcessingTask.CreateReferenceTask(taskIds[i], i++, EnumProcessingTaskSpeed.Slow, Tasks.TaskLaundryFolder));
            masterTasks.Add(ProcessingTask.CreateReferenceTask(taskIds[i], i++, EnumProcessingTaskSpeed.Moderate, Tasks.TaskDinner));


            // Start the Queue Engine for short fast processes
            QueueManager fastQueue = new QueueManager("Fast", 7);

            _ = Task.Run(() => fastQueue.Start());

            string name = "Jerry Seinfeld";
            //while ( true ) {
            for (i = 0; i < 10; i++)
            {
                foreach (ProcessingTask processingTask in masterTasks)
                {
                    ProcessingTask t = processingTask.CloneTask(name);
                    fastQueue.AddTask(t);

                    //t.Execute();
                }
            }

            Console.WriteLine("Items in Queue: {0}", fastQueue.QueueCount, Color.DarkCyan);

            while (fastQueue.HasItemsInQueue)
            {
                Console.WriteLine("Main Thread sleeping - still items in fast queue", Color.Yellow);
                Thread.Sleep(2000);
            }

            int j = 0;

		}



		public static bool JobMethod_DoChores (Action<ProcessingTask> addTaskMethod) {
            ProcessingTask task = _masterTasks.Clone(EnumTaskIDs.WashCar,"Porsche");
            addTaskMethod(task);
			return true;
        }


        public static bool JobMethod_EatBreakfast (Action<ProcessingTask> addTaskMethod) {
			ProcessingTask task = _masterTasks.Clone(EnumTaskIDs.EatBreakfast, "Western Omelete");
			addTaskMethod(task);
            return true;
        }




        private static void ConfigureServices(IServiceCollection services) {
            services.AddLogging(configure => configure.AddSerilog());



        }
	}
}
