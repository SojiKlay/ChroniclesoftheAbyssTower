using SQLite;

namespace ChroniclesoftheAbyssTower.Models
{
    /// <summary>
    /// ตัวละครของผู้เล่น (Arin)
    /// แต่ละ user สามารถมี player ได้หลายตัว (เผื่อ replay)
    /// </summary>
    [Table("Players")]
    public class Player
    {
        [PrimaryKey, AutoIncrement]
        public int PlayerId { get; set; }

        // อ้างอิงถึง User ที่สร้างตัวละครนี้
        [Indexed, NotNull]
        public int UserId { get; set; }

        [MaxLength(20), NotNull]
        public string PlayerName { get; set; } = "Arin";

        // ============== Stats ==============
        public int Hp { get; set; } = 100;
        public int MaxHp { get; set; } = 100;
        public int Attack { get; set; } = 10;
        public int Defense { get; set; } = 5;
        public int Gold { get; set; } = 50;
        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;

        // ============== Progression ==============
        public int CurrentFloor { get; set; } = 1;

        // floor ที่ผ่านสูงสุด (ใช้แสดงใน character page)
        public int HighestFloor { get; set; } = 1;

        // จำนวน choice ทั้งหมดที่ผู้เล่นเลือก (achievement / stats)
        public int TotalChoicesMade { get; set; } = 0;

        // ============== Status ==============
        // เกมจบแล้วหรือยัง (HP=0 = Bad Ending, ผ่าน Floor 30 = Good Ending)
        public bool IsGameCompleted { get; set; } = false;
        public string EndingType { get; set; } = string.Empty; // "Good", "Bad", หรือว่าง

        // ============== Timestamps ==============
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// อัตราส่วน HP ปัจจุบัน (0.0 - 1.0) สำหรับใช้ใน progress bar
        /// </summary>
        [Ignore]
        public double HpRatio => MaxHp > 0 ? (double)Hp / MaxHp : 0.0;

        /// <summary>
        /// ตรวจว่าตัวละครยังมีชีวิตอยู่ไหม
        /// </summary>
        [Ignore]
        public bool IsAlive => Hp > 0;
    }
}
