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
    /// ViewModel ของหน้าเล่นเกม (Story page)
    /// flow:
    /// 1. OnAppearing -> LoadPlayerAsync + LoadCurrentFloorAsync
    /// 2. ผู้ใช้เลือก choice -> ApplyChoiceAsync
    /// 3. แสดง outcome dialog -> reload floor หรือ goto ending
    /// </summary>
    public partial class StoryViewModel : BaseViewModel
    {
        private readonly PlayerService _playerService;
        private readonly StoryService _storyService;
        private readonly AudioService _audioService;

        public StoryViewModel(PlayerService playerService, StoryService storyService, AudioService audioService)
        {
            _playerService = playerService;
            _storyService = storyService;
            _audioService = audioService;
            Title = "หอคอยอเวจี";
        }

        // ============== Player State (top bar) ==============
        [ObservableProperty] private Player? player;
        [ObservableProperty] private string playerName = "นักผจญภัย";
        [ObservableProperty] private int playerHp;
        [ObservableProperty] private int playerMaxHp;
        [ObservableProperty] private double hpRatio;
        [ObservableProperty] private string hpDisplay = "0/0";
        [ObservableProperty] private int gold;
        [ObservableProperty] private int level;
        [ObservableProperty] private int currentFloor;
        [ObservableProperty] private string floorDisplay = "0/30";

        // ============== Story State ==============
        [ObservableProperty] private StoryEvent? currentEvent;
        [ObservableProperty] private string floorTitle = "";
        [ObservableProperty] private string narrative = "";
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasFloorIcon))]
        private string floorIcon = "🗝️";
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasFloorImage))]
        private string floorImage = "";
        [ObservableProperty] private string eventType = "Story";

        public bool HasFloorIcon => !string.IsNullOrWhiteSpace(FloorIcon);
        public bool HasFloorImage => !string.IsNullOrWhiteSpace(FloorImage);

        // ============== Choice Feedback ==============
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasOutcomeFeedback))]
        private string outcomeTitle = "";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasOutcomeFeedback))]
        private string outcomeText = "";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasOutcomeStats))]
        private string outcomeStats = "";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasOutcomeRewards))]
        private string outcomeRewards = "";

        [ObservableProperty] private Color outcomeAccentColor = Color.FromArgb("#D4AF37");

        public bool HasOutcomeFeedback =>
            !string.IsNullOrWhiteSpace(OutcomeTitle) ||
            !string.IsNullOrWhiteSpace(OutcomeText);
        public bool HasOutcomeStats => !string.IsNullOrWhiteSpace(OutcomeStats);
        public bool HasOutcomeRewards => !string.IsNullOrWhiteSpace(OutcomeRewards);

        public ObservableCollection<ChoiceItemVm> Choices { get; } = new();

        // ============== Lifecycle ==============

        private void ShowOutcome(ChoiceOutcome outcome)
        {
            OutcomeTitle = outcome.PlayerDied
                ? "ผลลัพธ์ร้ายแรง"
                : outcome.GameCompleted
                    ? "บทสรุปกำลังเปิดเผย"
                    : "ผลจากการตัดสินใจล่าสุด";

            OutcomeText = string.IsNullOrWhiteSpace(outcome.ResultText)
                ? "การตัดสินใจของเจ้าส่งผลต่อเส้นทางในหอคอยแล้ว"
                : outcome.ResultText;

            var stats = new List<string>();
            if (outcome.HpDelta != 0) stats.Add($"HP {(outcome.HpDelta > 0 ? "+" : "")}{outcome.HpDelta}");
            if (outcome.GoldDelta != 0) stats.Add($"Gold {(outcome.GoldDelta > 0 ? "+" : "")}{outcome.GoldDelta}");
            if (outcome.ExpDelta > 0) stats.Add($"EXP +{outcome.ExpDelta}");
            if (outcome.LeveledUp) stats.Add("Level UP!");
            OutcomeStats = stats.Count > 0
                ? string.Join("  •  ", stats)
                : "";

            var rewards = new List<string>();
            if (!string.IsNullOrWhiteSpace(outcome.ItemAcquired))
                rewards.Add($"ได้รับ {outcome.ItemAcquired} x{outcome.ItemAcquiredQty}");
            if (!string.IsNullOrWhiteSpace(outcome.ItemConsumed))
                rewards.Add($"ใช้ {outcome.ItemConsumed}");
            if (outcome.StoryJournalUnlocked && outcome.UnlockedJournal != null)
                rewards.Add($"ปลดล็อก Journal: {outcome.UnlockedJournal.Title}");
            OutcomeRewards = rewards.Count > 0
                ? string.Join("\n", rewards)
                : "";

            OutcomeAccentColor = outcome.PlayerDied || outcome.HpDelta < 0
                ? Color.FromArgb("#C53030")
                : !string.IsNullOrWhiteSpace(outcome.ItemAcquired) || outcome.GoldDelta > 0 || outcome.StoryJournalUnlocked
                    ? Color.FromArgb("#D4AF37")
                    : Color.FromArgb("#9F7AEA");
        }

        /// <summary>
        /// เรียกตอน OnAppearing ของหน้า — load player + floor
        /// </summary>
        public async Task OnAppearingAsync()
        {
            try
            {
                IsBusy = true;
                ClearError();

                Player = await _playerService.GetActivePlayerAsync();
                if (Player == null)
                {
                    SetError("ไม่พบตัวละครที่เล่นอยู่ กรุณาเริ่มเกมใหม่");
                    return;
                }

                if (Player.IsGameCompleted)
                {
                    // เกมจบแล้ว ส่งไปหน้า ending ตาม EndingType ที่บันทึกไว้
                    await GoToEndingAsync(Player.EndingType);
                    return;
                }

                await LoadCurrentFloorAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[StoryVM.OnAppearing] {ex}");
                SetError("ไม่สามารถโหลดข้อมูลเกมได้");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// โหลด floor ปัจจุบันของ player
        /// </summary>
        private async Task LoadCurrentFloorAsync()
        {
            if (Player == null) return;

            // refresh top bar
            UpdateTopBar();

            CurrentEvent = await _storyService.GetCurrentFloorEventAsync(Player);
            if (CurrentEvent == null)
            {
                SetError($"ไม่พบข้อมูลชั้นที่ {Player.CurrentFloor}");
                return;
            }

            FloorTitle = CurrentEvent.Title;
            Narrative = CurrentEvent.Narrative;
            FloorIcon = CurrentEvent.Icon;
            FloorImage = CurrentEvent.ImageFile ?? "";
            EventType = CurrentEvent.EventType;

            // setup 3 choices (ตรวจ requires item)
            Choices.Clear();
            for (int i = 0; i < CurrentEvent.Choices.Count; i++)
            {
                var choice = CurrentEvent.Choices[i];
                var lockReason = await _storyService.GetChoiceLockReasonAsync(Player, choice);
                bool canSelect = lockReason == null;

                Choices.Add(new ChoiceItemVm
                {
                    Index = i,
                    Text = await _storyService.TranslateItemNamesAsync(choice.Text),
                    IsEnabled = canSelect,
                    LockReason = canSelect
                        ? null
                        : $"🔒 {lockReason}"
                });
            }
        }

        /// <summary>
        /// อัปเดตค่าใน top bar (HP/Gold/Floor)
        /// </summary>
        private void UpdateTopBar()
        {
            if (Player == null) return;

            PlayerName = Player.PlayerName;
            PlayerHp = Player.Hp;
            PlayerMaxHp = Player.MaxHp;
            HpRatio = Player.HpRatio;
            HpDisplay = $"{Player.Hp}/{Player.MaxHp}";
            Gold = Player.Gold;
            Level = Player.Level;
            CurrentFloor = Player.CurrentFloor;
            FloorDisplay = $"ชั้น {Player.CurrentFloor}/{AppConstants.TotalFloors}";
        }

        // ============== Choice Selection ==============

        /// <summary>
        /// เลือก choice (เรียกจาก ChoiceButton ใน UI)
        /// แยกเป็น 2 phase เพื่อป้องกัน UI deadlock:
        ///   Phase 1 (IsBusy=true): apply choice + reload data
        ///   Phase 2 (IsBusy=false): แสดง dialog + navigate
        /// MAUI Shell มี known issue ถ้า DisplayAlert ทำงานพร้อม overlay ที่ block input → ค้าง
        /// </summary>
        [RelayCommand]
        private async Task SelectChoiceAsync(ChoiceItemVm? choice)
        {
            if (choice == null || Player == null || CurrentEvent == null) return;
            if (!choice.IsEnabled || IsBusy) return;

            ChoiceOutcome? outcome = null;
            var selectedStoryChoice = CurrentEvent.Choices[choice.Index];

            // ===== Phase 1: apply choice ใน DB =====
            try
            {
                IsBusy = true;
                outcome = await _storyService.ApplyChoiceAsync(Player, CurrentEvent, choice.Index);

                // refresh player state จาก DB (StoryService update แล้ว)
                if (outcome.Success)
                {
                    Player = await _playerService.GetActivePlayerAsync();
                    UpdateTopBar();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[StoryVM.SelectChoice.Phase1] {ex}");
                IsBusy = false;
                // รอ frame ถัดไปก่อนแสดง alert เพื่อให้ overlay หาย
                await Task.Yield();
                await Shell.Current.DisplayAlert("เกิดข้อผิดพลาด", "ไม่สามารถดำเนินการได้: " + ex.Message, "ปิด");
                return;
            }
            finally
            {
                IsBusy = false;
            }

            if (outcome == null) return;

            // ===== Phase 2: dialog + navigation (ไม่อยู่ใน IsBusy block) =====
            try
            {
                // รอ overlay ปลด + UI redraw ก่อน DisplayAlert (สำคัญมากสำหรับ MAUI 10)
                await Task.Yield();

                if (!outcome.Success)
                {
                    await Shell.Current.DisplayAlert(
                        "เลือกไม่ได้",
                        outcome.RejectMessage ?? "ตัวเลือกนี้ใช้ไม่ได้",
                        "ปิด");
                    return;
                }

                ShowOutcome(outcome);
                await PlayOutcomeSfxAsync(outcome, selectedStoryChoice);

                if (outcome.PlayerDied)
                {
                    await GoToEndingAsync("Bad");
                    return;
                }

                if (outcome.GameCompleted)
                {
                    await GoToEndingAsync(string.IsNullOrEmpty(outcome.EndingType) ? "Good" : outcome.EndingType);
                    return;
                }

                if (outcome.ShouldAdvanceFloor || !outcome.GameCompleted)
                {
                    // reload floor — ไม่ใช้ IsBusy ตรงนี้ เพราะจะ block UI ในขณะที่ alert เพิ่ง close
                    await LoadCurrentFloorAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[StoryVM.SelectChoice.Phase2] {ex}");
            }
        }

        private async Task PlayOutcomeSfxAsync(ChoiceOutcome outcome, StoryChoice choice)
        {
            var choiceText = choice.Text ?? string.Empty;
            var resultText = choice.ResultText ?? string.Empty;
            var storyText = $"{choiceText} {resultText}";

            if (!string.IsNullOrWhiteSpace(outcome.ItemConsumed))
            {
                if (storyText.Contains("ดื่ม") && storyText.Contains("Potion"))
                {
                    await _audioService.PlaySfxAsync(AudioService.ItemHealSfx);
                    return;
                }

                if (outcome.ItemConsumed.Contains("Key", StringComparison.OrdinalIgnoreCase))
                {
                    await _audioService.PlaySfxAsync(AudioService.ItemKeySfx);
                    return;
                }

                if (storyText.Contains("ยื่น") || storyText.Contains("ให้กับ") || storyText.Contains("มอบ"))
                {
                    // This is giving an item to someone else, so let reward sounds below carry the moment.
                }
                else if (string.IsNullOrWhiteSpace(outcome.ItemAcquired) && outcome.GoldDelta <= 0)
                {
                    await _audioService.PlaySfxAsync(AudioService.ItemUseSfx);
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(outcome.ItemAcquired))
            {
                await _audioService.PlaySfxAsync(AudioService.ItemGetSfx);
                return;
            }

            if (outcome.GoldDelta > 0)
            {
                await _audioService.PlaySfxAsync(AudioService.ItemGoldSfx);
                return;
            }
        }

        /// <summary>
        /// แสดง dialog ผลลัพธ์ของ choice (เน้นข้อความสั้น+ stat changes)
        /// ใช้ MainThread เพื่อให้ DisplayAlert ทำงานบน UI thread แน่นอน
        /// </summary>
        private static Task ShowOutcomeAsync(ChoiceOutcome outcome)
        {
            var lines = new List<string>();
            if (!string.IsNullOrWhiteSpace(outcome.ResultText))
                lines.Add(outcome.ResultText);

            var changes = new List<string>();
            if (outcome.HpDelta != 0) changes.Add($"HP {(outcome.HpDelta > 0 ? "+" : "")}{outcome.HpDelta}");
            if (outcome.GoldDelta != 0) changes.Add($"💰 {(outcome.GoldDelta > 0 ? "+" : "")}{outcome.GoldDelta}");
            if (outcome.ExpDelta > 0) changes.Add($"✨ EXP +{outcome.ExpDelta}");
            if (changes.Count > 0)
                lines.Add(string.Join("  •  ", changes));

            if (!string.IsNullOrWhiteSpace(outcome.ItemAcquired))
                lines.Add($"🎁 ได้รับ: {outcome.ItemAcquired} x{outcome.ItemAcquiredQty}");
            if (!string.IsNullOrWhiteSpace(outcome.ItemConsumed))
                lines.Add($"🔥 ใช้: {outcome.ItemConsumed}");
            if (outcome.LeveledUp)
                lines.Add("⬆️ Level UP!");
            if (outcome.StoryJournalUnlocked && outcome.UnlockedJournal != null)
                lines.Add($"📖 ปลดล็อก Journal: {outcome.UnlockedJournal.Title}");

            var message = lines.Count > 0 ? string.Join("\n\n", lines) : "ดำเนินการสำเร็จ";

            return MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Shell.Current.DisplayAlert("ผลลัพธ์", message, "ต่อไป");
            });
        }

        // ============== Navigation ==============

        private static async Task GoToEndingAsync(string endingType)
        {
            // ใช้ query string ส่ง type ไป EndingViewModel
            var safeType = string.IsNullOrEmpty(endingType) ? "Good" : endingType;
            await Shell.Current.GoToAsync($"{AppConstants.RouteEnding}?type={safeType}");
        }

        [RelayCommand]
        private async Task GoToCharacterAsync()
        {
            await Shell.Current.GoToAsync(AppConstants.RouteCharacter);
        }

        [RelayCommand]
        private async Task GoToInventoryAsync()
        {
            await Shell.Current.GoToAsync(AppConstants.RouteInventory);
        }

        [RelayCommand]
        private async Task GoToJournalAsync()
        {
            await Shell.Current.GoToAsync(AppConstants.RouteJournal);
        }

        [RelayCommand]
        private async Task BackToMenuAsync()
        {
            var confirm = await Shell.Current.DisplayAlert(
                "ออกจากเกม?",
                "ต้องการกลับเมนูหลักหรือไม่? (ความคืบหน้าจะถูกบันทึกอัตโนมัติ)",
                "ออก", "เล่นต่อ");
            if (!confirm) return;

            await Shell.Current.GoToAsync(AppConstants.RouteMainMenu);
        }

    }

    /// <summary>
    /// ViewModel ย่อยสำหรับแต่ละ choice ใน UI
    /// </summary>
    public partial class ChoiceItemVm : ObservableObject
    {
        public int Index { get; set; }

        [ObservableProperty] private string text = "";
        [ObservableProperty] private bool isEnabled = true;
        [ObservableProperty] private string? lockReason;

        public bool ShowLockReason => !IsEnabled && !string.IsNullOrEmpty(LockReason);

        partial void OnIsEnabledChanged(bool value) => OnPropertyChanged(nameof(ShowLockReason));
        partial void OnLockReasonChanged(string? value) => OnPropertyChanged(nameof(ShowLockReason));
    }
}
