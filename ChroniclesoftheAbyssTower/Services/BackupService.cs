using System.Text;
using ChroniclesoftheAbyssTower.Helpers;
using ChroniclesoftheAbyssTower.Models;
using Newtonsoft.Json;

namespace ChroniclesoftheAbyssTower.Services
{
    /// <summary>
    /// Service จัดการ Export/Import Backup (Phase 6)
    /// - Export: บันทึก User + ตัวละครทั้งหมด + Inventory/Journal/Save เป็น JSON
    /// - Import: อ่าน JSON + restore กลับเข้า DB
    ///
    /// ไฟล์เก็บที่ FileSystem.AppDataDirectory/AbyssTowerBackups/
    /// เพื่อ share ออกใช้ FileSaver/FilePicker ผ่าน Platform (ผู้ใช้สามารถ copy ไป cloud ได้เอง)
    /// </summary>
    public class BackupService
    {
        private readonly DatabaseService _databaseService;
        private readonly JsonSerializerSettings _jsonSettings;

        public BackupService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
            };
        }

        /// <summary>
        /// Path ของโฟลเดอร์ backup
        /// </summary>
        public string BackupFolderPath =>
            Path.Combine(FileSystem.AppDataDirectory, AppConstants.BackupFolderName);

        // ============== Export ==============

        /// <summary>
        /// Export ข้อมูลของ user ปัจจุบันเป็น JSON file
        /// คืนค่า path ของไฟล์ที่สร้าง
        /// </summary>
        public async Task<string> ExportToFileAsync(int userId)
        {
            var data = await BuildBackupDataAsync(userId);
            var json = JsonConvert.SerializeObject(data, _jsonSettings);

            // สร้างโฟลเดอร์ถ้ายังไม่มี
            Directory.CreateDirectory(BackupFolderPath);

            // ตั้งชื่อไฟล์ตามเวลา
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"abyss_backup_{timestamp}{AppConstants.BackupFileExtension}";
            var fullPath = Path.Combine(BackupFolderPath, fileName);

            await File.WriteAllTextAsync(fullPath, json, Encoding.UTF8);
            return fullPath;
        }

        /// <summary>
        /// สร้าง BackupData object จาก DB (ใช้ใน export และเช็ค preview)
        /// </summary>
        public async Task<BackupData> BuildBackupDataAsync(int userId)
        {
            var conn = await _databaseService.GetConnectionAsync();

            var user = await conn.Table<User>().Where(u => u.UserId == userId).FirstOrDefaultAsync();
            var players = await conn.Table<Player>().Where(p => p.UserId == userId).ToListAsync();
            var playerIds = players.Select(p => p.PlayerId).ToList();

            var inventory = playerIds.Count > 0
                ? await conn.Table<InventoryItem>().Where(i => playerIds.Contains(i.PlayerId)).ToListAsync()
                : new List<InventoryItem>();

            var journals = playerIds.Count > 0
                ? await conn.Table<Journal>().Where(j => playerIds.Contains(j.PlayerId)).ToListAsync()
                : new List<Journal>();

            var saves = await conn.Table<SaveData>().Where(s => s.UserId == userId).ToListAsync();

            var progress = playerIds.Count > 0
                ? await conn.Table<StoryProgress>().Where(p => playerIds.Contains(p.PlayerId)).ToListAsync()
                : new List<StoryProgress>();

            return new BackupData
            {
                SchemaVersion = AppConstants.BackupSchemaVersion,
                CreatedAt = DateTime.UtcNow,
                AppName = "ChroniclesoftheAbyssTower",
                User = user,
                Players = players,
                InventoryItems = inventory,
                Journals = journals,
                SaveData = saves,
                StoryProgress = progress,
            };
        }

        /// <summary>
        /// ดึงรายการไฟล์ backup ทั้งหมดที่อยู่ในโฟลเดอร์ (เรียงล่าสุดก่อน)
        /// </summary>
        public List<string> GetBackupFiles()
        {
            if (!Directory.Exists(BackupFolderPath)) return new List<string>();
            return Directory.GetFiles(BackupFolderPath, $"*{AppConstants.BackupFileExtension}")
                            .OrderByDescending(f => File.GetLastWriteTime(f))
                            .ToList();
        }

        // ============== Import ==============

        /// <summary>
        /// อ่านไฟล์ backup + parse เป็น object (ไม่ apply เข้า DB)
        /// ใช้สำหรับ preview ก่อน restore
        /// </summary>
        public async Task<BackupData?> PreviewFileAsync(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            var json = await File.ReadAllTextAsync(filePath);
            return JsonConvert.DeserializeObject<BackupData>(json);
        }

        /// <summary>
        /// Restore backup เข้า DB ของ user ปัจจุบัน
        /// - ลบข้อมูลเดิมของ user คนนี้ก่อน
        /// - Insert ข้อมูลใหม่จาก backup
        /// **WARNING**: destructive operation
        /// </summary>
        public async Task RestoreAsync(int userId, BackupData backup)
        {
            if (backup.User == null)
                throw new InvalidOperationException("ไฟล์ backup ไม่ถูกต้อง (ไม่มี User)");

            var conn = await _databaseService.GetConnectionAsync();

            // ลบของเดิมของ user นี้ก่อน
            var oldPlayers = await conn.Table<Player>().Where(p => p.UserId == userId).ToListAsync();
            var oldPlayerIds = oldPlayers.Select(p => p.PlayerId).ToList();

            foreach (var pid in oldPlayerIds)
            {
                await conn.ExecuteAsync("DELETE FROM InventoryItems WHERE PlayerId = ?", pid);
                await conn.ExecuteAsync("DELETE FROM Journals WHERE PlayerId = ?", pid);
                await conn.ExecuteAsync("DELETE FROM StoryProgress WHERE PlayerId = ?", pid);
            }
            await conn.ExecuteAsync("DELETE FROM Players WHERE UserId = ?", userId);
            await conn.ExecuteAsync("DELETE FROM SaveData WHERE UserId = ?", userId);

            // map PlayerId เก่า → ใหม่
            var idMap = new Dictionary<int, int>();

            // Insert players (ใช้ UserId ปัจจุบัน + reset PlayerId)
            foreach (var p in backup.Players)
            {
                var oldId = p.PlayerId;
                p.PlayerId = 0;
                p.UserId = userId;
                await conn.InsertAsync(p);
                idMap[oldId] = p.PlayerId;
            }

            // Insert inventory
            foreach (var inv in backup.InventoryItems)
            {
                if (!idMap.TryGetValue(inv.PlayerId, out var newPid)) continue;
                inv.InventoryId = 0;
                inv.PlayerId = newPid;
                inv.ItemData = null;
                await conn.InsertAsync(inv);
            }

            // Insert journals
            foreach (var j in backup.Journals)
            {
                if (!idMap.TryGetValue(j.PlayerId, out var newPid)) continue;
                j.JournalId = 0;
                j.PlayerId = newPid;
                await conn.InsertAsync(j);
            }

            // Insert progress
            foreach (var p in backup.StoryProgress)
            {
                if (!idMap.TryGetValue(p.PlayerId, out var newPid)) continue;
                p.ProgressId = 0;
                p.PlayerId = newPid;
                await conn.InsertAsync(p);
            }

            // Insert saves (อ้าง UserId เท่านั้น)
            foreach (var s in backup.SaveData)
            {
                s.SaveId = 0;
                s.UserId = userId;
                await conn.InsertAsync(s);
            }

            // ล้าง active player (ให้ user เลือกใหม่)
            SessionManager.ClearActivePlayer();
        }

        // ============== Delete Backup File ==============

        public void DeleteBackupFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
