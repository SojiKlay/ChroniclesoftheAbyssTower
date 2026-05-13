using ChroniclesoftheAbyssTower.Helpers;
using ChroniclesoftheAbyssTower.Models;
using ChroniclesoftheAbyssTower.Services;
using ChroniclesoftheAbyssTower.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChroniclesoftheAbyssTower.ViewModels
{
    /// <summary>
    /// ViewModel ของหน้า Journal Editor (Phase 5)
    /// รับ:
    ///   ?id=N  -> โหมดแก้ไข journal id = N
    ///   ?floor=N -> โหมดสร้างใหม่ที่ FloorNumber = N
    /// </summary>
    [QueryProperty(nameof(EditingId), "id")]
    [QueryProperty(nameof(InitialFloor), "floor")]
    public partial class JournalEditorViewModel : BaseViewModel
    {
        private readonly JournalService _journalService;
        private readonly PlayerService _playerService;

        private Journal? _existing;

        public JournalEditorViewModel(JournalService journalService, PlayerService playerService)
        {
            _journalService = journalService;
            _playerService = playerService;
            Title = "บันทึกของฉัน";
        }

        // ============== Query Params ==============

        private int editingId;
        public int EditingId
        {
            get => editingId;
            set
            {
                if (SetProperty(ref editingId, value) && value > 0)
                {
                    IsEditMode = true;
                }
            }
        }

        private int initialFloor;
        public int InitialFloor
        {
            get => initialFloor;
            set
            {
                if (SetProperty(ref initialFloor, value) && value > 0 && !IsEditMode)
                {
                    FloorNumber = value;
                }
            }
        }

        // ============== Form State ==============

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HeaderText))]
        [NotifyPropertyChangedFor(nameof(SaveButtonText))]
        [NotifyPropertyChangedFor(nameof(CanDelete))]
        private bool isEditMode;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsTitleInvalid))]
        [NotifyPropertyChangedFor(nameof(TitleCharCount))]
        private string entryTitle = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsContentInvalid))]
        [NotifyPropertyChangedFor(nameof(ContentCharCount))]
        private string entryContent = string.Empty;

        [ObservableProperty] private int floorNumber = 1;

        // Validation flags
        public bool IsTitleInvalid => string.IsNullOrWhiteSpace(EntryTitle);
        public bool IsContentInvalid => string.IsNullOrWhiteSpace(EntryContent);

        // Character count display
        public string TitleCharCount => $"{EntryTitle?.Length ?? 0}/50";
        public string ContentCharCount => $"{EntryContent?.Length ?? 0}/500";

        // Header / Button labels
        public string HeaderText => IsEditMode ? "แก้ไขบันทึก" : "เขียนบันทึกใหม่";
        public string SaveButtonText => IsEditMode ? "บันทึกการแก้ไข" : "เพิ่มบันทึก";
        public bool CanDelete => IsEditMode;

        // ============== Lifecycle ==============

        public async Task OnAppearingAsync()
        {
            try
            {
                IsBusy = true;
                ClearError();

                if (IsEditMode && EditingId > 0)
                {
                    _existing = await _journalService.GetByIdAsync(EditingId);
                    if (_existing == null)
                    {
                        SetError("ไม่พบบันทึกที่ต้องการแก้ไข");
                        return;
                    }
                    if (_existing.JournalType == JournalType.Story)
                    {
                        SetError("บันทึกเนื้อเรื่องไม่สามารถแก้ไขได้");
                        return;
                    }

                    EntryTitle = _existing.Title;
                    EntryContent = _existing.Content;
                    FloorNumber = _existing.FloorNumber;
                }
                else
                {
                    // โหมดสร้างใหม่ — ดึง current floor จาก player
                    if (FloorNumber <= 0)
                    {
                        var player = await _playerService.GetActivePlayerAsync();
                        FloorNumber = player?.CurrentFloor ?? 1;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[JournalEditorVM.OnAppearing] {ex}");
                SetError("ไม่สามารถโหลดข้อมูลได้");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // ============== Commands ==============

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (IsBusy) return;
            if (IsTitleInvalid)
            {
                SetError("กรุณาใส่ชื่อบันทึก");
                return;
            }
            if (IsContentInvalid)
            {
                SetError("กรุณาเขียนเนื้อหา");
                return;
            }

            try
            {
                IsBusy = true;
                ClearError();

                if (IsEditMode && _existing != null)
                {
                    await _journalService.UpdatePlayerJournalAsync(_existing, EntryTitle, EntryContent);
                }
                else
                {
                    var player = await _playerService.GetActivePlayerAsync();
                    if (player == null)
                    {
                        SetError("ไม่พบตัวละครที่เล่นอยู่");
                        return;
                    }
                    var floor = FloorNumber > 0 ? FloorNumber : player.CurrentFloor;
                    await _journalService.CreatePlayerJournalAsync(player.PlayerId, floor, EntryTitle, EntryContent);
                }

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[JournalEditorVM.Save] {ex}");
                SetError("บันทึกไม่สำเร็จ: " + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            if (!IsEditMode || _existing == null) return;

            var confirm = await Shell.Current.DisplayAlert(
                "ลบบันทึก?",
                "ต้องการลบบันทึกนี้หรือไม่? ไม่สามารถกู้คืนได้",
                "ลบ", "ยกเลิก");
            if (!confirm) return;

            try
            {
                IsBusy = true;
                await _journalService.DeletePlayerJournalAsync(_existing);
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[JournalEditorVM.Delete] {ex}");
                SetError("ลบไม่สำเร็จ: " + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        // ============== Auto-update character counts ==============

        partial void OnEntryTitleChanged(string value) => OnPropertyChanged(nameof(TitleCharCount));
        partial void OnEntryContentChanged(string value) => OnPropertyChanged(nameof(ContentCharCount));
    }
}
