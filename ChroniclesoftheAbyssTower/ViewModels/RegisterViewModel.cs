using ChroniclesoftheAbyssTower.Helpers;
using ChroniclesoftheAbyssTower.Services;
using ChroniclesoftheAbyssTower.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChroniclesoftheAbyssTower.ViewModels
{
    /// <summary>
    /// ViewModel สำหรับหน้า Register (สมัครสมาชิก)
    /// </summary>
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string confirmPassword = string.Empty;

        public RegisterViewModel(AuthService authService)
        {
            _authService = authService;
            Title = "สมัครสมาชิก";
        }

        /// <summary>
        /// กดปุ่ม Register
        /// </summary>
        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (IsBusy) return;

            ClearError();
            IsBusy = true;

            try
            {
                var result = await _authService.RegisterAsync(
                    Username.Trim(),
                    Password,
                    ConfirmPassword);

                if (!result.Success)
                {
                    SetError(result.ErrorMessage ?? "สมัครสมาชิกไม่สำเร็จ");
                    return;
                }

                // ล้าง password ออกจากหน่วยความจำ
                Password = string.Empty;
                ConfirmPassword = string.Empty;

                // ไปหน้า Main Menu (auto-login หลังสมัคร)
                await Shell.Current.GoToAsync(AppConstants.RouteMainMenu);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RegisterViewModel] {ex}");
                SetError("เกิดข้อผิดพลาด กรุณาลองใหม่");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// ย้อนกลับไปหน้า Login
        /// </summary>
        [RelayCommand]
        private async Task GoToLoginAsync()
        {
            ClearError();
            await Shell.Current.GoToAsync(AppConstants.RouteLogin);
        }
    }
}
