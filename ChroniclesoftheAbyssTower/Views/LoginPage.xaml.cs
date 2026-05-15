using ChroniclesoftheAbyssTower.Services;
using ChroniclesoftheAbyssTower.ViewModels;

namespace ChroniclesoftheAbyssTower.Views
{
    /// <summary>
    /// หน้า Login - Code-behind ใช้แค่ inject ViewModel ผ่าน DI
    /// Logic อยู่ใน LoginViewModel ทั้งหมด (MVVM)
    /// </summary>
    public partial class LoginPage : ContentPage
    {
        private readonly AudioService _audioService;

        public LoginPage(LoginViewModel viewModel, AudioService audioService)
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
        }
    }
}
