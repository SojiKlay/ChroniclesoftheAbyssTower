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
    /// ViewModel ของหน้า Journal (Phase 5)
    /// แบ่ง 2 tab:
    ///   - Story: ปลดล็อกอัตโนมัติ read-only
    ///   - Player: เขียนเอง + CRUD ครบ
    /// </summary>
    public partial class JournalViewModel : BaseViewModel
    {
        private readonly JournalService _journalService;
        private readonly PlayerService _playerService;

        private List<Journal> _allJournals = new();

        public JournalViewModel(JournalService journalService, PlayerService playerService)
        {
            _journalService = journalService;
            _playerService = playerService;
            Title = "บันทึก";
        }

        // ============== State ==============

        [ObservableProperty] private Player? player;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasNoJournals))]
        private ObservableCollection<Journal> journals = new();

        public bool HasNoJournals => Journals.Count == 0 && !IsBusy;

        [ObservableProperty] private string searchText = string.Empty;

        // tab ที่เลือก: "Story" หรือ "Player"
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsStoryTab))]
        [NotifyPropertyChangedFor(nameof(IsPlayerTab))]
        [NotifyPropertyChangedFor(nameof(EmptyMessage))]
        [NotifyPropertyChangedFor(nameof(CanAddJournal))]
        private string activeTab = "Story";

        public bool IsStoryTab => ActiveTab == "Story";
        public bool IsPlayerTab => ActiveTab == "Player";

        // ข้อความ empty state ขึ้นกับ tab ปัจจุบัน
        public string EmptyMessage => IsStoryTab
            ? "ยังไม่ปลดล็อกบันทึกใดๆ\nผจญภัยในหอคอยเพื่อปลดล็อก"
            : "ยังไม่มีบันทึกของเจ้า\nกดปุ่ม + เพื่อเพิ่มบันทึกใหม่";

        public bool CanAddJournal => IsPlayerTab;

        [ObservableProperty] private int storyCount;
        [ObservableProperty] private int playerCount;

        // ============== Lifecycle ==============

        public async Task OnAppearingAsync()
        {
            try
            {
                IsBusy = true;
                ClearError();

                Player = await _playerService.GetActivePlayerAsync();
                if (Player == null)
                {
                    SetError("ไม่พบตัวละครที่เล่นอยู่");
                    return;
                }

                await ReloadAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[JournalVM.OnAppearing] {ex}");
                SetError("ไม่สามารถโหลดบันทึกได้");
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(HasNoJournals));
            }
        }

        private async Task ReloadAsync()
        {
            if (Player == null) return;
            _allJournals = await _journalService.GetByPlayerAsync(Player.PlayerId);
            StoryCount = _allJournals.Count(j => j.JournalType == JournalType.Story);
            PlayerCount = _allJournals.Count(j => j.JournalType == JournalType.Player);
            ApplyFilterAndSearch();
        }

        // ============== Filter / Search ==============

        partial void OnSearchTextChanged(string value) => ApplyFilterAndSearch();
        partial void OnActiveTabChanged(string value) => ApplyFilterAndSearch();

        private void ApplyFilterAndSearch()
        {
            var typeFilter = IsStoryTab ? JournalType.Story : JournalType.Player;
            IEnumerable<Journal> result = _allJournals.Where(j => j.JournalType == typeFilter);

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var keyword = SearchText.Trim().ToLowerInvariant();
                result = result.Where(j => j.Title.ToLowerInvariant().Contains(keyword)
                                        || j.Content.ToLowerInvariant().Contains(keyword));
            }

            Journals = new ObservableCollection<Journal>(result.OrderBy(j => j.FloorNumber).ThenByDescending(j => j.UpdatedAt));
            OnPropertyChanged(nameof(HasNoJournals));
        }

        // ============== Commands ==============

        [RelayCommand]
        private void SwitchTab(string? tab)
        {
            if (!string.IsNullOrEmpty(tab)) ActiveTab = tab;
        }

        /// <summary>
        /// แตะ journal → เปิด detail (สำหรับ Story = read-only, Player = แก้ไขได้)
        /// ใช้ MainThread เพื่อให้ DisplayAlert/Navigate ทำงานบน UI thread
        /// </summary>
        [RelayCommand]
        private async Task OpenJournalAsync(Journal? journal)
        {
            if (journal == null) return;

            try
            {
                if (journal.JournalType == JournalType.Story)
                {
                    // Story: read-only - แสดงเนื้อหาผ่าน DisplayAlert
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await Shell.Current.DisplayAlert(
                            $"📖 {journal.Title}  (ชั้น {journal.FloorNumber})",
                            journal.Content,
                            "ปิด");
                    });
                }
                else
                {
                    // Player: ไปหน้า editor
                    await Shell.Current.GoToAsync($"{AppConstants.RouteJournalEditor}?id={journal.JournalId}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[JournalVM.OpenJournal] {ex}");
            }
        }

        /// <summary>
        /// เพิ่มบันทึกใหม่ (Player)
        /// </summary>
        [RelayCommand]
        private async Task AddJournalAsync()
        {
            if (!CanAddJournal || Player == null) return;
            // ไปหน้า editor แบบไม่ส่ง id → จะเป็นโหมดสร้างใหม่
            await Shell.Current.GoToAsync($"{AppConstants.RouteJournalEditor}?floor={Player.CurrentFloor}");
        }

        /// <summary>
        /// ลบบันทึก (Player journal เท่านั้น)
        /// </summary>
        [RelayCommand]
        private async Task DeleteJournalAsync(Journal? journal)
        {
            if (journal == null) return;
            if (journal.JournalType == JournalType.Story)
            {
                await Shell.Current.DisplayAlert("ลบไม่ได้", "บันทึกเนื้อเรื่องไม่สามารถลบได้", "ปิด");
                return;
            }

            var confirm = await Shell.Current.DisplayAlert(
                "ลบบันทึก?",
                $"ต้องการลบบันทึก '{journal.Title}' หรือไม่?\nไม่สามารถกู้คืนได้",
                "ลบ", "ยกเลิก");
            if (!confirm) return;

            try
            {
                await _journalService.DeletePlayerJournalAsync(journal);
                await ReloadAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[JournalVM.Delete] {ex}");
                await Shell.Current.DisplayAlert("ผิดพลาด", ex.Message, "ปิด");
            }
        }

        [RelayCommand]
        private async Task BackAsync() => await Shell.Current.GoToAsync("..");
    }
}
