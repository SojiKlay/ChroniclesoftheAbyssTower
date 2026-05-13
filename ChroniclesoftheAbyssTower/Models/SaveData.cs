using SQLite;

namespace ChroniclesoftheAbyssTower.Models
{
    /// <summary>
    /// บันทึกการเล่น (Save Slot)
    /// แต่ละ slot เก็บ snapshot ของ player + inventory + journals + progress เป็น JSON
    /// </summary>
    [Table("SaveData")]
    public class SaveData
    {
        [PrimaryKey, AutoIncrement]
        public int SaveId { get; set; }

        [Indexed, NotNull]
        public int UserId { get; set; }

        // หมายเลข slot (1, 2, 3) - 1 user จะมี save ได้ 3 slot
        [NotNull]
        public int SaveSlot { get; set; }

        // ชื่อ save (อ่านง่าย เช่น "Floor 5 - Tutorial")
        [MaxLength(100)]
        public string SaveName { get; set; } = string.Empty;

        // เวลาที่ save (UTC)
        [NotNull]
        public DateTime SaveDate { get; set; } = DateTime.UtcNow;

        // ===== Snapshot ข้อมูลทั้งหมด (เก็บเป็น JSON) =====
        // Player JSON
        [NotNull]
        public string PlayerSnapshot { get; set; } = "{}";

        // List<InventoryItem> JSON
        [NotNull]
        public string InventorySnapshot { get; set; } = "[]";

        // List<Journal> JSON
        [NotNull]
        public string JournalSnapshot { get; set; } = "[]";

        // List<StoryProgress> JSON
        [NotNull]
        public string ProgressSnapshot { get; set; } = "[]";

        // ===== Quick Display Fields (denormalized สำหรับแสดงใน save list) =====
        [MaxLength(20)]
        public string PlayerName { get; set; } = string.Empty;

        public int CurrentFloor { get; set; }

        public int Hp { get; set; }

        public int Level { get; set; }

        public int PlayTimeSeconds { get; set; } = 0;
    }
}
