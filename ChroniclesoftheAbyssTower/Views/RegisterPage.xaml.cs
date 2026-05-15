using ChroniclesoftheAbyssTower.Services;
using ChroniclesoftheAbyssTower.ViewModels;

namespace ChroniclesoftheAbyssTower.Views
{
    /// <summary>
    /// หน้าสมัครสมาชิก - logic อยู่ใน RegisterViewModel
    /// </summary>
    public partial class RegisterPage : ContentPage
    {
        private readonly AudioService _audioService;

        public RegisterPage(RegisterViewModel viewModel, AudioService audioService)
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
