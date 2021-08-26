using NUnit.Framework;
using SlugEnt.ProcessQueueManager;

namespace Test_ConcurrentEngine
{
    [TestFixture]
	public class Test_ProcessingTask {
		private const string NAME = "aTask";

		private const int TYPE_ID = 355395;

		private const EnumProcessingTaskSpeed SPEED = EnumProcessingTaskSpeed.Fast;

		private long ID = 0;

		// These initial tests must be run in order to validate the Id property.
		[Test, Order(1)]
		public void Construction () {
			ProcessingTask t = new ProcessingTask(NAME,TYPE_ID,SPEED,TaskMethodSuccess);

			ID++;
			Assert.AreEqual(NAME,t.Name,"A10:");
			Assert.AreEqual(TYPE_ID, t.TaskTypeId, "A20:");
			Assert.AreEqual(SPEED, t.TaskSpeed, "A30:");
			//Assert.AreEqual(ID, t.Id, "A40:");
			Assert.IsNull(t.Payload,"A50:");
			Assert.IsNotNull(t.MethodToExecute,"A60:");
			Assert.IsFalse(t.IsReferenceTask,"A70:");
			Assert.IsTrue(t.Execute(),"A200:");
		}


		[Test, Order(10)]
		public void CreateReferenceTask () {
			ProcessingTask t = ProcessingTask.CreateReferenceTask(NAME, TYPE_ID, SPEED, TaskMethodSuccess);
			ID++;
			Assert.AreEqual(NAME, t.Name, "A10:");
			Assert.AreEqual(TYPE_ID, t.TaskTypeId, "A20:");
			Assert.AreEqual(SPEED, t.TaskSpeed, "A30:");
			//Assert.AreEqual(ID, t.Id, "A40:");
			Assert.IsNull(t.Payload, "A50:");
			Assert.IsNotNull(t.MethodToExecute, "A60:");

			// Reference Task.  They cannot be executed...
			Assert.IsTrue(t.IsReferenceTask, "A70:");
			Assert.IsFalse(t.Execute(), "A200:");
		}



		[Test, Order(20)]
		public void CloneTask_NullPayload()
		{
			ProcessingTask t = ProcessingTask.CreateReferenceTask(NAME, TYPE_ID, SPEED, TaskMethodSuccess);
			ID++;
			Assert.AreEqual(NAME, t.Name, "A10:");
			Assert.AreEqual(TYPE_ID, t.TaskTypeId, "A20:");
			Assert.AreEqual(SPEED, t.TaskSpeed, "A30:");
			//Assert.AreEqual(ID, t.Id, "A40:");
			Assert.IsNull(t.Payload, "A50:");
			Assert.IsNotNull(t.MethodToExecute, "A60:");

			// Reference Task.  They cannot be executed...
			Assert.IsTrue(t.IsReferenceTask, "A70:");
			Assert.IsFalse(t.Execute(), "A99:");


			// Clone IT
			ProcessingTask t2 = t.CloneTask();
			ID++;

			Assert.AreEqual(NAME, t2.Name, "A110:");
			Assert.AreEqual(TYPE_ID, t2.TaskTypeId, "A120:");
			Assert.AreEqual(SPEED, t2.TaskSpeed, "A130:");
			//Assert.AreEqual(ID, t2.Id, "A140:");
			Assert.IsNull(t2.Payload, "A150:");
			Assert.IsNotNull(t2.MethodToExecute, "A160:");

			// A copied reference task produces a normal task
			Assert.IsFalse(t2.IsReferenceTask, "A170:");
			Assert.IsTrue(t2.Execute(), "A199:");
		}




		[Test, Order(20)]
		public void CloneTask_WithPayload()
		{
			ProcessingTask t = ProcessingTask.CreateReferenceTask(NAME, TYPE_ID, SPEED, TaskMethodSuccess);
			ID++;
			Assert.AreEqual(NAME, t.Name, "A10:");
			Assert.AreEqual(TYPE_ID, t.TaskTypeId, "A20:");
			Assert.AreEqual(SPEED, t.TaskSpeed, "A30:");
			//Assert.AreEqual(ID, t.Id, "A40:");
			Assert.IsNull(t.Payload, "A50:");
			Assert.IsNotNull(t.MethodToExecute, "A60:");

			// Reference Task.  They cannot be executed...
			Assert.IsTrue(t.IsReferenceTask, "A70:");
			Assert.IsFalse(t.Execute(), "A99:");


			// Clone IT
			string payload = "Heavy Payload";
			ProcessingTask t2 = t.CloneTask(payload);
			ID++;

			Assert.AreEqual(NAME, t2.Name, "A110:");
			Assert.AreEqual(TYPE_ID, t2.TaskTypeId, "A120:");
			Assert.AreEqual(SPEED, t2.TaskSpeed, "A130:");
			//Assert.AreEqual(ID, t2.Id, "A140:");
			Assert.AreEqual(payload,t2.Payload, "A150:");
			Assert.IsNotNull(t2.MethodToExecute, "A160:");

			// A copied reference task produces a normal task
			Assert.IsFalse(t2.IsReferenceTask, "A170:");
			Assert.IsTrue(t2.Execute(), "A199:");
		}



		public bool TaskMethodSuccess (object payload) {
			return true;
		}

		public bool TaskMethodFailure(object payload)
		{
			return false;
		}
	}
}
