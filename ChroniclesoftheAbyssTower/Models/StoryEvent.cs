namespace ChroniclesoftheAbyssTower.Models
{
    /// <summary>
    /// เหตุการณ์เนื้อเรื่องของแต่ละ Floor
    /// โหลดจาก Floors.json ตอน startup (ไม่บันทึกใน DB)
    /// </summary>
    public class StoryEvent
    {
        public int FloorNumber { get; set; }

        // ชื่อ floor เช่น "The Iron Gate"
        public string Title { get; set; } = string.Empty;

        // ข้อความ event หลัก (เนื้อเรื่อง)
        public string Narrative { get; set; } = string.Empty;

        // category เช่น "Trap", "Boss", "Shop", "Story"
        public string EventType { get; set; } = "Story";

        // ตัวเลือก (โดยปกติ 3 ตัวเลือก)
        public List<StoryChoice> Choices { get; set; } = new();

        // story journal ที่ unlock เมื่อเข้า floor นี้ (key อ้าง JSON)
        public string? StoryJournalKey { get; set; }

        // สำหรับ Boss floor: HP/Attack ของ Boss (optional)
        public int? BossHp { get; set; }
        public int? BossAttack { get; set; }
        public string? BossName { get; set; }

        // emoji หรือ icon สำหรับ floor
        public string Icon { get; set; } = "🗝️";

        // เป็น final floor หรือไม่
        public bool IsFinalFloor { get; set; } = false;
    }
}
