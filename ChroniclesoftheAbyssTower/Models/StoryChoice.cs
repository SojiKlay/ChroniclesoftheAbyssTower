namespace ChroniclesoftheAbyssTower.Models
{
    /// <summary>
    /// ตัวเลือกของผู้เล่นในแต่ละ Floor
    /// </summary>
    public class StoryChoice
    {
        // ข้อความตัวเลือก (เช่น "พังประตู", "ค้นหา Rune Key")
        public string Text { get; set; } = string.Empty;

        // ผลลัพธ์เป็นข้อความ (แสดงหลังเลือก)
        public string ResultText { get; set; } = string.Empty;
        public string? RepeatResultText { get; set; }

        // ============== Effects ==============
        public int HpDelta { get; set; } = 0;       // +/- HP
        public int GoldDelta { get; set; } = 0;     // +/- Gold
        public int ExpDelta { get; set; } = 0;      // +/- EXP

        // ชื่อ item ที่ได้รับเพิ่มเข้า inventory
        public string? ItemReward { get; set; }
        public int ItemRewardQuantity { get; set; } = 1;

        // ชื่อ item ที่ต้องใช้ (consume) ตอนเลือก choice นี้
        public string? RequiredItem { get; set; }
        public List<string>? RequiredItems { get; set; }

        // ชื่อ item ที่ต้องใช้แต่ไม่ consume (เช่น Boss Key)
        public string? RequiredItemNoConsume { get; set; }

        // ============== Conditional ==============
        // เลือกได้ก็ต่อเมื่อมี item นี้ (จะ disable ถ้าไม่มี)
        public string? RequiresItem { get; set; }
        public List<string>? RequiresItems { get; set; }

        // ถ้าผู้เล่นมี item นี้แล้ว จะล็อก choice นี้ไว้ (เช่น ค้นหากุญแจซ้ำไม่ได้)
        public string? BlockedByItem { get; set; }
        public List<string>? BlockedByItems { get; set; }

        // ============== Special Actions ==============
        // ถ้า true: เลื่อน floor +1 หลังเลือก (default = true)
        public bool AdvanceFloor { get; set; } = true;

        // จบเกมแบบไหน ("", "Good", "Bad")
        public string EndingType { get; set; } = string.Empty;
    }
}
