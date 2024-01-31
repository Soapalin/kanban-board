

using MauiApp2.Model;
using System.Diagnostics;

namespace MauiApp2.Views
{
    public partial class NewTaskForm : ContentPage
    {

        public KanbanTask task {  get; set; }

        public CSVDatabase taskDatabase { get; set; }

        public NewTaskForm(CSVDatabase db, KanbanTask tsk)
        {
            this.WidthRequest = 500;
            this.HeightRequest = 800;
            taskDatabase = db;
            task = tsk;
            InitializeComponent();

            TaskTypePicker.SelectedIndex = (int)task.Task_Type;
            TaskStatusPicker.SelectedIndex = (int)task.Status;
            PriorityPicker.SelectedIndex = (int)task.Priority;
            NameEntry.Text = task.Name;
            DescriptionEntry.Text = task.Description;

            



        }


        private async void OnCancel(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(true);

        }

        private async void OnSave(object sender, EventArgs e)
        {
            if (NameEntry.Text == "")
            {
                await DisplayAlert("Alert", "Summary cannot be left empty", "OK");
                return;

            }
            task.Name = NameEntry.Text;

            task.Description = DescriptionEntry.Text;
            task.Priority = (Priority)PriorityPicker.SelectedIndex;
            task.Task_Type = (TaskType)TaskTypePicker.SelectedIndex;
            task.Status = (TaskCompletion)TaskStatusPicker.SelectedIndex;

            Debug.WriteLine(task.Name);
            Debug.WriteLine(task.Description);
            Debug.WriteLine(task.Priority);
            Debug.WriteLine(task.Task_Type);
            Debug.WriteLine(task.Status);

            taskDatabase.SaveSingleTask(task);
            

            await Navigation.PopModalAsync(true);

        }

    }
}
