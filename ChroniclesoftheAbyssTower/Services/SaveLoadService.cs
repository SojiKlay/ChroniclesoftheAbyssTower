using ChroniclesoftheAbyssTower.Helpers;
using ChroniclesoftheAbyssTower.Models;
using Newtonsoft.Json;

namespace ChroniclesoftheAbyssTower.Services
{
    /// <summary>
    /// Service จัดการ Save Slot (Phase 6)
    /// - User คนหนึ่งมี save ได้สูงสุด 3 slot (1-3)
    /// - แต่ละ slot เก็บ snapshot ของ player + inventory + journal + progress เป็น JSON
    /// - Save = serialize ปัจจุบันลง slot
    /// - Load = deserialize + apply กลับเข้า DB (โดย create player ใหม่)
    /// </summary>
    public class SaveLoadService
    {
        private readonly DatabaseService _databaseService;
        private readonly PlayerService _playerService;
        private readonly InventoryService _inventoryService;
        private readonly JournalService _journalService;

        public SaveLoadService(
            DatabaseService databaseService,
            PlayerService playerService,
            InventoryService inventoryService,
            JournalService journalService)
        {
            _databaseService = databaseService;
            _playerService = playerService;
            _inventoryService = inventoryService;
            _journalService = journalService;
        }

        // ============== Read ==============

        /// <summary>
        /// ดึง save slot ทั้งหมดของ user (3 slot - slot ที่ยังไม่ใช้จะเป็น null)
        /// </summary>
        public async Task<List<SaveData?>> GetAllSlotsAsync(int userId)
        {
            var conn = await _databaseService.GetConnectionAsync();
            var saves = await conn.Table<SaveData>()
                .Where(s => s.UserId == userId)
                .ToListAsync();

            var slots = new List<SaveData?>(AppConstants.MaxSaveSlots);
            for (int i = 1; i <= AppConstants.MaxSaveSlots; i++)
            {
                slots.Add(saves.FirstOrDefault(s => s.SaveSlot == i));
            }
            return slots;
        }

        public async Task<SaveData?> GetSlotAsync(int userId, int slot)
        {
            var conn = await _databaseService.GetConnectionAsync();
            return await conn.Table<SaveData>()
                .Where(s => s.UserId == userId && s.SaveSlot == slot)
                .FirstOrDefaultAsync();
        }

        // ============== Save ==============

        /// <summary>
        /// บันทึก snapshot ปัจจุบันของ active player ลงใน slot
        /// ถ้า slot มี save อยู่แล้ว → overwrite
        /// </summary>
        public async Task<SaveData> SaveToSlotAsync(int userId, int slot, string? customName = null)
        {
            if (slot < 1 || slot > AppConstants.MaxSaveSlots)
                throw new ArgumentException($"Slot ต้องอยู่ระหว่าง 1-{AppConstants.MaxSaveSlots}", nameof(slot));

            var player = await _playerService.GetActivePlayerAsync();
            if (player == null)
                throw new InvalidOperationException("ไม่พบตัวละครที่กำลังเล่นอยู่");

            // โหลด inventory + journals + progress
            var inventory = await _inventoryService.GetByPlayerAsync(player.PlayerId);
            var journals = await _journalService.GetByPlayerAsync(player.PlayerId);

            var conn = await _databaseService.GetConnectionAsync();
            var progress = await conn.Table<StoryProgress>()
                .Where(p => p.PlayerId == player.PlayerId)
                .ToListAsync();

            // Serialize
            var playerJson = JsonConvert.SerializeObject(player);
            var inventoryJson = JsonConvert.SerializeObject(inventory);
            var journalJson = JsonConvert.SerializeObject(journals);
            var progressJson = JsonConvert.SerializeObject(progress);

            // หา save เดิม
            var existing = await GetSlotAsync(userId, slot);
            var saveData = existing ?? new SaveData
            {
                UserId = userId,
                SaveSlot = slot,
            };

            saveData.SaveName = string.IsNullOrWhiteSpace(customName)
                ? $"{player.PlayerName} - ชั้นที่ {player.CurrentFloor}"
                : customName.Trim();
            saveData.SaveDate = DateTime.UtcNow;
            saveData.PlayerSnapshot = playerJson;
            saveData.InventorySnapshot = inventoryJson;
            saveData.JournalSnapshot = journalJson;
            saveData.ProgressSnapshot = progressJson;
            saveData.PlayerName = player.PlayerName;
            saveData.CurrentFloor = player.CurrentFloor;
            saveData.Hp = player.Hp;
            saveData.Level = player.Level;

            if (existing == null)
                await conn.InsertAsync(saveData);
            else
                await conn.UpdateAsync(saveData);

            return saveData;
        }

        // ============== Load ==============

        /// <summary>
        /// โหลด save จาก slot → สร้าง player ใหม่ + apply ข้อมูลทั้งหมด
        /// คืนค่า PlayerId ใหม่ที่ใช้เล่นต่อ
        /// </summary>
        public async Task<int> LoadFromSlotAsync(int userId, int slot)
        {
            var save = await GetSlotAsync(userId, slot);
            if (save == null)
                throw new InvalidOperationException($"Slot {slot} ว่างเปล่า");

            // Deserialize player + reset PlayerId (สร้างใหม่)
            var loadedPlayer = JsonConvert.DeserializeObject<Player>(save.PlayerSnapshot);
            if (loadedPlayer == null)
                throw new InvalidOperationException("ข้อมูล save เสียหาย");

            loadedPlayer.PlayerId = 0; // ให้ AutoIncrement สร้างใหม่
            await _playerService.DeleteIncompletePlayersForUserAsync(userId);

            loadedPlayer.UserId = userId;
            loadedPlayer.UpdatedAt = DateTime.UtcNow;

            var conn = await _databaseService.GetConnectionAsync();
            await conn.InsertAsync(loadedPlayer);
            var newPlayerId = loadedPlayer.PlayerId;

            // โหลด inventory
            var loadedInventory = JsonConvert.DeserializeObject<List<InventoryItem>>(save.InventorySnapshot) ?? new();
            foreach (var inv in loadedInventory)
            {
                inv.InventoryId = 0;
                inv.PlayerId = newPlayerId;
                inv.ItemData = null; // [Ignore] field, ไม่ insert
                await conn.InsertAsync(inv);
            }

            // โหลด journals
            var loadedJournals = JsonConvert.DeserializeObject<List<Journal>>(save.JournalSnapshot) ?? new();
            foreach (var j in loadedJournals)
            {
                j.JournalId = 0;
                j.PlayerId = newPlayerId;
                await conn.InsertAsync(j);
            }

            // โหลด progress
            var loadedProgress = JsonConvert.DeserializeObject<List<StoryProgress>>(save.ProgressSnapshot) ?? new();
            foreach (var p in loadedProgress)
            {
                p.ProgressId = 0;
                p.PlayerId = newPlayerId;
                await conn.InsertAsync(p);
            }

            // set active player
            await SessionManager.SetActivePlayerIdAsync(newPlayerId);
            return newPlayerId;
        }

        // ============== Delete ==============

        public async Task DeleteSlotAsync(int userId, int slot)
        {
            var save = await GetSlotAsync(userId, slot);
            if (save == null) return;
            await _databaseService.DeleteAsync(save);
        }
    }
}
