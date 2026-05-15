using ChroniclesoftheAbyssTower.Services;
using ChroniclesoftheAbyssTower.ViewModels;

namespace ChroniclesoftheAbyssTower.Views
{
    /// <summary>
    /// หน้าจบเกม — แสดง ending text + stats summary + restart/menu
    /// </summary>
    public partial class EndingPage : ContentPage
    {
        private readonly AudioService _audioService;

        public EndingPage(EndingViewModel viewModel, AudioService audioService)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _audioService = audioService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is EndingViewModel vm)
            {
                await vm.OnAppearingAsync();
                await PlayEndingBgmAsync(vm.EndingType);
            }
        }

        private Task PlayEndingBgmAsync(string endingType)
        {
            return endingType switch
            {
                "Bad" => _audioService.PlayBgmAsync(
                    "Audio/Bgm/bad_ending.wav",
                    "Audio/Bgm/bad_ending_02.wav"),
                "TrueGood" => _audioService.PlayBgmAsync("Audio/Bgm/true_good_ending.mp3"),
                _ => _audioService.PlayBgmAsync("Audio/Bgm/good_ending.mp3")
            };
        }
    }
}
