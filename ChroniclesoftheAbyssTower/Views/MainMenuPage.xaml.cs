using ChroniclesoftheAbyssTower.ViewModels;

namespace ChroniclesoftheAbyssTower.Views
{
    /// <summary>
    /// หน้าเมนูหลัก
    /// Phase 2: placeholder - มีแค่ greeting + logout
    /// Phase 3: จะใส่ New Game / Continue / Backup / Settings / Exit
    /// </summary>
    public partial class MainMenuPage : ContentPage
    {
        public MainMenuPage(MainMenuViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is MainMenuViewModel vm)
            {
                await vm.OnAppearingAsync();
            }
        }
    }
}
