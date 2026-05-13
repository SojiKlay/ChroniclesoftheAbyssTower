using ChroniclesoftheAbyssTower.Services;
using ChroniclesoftheAbyssTower.ViewModels;
using ChroniclesoftheAbyssTower.Views;

namespace ChroniclesoftheAbyssTower
{
    /// <summary>
    /// ลงทะเบียน Services, ViewModels, Pages เข้า DI Container
    /// แยกออกจาก MauiProgram.cs เพื่อให้อ่านง่าย และใน Phase ต่อๆ ไปแค่เพิ่มในไฟล์นี้
    /// </summary>
    public static partial class MauiProgram
    {
        /// <summary>
        /// ลงทะเบียน Service ทั้งหมด (singleton เป็นหลัก เพราะส่วนใหญ่ไม่มี state ที่ต้องแยกต่อ request)
        /// </summary>
        public static void RegisterServices(this MauiAppBuilder builder)
        {
            // Phase 0: Validation - stateless
            builder.Services.AddSingleton<ValidationService>();

            // Phase 1: Database + Seed
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<SeedDataService>();

            // Phase 2: Auth
            builder.Services.AddSingleton<AuthService>();

            // Phase 3: Player + Settings
            builder.Services.AddSingleton<PlayerService>();
            builder.Services.AddSingleton<SettingsService>();

            // Phase 4: Inventory + Journal + Story (data layer)
            builder.Services.AddSingleton<InventoryService>();
            builder.Services.AddSingleton<JournalService>();
            builder.Services.AddSingleton<StoryService>();

            // Phase 6: Save / Load / Backup
            builder.Services.AddSingleton<SaveLoadService>();
            builder.Services.AddSingleton<BackupService>();

            // หมายเหตุ: CloudBackup / Audio services จะเพิ่มใน Phase 7 เมื่อใส่ package กลับ
        }

        /// <summary>
        /// ลงทะเบียน ViewModel ทั้งหมด
        /// ใช้ Transient เพื่อให้ทุกหน้าได้ instance ใหม่ทุกครั้งที่ navigate
        /// </summary>
        public static void RegisterViewModels(this MauiAppBuilder builder)
        {
            // Phase 2: Auth ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();

            // Phase 3: Main + Character + Settings ViewModels
            builder.Services.AddTransient<MainMenuViewModel>();
            builder.Services.AddTransient<CharacterViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();

            // Phase 4: Story ViewModels
            builder.Services.AddTransient<IntroStoryViewModel>();
            builder.Services.AddTransient<StoryViewModel>();
            builder.Services.AddTransient<EndingViewModel>();

            // Phase 5: Inventory + Journal ViewModels
            builder.Services.AddTransient<InventoryViewModel>();
            builder.Services.AddTransient<JournalViewModel>();
            builder.Services.AddTransient<JournalEditorViewModel>();

            // Phase 6: Save/Load + Backup ViewModels
            builder.Services.AddTransient<SaveLoadViewModel>();
            builder.Services.AddTransient<BackupViewModel>();
        }

        /// <summary>
        /// ลงทะเบียน Page ทั้งหมด
        /// ใช้ Transient เช่นกัน
        /// </summary>
        public static void RegisterPages(this MauiAppBuilder builder)
        {
            // Phase 2: Auth Pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();

            // Phase 3: Main + Character + Settings Pages
            builder.Services.AddTransient<MainMenuPage>();
            builder.Services.AddTransient<CharacterPage>();
            builder.Services.AddTransient<SettingsPage>();

            // Phase 4: Story Pages
            builder.Services.AddTransient<IntroStoryPage>();
            builder.Services.AddTransient<StoryPage>();
            builder.Services.AddTransient<EndingPage>();

            // Phase 5: Inventory + Journal Pages
            builder.Services.AddTransient<InventoryPage>();
            builder.Services.AddTransient<JournalPage>();
            builder.Services.AddTransient<JournalEditorPage>();

            // Phase 6: Save/Load + Backup Pages
            builder.Services.AddTransient<SaveLoadPage>();
            builder.Services.AddTransient<BackupPage>();
        }
    }
}

