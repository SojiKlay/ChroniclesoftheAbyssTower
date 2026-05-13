using ChroniclesoftheAbyssTower.ViewModels;

namespace ChroniclesoftheAbyssTower.Views
{
    /// <summary>
    /// หน้าเล่นเกมหลัก — แสดง floor narrative + 3 choices
    /// </summary>
    public partial class StoryPage : ContentPage
    {
        public StoryPage(StoryViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is StoryViewModel vm)
            {
                await vm.OnAppearingAsync();
            }
        }
    }
}
