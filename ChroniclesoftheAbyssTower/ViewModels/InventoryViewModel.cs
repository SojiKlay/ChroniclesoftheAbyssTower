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
    /// ViewModel ของหน้า Inventory (Phase 5)
    /// - แสดงรายการ Item ที่ผู้เล่นมี
    /// - กรองตามประเภท
    /// - ค้นหาด้วย keyword
    /// - ใช้ item (consume) สำหรับ Healing items
    /// - ดูรายละเอียด item (DisplayAlert)
    /// </summary>
    public partial class InventoryViewModel : BaseViewModel
    {
        private readonly InventoryService _inventoryService;
        private readonly PlayerService _playerService;
        private readonly AudioService _audioService;

        // เก็บข้อมูลดิบทั้งหมดเอาไว้ทำ filter ใน memory (ลด query)
        private List<InventoryItem> _allItems = new();

        public InventoryViewModel(InventoryService inventoryService, PlayerService playerService, AudioService audioService)
        {
            _inventoryService = inventoryService;
            _playerService = playerService;
            _audioService = audioService;
            Title = "กระเป๋าของฉัน";
        }

        // ============== State ==============

        [ObservableProperty] private Player? player;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasNoItems))]
        private ObservableCollection<InventoryItem> items = new();

        public bool HasNoItems => Items.Count == 0 && !IsBusy;

        [ObservableProperty] private string searchText = string.Empty;
        [ObservableProperty] private string activeFilter = "All"; // All / Healing / Story / Key / Consumable
        [ObservableProperty] private string playerName = string.Empty;
        [ObservableProperty] private int gold;
        [ObservableProperty] private string itemCountDisplay = "0 / 30";

        // ============== Lifecycle ==============

        /// <summary>
        /// โหลดข้อมูล inventory + player ทุกครั้งที่หน้าเปิด
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
                    SetError("ไม่พบตัวละครที่เล่นอยู่");
                    return;
                }

                PlayerName = Player.PlayerName;
                Gold = Player.Gold;

                _allItems = await _inventoryService.GetByPlayerAsync(Player.PlayerId);
                ApplyFilterAndSearch();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[InventoryVM.OnAppearing] {ex}");
                SetError("ไม่สามารถโหลด inventory ได้");
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(HasNoItems));
            }
        }

        // ============== Filter / Search ==============

        partial void OnSearchTextChanged(string value) => ApplyFilterAndSearch();
        partial void OnActiveFilterChanged(string value) => ApplyFilterAndSearch();

        /// <summary>
        /// กรองและค้นหาจาก _allItems → ใส่ใน Items collection
        /// </summary>
        private void ApplyFilterAndSearch()
        {
            IEnumerable<InventoryItem> result = _allItems;

            // กรองตามประเภท
            if (ActiveFilter != "All" && Enum.TryParse<ItemType>(ActiveFilter, out var type))
            {
                result = result.Where(i => i.DisplayType == type);
            }

            // ค้นหาด้วย keyword
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var keyword = SearchText.Trim().ToLowerInvariant();
                result = result.Where(i => i.DisplayName.ToLowerInvariant().Contains(keyword)
                                        || i.DisplayDescription.ToLowerInvariant().Contains(keyword));
            }

            var list = result.ToList();
            Items = new ObservableCollection<InventoryItem>(list);
            ItemCountDisplay = $"{list.Count} / {AppConstants.MaxInventorySize}";
            OnPropertyChanged(nameof(HasNoItems));
        }

        // ============== Commands ==============

        /// <summary>
        /// แตะ chip กรองประเภท
        /// </summary>
        [RelayCommand]
        private void SetFilter(string? filter)
        {
            if (!string.IsNullOrEmpty(filter)) ActiveFilter = filter;
        }

        /// <summary>
        /// แตะ item → แสดงรายละเอียด พร้อมปุ่ม "ใช้" สำหรับ Healing item
        /// </summary>
        [RelayCommand]
        private async Task ShowItemDetailAsync(InventoryItem? item)
        {
            if (item == null || Player == null || item.ItemData == null) return;

            // สร้างข้อความ detail
            var lines = new List<string>
            {
                item.DisplayDescription,
                "",
                $"ประเภท: {item.DisplayType}",
                $"จำนวน: {item.Quantity}",
            };
            if (item.ItemData.EffectValue > 0)
                lines.Add($"ค่า Effect: {item.ItemData.EffectValue}");
            if (item.ItemData.ShopPrice > 0)
                lines.Add($"ราคา: {item.ItemData.ShopPrice} 💰");

            var detail = string.Join("\n", lines);

            // ตรวจว่าเป็น Healing item ที่ใช้ได้หรือไม่
            bool canUse = item.DisplayType == ItemType.Healing
                          && item.ItemData.IsConsumable
                          && item.Quantity > 0;

            if (canUse)
            {
                var useConfirm = await Shell.Current.DisplayAlert(
                    $"{item.DisplayIcon}  {item.DisplayName}",
                    detail,
                    "ใช้", "ปิด");
                if (useConfirm)
                {
                    await UseItemAsync(item);
                }
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    $"{item.DisplayIcon}  {item.DisplayName}",
                    detail,
                    "ปิด");
            }
        }

        /// <summary>
        /// ใช้ item: ลด quantity และ apply effect (Healing → ฟื้น HP)
        /// </summary>
        private async Task UseItemAsync(InventoryItem item)
        {
            if (Player == null || item.ItemData == null) return;

            try
            {
                IsBusy = true;

                // Apply effect ตามประเภท
                int hpDelta = 0;
                if (item.DisplayType == ItemType.Healing)
                {
                    hpDelta = item.ItemData.EffectValue;
                    Player = await _playerService.ApplyHpDeltaAsync(Player, hpDelta);
                }

                // ลด quantity
                await _audioService.PlayItemUseAsync(item.DisplayType);
                await _inventoryService.UpdateQuantityAsync(item, item.Quantity - 1);

                // refresh list
                _allItems = await _inventoryService.GetByPlayerAsync(Player.PlayerId);
                ApplyFilterAndSearch();

                // แสดงผล
                var msg = hpDelta > 0
                    ? $"ใช้ {item.DisplayName} - ฟื้น HP +{hpDelta}\nHP ปัจจุบัน: {Player.Hp}/{Player.MaxHp}"
                    : $"ใช้ {item.DisplayName} เรียบร้อย";
                await Shell.Current.DisplayAlert("✨ ใช้ Item สำเร็จ", msg, "ดี");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[InventoryVM.UseItem] {ex}");
                await Shell.Current.DisplayAlert("ผิดพลาด", "ไม่สามารถใช้ item ได้: " + ex.Message, "ปิด");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// ลบ item ออกจาก inventory (ทิ้ง)
        /// </summary>
        [RelayCommand]
        private async Task DiscardItemAsync(InventoryItem? item)
        {
            if (item == null || Player == null) return;

            var confirm = await Shell.Current.DisplayAlert(
                "ทิ้ง Item?",
                $"ต้องการทิ้ง '{item.DisplayName}' ({item.Quantity} ชิ้น) หรือไม่?\nไม่สามารถกู้คืนได้",
                "ทิ้ง", "ยกเลิก");
            if (!confirm) return;

            try
            {
                await _inventoryService.DeleteAsync(item);
                await _audioService.PlaySfxAsync(AudioService.ItemDiscardSfx);
                _allItems = await _inventoryService.GetByPlayerAsync(Player.PlayerId);
                ApplyFilterAndSearch();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[InventoryVM.Discard] {ex}");
            }
        }

        [RelayCommand]
        private async Task BackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
