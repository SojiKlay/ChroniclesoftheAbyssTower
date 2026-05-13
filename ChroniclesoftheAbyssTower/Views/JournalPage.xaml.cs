using ChroniclesoftheAbyssTower.ViewModels;

namespace ChroniclesoftheAbyssTower.Views
{
    /// <summary>
    /// หน้า Journal - แสดงบันทึก Story (read-only) + Player (CRUD)
    /// </summary>
    public partial class JournalPage : ContentPage
    {
        public JournalPage(JournalViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is JournalViewModel vm)
            {
                await vm.OnAppearingAsync();
            }
        }
    }
}
