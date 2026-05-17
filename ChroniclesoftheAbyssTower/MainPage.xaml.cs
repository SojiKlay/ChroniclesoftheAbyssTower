namespace ChroniclesoftheAbyssTower
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object? sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"แตะแล้ว {count} ครั้ง";
            else
                CounterBtn.Text = $"แตะแล้ว {count} ครั้ง";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}
