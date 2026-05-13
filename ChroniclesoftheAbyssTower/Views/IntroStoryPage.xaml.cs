using ChroniclesoftheAbyssTower.ViewModels;

namespace ChroniclesoftheAbyssTower.Views
{
    /// <summary>
    /// หน้าเนื้อเรื่องเปิดเกม (อ่านบทนำก่อนเข้าหอคอย)
    /// </summary>
    public partial class IntroStoryPage : ContentPage
    {
        public IntroStoryPage(IntroStoryViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
