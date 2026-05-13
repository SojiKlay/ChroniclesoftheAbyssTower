using ChroniclesoftheAbyssTower.ViewModels;

namespace ChroniclesoftheAbyssTower.Views
{
    /// <summary>
    /// หน้า Inventory - แสดง Item ที่ผู้เล่นมีในกระเป๋า
    /// </summary>
    public partial class InventoryPage : ContentPage
    {
        public InventoryPage(InventoryViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is InventoryViewModel vm)
            {
                await vm.OnAppearingAsync();
            }
        }
    }
}
