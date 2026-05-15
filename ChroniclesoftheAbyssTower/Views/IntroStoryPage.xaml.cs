using ChroniclesoftheAbyssTower.Services;
using ChroniclesoftheAbyssTower.ViewModels;

namespace ChroniclesoftheAbyssTower.Views
{
    /// <summary>
    /// หน้าเนื้อเรื่องเปิดเกม (อ่านบทนำก่อนเข้าหอคอย)
    /// </summary>
    public partial class IntroStoryPage : ContentPage
    {
        private readonly AudioService _audioService;

        public IntroStoryPage(IntroStoryViewModel viewModel, AudioService audioService)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _audioService = audioService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _audioService.PlayBgmAsync(
                "Audio/Bgm/intro_story.mp3",
                "Audio/Bgm/intro_story_02.mp3");
        }
    }
}
