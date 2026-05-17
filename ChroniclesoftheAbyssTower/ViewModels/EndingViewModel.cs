using ChroniclesoftheAbyssTower.Helpers;
using ChroniclesoftheAbyssTower.Models;
using ChroniclesoftheAbyssTower.Services;
using ChroniclesoftheAbyssTower.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChroniclesoftheAbyssTower.ViewModels
{
    /// <summary>
    /// ViewModel ของหน้า Ending (จบเกม)
    /// รับ ?type=Good|Bad|TrueGood ผ่าน QueryProperty
    /// </summary>
    [QueryProperty(nameof(EndingType), "type")]
    public partial class EndingViewModel : BaseViewModel
    {
        private readonly PlayerService _playerService;

        public EndingViewModel(PlayerService playerService)
        {
            _playerService = playerService;
            Title = "บทสรุป";
        }

        // ============== Query parameter ==============
        private string endingType = "Good";
        public string EndingType
        {
            get => endingType;
            set
            {
                if (SetProperty(ref endingType, value))
                {
                    LoadEndingContent();
                }
            }
        }

        // ============== Display Properties ==============
        [ObservableProperty] private string endingTitle = "";
        [ObservableProperty] private string endingSubtitle = "";
        [ObservableProperty] private string endingNarrative = "";
        [ObservableProperty] private string endingImage = "";
        [ObservableProperty] private Color endingColor = Colors.White;

        // Stats สรุปเกม
        [ObservableProperty] private string playerNameDisplay = "";
        [ObservableProperty] private string finalLevelDisplay = "0";
        [ObservableProperty] private string highestFloorDisplay = "0";
        [ObservableProperty] private string totalChoicesDisplay = "0";
        [ObservableProperty] private string finalGoldDisplay = "0";

        // ============== Lifecycle ==============

        public async Task OnAppearingAsync()
        {
            try
            {
                IsBusy = true;

                var player = await _playerService.GetActivePlayerAsync();
                if (player != null)
                {
                    PlayerNameDisplay = player.PlayerName;
                    FinalLevelDisplay = player.Level.ToString();
                    HighestFloorDisplay = player.HighestFloor.ToString();
                    TotalChoicesDisplay = player.TotalChoicesMade.ToString();
                    FinalGoldDisplay = player.Gold.ToString();
                }

                LoadEndingContent();
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// โหลดข้อความบทสรุปตาม EndingType
        /// (เนื้อหาจาก STORY.md)
        /// </summary>
        private void LoadEndingContent()
        {
            switch (EndingType)
            {
                case "Bad":
                    EndingImage = "ending_bad.jpg";
                    EndingTitle = "บทสรุปแห่งความมืด";
                    EndingSubtitle = "ฉากจบแห่งความมืด";
                    EndingColor = Color.FromArgb("#C53030"); // AbyssBlood
                    EndingNarrative =
                        "อารินไม่สามารถผ่านพ้นการทดสอบของหอคอยได้\n\n" +
                        "เขาสิ้นใจในความมืดมิด\n" +
                        "ไม่มีใครรู้ว่าเกิดอะไรขึ้นกับเอลีน่า\n\n" +
                        "หอคอยอเวจียังคงตั้งตระหง่าน\n" +
                        "หมอกอเวจียังคงคืบคลานไปยังเมืองอื่นๆ\n\n" +
                        "ความจริงถูกฝังกลบใต้หินสีดำ...\n" +
                        "และตำนานยังคงรอผู้กล้าคนต่อไป";
                    break;

                case "TrueGood":
                    EndingImage = "ending_true_good.jpg";
                    EndingTitle = "บทสรุปที่แท้จริง";
                    EndingSubtitle = "ฉากจบที่แท้จริง";
                    EndingColor = Color.FromArgb("#D4AF37"); // AbyssGold
                    EndingNarrative =
                        "อารินค้นพบความจริงทั้งหมด\n\n" +
                        "หมอกอเวจีไม่ใช่ความชั่วร้าย\n" +
                        "แต่เป็น \"ความเศร้าของผู้พิทักษ์เก่า\"\n" +
                        "ที่ตายอย่างไม่เป็นธรรม\n\n" +
                        "อารินยอมรับบทบาทผู้พิทักษ์ใหม่\n" +
                        "ใช้พลังของตัวเองเพื่อเปลี่ยนหมอกเป็นแสง\n\n" +
                        "เอลีน่ากลับมาอย่างปลอดภัย\n" +
                        "หอคอยกลายเป็นสัญลักษณ์แห่งความหวัง\n\n" +
                        "และตำนานของอัศวินสาปแช่ง\n" +
                        "ก็จบลงด้วยแสงแห่งคำสาบานใหม่...";
                    break;

                case "Good":
                default:
                    EndingImage = "ending_good.jpg";
                    EndingTitle = "บทสรุปแห่งวีรบุรุษ";
                    EndingSubtitle = "ฉากจบแห่งวีรบุรุษ";
                    EndingColor = Color.FromArgb("#9F7AEA"); // AbyssPurpleLight
                    EndingNarrative =
                        "อารินขึ้นถึงยอดหอคอยอเวจี\n\n" +
                        "เขาเอาชนะการทดสอบทั้งสามสิบชั้น\n" +
                        "และทำลายแหล่งกำเนิดของหมอก\n\n" +
                        "หมอกอเวจีเริ่มจางหายไป\n" +
                        "เมืองต่างๆ กลับมามีชีวิตอีกครั้ง\n\n" +
                        "เอลีน่ากลับมาแล้ว\n" +
                        "แม้จะแลกด้วยบาดแผลและความทรงจำบางส่วน\n\n" +
                        "ตำนานของอัศวินสาปแช่ง\n" +
                        "ก็กลายเป็นตำนานของวีรบุรุษ\n" +
                        "ที่ไม่ยอมแพ้แม้โลกจะหันหลังให้";
                    break;
            }
        }

        // ============== Commands ==============

        /// <summary>
        /// เริ่มเกมใหม่ (ลบ player ปัจจุบัน + ไปเริ่มใหม่)
        /// </summary>
        [RelayCommand]
        private async Task NewGameAsync()
        {
            var confirm = await Shell.Current.DisplayAlert(
                "เริ่มเกมใหม่?",
                "ตัวละครปัจจุบันจะถูกลบและเริ่มเกมใหม่ทั้งหมด ต้องการดำเนินการต่อหรือไม่?",
                "เริ่มใหม่", "ยกเลิก");
            if (!confirm) return;

            try
            {
                IsBusy = true;
                var player = await _playerService.GetActivePlayerAsync();
                if (player != null)
                {
                    await _playerService.DeleteWithRelatedDataAsync(player);
                    SessionManager.ClearActivePlayer();
                }
                await Shell.Current.GoToAsync(AppConstants.RouteMainMenu);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// กลับเมนูหลัก (ไม่ลบ save)
        /// </summary>
        [RelayCommand]
        private async Task BackToMenuAsync()
        {
            await Shell.Current.GoToAsync(AppConstants.RouteMainMenu);
        }
    }
}
