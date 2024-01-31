
using MauiApp2.Model;

using System.Collections.ObjectModel;
using System.Diagnostics;


namespace MauiApp2.Views
{
    public partial class KanbanBoard : ContentPage
    {
        public ObservableCollection<KanbanTask> BACKLOG_List { get; set; }
        public ObservableCollection<KanbanTask> TODAY_FOCUS_List { get; set; }
        public ObservableCollection<KanbanTask> DOING_List { get; set; }
        public ObservableCollection<KanbanTask> DONE_List { get; set; }
        public object CompletionProgress { get; private set; }

        
        private CSVDatabase taskDatabase { get; set; }





        public KanbanBoard()
        {
            InitializeComponent();


            BACKLOG_List = new ObservableCollection<KanbanTask>();
            TODAY_FOCUS_List = new ObservableCollection<KanbanTask>();
            DOING_List = new ObservableCollection<KanbanTask>();
            DONE_List = new ObservableCollection<KanbanTask>();



            taskDatabase = new CSVDatabase("C:\\Users\\lucien.tran\\Documents\\Kanban.csv");
            LoadTaskFromDB();
            helloLabel.Text = "Hello, World!";

            BACKLOG.ItemsSource = BACKLOG_List;
            TODAY_FOCUS.ItemsSource = TODAY_FOCUS_List;
            DOING.ItemsSource = DOING_List;
            DONE.ItemsSource = DONE_List;


            

            //ListView TaskList = new ListView();
            //TaskList.ItemsSource = innerTaskList;
            //Console.WriteLine(TaskList.ItemsSource);
            //TaskList.SetBinding(ItemsView.ItemsSourceProperty, "KanbanTask");




        }



        private void OnSliderDragCompleted(object sender, EventArgs e)
        {
            Slider slider = (Slider) sender;
            KanbanTask currentTask = (KanbanTask)slider.BindingContext;
            //currentTask.Completion = (double) e.Data.Text;
            SaveTaskToFile(currentTask);
        }
        


        private void OnSliderValueChanged(object sender, ValueChangedEventArgs e) {
            double value = e.NewValue;
            helloLabel.Text = value.ToString();
            Slider slider = (Slider)sender;
            KanbanTask currentTask = (KanbanTask)slider.BindingContext;
            if (currentTask != null)
            {
                currentTask.Completion = value;
            }



        }

        private void SaveTaskToFile(KanbanTask task)
        {
            taskDatabase.SaveSingleTask(task);
        }

        private async void PickFile(object sender, EventArgs e)
        {


            var customFileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                {
                    DevicePlatform.iOS, new[]
                    {
                        "com.microsoft.word.doc",
                        "org.openxmlformats.wordprocessingml.document"
                    }
                },
                {
                    DevicePlatform.Android, new[]
                    {
                        "application/msword",
                        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                    }
                },
                {
                    DevicePlatform.WinUI, new[]
                    {
                        "csv"
                    }
                },
            });
            var csv = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Pick KanbanBoard Config File",
                FileTypes = customFileTypes
            });

            if (csv != null)
            {
                var csvPath = csv.FullPath.ToString();
                helloLabel.Text = csvPath;
                FilePickerEntry.Text = csvPath;
                //string readText = File.ReadAllText(csvPath);

                //Debug.WriteLine(readText);

                //string[] readLines = readText.Split("\n");
                //Debug.WriteLine(readLines[0]);

                taskDatabase = new CSVDatabase(csvPath);
                LoadTaskFromDB();


            } 

        }

        private void LoadTaskFromDB()
        {
            DOING_List.Clear();
            BACKLOG_List.Clear();
            TODAY_FOCUS_List.Clear();
            DONE_List.Clear();

            taskDatabase.PopulateTasks();



            for (int i = 0; i < taskDatabase.AllTasks.Count; i++)
            {
                KanbanTask exampleTask = taskDatabase.AllTasks[i];
                switch (exampleTask.Status)
                {
                    case TaskCompletion.BACKLOG:
                        BACKLOG_List.Add(exampleTask);
                        break;
                    case TaskCompletion.FOCUS:
                        TODAY_FOCUS_List.Add(exampleTask);
                        break;
                    case TaskCompletion.DOING:
                        DOING_List.Add(exampleTask);
                        break;
                    case TaskCompletion.DONE:
                        DONE_List.Add(exampleTask);
                        break;

                }

            }
        }

        private async void OnCreateNewTask(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Debug.WriteLine(button.CommandParameter);
            TaskCompletion completion = new TaskCompletion();
            switch(button.CommandParameter)
            {
                case "BACKLOG":
                    completion = TaskCompletion.BACKLOG; break;
                case "FOCUS":
                    completion = TaskCompletion.FOCUS; break;
                case "DOING":
                    completion = TaskCompletion.DOING; break;
                case "DONE":
                    completion = TaskCompletion.DONE; break;
                default:
                    completion = TaskCompletion.BACKLOG; break;
            }
            //await Shell.Current.GoToAsync(new NewTaskForm(taskDatabase, new KanbanTask("", "", Priority.LOW, TaskType.TEST_CYCLE, TaskCompletion.BACKLOG)));

            await Navigation.PushModalAsync(new NewTaskForm(taskDatabase, new KanbanTask("", "", Priority.HIGH, TaskType.TEST_CYCLE, completion)));

        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            Debug.WriteLine("OnDragOver");
        }

        private void OnDragLeave(object sender, DragEventArgs e)
        {
            Debug.WriteLine("OnDragLeave");
        }

        private void OnCellAdded(object sender, DropEventArgs e)
        {
            Debug.WriteLine("OnCellAdded");
            DropGestureRecognizer gest = (DropGestureRecognizer)sender;

            KanbanTask dropTask = (KanbanTask)gest.BindingContext;
            Debug.WriteLine(dropTask.Name);
            KanbanTask dragTask = (KanbanTask)e.Data.Properties["DraggedTask"];
            Debug.WriteLine(dragTask.Name);

            if (dragTask.Status == dropTask.Status)
            {
                return;
            }
            int index = -1;
            switch (dropTask.Status)
            {
                case TaskCompletion.BACKLOG:
                    RemoveByStatus(dragTask);
                    dragTask.Status = TaskCompletion.BACKLOG;
                    index = BACKLOG_List.IndexOf(dropTask);
                    BACKLOG_List.Insert(index+1, dragTask);
                    break;
                case TaskCompletion.FOCUS:
                    RemoveByStatus(dragTask);
                    dragTask.Status = TaskCompletion.FOCUS;
                    index = TODAY_FOCUS_List.IndexOf(dropTask);
                    TODAY_FOCUS_List.Insert(index + 1, dragTask);
                    break;
                case TaskCompletion.DOING:
                    RemoveByStatus(dragTask);
                    dragTask.Status = TaskCompletion.DOING;
                    index = DOING_List.IndexOf(dropTask);
                    DOING_List.Insert(index + 1, dragTask);
                    break;
                case TaskCompletion.DONE:
                    RemoveByStatus(dragTask);
                    dragTask.Status = TaskCompletion.DONE;
                    dragTask.Completion = 1;
                    index = DONE_List.IndexOf(dropTask);
                    DONE_List.Insert(index + 1, dragTask);
                    break;
            }
            SaveTaskToFile(dragTask);
        }

        private void OnCellAddedVSL(object sender, DropEventArgs e)
        {
            Debug.WriteLine("OnCellAddedVSL");
            DropGestureRecognizer gest = (DropGestureRecognizer)sender;
            Debug.WriteLine(gest.BindingContext);
            ListView dropTaskList = (ListView) gest.BindingContext;
            Debug.WriteLine(dropTaskList);


            TaskCompletion dropTaskStatus = new TaskCompletion();
            if (dropTaskList.ItemsSource.Equals(TODAY_FOCUS_List))
            {
                dropTaskStatus = TaskCompletion.FOCUS;
            } else if (dropTaskList.ItemsSource.Equals(DOING_List))
            {
                dropTaskStatus = TaskCompletion.DOING;
            } else if (dropTaskList.ItemsSource.Equals(DONE_List))
            {
                dropTaskStatus = TaskCompletion.DONE;   
            } else if (dropTaskList.ItemsSource.Equals(BACKLOG_List))
            {
                dropTaskStatus = TaskCompletion.BACKLOG;
            }
            else
            {
                return;
            }

            KanbanTask dragTask = (KanbanTask)e.Data.Properties["DraggedTask"];
            if (dragTask.Status == dropTaskStatus)
            {
                return;
            }
            Debug.WriteLine(dragTask.Name);
            Debug.WriteLine(dragTask.Description);


            switch (dropTaskStatus)
            {
                case TaskCompletion.BACKLOG:
                    RemoveByStatus(dragTask);
                    dragTask.Status = TaskCompletion.BACKLOG;
                    if(BACKLOG_List.Any())
                    {
                        BACKLOG_List.Insert(0,dragTask);
                    } else
                    {
                        BACKLOG_List.Add(dragTask);
                    }
                    
                    break;
                case TaskCompletion.FOCUS:
                    RemoveByStatus(dragTask);
                    dragTask.Status = TaskCompletion.FOCUS;
                    if(TODAY_FOCUS_List.Any())
                    {
                        TODAY_FOCUS_List.Insert(0, dragTask);
                    } else
                    {
                        TODAY_FOCUS_List.Add(dragTask);
                    }
                    break;
                case TaskCompletion.DOING:
                    RemoveByStatus(dragTask);
                    dragTask.Status = TaskCompletion.DOING;
                    if(DOING_List.Any())
                    {
                        DOING_List.Insert(0, dragTask);
                    } else
                    {
                        DOING_List.Add(dragTask);
                    }
                    break;
                case TaskCompletion.DONE:
                    RemoveByStatus(dragTask);
                    dragTask.Status = TaskCompletion.DONE;
                    dragTask.Completion = 1;
                    if(DONE_List.Any())
                    {
                        DONE_List.Insert(0, dragTask);
                    } else
                    {
                        DONE_List.Add(dragTask);
                    }
                    break;
            }
            SaveTaskToFile(dragTask);



        }

        private void RemoveByStatus(KanbanTask task)
        {
            int index = -1;
            switch (task.Status)
            {
                case TaskCompletion.BACKLOG:
                    index = BACKLOG_List.IndexOf(task);
                    if (index != -1)
                    {
                        BACKLOG_List.RemoveAt(index);
                    }
                    break;
                case TaskCompletion.FOCUS:
                    index = TODAY_FOCUS_List.IndexOf(task);
                    if (index != -1)
                    {
                        TODAY_FOCUS_List.RemoveAt(index);
                    }
                    break;
                case TaskCompletion.DOING:
                    index = DOING_List.IndexOf(task);
                    if (index != -1)
                    {
                        DOING_List.RemoveAt(index);

                    }
                    break;
                case TaskCompletion.DONE:
                    index = DONE_List.IndexOf(task);
                    if (index != -1)
                    {
                        DONE_List.RemoveAt(index);

                    }
                    break;

            }
            //SaveTaskToFile(task);
        }



        private void OnCellDropped(object sender, EventArgs e)
        {
            Debug.WriteLine("OnCellDropped");
            DragGestureRecognizer gest = (DragGestureRecognizer)sender;
            Debug.WriteLine(gest.BindingContext);
        }

        private void OnCellRemoved(object sender, DragStartingEventArgs e)
        {
            Debug.WriteLine("OnCellRemoved");
            DragGestureRecognizer gest = (DragGestureRecognizer)sender;
            KanbanTask task = (KanbanTask)gest.BindingContext;
            e.Data.Properties.Add("DraggedTask", task);
            Debug.WriteLine(task.Name);

        }



        private async void OnDoubleTapped(object sender, EventArgs e)
        {
            Debug.WriteLine("OnDoubleTappedCell");
            VerticalStackLayout layout = (VerticalStackLayout)sender;
            KanbanTask task = (KanbanTask)layout.BindingContext;
            Debug.WriteLine(task.Name);
            await Navigation.PushModalAsync(new NewTaskForm(taskDatabase, task));


        }

        private void OnBackClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("OnBackClicked");
            Button button = (Button)sender;
            KanbanTask task =  (KanbanTask) button.BindingContext;
            
            Debug.WriteLine(task.UUID);
            int index = -1;
            switch(task.Status)
            {
                case TaskCompletion.FOCUS:
                    index = TODAY_FOCUS_List.IndexOf(task);
                    if (index != -1)
                    {
                        TODAY_FOCUS_List.RemoveAt(index);
                        task.Status = TaskCompletion.BACKLOG;
                        BACKLOG_List.Add(task); 
                    }
                    break;
                case TaskCompletion.DOING:
                    index = DOING_List.IndexOf(task);
                    if (index != -1)
                    {
                        DOING_List.RemoveAt(index);
                        task.Status = TaskCompletion.FOCUS;
                        TODAY_FOCUS_List.Add(task);
                    }
                    break;
                case TaskCompletion.DONE:
                    index = DONE_List.IndexOf(task);
                    if (index != -1)
                    {
                        DONE_List.RemoveAt(index);
                        task.Status = TaskCompletion.DOING;
                        task.Completion = 0;
                        DOING_List.Add(task);
                    }
                    break;
                default:
                    break;
            }
            SaveTaskToFile(task);



        }

        private void OnNextClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("OnNextClicked");
            Button button = (Button)sender;
            KanbanTask task = (KanbanTask)button.BindingContext;
            Debug.WriteLine(task.UUID);
            int index = -1;
            switch (task.Status)
            {
                case TaskCompletion.BACKLOG:
                    index = BACKLOG_List.IndexOf(task);
                    if (index != -1)
                    {
                        BACKLOG_List.RemoveAt(index);
                        task.Status = TaskCompletion.FOCUS;
                        TODAY_FOCUS_List.Add(task);
                    }
                    break;
                case TaskCompletion.FOCUS:
                    index = TODAY_FOCUS_List.IndexOf(task);
                    if (index != -1)
                    {
                        TODAY_FOCUS_List.RemoveAt(index); 
                        task.Status = TaskCompletion.DOING;
                        DOING_List.Add(task);
                    }
                    break;
                case TaskCompletion.DOING:
                    index = DOING_List.IndexOf(task);
                    if (index != -1)
                    {
                        DOING_List.RemoveAt(index);
                        task.Status = TaskCompletion.DONE;
                        task.Completion = 1;
                        DONE_List.Add(task);
                    }
                    break;
                default:
                    break;


            }
            SaveTaskToFile(task);

        }

        private void OnDelete(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            KanbanTask deleteTask = (KanbanTask)button.BindingContext;
            RemoveByStatus(deleteTask);
            taskDatabase.DeleteSingleTask(deleteTask.UUID);
        }


        private void OnPrioritySort(object sender, EventArgs e)
        {
            Debug.WriteLine("OnPrioritySort");
        }

        private void OnCompletionSort(object sender, EventArgs e)
        {
            Debug.WriteLine("OnCompletionSort");
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("onAppearing");
            LoadTaskFromDB();
        }





    }
}
