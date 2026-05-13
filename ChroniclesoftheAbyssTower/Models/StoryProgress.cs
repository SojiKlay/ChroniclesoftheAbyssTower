using SQLite;

namespace ChroniclesoftheAbyssTower.Models
{
    /// <summary>
    /// บันทึกความคืบหน้าของเนื้อเรื่อง
    /// 1 row ต่อ 1 floor ต่อ 1 player ที่ผู้เล่นเข้าไปเล่น
    /// </summary>
    [Table("StoryProgress")]
    public class StoryProgress
    {
        [PrimaryKey, AutoIncrement]
        public int ProgressId { get; set; }

        [Indexed, NotNull]
        public int PlayerId { get; set; }

        [NotNull]
        public int FloorNumber { get; set; }

        // ผ่าน floor นี้แล้วหรือไม่
        public bool IsCompleted { get; set; } = false;

        // index ของ choice ที่เลือก (0, 1, 2 = ตัวเลือกที่ 1, 2, 3)
        public int ChoiceMade { get; set; } = -1;

        // ข้อความ choice ที่เลือก (denormalized สำหรับแสดง history)
        [MaxLength(255)]
        public string ChoiceText { get; set; } = string.Empty;

        // เวลาที่ผ่าน
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    }
}
