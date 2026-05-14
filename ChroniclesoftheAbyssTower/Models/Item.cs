using SQLite;

namespace ChroniclesoftheAbyssTower.Models
{
    /// <summary>
    /// ประเภทของ Item (ตาม ITEM_SYSTEM.md)
    /// </summary>
    public enum ItemType
    {
        Healing = 0,    // ฟื้น HP
        Story = 1,      // ใช้ในเนื้อเรื่อง
        Key = 2,        // ใช้เปิดทาง
        Currency = 3,   // เงิน (จริงๆ Gold เก็บแยกใน Player แต่เผื่อขยาย)
        Consumable = 4  // อื่นๆ ที่ใช้แล้วหมด
    }

    /// <summary>
    /// Master data ของ Item ทุกชนิดในเกม
    /// (เก็บข้อมูล static ของ item เช่น Health Potion = HP+20)
    /// </summary>
    [Table("Items")]
    public class Item
    {
        [PrimaryKey, AutoIncrement]
        public int ItemId { get; set; }

        [Indexed(Unique = true), MaxLength(50), NotNull]
        public string ItemName { get; set; } = string.Empty;

        [MaxLength(80)]
        public string ThaiName { get; set; } = string.Empty;

        [NotNull]
        public ItemType ItemType { get; set; }

        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        // ค่า effect (เช่น Health Potion = 20 → HP+20)
        public int EffectValue { get; set; } = 0;

        // icon emoji หรือชื่อ Material Icon ใช้แสดงใน UI
        [MaxLength(20)]
        public string IconKey { get; set; } = "📦";

        // ราคาใน Shop (0 = ขายไม่ได้)
        public int ShopPrice { get; set; } = 0;

        // ใช้แล้วหายไปหรือไม่ (Story/Key item ส่วนใหญ่ไม่หาย)
        public bool IsConsumable { get; set; } = true;
    }
}
