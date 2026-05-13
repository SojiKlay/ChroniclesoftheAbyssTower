using ChroniclesoftheAbyssTower.Helpers;
using ChroniclesoftheAbyssTower.Services;
using ChroniclesoftheAbyssTower.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChroniclesoftheAbyssTower.ViewModels
{
    /// <summary>
    /// ViewModel สำหรับหน้า Login
    /// </summary>
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
            Title = "เข้าสู่ระบบ";
        }

        /// <summary>
        /// กดปุ่ม Login
        /// </summary>
        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy) return;

            ClearError();
            IsBusy = true;

            try
            {
                var result = await _authService.LoginAsync(Username.Trim(), Password);

                if (!result.Success)
                {
                    SetError(result.ErrorMessage ?? "เข้าสู่ระบบไม่สำเร็จ");
                    return;
                }

                // ล้าง password ออกจากหน่วยความจำ
                Password = string.Empty;

                // ไปหน้า Main Menu (route นี้ยังไม่มีจริงจนกว่าจะถึง Phase 3 -
                // ถ้ารันก่อน Phase 3 จะ navigate ไม่ได้แต่ session จะถูกตั้งแล้ว)
                await Shell.Current.GoToAsync(AppConstants.RouteMainMenu);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoginViewModel] {ex}");
                SetError("เกิดข้อผิดพลาด กรุณาลองใหม่");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// ไปหน้า Register
        /// </summary>
        [RelayCommand]
        private async Task GoToRegisterAsync()
        {
            ClearError();
            await Shell.Current.GoToAsync(AppConstants.RouteRegister);
        }
    }
}
