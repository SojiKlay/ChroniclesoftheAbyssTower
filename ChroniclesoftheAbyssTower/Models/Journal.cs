using SQLite;

namespace ChroniclesoftheAbyssTower.Models
{
    /// <summary>
    /// ประเภทของ Journal
    /// </summary>
    public enum JournalType
    {
        Story = 0,   // ระบบ unlock อัตโนมัติ - read-only
        Player = 1   // ผู้เล่นเขียนเอง - CRUD ครบ
    }

    /// <summary>
    /// บันทึก / Journal ของผู้เล่น
    /// แบ่งเป็น Story Journal (auto unlock) และ Player Journal (เขียนเอง)
    /// </summary>
    [Table("Journals")]
    public class Journal
    {
        [PrimaryKey, AutoIncrement]
        public int JournalId { get; set; }

        [Indexed, NotNull]
        public int PlayerId { get; set; }

        [NotNull]
        public JournalType JournalType { get; set; }

        // ชั้นที่ปลดล็อก/เขียน journal นี้ (สำหรับ filter)
        [Indexed]
        public int FloorNumber { get; set; }

        [MaxLength(50), NotNull]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500), NotNull]
        public string Content { get; set; } = string.Empty;

        // story journal มี ID อ้างอิงจาก seed data (เช่น "JR-001")
        // player journal เป็นค่าว่าง
        [MaxLength(20)]
        public string StoryKey { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
