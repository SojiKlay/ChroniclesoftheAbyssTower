namespace ChroniclesoftheAbyssTower.Models
{
    /// <summary>
    /// DTO สำหรับ Export/Import Backup (JSON)
    /// บรรจุข้อมูลทุก table ของ user คนนี้
    /// </summary>
    public class BackupData
    {
        // version ของ schema เพื่อรองรับการ migrate ในอนาคต
        public string SchemaVersion { get; set; } = "1.0";

        // เวลาที่สร้าง backup
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ชื่อแอป (เผื่อใช้ตรวจสอบไฟล์)
        public string AppName { get; set; } = "ChroniclesoftheAbyssTower";

        // ข้อมูลผู้ใช้ (รวม password hash + salt - ยังเข้ารหัสอยู่)
        public User? User { get; set; }

        // ตัวละครทั้งหมดของ user คนนี้
        public List<Player> Players { get; set; } = new();

        // Inventory ของทุก player
        public List<InventoryItem> InventoryItems { get; set; } = new();

        // Journal ของทุก player
        public List<Journal> Journals { get; set; } = new();

        // Save slot ของ user
        public List<SaveData> SaveData { get; set; } = new();

        // Story progress
        public List<StoryProgress> StoryProgress { get; set; } = new();
    }
}
