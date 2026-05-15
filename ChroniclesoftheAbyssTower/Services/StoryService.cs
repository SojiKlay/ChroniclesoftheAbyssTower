using ChroniclesoftheAbyssTower.Models;

namespace ChroniclesoftheAbyssTower.Services
{
    /// <summary>
    /// ผลลัพธ์ของการเลือก choice แต่ละครั้ง
    /// แสดงผลใน UI หลังเลือก (ก่อนเลื่อนชั้น)
    /// </summary>
    public class ChoiceOutcome
    {
        public bool Success { get; set; }
        public string? RejectMessage { get; set; }      // ถ้า Success=false: เหตุผล (เช่น ไม่มี item)
        public string ResultText { get; set; } = "";    // ข้อความผลลัพธ์
        public int HpDelta { get; set; }
        public int GoldDelta { get; set; }
        public int ExpDelta { get; set; }
        public string? ItemAcquired { get; set; }
        public int ItemAcquiredQty { get; set; }
        public string? ItemConsumed { get; set; }
        public bool LeveledUp { get; set; }
        public bool PlayerDied { get; set; }
        public bool GameCompleted { get; set; }
        public string EndingType { get; set; } = "";
        public bool ShouldAdvanceFloor { get; set; }
        public bool StoryJournalUnlocked { get; set; }
        public Journal? UnlockedJournal { get; set; }
    }

    /// <summary>
    /// Story / Choice Engine
    /// 1. Load floor data จาก JSON
    /// 2. ตรวจ choice availability (มี item ที่ต้องการไหม)
    /// 3. apply effect (HP/Gold/EXP/Item)
    /// 4. unlock story journal
    /// 5. เลื่อน floor
    /// 6. ตรวจ ending
    /// </summary>
    public class StoryService
    {
        private readonly DatabaseService _databaseService;
        private readonly SeedDataService _seedDataService;
        private readonly PlayerService _playerService;
        private readonly InventoryService _inventoryService;
        private readonly JournalService _journalService;
        private Dictionary<string, string>? _itemDisplayNames;

        public StoryService(
            DatabaseService databaseService,
            SeedDataService seedDataService,
            PlayerService playerService,
            InventoryService inventoryService,
            JournalService journalService)
        {
            _databaseService = databaseService;
            _seedDataService = seedDataService;
            _playerService = playerService;
            _inventoryService = inventoryService;
            _journalService = journalService;
        }

        // ============== Load ==============

        /// <summary>
        /// ดึง event ของ floor ที่ player อยู่ปัจจุบัน
        /// </summary>
        public Task<StoryEvent?> GetCurrentFloorEventAsync(Player player)
        {
            return _seedDataService.GetFloorAsync(player.CurrentFloor);
        }

        public async Task<string> TranslateItemNamesAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            var displayNames = await GetItemDisplayNamesAsync();
            foreach (var item in displayNames.OrderByDescending(i => i.Key.Length))
            {
                text = text.Replace(item.Key, item.Value, StringComparison.OrdinalIgnoreCase);
            }

            return text;
        }

        private async Task<string> GetItemDisplayNameAsync(string itemName)
        {
            var displayNames = await GetItemDisplayNamesAsync();
            return displayNames.TryGetValue(itemName, out var displayName)
                ? displayName
                : itemName;
        }

        private async Task<Dictionary<string, string>> GetItemDisplayNamesAsync()
        {
            if (_itemDisplayNames != null) return _itemDisplayNames;

            var conn = await _databaseService.GetConnectionAsync();
            var items = await conn.Table<Item>().ToListAsync();

            _itemDisplayNames = items.ToDictionary(
                item => item.ItemName,
                item => string.IsNullOrWhiteSpace(item.ThaiName) ? item.ItemName : item.ThaiName,
                StringComparer.OrdinalIgnoreCase);

            return _itemDisplayNames;
        }

        // ============== Choice Availability ==============

        /// <summary>
        /// ตรวจว่า choice นี้เลือกได้ไหม (มี item ที่ต้องการไหม)
        /// </summary>
        public async Task<bool> CanSelectChoiceAsync(Player player, StoryChoice choice)
        {
            // ถ้าต้องการ item พิเศษ → เช็คว่ามีหรือเปล่า
            return await GetChoiceLockReasonAsync(player, choice) == null;
        }

        public async Task<string?> GetChoiceLockReasonAsync(Player player, StoryChoice choice)
        {
            foreach (var itemName in GetBlockedItemNames(choice))
            {
                var has = await _inventoryService.HasItemAsync(player.PlayerId, itemName);
                if (has)
                    return $"มี '{await GetItemDisplayNameAsync(itemName)}' แล้ว";
            }

            foreach (var itemName in GetRequiredItemNames(choice))
            {
                var has = await _inventoryService.HasItemAsync(player.PlayerId, itemName);
                if (!has)
                    return $"ต้องมี '{await GetItemDisplayNameAsync(itemName)}' จึงจะเลือกได้";
            }

            if (choice.GoldDelta < 0 && player.Gold < Math.Abs(choice.GoldDelta))
                return $"ต้องมี Gold อย่างน้อย {Math.Abs(choice.GoldDelta)}";

            return null;
        }

        private static List<string> GetRequiredItemNames(StoryChoice choice)
        {
            var itemNames = new List<string>();

            if (!string.IsNullOrWhiteSpace(choice.RequiresItem))
                itemNames.Add(choice.RequiresItem);

            if (choice.RequiresItems != null)
                itemNames.AddRange(choice.RequiresItems.Where(name => !string.IsNullOrWhiteSpace(name)));

            if (!string.IsNullOrWhiteSpace(choice.RequiredItemNoConsume))
                itemNames.Add(choice.RequiredItemNoConsume);

            return itemNames
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static List<string> GetBlockedItemNames(StoryChoice choice)
        {
            var itemNames = new List<string>();

            if (!string.IsNullOrWhiteSpace(choice.BlockedByItem))
                itemNames.Add(choice.BlockedByItem);

            if (choice.BlockedByItems != null)
                itemNames.AddRange(choice.BlockedByItems.Where(name => !string.IsNullOrWhiteSpace(name)));

            return itemNames
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static List<string> GetConsumedItemNames(StoryChoice choice)
        {
            var itemNames = new List<string>();

            if (!string.IsNullOrWhiteSpace(choice.RequiredItem))
                itemNames.Add(choice.RequiredItem);

            if (choice.RequiredItems != null)
                itemNames.AddRange(choice.RequiredItems.Where(name => !string.IsNullOrWhiteSpace(name)));

            return itemNames
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private async Task<bool> IsRepeatRewardChoiceAsync(int playerId, StoryChoice choice)
        {
            if (string.IsNullOrWhiteSpace(choice.ItemReward) ||
                string.IsNullOrWhiteSpace(choice.RepeatResultText))
                return false;

            return await _inventoryService.HasItemAsync(playerId, choice.ItemReward);
        }

        // ============== Apply Choice ==============

        /// <summary>
        /// ลงผลของ choice ที่ผู้เล่นเลือก
        /// flow:
        /// 1. ตรวจว่าเลือกได้ (require item)
        /// 2. consume item ที่ต้องใช้
        /// 3. apply HP/Gold/EXP delta
        /// 4. ให้ item reward
        /// 5. unlock story journal (ถ้ามี)
        /// 6. บันทึก progress
        /// 7. ตัดสินใจว่า advance floor / die / complete game
        /// </summary>
        public async Task<ChoiceOutcome> ApplyChoiceAsync(Player player, StoryEvent storyEvent, int choiceIndex)
        {
            var outcome = new ChoiceOutcome { ShouldAdvanceFloor = true };

            // ========== Validate ==========
            if (choiceIndex < 0 || choiceIndex >= storyEvent.Choices.Count)
            {
                outcome.Success = false;
                outcome.RejectMessage = "ตัวเลือกไม่ถูกต้อง";
                return outcome;
            }
            var choice = storyEvent.Choices[choiceIndex];
            var isRepeatRewardChoice = await IsRepeatRewardChoiceAsync(player.PlayerId, choice);

            // ตรวจ requires item (เลือกได้ไหม)
            var lockReason = await GetChoiceLockReasonAsync(player, choice);
            if (lockReason != null)
            {
                outcome.Success = false;
                outcome.RejectMessage = lockReason;
                return outcome;
            }

            outcome.Success = true;
            outcome.ResultText = await TranslateItemNamesAsync(isRepeatRewardChoice ? choice.RepeatResultText! : choice.ResultText);
            outcome.HpDelta = isRepeatRewardChoice ? 0 : choice.HpDelta;
            outcome.GoldDelta = isRepeatRewardChoice ? 0 : choice.GoldDelta;
            outcome.ExpDelta = isRepeatRewardChoice ? 0 : choice.ExpDelta;
            outcome.ShouldAdvanceFloor = choice.AdvanceFloor;
            outcome.EndingType = choice.EndingType;

            // ========== Consume item ที่ต้องใช้ ==========
            foreach (var itemName in GetConsumedItemNames(choice))
            {
                var consumed = await _inventoryService.ConsumeItemAsync(player.PlayerId, itemName);
                if (consumed != null)
                {
                    var consumedName = string.IsNullOrWhiteSpace(consumed.ThaiName)
                        ? consumed.ItemName
                        : consumed.ThaiName;
                    outcome.ItemConsumed = string.IsNullOrWhiteSpace(outcome.ItemConsumed)
                        ? consumedName
                        : $"{outcome.ItemConsumed}, {consumedName}";
                }
            }

            // ========== Apply HP / Gold deltas ==========
            if (outcome.HpDelta != 0)
                await _playerService.ApplyHpDeltaAsync(player, outcome.HpDelta);

            if (outcome.GoldDelta != 0)
                await _playerService.ApplyGoldDeltaAsync(player, outcome.GoldDelta);

            // ========== EXP + auto level up ==========
            if (outcome.ExpDelta > 0)
            {
                var (_, leveledUp) = await _playerService.AddExperienceAsync(player, outcome.ExpDelta);
                outcome.LeveledUp = leveledUp;
            }

            // ========== Item reward ==========
            if (!isRepeatRewardChoice && !string.IsNullOrWhiteSpace(choice.ItemReward) && choice.ItemRewardQuantity > 0)
            {
                try
                {
                    var alreadyHasUniqueItem = InventoryService.IsUniqueStoryItem(choice.ItemReward) &&
                        await _inventoryService.HasItemAsync(player.PlayerId, choice.ItemReward);

                    await _inventoryService.AddItemAsync(player.PlayerId, choice.ItemReward, choice.ItemRewardQuantity);

                    if (!alreadyHasUniqueItem)
                    {
                        outcome.ItemAcquired = await GetItemDisplayNameAsync(choice.ItemReward);
                        outcome.ItemAcquiredQty = choice.ItemRewardQuantity;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[StoryService] Item reward failed: {ex.Message}");
                }
            }

            // ========== Unlock story journal ของ floor นี้ ==========
            if (!string.IsNullOrWhiteSpace(storyEvent.StoryJournalKey))
            {
                var journal = await _journalService.UnlockStoryJournalAsync(player.PlayerId, storyEvent.StoryJournalKey);
                if (journal != null)
                {
                    outcome.StoryJournalUnlocked = true;
                    outcome.UnlockedJournal = journal;
                }
            }

            // ========== Increment choice counter ==========
            player.TotalChoicesMade++;
            await _playerService.UpdateAsync(player);

            // ========== บันทึก progress ==========
            await SaveProgressAsync(player.PlayerId, storyEvent.FloorNumber, choiceIndex, choice.Text);

            // ========== ตรวจ ending จาก choice ==========
            if (!string.IsNullOrWhiteSpace(choice.EndingType))
            {
                outcome.GameCompleted = true;
                await _playerService.CompleteGameAsync(player, choice.EndingType);
                return outcome;
            }

            // ========== ตรวจการตาย ==========
            if (player.Hp <= 0)
            {
                outcome.PlayerDied = true;
                outcome.ShouldAdvanceFloor = false;
                outcome.GameCompleted = true;
                outcome.EndingType = "Bad";
                await _playerService.CompleteGameAsync(player, "Bad");
                return outcome;
            }

            // ========== เลื่อน floor ==========
            if (outcome.ShouldAdvanceFloor)
            {
                if (storyEvent.IsFinalFloor)
                {
                    // ผ่าน final floor โดยไม่มี ending → default ใช้ Good
                    outcome.GameCompleted = true;
                    await _playerService.CompleteGameAsync(player, "Good");
                    outcome.EndingType = "Good";
                }
                else
                {
                    await _playerService.AdvanceFloorAsync(player);
                }
            }

            return outcome;
        }

        // ============== Progress ==============

        /// <summary>
        /// บันทึก progress ของ floor + choice ที่เลือก
        /// </summary>
        private async Task SaveProgressAsync(int playerId, int floorNumber, int choiceIndex, string choiceText)
        {
            var conn = await _databaseService.GetConnectionAsync();

            var existing = await conn.Table<StoryProgress>()
                .Where(p => p.PlayerId == playerId && p.FloorNumber == floorNumber)
                .FirstOrDefaultAsync();

            if (existing != null)
            {
                existing.IsCompleted = true;
                existing.ChoiceMade = choiceIndex;
                existing.ChoiceText = choiceText;
                existing.CompletedAt = DateTime.UtcNow;
                await conn.UpdateAsync(existing);
            }
            else
            {
                await conn.InsertAsync(new StoryProgress
                {
                    PlayerId = playerId,
                    FloorNumber = floorNumber,
                    IsCompleted = true,
                    ChoiceMade = choiceIndex,
                    ChoiceText = choiceText,
                    CompletedAt = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// ดึง progress ทั้งหมดของ player
        /// </summary>
        public async Task<List<StoryProgress>> GetProgressAsync(int playerId)
        {
            var conn = await _databaseService.GetConnectionAsync();
            return await conn.Table<StoryProgress>()
                .Where(p => p.PlayerId == playerId)
                .OrderBy(p => p.FloorNumber)
                .ToListAsync();
        }
    }
}
