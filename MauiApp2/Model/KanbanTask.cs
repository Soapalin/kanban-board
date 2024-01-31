

using Microsoft.Maui.Graphics.Text;
using System.Threading.Tasks;

namespace MauiApp2.Model
{
    public enum TaskType
    {
        BUG,
        TEST_CYCLE,
        TEST_SPEC,
        TASK,
        TEST_FRAMEWORK,
        TEST_SCRIPT,
        TD
    }

    public enum TaskCompletion
    {
        BACKLOG,
        FOCUS,
        DOING,
        DONE
    }

    public enum Priority
    {
        LOW,
        MEDIUM,
        HIGH,
        HIGHEST
    }


    public class KanbanTask
    {

        public string UUID { get ; set; }   
        public string Name { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public double Completion { get; set; }
        public TaskType Task_Type { get; set; }
        public TaskCompletion Status { get; set; }

        public KanbanTask(string name, string description, Priority priority, TaskType taskType, TaskCompletion status, double completion=0)
        {
            UUID = Guid.NewGuid().ToString();   
            Name = name;
            Description= description;
            Priority = priority;
            Completion = completion;
            Task_Type = taskType;
            Status = status;
        }


        public KanbanTask ConvertArrayToTask(string[] array)
        {
            UUID = array[0];
            Name = array[1];
            Description = array[2];
            Priority = Priority.HIGH;
            switch (array[3])
            {
                case "LOW":
                    Priority = Priority.LOW;
                    break;
                case "MEDIUM":
                    Priority = Priority.MEDIUM;
                    break;
                case "HIGH":
                    Priority = Priority.HIGH;
                    break;
            }
            Task_Type = TaskType.TASK;
            switch (array[4])
            {
                case "TEST_CYCLE":
                    Task_Type = TaskType.TEST_CYCLE; break;
                case "TEST_SPEC":
                    Task_Type = TaskType.TEST_SPEC; break;
                case "TASK":
                    Task_Type = TaskType.TASK; break;
                case "BUG":
                    Task_Type = TaskType.BUG; break;
                case "TEST_FRAMEWORK":
                    Task_Type = TaskType.TEST_FRAMEWORK; break;
                case "TEST_SCRIPT":
                    Task_Type = TaskType.TEST_SCRIPT; break;
                case "TD":
                    Task_Type = TaskType.TD; break;
            }
            Status = TaskCompletion.BACKLOG;
            switch(array[5])
            {
                case "BACKLOG":
                    Status = TaskCompletion.BACKLOG;break;
                case "FOCUS":
                    Status = TaskCompletion.FOCUS; break;
                case "DOING":
                    Status = TaskCompletion.DOING; break;
                case "DONE":
                    Status = TaskCompletion.DONE; break;   
            }
            Completion = Convert.ToDouble(array[6]);

            return this;


        }


        public string ToCSVString()
        {
            return String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6:N4}\",", UUID, Name, Description, Priority, Task_Type,Status, Completion);
        }

    }
}
