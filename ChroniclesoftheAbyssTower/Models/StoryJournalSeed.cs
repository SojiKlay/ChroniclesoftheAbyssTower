namespace ChroniclesoftheAbyssTower.Models
{
    /// <summary>
    /// Story Journal seed data (โหลดจาก StoryJournals.json)
    /// จะถูกแปลงเป็น Journal ตอน unlock
    /// </summary>
    public class StoryJournalSeed
    {
        // คีย์เช่น "JR-001"
        public string Key { get; set; } = string.Empty;
        public int FloorNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
