using ChroniclesoftheAbyssTower.Helpers;
using ChroniclesoftheAbyssTower.Models;

namespace ChroniclesoftheAbyssTower.Services
{
    /// <summary>
    /// Service จัดการ Player (ตัวละคร) - CRUD + business logic
    /// 1 user มี player ได้หลายตัว แต่ active ทีละตัว
    /// </summary>
    public class PlayerService
    {
        private readonly DatabaseService _databaseService;

        public PlayerService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // ============== Create ==============

        /// <summary>
        /// สร้างตัวละครใหม่สำหรับ user คนนี้ (เริ่ม New Game)
        /// </summary>
        public async Task<Player> CreateNewPlayerAsync(int userId, string playerName)
        {
            await DeleteIncompletePlayersForUserAsync(userId);

            var player = new Player
            {
                UserId = userId,
                PlayerName = string.IsNullOrWhiteSpace(playerName) ? "ผู้เล่น" : playerName.Trim(),
                Hp = AppConstants.StartingHp,
                MaxHp = AppConstants.StartingMaxHp,
                Attack = AppConstants.StartingAttack,
                Defense = AppConstants.StartingDefense,
                Gold = AppConstants.StartingGold,
                Level = AppConstants.StartingLevel,
                CurrentFloor = AppConstants.StartingFloor,
                HighestFloor = AppConstants.StartingFloor,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var conn = await _databaseService.GetConnectionAsync();
            await conn.InsertAsync(player);

            // ตั้งให้เป็น active player ทันที
            await SessionManager.SetActivePlayerIdAsync(player.PlayerId);

            return player;
        }

        // ============== Read ==============

        /// <summary>
        /// ดึง active player ของ user ที่ login อยู่
        /// </summary>
        public async Task<Player?> GetActivePlayerAsync()
        {
            var playerId = await SessionManager.GetActivePlayerIdAsync();
            if (!playerId.HasValue) return null;

            return await _databaseService.GetAsync<Player>(playerId.Value);
        }

        /// <summary>
        /// ดึง player ตาม id
        /// </summary>
        public Task<Player?> GetByIdAsync(int playerId)
        {
            return _databaseService.GetAsync<Player>(playerId);
        }

        /// <summary>
        /// ดึง player ทั้งหมดของ user (เรียงล่าสุดก่อน)
        /// </summary>
        public async Task<List<Player>> GetByUserAsync(int userId)
        {
            var conn = await _databaseService.GetConnectionAsync();
            return await conn.Table<Player>()
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.UpdatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// ดึง player ล่าสุดของ user (สำหรับ Continue Game)
        /// </summary>
        public async Task<Player?> GetLatestByUserAsync(int userId)
        {
            var conn = await _databaseService.GetConnectionAsync();
            return await conn.Table<Player>()
                .Where(p => p.UserId == userId && !p.IsGameCompleted)
                .OrderByDescending(p => p.UpdatedAt)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// ลบ working copy ที่ยังเล่นไม่จบของ user คนนี้
        /// ใช้ก่อน New Game / Load Game เพื่อไม่ให้ Continue ย้อนไปเล่นตัวละครเก่าที่เคยโหลดไว้
        /// </summary>
        public async Task DeleteIncompletePlayersForUserAsync(int userId)
        {
            var conn = await _databaseService.GetConnectionAsync();
            var players = await conn.Table<Player>()
                .Where(p => p.UserId == userId && !p.IsGameCompleted)
                .ToListAsync();

            foreach (var player in players)
            {
                await conn.ExecuteAsync("DELETE FROM InventoryItems WHERE PlayerId = ?", player.PlayerId);
                await conn.ExecuteAsync("DELETE FROM Journals WHERE PlayerId = ?", player.PlayerId);
                await conn.ExecuteAsync("DELETE FROM StoryProgress WHERE PlayerId = ?", player.PlayerId);
                await conn.DeleteAsync(player);
            }
        }

        // ============== Update ==============

        /// <summary>
        /// บันทึกค่าทั้งหมดของ player กลับลง DB
        /// </summary>
        public async Task UpdateAsync(Player player)
        {
            player.UpdatedAt = DateTime.UtcNow;
            await _databaseService.UpdateAsync(player);
        }

        /// <summary>
        /// อัปเดต HP (ใช้กับ +/- จาก story choices)
        /// clamp ไม่ให้เกิน MaxHp และไม่ติดลบ
        /// </summary>
        public async Task<Player> ApplyHpDeltaAsync(Player player, int delta)
        {
            player.Hp = Math.Clamp(player.Hp + delta, 0, player.MaxHp);
            await UpdateAsync(player);
            return player;
        }

        /// <summary>
        /// อัปเดต Gold (clamp ไม่ติดลบ)
        /// </summary>
        public async Task<Player> ApplyGoldDeltaAsync(Player player, int delta)
        {
            player.Gold = Math.Max(0, player.Gold + delta);
            await UpdateAsync(player);
            return player;
        }

        /// <summary>
        /// ให้ EXP และ level up อัตโนมัติถ้า EXP ถึงเกณฑ์
        /// formula: ต้องการ EXP = level * 100
        /// </summary>
        public async Task<(Player Player, bool LeveledUp)> AddExperienceAsync(Player player, int exp)
        {
            player.Experience += exp;
            bool leveledUp = false;

            while (player.Experience >= player.Level * 100)
            {
                player.Experience -= player.Level * 100;
                player.Level++;
                player.MaxHp += 10;
                player.Hp = Math.Min(player.MaxHp, player.Hp + 10); // heal 10 ตอน level up
                player.Attack += 2;
                player.Defense += 1;
                leveledUp = true;
            }

            await UpdateAsync(player);
            return (player, leveledUp);
        }

        /// <summary>
        /// เลื่อน floor หลังผ่าน floor ปัจจุบัน
        /// </summary>
        public async Task AdvanceFloorAsync(Player player)
        {
            player.CurrentFloor = Math.Min(AppConstants.TotalFloors, player.CurrentFloor + 1);
            if (player.CurrentFloor > player.HighestFloor)
                player.HighestFloor = player.CurrentFloor;
            await UpdateAsync(player);
        }

        /// <summary>
        /// จบเกม - บันทึก ending type
        /// </summary>
        public async Task CompleteGameAsync(Player player, string endingType)
        {
            player.IsGameCompleted = true;
            player.EndingType = endingType;
            await UpdateAsync(player);
        }

        // ============== Delete ==============

        /// <summary>
        /// ลบ player (ใช้กับ Delete Save)
        /// </summary>
        public Task DeleteAsync(Player player)
        {
            return _databaseService.DeleteAsync(player);
        }
    }
}
