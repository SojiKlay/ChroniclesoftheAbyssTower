using ChroniclesoftheAbyssTower.Helpers;
using ChroniclesoftheAbyssTower.Models;
using ChroniclesoftheAbyssTower.Services;
using ChroniclesoftheAbyssTower.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChroniclesoftheAbyssTower.ViewModels
{
    /// <summary>
    /// ViewModel สำหรับหน้า Character (แสดงสถานะตัวละคร)
    /// </summary>
    public partial class CharacterViewModel : BaseViewModel
    {
        private readonly PlayerService _playerService;

        // Player ปัจจุบัน
        [ObservableProperty]
        private Player? player;

        // คำนวณค่าที่แสดงผล (HP ratio, % สำหรับ progress bar, ฯลฯ)
        [ObservableProperty]
        private double hpRatio;

        [ObservableProperty]
        private string hpDisplay = "0/0";

        [ObservableProperty]
        private string floorDisplay = "0/30";

        [ObservableProperty]
        private double floorProgress;

        [ObservableProperty]
        private string expDisplay = "0/0";

        [ObservableProperty]
        private double expProgress;

        public CharacterViewModel(PlayerService playerService)
        {
            _playerService = playerService;
            Title = "ตัวละคร";
        }

        /// <summary>
        /// โหลดข้อมูล player ทุกครั้งที่หน้าปรากฏ
        /// </summary>
        public async Task OnAppearingAsync()
        {
            try
            {
                IsBusy = true;
                Player = await _playerService.GetActivePlayerAsync();

                if (Player == null)
                {
                    SetError("ไม่พบข้อมูลตัวละคร กรุณาเริ่มเกมใหม่");
                    return;
                }

                // ============== คำนวณค่าแสดงผล ==============
                HpRatio = Player.HpRatio;
                HpDisplay = $"{Player.Hp}/{Player.MaxHp}";

                FloorDisplay = $"{Player.CurrentFloor}/{AppConstants.TotalFloors}";
                FloorProgress = (double)Player.CurrentFloor / AppConstants.TotalFloors;

                int requiredExp = Player.Level * 100;
                ExpDisplay = $"{Player.Experience}/{requiredExp}";
                ExpProgress = requiredExp > 0 ? (double)Player.Experience / requiredExp : 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CharacterVM.OnAppearing] {ex}");
                SetError("ไม่สามารถโหลดข้อมูลตัวละครได้");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // ============== Commands ==============

        /// <summary>
        /// ไปหน้า Story (เริ่ม / เล่นต่อ)
        /// </summary>
        [RelayCommand]
        private async Task EnterTowerAsync()
        {
            if (Player == null) return;
            await Shell.Current.GoToAsync(AppConstants.RouteStory);
        }

        /// <summary>
        /// ไปหน้า Inventory
        /// </summary>
        [RelayCommand]
        private async Task GoToInventoryAsync()
        {
            await Shell.Current.GoToAsync(AppConstants.RouteInventory);
        }

        /// <summary>
        /// ไปหน้า Journal
        /// </summary>
        [RelayCommand]
        private async Task GoToJournalAsync()
        {
            await Shell.Current.GoToAsync(AppConstants.RouteJournal);
        }

        /// <summary>
        /// กลับเมนูหลัก
        /// </summary>
        [RelayCommand]
        private async Task BackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

    }
}
