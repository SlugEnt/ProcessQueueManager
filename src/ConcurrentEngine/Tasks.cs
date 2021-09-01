using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public static class Tasks
    {

        public static bool Task_EatBreakfast(object a)
        {
            Console.WriteLine("Eating Breakfast: [ " + a.ToString() + " ]");
            return true;
        }


        public static bool TaskWashCar(object a)
        {
            Thread.Sleep(15);
            //int sleep = RandomSleep(15);
            Console.WriteLine("Wash the {0} - slept: {1}", a.ToString(),15);
            return true;
        }


        public static bool TaskTakeOutGarbage(object a)
        {
            int sleep = RandomSleep(3);
            Console.WriteLine("Garbage taken to curb by {0}  - slept: {1}", a.ToString(), sleep);
            return true;
        }



        public static bool TaskWashDishes(object a)
        {
            int sleep = RandomSleep(3);
            Console.WriteLine("Dishes Washed.  Wife happy - Good Job {0} - slept: {1}", a.ToString(), sleep);
            return true;
        }


        public static bool TaskLaundryWashed(object a)
        {
            throw new ApplicationException();
            int sleep = RandomSleep(1);
            Console.WriteLine("Laundry in washer - {0} slept: {1}", a.ToString(), sleep);
            return true;
        }


        public static bool TaskLaundryDried(object a)
        {
            int sleep = RandomSleep(1);
            Console.WriteLine("Laundry in dryer {0}  - slept: {1}", a.ToString(), sleep);
            return true;
        }


        public static bool TaskLaundryFolder(object a)
        {
            int sleep = RandomSleep(6);
            Console.WriteLine("Laundry has been folded {0} - slept: {1}", a.ToString(), sleep);
            return true;
        }


        public static bool TaskDinner(object a)
        {
            int sleep = RandomSleep(4);
            Console.WriteLine("Dinner has been served {0} - slept: {1}", a.ToString(), sleep);
            return true;
        }


        public static int RandomSleep(int factor)
        {
            Random random = new Random();


            int sleepTime = random.Next(1000 * factor);
            Thread.Sleep(sleepTime);
            return sleepTime;
        }

	}
}
