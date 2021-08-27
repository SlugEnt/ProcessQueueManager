# ProcessQueueManager

A Means to manage processes that require threads to run and in which the threads should be run in different queues depending on expected run time of said task.

There are 4 main parts of this process queue manager.
* Concurrent Engine
* Queue Manager 
* Periodic Jobs
* Processing Tasks


## Concurrent Engine
The Process Queue Manager is actually comprised of an engine - ConcurrentEngine that is responsible for managing jobs.  The ConcurrentEngine has 3 queues that are simultaneously running all the time.  These are
* Fast - tasks that can typically run in under 3 seconds.
* Medium - Tasks that can typically run in under 2 minutes
* Slow - Tasks that are expected to take more than 2 minutes to run.  


##QueueManager
Each queue has a maximum number of tasks that it can run in parallel and an expectation of when a task will complete.  If a task has not completed within an allotted time period it will be killed.
The queues support a graceful shutdown, such that if the Engine is shutting down it can send a command to the Queue to stop accepting any further tasks and await completion of any in progress.

## PeriodicJob
The Concurrent Engine works with jobs.  A job is an object that has a run window, which is a time of day it is allowed to run, a pointer to a method of code to execute.  
* Outside of the window, the job will not be run.
* Jobs have a Check Interval as well, which is how often the job is run.  

For instance a job can have a Run Interval of 3pm - 6pm and a check interval of 2m.  This means the job will run every 2 minutes between 3pm and 6pm.  

Jobs also have a MethodToRun.  This is a function pointer that points to a method in the calling application which contains the actual code for the job.  This allows the job to access all the internal data of the application without having to pass it thru one or more parameter arguments.

They also have an AddTask method, which is a function pointer to the Concurrent Engines's Add Task method.  This allows the Job to add one or more tasks to the engine.

A Job should be short lived (less than 2-3 seconds max), if it typically needs any more time than this, then it should schedule a task to perform its actual work.  A job will typically identify things that need to be done and schedule ProcessingTasks to complete them.


ProcessingTask
A ProcessingTask is some processing that needs to be performed.  It has an expected run time, which allows the engine to schedule it onto the appropriate queue.  It also has a function pointer that points to a method in the calling application that performs the actual work of the task.  This allows access to the class variables of the calling class.

A processing task also has a payload parameter which is defined as an object.  This is how the Job can pass information to the task about a specific entity that it needs to process for instance.

There are 2 types of tasks:
* Reference Tasks - which are tasks that serve as a template for real tasks.  They cannot be run.  
* Operational Tasks - which are tasks that can be run.  They can be created from scratch or cloned from a Referrence Task.

