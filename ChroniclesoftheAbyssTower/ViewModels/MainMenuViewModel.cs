using ChroniclesoftheAbyssTower.Helpers;
using ChroniclesoftheAbyssTower.Services;
using ChroniclesoftheAbyssTower.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChroniclesoftheAbyssTower.ViewModels
{
    /// <summary>
    /// ViewModel สำหรับหน้าเมนูหลัก
    /// </summary>
    public partial class MainMenuViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly PlayerService _playerService;

        [ObservableProperty]
        private string username = string.Empty;

        [ObservableProperty]
        private string greetingText = "กำลังโหลด...";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasNoSavedGame))]
        private bool hasSavedGame;

        public bool HasNoSavedGame => !HasSavedGame;

        [ObservableProperty]
        private string continueInfo = string.Empty;

        public MainMenuViewModel(AuthService authService, PlayerService playerService)
        {
            _authService = authService;
            _playerService = playerService;
            Title = "เมนูหลัก";
        }

        public async Task OnAppearingAsync()
        {
            try
            {
                Username = await SessionManager.GetUsernameAsync() ?? string.Empty;
                GreetingText = !string.IsNullOrWhiteSpace(Username)
                    ? $"ยินดีต้อนรับ {Username}\nหอคอยอเวจีกำลังรอเจ้าอยู่..."
                    : "ยังไม่ได้เข้าสู่ระบบ";

                var userId = await SessionManager.GetUserIdAsync();
                if (!userId.HasValue)
                    return;

                var active = await _playerService.GetActivePlayerAsync();
                if (active != null && active.UserId == userId.Value)
                {
                    if (active.IsGameCompleted)
                    {
                        await _playerService.DeleteIncompletePlayersForUserAsync(userId.Value);
                        SessionManager.ClearActivePlayer();
                        HasSavedGame = false;
                        ContinueInfo = string.Empty;
                        return;
                    }

                    HasSavedGame = true;
                    ContinueInfo = FormatContinueInfo(active.PlayerName, active.CurrentFloor, active.Hp, active.MaxHp);
                    return;
                }

                var latest = await _playerService.GetLatestByUserAsync(userId.Value);
                if (latest != null)
                {
                    HasSavedGame = true;
                    ContinueInfo = FormatContinueInfo(latest.PlayerName, latest.CurrentFloor, latest.Hp, latest.MaxHp);
                    await SessionManager.SetActivePlayerIdAsync(latest.PlayerId);
                }
                else
                {
                    HasSavedGame = false;
                    ContinueInfo = string.Empty;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainMenuVM.OnAppearing] {ex}");
            }
        }

        private static string FormatContinueInfo(string playerName, int currentFloor, int hp, int maxHp)
        {
            return $"{playerName} - ชั้นที่ {currentFloor}/{AppConstants.TotalFloors} - HP {hp}/{maxHp}";
        }

        [RelayCommand]
        private async Task NewGameAsync()
        {
            if (IsBusy) return;

            if (HasSavedGame)
            {
                var confirm = await Shell.Current.DisplayAlert(
                    "เริ่มเกมใหม่?",
                    "เจ้ามีเกมที่เล่นค้างอยู่\nต้องการเริ่มใหม่จริงหรือ?\n(เกมเก่าจะยังอยู่ในช่องเซฟ ถ้าเคยบันทึกไว้)",
                    "เริ่มใหม่",
                    "ยกเลิก");
                if (!confirm) return;
            }

            var name = await Shell.Current.DisplayPromptAsync(
                "ตั้งชื่อผู้เล่น",
                "ชื่อนี้ใช้แสดงในเมนูและข้อมูลบันทึกการเล่น",
                "ยืนยัน",
                "ยกเลิก",
                placeholder: "ผู้เล่น",
                maxLength: 20,
                initialValue: "ผู้เล่น");

            if (string.IsNullOrWhiteSpace(name)) return;

            try
            {
                IsBusy = true;
                var userId = await SessionManager.GetUserIdAsync();
                if (!userId.HasValue)
                {
                    await Shell.Current.GoToAsync(AppConstants.RouteLogin);
                    return;
                }

                await _playerService.CreateNewPlayerAsync(userId.Value, name);
                await Shell.Current.GoToAsync(AppConstants.RouteIntroStory);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainMenuVM.NewGame] {ex}");
                await Shell.Current.DisplayAlert("ข้อผิดพลาด", "สร้างตัวละครไม่สำเร็จ", "ปิด");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ContinueGameAsync()
        {
            if (IsBusy || !HasSavedGame) return;

            try
            {
                IsBusy = true;
                await Shell.Current.GoToAsync(AppConstants.RouteStory);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToCharacterAsync()
        {
            if (!HasSavedGame)
            {
                await Shell.Current.DisplayAlert("ยังไม่มีตัวละคร", "กรุณาเริ่มเกมใหม่ก่อน", "ปิด");
                return;
            }

            await Shell.Current.GoToAsync(AppConstants.RouteCharacter);
        }

        [RelayCommand]
        private async Task GoToSettingsAsync()
        {
            await Shell.Current.GoToAsync(AppConstants.RouteSettings);
        }

        [RelayCommand]
        private async Task GoToBackupAsync()
        {
            await Shell.Current.GoToAsync(AppConstants.RouteBackup);
        }

        [RelayCommand]
        private async Task GoToSaveLoadAsync()
        {
            await Shell.Current.GoToAsync(AppConstants.RouteSaveLoad);
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            var confirm = await Shell.Current.DisplayAlert(
                "ออกจากระบบ",
                "คุณต้องการออกจากระบบหรือไม่?",
                "ออก",
                "ยกเลิก");
            if (!confirm) return;

            await _authService.LogoutAsync();
            await Shell.Current.GoToAsync(AppConstants.RouteLogin);
        }

        [RelayCommand]
        private async Task ExitAsync()
        {
            var confirm = await Shell.Current.DisplayAlert(
                "ออกจากเกม",
                "ต้องการปิดแอปหรือไม่?",
                "ปิด",
                "ยกเลิก");
            if (!confirm) return;

#if ANDROID
            var activity = Platform.CurrentActivity;
            activity?.Finish();
#elif WINDOWS
            Application.Current?.Quit();
#else
            await Task.CompletedTask;
#endif
        }
    }
}
