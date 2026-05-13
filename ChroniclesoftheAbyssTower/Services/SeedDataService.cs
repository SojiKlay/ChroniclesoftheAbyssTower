using ChroniclesoftheAbyssTower.Models;
using Newtonsoft.Json;

namespace ChroniclesoftheAbyssTower.Services
{
    /// <summary>
    /// โหลดข้อมูลตั้งต้น (Items, Floors, StoryJournals) จาก JSON ใน Resources/Raw
    /// </summary>
    public class SeedDataService
    {
        private readonly DatabaseService _databaseService;

        // Cache เพื่อไม่ต้องอ่านไฟล์ซ้ำ
        private List<StoryEvent>? _floors;
        private List<StoryJournalSeed>? _storyJournals;

        public SeedDataService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// อ่านไฟล์ JSON จาก Resources/Raw (ผ่าน MAUI FileSystem)
        /// </summary>
        private static async Task<string> LoadRawFileAsync(string fileName)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        /// <summary>
        /// โหลด Floors จาก floors.json (cache)
        /// </summary>
        public async Task<List<StoryEvent>> GetFloorsAsync()
        {
            if (_floors != null) return _floors;

            var json = await LoadRawFileAsync("floors.json");
            _floors = JsonConvert.DeserializeObject<List<StoryEvent>>(json) ?? new();
            return _floors;
        }

        /// <summary>
        /// ดึง floor เฉพาะหมายเลข
        /// </summary>
        public async Task<StoryEvent?> GetFloorAsync(int floorNumber)
        {
            var floors = await GetFloorsAsync();
            return floors.FirstOrDefault(f => f.FloorNumber == floorNumber);
        }

        /// <summary>
        /// โหลด Story Journals จาก story_journals.json (cache)
        /// </summary>
        public async Task<List<StoryJournalSeed>> GetStoryJournalsAsync()
        {
            if (_storyJournals != null) return _storyJournals;

            var json = await LoadRawFileAsync("story_journals.json");
            _storyJournals = JsonConvert.DeserializeObject<List<StoryJournalSeed>>(json) ?? new();
            return _storyJournals;
        }

        /// <summary>
        /// ดึง story journal seed จาก key
        /// </summary>
        public async Task<StoryJournalSeed?> GetStoryJournalByKeyAsync(string key)
        {
            var seeds = await GetStoryJournalsAsync();
            return seeds.FirstOrDefault(s => s.Key == key);
        }

        /// <summary>
        /// Seed master data Items เข้า database (ทำครั้งเดียวตอน fresh install)
        /// </summary>
        public async Task SeedItemsAsync()
        {
            var conn = await _databaseService.GetConnectionAsync();
            var existing = await conn.Table<Item>().CountAsync();
            if (existing > 0) return; // มี item อยู่แล้ว ข้าม

            var json = await LoadRawFileAsync("items.json");
            var items = JsonConvert.DeserializeObject<List<Item>>(json);
            if (items == null || items.Count == 0) return;

            await conn.InsertAllAsync(items);
        }

        /// <summary>
        /// เรียก seed ทุกอย่างที่จำเป็น (เรียกตอน startup)
        /// </summary>
        public async Task SeedAllAsync()
        {
            await _databaseService.InitializeAsync();
            await SeedItemsAsync();
        }
    }
}
