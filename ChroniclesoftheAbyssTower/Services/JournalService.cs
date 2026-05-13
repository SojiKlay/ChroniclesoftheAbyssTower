using ChroniclesoftheAbyssTower.Models;

namespace ChroniclesoftheAbyssTower.Services
{
    /// <summary>
    /// Service จัดการ Journal - CRUD ครบสำหรับ Player Journal,
    /// auto-unlock สำหรับ Story Journal
    /// </summary>
    public class JournalService
    {
        private readonly DatabaseService _databaseService;
        private readonly SeedDataService _seedDataService;

        public JournalService(DatabaseService databaseService, SeedDataService seedDataService)
        {
            _databaseService = databaseService;
            _seedDataService = seedDataService;
        }

        // ============== Read ==============

        /// <summary>
        /// ดึง journal ทั้งหมดของ player (เรียงล่าสุดก่อน)
        /// </summary>
        public async Task<List<Journal>> GetByPlayerAsync(int playerId)
        {
            var conn = await _databaseService.GetConnectionAsync();
            return await conn.Table<Journal>()
                .Where(j => j.PlayerId == playerId)
                .OrderBy(j => j.FloorNumber)
                .ToListAsync();
        }

        /// <summary>
        /// ดึงเฉพาะตามประเภท
        /// </summary>
        public async Task<List<Journal>> GetByPlayerAndTypeAsync(int playerId, JournalType type)
        {
            var conn = await _databaseService.GetConnectionAsync();
            return await conn.Table<Journal>()
                .Where(j => j.PlayerId == playerId && j.JournalType == type)
                .OrderBy(j => j.FloorNumber)
                .ThenByDescending(j => j.CreatedAt)
                .ToListAsync();
        }

        public Task<Journal?> GetByIdAsync(int journalId)
        {
            return _databaseService.GetAsync<Journal>(journalId);
        }

        // ============== Create ==============

        /// <summary>
        /// สร้าง Player Journal (ผู้เล่นเขียนเอง)
        /// </summary>
        public async Task<Journal> CreatePlayerJournalAsync(int playerId, int floorNumber, string title, string content)
        {
            var journal = new Journal
            {
                PlayerId = playerId,
                JournalType = JournalType.Player,
                FloorNumber = floorNumber,
                Title = title.Trim(),
                Content = content.Trim(),
                StoryKey = string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var conn = await _databaseService.GetConnectionAsync();
            await conn.InsertAsync(journal);
            return journal;
        }

        /// <summary>
        /// Unlock Story Journal จาก seed key (ถ้ายังไม่ unlock มาก่อน)
        /// </summary>
        public async Task<Journal?> UnlockStoryJournalAsync(int playerId, string storyKey)
        {
            if (string.IsNullOrWhiteSpace(storyKey)) return null;

            var conn = await _databaseService.GetConnectionAsync();

            // ตรวจว่า unlock ไปแล้วหรือยัง
            var existing = await conn.Table<Journal>()
                .Where(j => j.PlayerId == playerId && j.StoryKey == storyKey)
                .FirstOrDefaultAsync();
            if (existing != null) return null;

            // โหลด seed
            var seed = await _seedDataService.GetStoryJournalByKeyAsync(storyKey);
            if (seed == null) return null;

            var journal = new Journal
            {
                PlayerId = playerId,
                JournalType = JournalType.Story,
                FloorNumber = seed.FloorNumber,
                Title = seed.Title,
                Content = seed.Content,
                StoryKey = seed.Key,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await conn.InsertAsync(journal);
            return journal;
        }

        // ============== Update ==============

        /// <summary>
        /// แก้ไข Player Journal (Story Journal แก้ไขไม่ได้)
        /// </summary>
        public async Task UpdatePlayerJournalAsync(Journal journal, string newTitle, string newContent)
        {
            if (journal.JournalType == JournalType.Story)
                throw new InvalidOperationException("Story Journal ไม่สามารถแก้ไขได้");

            journal.Title = newTitle.Trim();
            journal.Content = newContent.Trim();
            journal.UpdatedAt = DateTime.UtcNow;

            await _databaseService.UpdateAsync(journal);
        }

        // ============== Delete ==============

        /// <summary>
        /// ลบ Player Journal (Story Journal ลบไม่ได้)
        /// </summary>
        public Task DeletePlayerJournalAsync(Journal journal)
        {
            if (journal.JournalType == JournalType.Story)
                throw new InvalidOperationException("Story Journal ไม่สามารถลบได้");

            return _databaseService.DeleteAsync(journal);
        }

        // ============== Search ==============

        /// <summary>
        /// ค้นหา journal ตาม keyword (Title หรือ Content)
        /// </summary>
        public async Task<List<Journal>> SearchAsync(int playerId, string? keyword, JournalType? type = null)
        {
            var all = type.HasValue
                ? await GetByPlayerAndTypeAsync(playerId, type.Value)
                : await GetByPlayerAsync(playerId);

            if (string.IsNullOrWhiteSpace(keyword)) return all;

            keyword = keyword.Trim().ToLowerInvariant();
            return all.Where(j => j.Title.ToLowerInvariant().Contains(keyword)
                               || j.Content.ToLowerInvariant().Contains(keyword))
                      .ToList();
        }
    }
}
