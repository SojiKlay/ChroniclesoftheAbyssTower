using ChroniclesoftheAbyssTower.ViewModels;

namespace ChroniclesoftheAbyssTower.Views
{
    /// <summary>
    /// หน้าจบเกม — แสดง ending text + stats summary + restart/menu
    /// </summary>
    public partial class EndingPage : ContentPage
    {
        public EndingPage(EndingViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is EndingViewModel vm)
            {
                await vm.OnAppearingAsync();
            }
        }
    }
}
