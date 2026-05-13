using ChroniclesoftheAbyssTower.ViewModels;

namespace ChroniclesoftheAbyssTower.Views
{
    /// <summary>
    /// หน้า Backup - export/import/share/restore ไฟล์ JSON
    /// </summary>
    public partial class BackupPage : ContentPage
    {
        public BackupPage(BackupViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is BackupViewModel vm)
            {
                await vm.OnAppearingAsync();
            }
        }
    }
}
