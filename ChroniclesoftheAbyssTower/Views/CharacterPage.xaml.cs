using ChroniclesoftheAbyssTower.ViewModels;

namespace ChroniclesoftheAbyssTower.Views
{
    /// <summary>
    /// หน้าแสดงสถานะตัวละคร (Character Sheet)
    /// </summary>
    public partial class CharacterPage : ContentPage
    {
        public CharacterPage(CharacterViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is CharacterViewModel vm)
            {
                await vm.OnAppearingAsync();
            }
        }
    }
}
