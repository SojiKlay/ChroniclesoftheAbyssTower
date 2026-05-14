using ChroniclesoftheAbyssTower.Models;
using ChroniclesoftheAbyssTower.Helpers;

namespace ChroniclesoftheAbyssTower.Services
{
    /// <summary>
    /// Service จัดการ Inventory ของผู้เล่น - CRUD + business logic
    /// </summary>
    public class InventoryService
    {
        private static readonly HashSet<string> UniqueStoryItems = new(StringComparer.OrdinalIgnoreCase)
        {
            "Broken Pendant",
            "Tower Map",
            "Boss Key",
            "Mist Crystal",
            "Knight's Sword",
            "Iron Shield"
        };

        private readonly DatabaseService _databaseService;

        public InventoryService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // ============== Read ==============

        /// <summary>
        /// ดึง Inventory ของ player พร้อม join ข้อมูล Item
        /// (sqlite-net-pcl ไม่ทำ join อัตโนมัติ - ต้องโหลด Items แยกแล้วผูกเอง)
        /// </summary>
        public async Task<List<InventoryItem>> GetByPlayerAsync(int playerId)
        {
            var conn = await _databaseService.GetConnectionAsync();

            var inventory = await conn.Table<InventoryItem>()
                .Where(i => i.PlayerId == playerId && i.Quantity > 0)
                .OrderByDescending(i => i.AcquiredAt)
                .ToListAsync();

            // โหลด Items ทั้งหมดที่ใช้ในครั้งเดียว แล้ว map กลับ (ลด query)
            var itemIds = inventory.Select(i => i.ItemId).Distinct().ToList();
            var items = await conn.Table<Item>()
                .Where(i => itemIds.Contains(i.ItemId))
                .ToListAsync();
            var itemMap = items.ToDictionary(i => i.ItemId);

            foreach (var inv in inventory)
            {
                if (itemMap.TryGetValue(inv.ItemId, out var data))
                    inv.ItemData = data;
            }

            return inventory;
        }

        /// <summary>
        /// ค้นหา item ที่ผู้เล่นมีตามชื่อ (ไม่สนตัวพิมพ์)
        /// </summary>
        public async Task<InventoryItem?> FindByNameAsync(int playerId, string itemName)
        {
            var conn = await _databaseService.GetConnectionAsync();

            // หา Item master ก่อน
            var item = await conn.Table<Item>()
                .Where(i => i.ItemName == itemName)
                .FirstOrDefaultAsync();
            if (item == null) return null;

            // หา InventoryItem
            var inv = await conn.Table<InventoryItem>()
                .Where(i => i.PlayerId == playerId && i.ItemId == item.ItemId && i.Quantity > 0)
                .FirstOrDefaultAsync();

            if (inv != null) inv.ItemData = item;
            return inv;
        }

        /// <summary>
        /// ตรวจว่าผู้เล่นมี item ชื่อนี้หรือไม่
        /// </summary>
        public async Task<bool> HasItemAsync(int playerId, string itemName)
        {
            var item = await FindByNameAsync(playerId, itemName);
            return item != null && item.Quantity > 0;
        }

        public static bool IsUniqueStoryItem(string itemName)
        {
            return UniqueStoryItems.Contains(itemName);
        }

        // ============== Create ==============

        /// <summary>
        /// เพิ่ม item เข้า inventory (ถ้ามีอยู่แล้ว เพิ่ม quantity)
        /// </summary>
        public async Task<InventoryItem> AddItemAsync(int playerId, string itemName, int quantity = 1)
        {
            if (quantity <= 0)
                throw new ArgumentException("จำนวนต้องมากกว่า 0", nameof(quantity));

            var conn = await _databaseService.GetConnectionAsync();

            // หา Item master
            var item = await conn.Table<Item>()
                .Where(i => i.ItemName == itemName)
                .FirstOrDefaultAsync();
            if (item == null)
                throw new InvalidOperationException($"ไม่พบ item ชื่อ '{itemName}' ใน master data");

            // ถ้ามีอยู่แล้ว → update quantity
            var existing = await conn.Table<InventoryItem>()
                .Where(i => i.PlayerId == playerId && i.ItemId == item.ItemId)
                .FirstOrDefaultAsync();

            if (existing != null)
            {
                if (IsUniqueStoryItem(itemName) && existing.Quantity > 0)
                {
                    existing.ItemData = item;
                    return existing;
                }

                existing.Quantity += quantity;
                await conn.UpdateAsync(existing);
                existing.ItemData = item;
                return existing;
            }

            var currentItemCount = await conn.Table<InventoryItem>()
                .Where(i => i.PlayerId == playerId && i.Quantity > 0)
                .CountAsync();
            if (currentItemCount >= AppConstants.MaxInventorySize)
                throw new InvalidOperationException($"Inventory เต็มแล้ว (สูงสุด {AppConstants.MaxInventorySize} ชนิด)");

            // ถ้ายังไม่มี → insert ใหม่
            var newItem = new InventoryItem
            {
                PlayerId = playerId,
                ItemId = item.ItemId,
                Quantity = quantity,
                AcquiredAt = DateTime.UtcNow,
                ItemData = item
            };
            await conn.InsertAsync(newItem);
            return newItem;
        }

        // ============== Update ==============

        /// <summary>
        /// อัปเดต quantity ของ inventory item
        /// ถ้า quantity = 0 → ลบ item ออก
        /// </summary>
        public async Task UpdateQuantityAsync(InventoryItem item, int newQuantity)
        {
            if (newQuantity <= 0)
            {
                await _databaseService.DeleteAsync(item);
                return;
            }
            item.Quantity = newQuantity;
            await _databaseService.UpdateAsync(item);
        }

        /// <summary>
        /// ใช้ item (-1 quantity)
        /// คืนค่า ItemData เพื่อให้ caller รู้ว่า item อะไรถูกใช้
        /// </summary>
        public async Task<Item?> ConsumeItemAsync(int playerId, string itemName)
        {
            var inv = await FindByNameAsync(playerId, itemName);
            if (inv == null) return null;

            await UpdateQuantityAsync(inv, inv.Quantity - 1);
            return inv.ItemData;
        }

        // ============== Delete ==============

        /// <summary>
        /// ลบ item (ทิ้งทั้ง stack)
        /// </summary>
        public Task DeleteAsync(InventoryItem item)
        {
            return _databaseService.DeleteAsync(item);
        }

        // ============== Search ==============

        /// <summary>
        /// ค้นหา item ใน inventory ด้วย keyword (ค้นจากชื่อ)
        /// </summary>
        public async Task<List<InventoryItem>> SearchAsync(int playerId, string? keyword)
        {
            var all = await GetByPlayerAsync(playerId);
            if (string.IsNullOrWhiteSpace(keyword)) return all;

            keyword = keyword.Trim().ToLowerInvariant();
            return all.Where(i => i.DisplayName.ToLowerInvariant().Contains(keyword)
                               || i.DisplayDescription.ToLowerInvariant().Contains(keyword))
                      .ToList();
        }

        /// <summary>
        /// กรองตาม type
        /// </summary>
        public async Task<List<InventoryItem>> FilterByTypeAsync(int playerId, ItemType? type)
        {
            var all = await GetByPlayerAsync(playerId);
            if (!type.HasValue) return all;
            return all.Where(i => i.DisplayType == type.Value).ToList();
        }
    }
}
