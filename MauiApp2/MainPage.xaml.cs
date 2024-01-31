
using MauiApp2.Views;

namespace MauiApp2
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        ListView listView = new ListView();
        

        public MainPage()
        {
            InitializeComponent();
            listView.SetBinding(ItemsView.ItemsSourceProperty, "Monkeys");
        }

        private async void OnNavigateKanban(object sender, EventArgs e)
        {
#if DEBUG
            //builder.Logging.AddDebug(); add logging to navigate to ... 
#endif
            await Navigation.PushAsync(new KanbanBoard());



        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count += 10;

            if (count >= 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Click here!";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
