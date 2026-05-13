using ChroniclesoftheAbyssTower.ViewModels;

namespace ChroniclesoftheAbyssTower.Views
{
    /// <summary>
    /// หน้า Login - Code-behind ใช้แค่ inject ViewModel ผ่าน DI
    /// Logic อยู่ใน LoginViewModel ทั้งหมด (MVVM)
    /// </summary>
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
