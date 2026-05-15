using ChroniclesoftheAbyssTower.Services;
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
        private readonly AudioService _audioService;

        public MainMenuPage(MainMenuViewModel viewModel, AudioService audioService)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _audioService = audioService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _audioService.PlayBgmAsync(
                "Audio/Bgm/main_menu_login_register.mp3",
                "Audio/Bgm/main_menu_login_register_02.mp3");

            if (BindingContext is MainMenuViewModel vm)
            {
                await vm.OnAppearingAsync();
            }
        }
    }
}
