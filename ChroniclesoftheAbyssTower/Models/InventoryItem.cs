using SQLite;

namespace ChroniclesoftheAbyssTower.Models
{
    /// <summary>
    /// Item ที่ผู้เล่นถืออยู่ในกระเป๋า (instance)
    /// ความสัมพันธ์: Player 1 - N InventoryItem - 1 Item
    /// </summary>
    [Table("InventoryItems")]
    public class InventoryItem
    {
        [PrimaryKey, AutoIncrement]
        public int InventoryId { get; set; }

        [Indexed, NotNull]
        public int PlayerId { get; set; }

        [Indexed, NotNull]
        public int ItemId { get; set; }

        public int Quantity { get; set; } = 1;

        public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;

        // เก็บข้อมูล item แบบ denormalize ไว้ที่นี่ด้วย เพื่อให้ load ครั้งเดียวจบ
        // (sqlite-net-pcl ไม่ทำ join อัตโนมัติ - join เองจะช้ากว่า)
        // จะถูก populate โดย InventoryService หลัง query
        [Ignore]
        public Item? ItemData { get; set; }

        /// <summary>
        /// ชื่อ item (สำหรับแสดงใน UI)
        /// </summary>
        [Ignore]
        public string DisplayName => !string.IsNullOrWhiteSpace(ItemData?.ThaiName)
            ? ItemData.ThaiName
            : ItemData?.ItemName ?? "Unknown Item";

        /// <summary>
        /// คำอธิบาย (สำหรับแสดงใน UI)
        /// </summary>
        [Ignore]
        public string DisplayDescription => ItemData?.Description ?? string.Empty;

        /// <summary>
        /// Icon (emoji หรือชื่อ Material Icon)
        /// </summary>
        [Ignore]
        public string DisplayIcon => ItemData?.IconKey ?? "📦";

        [Ignore]
        public string DisplayImage => ItemData?.ItemName switch
        {
            "Health Potion" => "health_potion.png",
            "Greater Potion" => "greater_potion.png",
            "Dark Water" => "dark_water.png",
            "Rune Key" => "rune_key.png",
            "Old Note" => "old_note.png",
            "Broken Pendant" => "broken_pendant.png",
            "Tower Map" => "tower_map.png",
            "Boss Key" => "boss_key.png",
            "Mist Crystal" => "mist_crystal.png",
            "Knight's Sword" => "knights_sword.png",
            "Iron Shield" => "iron_shield.png",
            "Phoenix Feather" => "phoenix_feather.png",
            _ => string.Empty
        };

        [Ignore]
        public bool HasDisplayImage => !string.IsNullOrWhiteSpace(DisplayImage);

        [Ignore]
        public bool HasNoDisplayImage => string.IsNullOrWhiteSpace(DisplayImage);

        /// <summary>
        /// ประเภทของ item (สำหรับ filter)
        /// </summary>
        [Ignore]
        public ItemType DisplayType => ItemData?.ItemType ?? ItemType.Consumable;
    }
}
