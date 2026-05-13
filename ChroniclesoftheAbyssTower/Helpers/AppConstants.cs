namespace ChroniclesoftheAbyssTower.Helpers
{
    /// <summary>
    /// ค่าคงที่ที่ใช้ทั่วทั้งแอปพลิเคชัน
    /// </summary>
    public static class AppConstants
    {
        // ============== Database ==============
        public const string DatabaseFileName = "abyss_tower.db3";

        // ============== Game Balance ==============
        public const int StartingHp = 100;
        public const int StartingMaxHp = 100;
        public const int StartingAttack = 10;
        public const int StartingDefense = 5;
        public const int StartingGold = 50;
        public const int StartingLevel = 1;
        public const int StartingFloor = 1;

        // จำนวน floor ทั้งหมด
        public const int TotalFloors = 30;

        // จำนวน save slot สูงสุด
        public const int MaxSaveSlots = 3;

        // จำนวน item ใน inventory สูงสุด (ป้องกัน inventory ล้น)
        public const int MaxInventorySize = 30;

        // ============== Routes (สำหรับ Shell.Current.GoToAsync) ==============
        public const string RouteLogin = "//login";
        public const string RouteRegister = "//register";
        public const string RouteMainMenu = "//main";
        public const string RouteIntroStory = "introStory";
        public const string RouteStory = "story";
        public const string RouteCharacter = "character";
        public const string RouteInventory = "inventory";
        public const string RouteJournal = "journal";
        public const string RouteJournalEditor = "journalEditor";
        public const string RouteSaveLoad = "saveLoad";
        public const string RouteBackup = "backup";
        public const string RouteSettings = "settings";
        public const string RouteEnding = "ending";

        // ============== Backup ==============
        public const string BackupFolderName = "AbyssTowerBackups";
        public const string BackupFileExtension = ".abysstower.json";
        public const string BackupSchemaVersion = "1.0";

        // ============== Settings Keys (Preferences) ==============
        public const string PrefBgmVolume = "pref_bgm_volume";
        public const string PrefSfxVolume = "pref_sfx_volume";
        public const string PrefBgmEnabled = "pref_bgm_enabled";
        public const string PrefSfxEnabled = "pref_sfx_enabled";
    }
}
