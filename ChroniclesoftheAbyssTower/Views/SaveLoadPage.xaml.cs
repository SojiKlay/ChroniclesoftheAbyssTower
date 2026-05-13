using ChroniclesoftheAbyssTower.ViewModels;

namespace ChroniclesoftheAbyssTower.Views
{
    /// <summary>
    /// หน้า Save/Load - 3 slot ของ save game
    /// </summary>
    public partial class SaveLoadPage : ContentPage
    {
        public SaveLoadPage(SaveLoadViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is SaveLoadViewModel vm)
            {
                await vm.OnAppearingAsync();
            }
        }
    }
}
