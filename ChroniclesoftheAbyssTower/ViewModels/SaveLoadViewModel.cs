using System.Collections.ObjectModel;
using ChroniclesoftheAbyssTower.Helpers;
using ChroniclesoftheAbyssTower.Models;
using ChroniclesoftheAbyssTower.Services;
using ChroniclesoftheAbyssTower.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChroniclesoftheAbyssTower.ViewModels
{
    /// <summary>
    /// ViewModel สำหรับหน้าเซฟ / โหลดเกม
    /// </summary>
    public partial class SaveLoadViewModel : BaseViewModel
    {
        private readonly SaveLoadService _saveLoadService;
        private readonly PlayerService _playerService;

        public SaveLoadViewModel(SaveLoadService saveLoadService, PlayerService playerService)
        {
            _saveLoadService = saveLoadService;
            _playerService = playerService;
            Title = "เซฟ / โหลดเกม";
        }

        public ObservableCollection<SaveSlotVm> Slots { get; } = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSave))]
        private bool hasActivePlayer;

        public bool CanSave => HasActivePlayer;

        [ObservableProperty]
        private string activePlayerInfo = "ยังไม่มีตัวละครที่กำลังเล่นอยู่";

        public async Task OnAppearingAsync()
        {
            try
            {
                IsBusy = true;
                ClearError();

                var userId = await SessionManager.GetUserIdAsync();
                if (!userId.HasValue)
                {
                    SetError("กรุณาเข้าสู่ระบบ");
                    return;
                }

                var activePlayer = await _playerService.GetActivePlayerAsync();
                HasActivePlayer = activePlayer != null;
                ActivePlayerInfo = activePlayer != null
                    ? $"{activePlayer.PlayerName} - ชั้น {activePlayer.CurrentFloor}/{AppConstants.TotalFloors} - พลังชีวิต {activePlayer.Hp}/{activePlayer.MaxHp}"
                    : "ยังไม่มีตัวละครที่กำลังเล่นอยู่";

                await ReloadSlotsAsync(userId.Value);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SaveLoadVM.OnAppearing] {ex}");
                SetError("ไม่สามารถโหลดข้อมูลเซฟได้");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ReloadSlotsAsync(int userId)
        {
            var data = await _saveLoadService.GetAllSlotsAsync(userId);

            Slots.Clear();
            for (int i = 0; i < AppConstants.MaxSaveSlots; i++)
            {
                var slotNum = i + 1;
                var save = data[i];
                Slots.Add(new SaveSlotVm
                {
                    SlotNumber = slotNum,
                    SlotLabel = $"ช่อง {slotNum}",
                    IsEmpty = save == null,
                    SaveName = save?.SaveName ?? "(ว่าง)",
                    PlayerName = save?.PlayerName ?? string.Empty,
                    FloorDisplay = save != null ? $"ชั้น {save.CurrentFloor}" : string.Empty,
                    HpDisplay = save != null ? $"พลังชีวิต {save.Hp}" : string.Empty,
                    LevelDisplay = save != null ? $"เลเวล {save.Level}" : string.Empty,
                    DateDisplay = save?.SaveDate.ToLocalTime().ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                });
            }
        }

        [RelayCommand]
        private async Task SaveToSlotAsync(SaveSlotVm? slot)
        {
            if (slot == null || IsBusy) return;
            if (!HasActivePlayer)
            {
                await Shell.Current.DisplayAlert("ไม่มีตัวละคร", "กรุณาเริ่มเกมก่อนถึงจะเซฟได้", "ปิด");
                return;
            }

            if (!slot.IsEmpty)
            {
                var confirm = await Shell.Current.DisplayAlert(
                    $"เขียนทับช่อง {slot.SlotNumber}?",
                    $"จะเขียนทับ '{slot.SaveName}'\nต้องการดำเนินการต่อหรือไม่?",
                    "เขียนทับ",
                    "ยกเลิก");
                if (!confirm) return;
            }

            try
            {
                IsBusy = true;
                var userId = await SessionManager.GetUserIdAsync();
                if (!userId.HasValue) return;

                await _saveLoadService.SaveToSlotAsync(userId.Value, slot.SlotNumber);
                await ReloadSlotsAsync(userId.Value);
                await Shell.Current.DisplayAlert("บันทึกแล้ว", $"บันทึกเกมลงช่อง {slot.SlotNumber} เรียบร้อย", "ตกลง");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SaveLoadVM.Save] {ex}");
                await Shell.Current.DisplayAlert("ผิดพลาด", "บันทึกไม่สำเร็จ: " + ex.Message, "ปิด");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task LoadFromSlotAsync(SaveSlotVm? slot)
        {
            if (slot == null || slot.IsEmpty || IsBusy) return;

            var confirm = await Shell.Current.DisplayAlert(
                $"โหลดช่อง {slot.SlotNumber}?",
                $"จะโหลด '{slot.SaveName}'\nตัวละครปัจจุบันจะถูกแทนที่ด้วยข้อมูลจากช่องเซฟนี้",
                "โหลด",
                "ยกเลิก");
            if (!confirm) return;

            try
            {
                IsBusy = true;
                var userId = await SessionManager.GetUserIdAsync();
                if (!userId.HasValue) return;

                await _saveLoadService.LoadFromSlotAsync(userId.Value, slot.SlotNumber);
                await Shell.Current.DisplayAlert("โหลดสำเร็จ", "กำลังเข้าสู่หอคอย...", "ตกลง");
                await Shell.Current.GoToAsync(AppConstants.RouteStory);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SaveLoadVM.Load] {ex}");
                await Shell.Current.DisplayAlert("ผิดพลาด", "โหลดไม่สำเร็จ: " + ex.Message, "ปิด");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteSlotAsync(SaveSlotVm? slot)
        {
            if (slot == null || slot.IsEmpty || IsBusy) return;

            var confirm = await Shell.Current.DisplayAlert(
                $"ลบช่อง {slot.SlotNumber}?",
                $"จะลบ '{slot.SaveName}' ถาวร\nไม่สามารถกู้คืนได้",
                "ลบ",
                "ยกเลิก");
            if (!confirm) return;

            try
            {
                IsBusy = true;
                var userId = await SessionManager.GetUserIdAsync();
                if (!userId.HasValue) return;

                await _saveLoadService.DeleteSlotAsync(userId.Value, slot.SlotNumber);
                await ReloadSlotsAsync(userId.Value);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SaveLoadVM.Delete] {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task BackAsync() => await Shell.Current.GoToAsync("..");
    }

    /// <summary>
    /// ViewModel ย่อยสำหรับแต่ละช่องเซฟ
    /// </summary>
    public partial class SaveSlotVm : ObservableObject
    {
        public int SlotNumber { get; set; }

        [ObservableProperty] private string slotLabel = "ช่อง";
        [ObservableProperty] private bool isEmpty = true;
        [ObservableProperty] private string saveName = string.Empty;
        [ObservableProperty] private string playerName = string.Empty;
        [ObservableProperty] private string floorDisplay = string.Empty;
        [ObservableProperty] private string hpDisplay = string.Empty;
        [ObservableProperty] private string levelDisplay = string.Empty;
        [ObservableProperty] private string dateDisplay = string.Empty;

        public bool IsNotEmpty => !IsEmpty;

        partial void OnIsEmptyChanged(bool value) => OnPropertyChanged(nameof(IsNotEmpty));
    }
}
