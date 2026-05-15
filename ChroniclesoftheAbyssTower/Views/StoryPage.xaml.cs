using ChroniclesoftheAbyssTower.Services;
using ChroniclesoftheAbyssTower.ViewModels;

namespace ChroniclesoftheAbyssTower.Views
{
    /// <summary>
    /// หน้าเล่นเกมหลัก — แสดง floor narrative + 3 choices
    /// </summary>
    public partial class StoryPage : ContentPage
    {
        private readonly AudioService _audioService;

        public StoryPage(StoryViewModel viewModel, AudioService audioService)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _audioService = audioService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _audioService.PlayBgmAsync(
                "Audio/Bgm/tower_ambient.mp3",
                "Audio/Bgm/tower_ambient_02.mp3");

            if (BindingContext is StoryViewModel vm)
            {
                await vm.OnAppearingAsync();
            }
        }
    }
}
