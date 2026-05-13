using ChroniclesoftheAbyssTower.ViewModels;

namespace ChroniclesoftheAbyssTower.Views
{
    /// <summary>
    /// หน้าเขียน/แก้ไข Journal ของผู้เล่น
    /// รับ query: ?id=N (แก้ไข) หรือ ?floor=N (สร้างใหม่)
    /// </summary>
    public partial class JournalEditorPage : ContentPage
    {
        public JournalEditorPage(JournalEditorViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is JournalEditorViewModel vm)
            {
                await vm.OnAppearingAsync();
            }
        }
    }
}
