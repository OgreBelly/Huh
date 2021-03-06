
using System.Collections.Generic;
using System.Linq;
using Huh.Core.Data;
using Huh.Core.Tasks;
using Huh.Engine.Tasks;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Huh.Engine.Test.Tasks
{
    public class TaskCollectionTest
    {
        private readonly TaskCollection taskCollection;
        public TaskCollectionTest ()
        {
            this.taskCollection = new TaskCollection();
        }

        [Test]
        public void TestAdd ()
        {
            this.taskCollection.Add(CreateTask("abc", 3));

            var returnedTask = this.taskCollection.TakeHighestPriorityTask("abc");

            returnedTask.KeyWord.ShouldBe("abc");
            returnedTask.Priority.ShouldBe(3);
            Assert.AreEqual(returnedTask.Records.FirstOrDefault().Content, "bla");
            returnedTask.Records.FirstOrDefault().Key.ShouldBe("name");

            this.taskCollection.TakeHighestPriorityTask().ShouldBeNull();
        }

        [Test]
        public void TestTakeAll ()
        {
            var expected = new List<ITask> 
            {
                CreateTask("abc", 1)
                , CreateTask("abc", 2)
                , CreateTask("abc", 5)
                , CreateTask("def", 2)
                , CreateTask("def", 2)
                , CreateTask("def", -1)
            };

            this.taskCollection.Add(expected);
            this.taskCollection.Empty.ShouldBeFalse();
            
            var result = this.taskCollection.TakeAll();

            this.taskCollection.Empty.ShouldBeTrue();
            result.Count.ShouldBe(expected.Count);
        }

        [Test]
        public void TestPriority ()
        {
            var tasks = new List<ITask> 
            {
                CreateTask("abc", 1)
                , CreateTask("abc", 2)
                , CreateTask("abc", 5)
                , CreateTask("def", 2)
                , CreateTask("def", 2)
                , CreateTask("def", -1)
            };

            this.taskCollection.Add(tasks);

            this.taskCollection.TakeHighestPriorityTask().Priority.ShouldBe(5);
            this.taskCollection.TakeHighestPriorityTask().Priority.ShouldBe(2);
            this.taskCollection.TakeHighestPriorityTask().Priority.ShouldBe(2);
            this.taskCollection.TakeHighestPriorityTask().Priority.ShouldBe(2);
            this.taskCollection.TakeHighestPriorityTask().Priority.ShouldBe(1);
            this.taskCollection.TakeHighestPriorityTask().Priority.ShouldBe(-1);

            this.taskCollection.TakeHighestPriorityTask().ShouldBeNull();

            this.taskCollection.Add(tasks);

            this.taskCollection.TakeHighestPriorityTask("def").Priority.ShouldBe(2);
            this.taskCollection.TakeHighestPriorityTask("def").Priority.ShouldBe(2);
            this.taskCollection.TakeHighestPriorityTask("def").Priority.ShouldBe(-1);

            this.taskCollection.TakeHighestPriorityTask("def").ShouldBeNull();

            this.taskCollection.TakeHighestPriorityTask("abc").Priority.ShouldBe(5);
            this.taskCollection.TakeHighestPriorityTask("abc").Priority.ShouldBe(2);
            this.taskCollection.TakeHighestPriorityTask("abc").Priority.ShouldBe(1);

            this.taskCollection.TakeHighestPriorityTask("abc").ShouldBeNull();
        }

        private ITask CreateTask (string keyword, int priority)
        {
            var task = new Mock<ITask>();

            task.SetupGet(m => m.KeyWord).Returns(keyword);
            task.SetupGet(m => m.Priority).Returns(priority);
            task.SetupGet(m => m.Records).Returns(new List<Core.Data.Record> {
                new Core.Data.Record("name","", "bla")
                });

            return task.Object;
        }
    }
}