

using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading.Tasks;

namespace MauiApp2.Model
{
    public class CSVDatabase
    {
        public string Path { get; set; }

        public List<KanbanTask> AllTasks { get; set; }


        public CSVDatabase(string path) 
        {
            Debug.WriteLine("DEBUG | CSVDatabase.Constructor | " + path);
            Path = path;
            AllTasks = new List<KanbanTask>();
        }    


        public void PopulateTasks()
        {
            using (TextFieldParser csvParser = new TextFieldParser(Path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(",");
                csvParser.HasFieldsEnclosedInQuotes = true;




                csvParser.ReadLine();

                while (!csvParser.EndOfData)
                {
                    string[] fields = csvParser.ReadFields();
                    if (fields != null)
                    {
                        KanbanTask exampleTask = new KanbanTask("", "", Priority.HIGH, TaskType.TEST_CYCLE, TaskCompletion.DONE, 1);
                        KanbanTask tmp = exampleTask.ConvertArrayToTask(fields);
                        Debug.WriteLine(tmp.UUID);
                        int index = AllTasks.FindIndex(task => task.UUID == tmp.UUID);
                        tmp.Description = tmp.Description.Replace("\\n", "\r");
                        if (index == -1)
                        {
                            
                            AllTasks.Add(tmp);
                            Debug.WriteLine(AllTasks.Count);

                        } else
                        {
                            AllTasks[index] = tmp;
                        }



                    }
                }

            }
        }


        //public KanbanTask GetSingleTaskByID(string id)
        //{

        //    return new KanbanTask();
        //}

        public void SaveToCSV()
        {
            Debug.WriteLine("DEBUG | CSVDatabase.SaveToCSV");
            string result = "ID,Name,Description,Priority,Task_Type,Status,Completion\n";
            for(int i = 0; i < AllTasks.Count; i++)
            {
                result += AllTasks[i].ToCSVString() + "\n";

            }

            File.WriteAllText(Path, result);
        }


        public void DeleteSingleTask(string taskID) 
        {
            int index = AllTasks.FindIndex(task => task.UUID == taskID);
            if (index != -1 ) { 
                AllTasks.RemoveAt(index);   
            }
            SaveToCSV();
        }

        


        public void SaveSingleTask(KanbanTask task)
        {
            Debug.WriteLine("DEBUG | CSVDatabase.SaveSingleTask | task.UUID " + task.UUID);
            string taskId = task.UUID;
            Debug.WriteLine(task.Description);
            Debug.WriteLine(task.Description.IndexOf("\r\n"));
            Debug.WriteLine(task.Description.IndexOf("\r"));
            Debug.WriteLine(task.Description.IndexOf("\\n"));
            Debug.WriteLine(task.Description.IndexOf("\n"));
            Debug.WriteLine(task.Description.IndexOf(Environment.NewLine));
            task.Description = task.Description.Replace("\r", "\\n");
            Debug.WriteLine(task.Description);
            Debug.WriteLine(task.Description.IndexOf('\n'));
            string taskString = task.ToCSVString();

            int index = AllTasks.FindIndex(task => task.UUID == taskId);    
            if (index == -1)
            {
                AllTasks.Add(task);
                
            }
            else
            {
                AllTasks[index] = task;
            }
            SaveToCSV();


        }
    }
}
