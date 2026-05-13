using ChroniclesoftheAbyssTower.Helpers;
using ChroniclesoftheAbyssTower.Services;
using ChroniclesoftheAbyssTower.Views;

namespace ChroniclesoftheAbyssTower
{
    /// <summary>
    /// Shell หลักของแอป
    /// - ลงทะเบียน relative routes (พุชเข้า navigation stack ปกติ)
    /// - ทำ seed data ตอน startup
    /// - ตรวจ session - ถ้า login อยู่ redirect ไป main
    /// </summary>
    public partial class AppShell : Shell
    {
        private bool _initialized;

        public AppShell()
        {
            InitializeComponent();

            // ลงทะเบียน relative routes ทั้งหมด
            // Routes ที่ขึ้นต้นด้วย // (login/register/main) เป็น absolute - มี ShellContent อยู่ใน XAML แล้ว
            // Routes อื่น (character/inventory/...) เป็น relative ต้อง register ที่นี่
            RegisterRoutes();

            // ใช้ Loaded event แทน fire-forget เพื่อรอให้ Shell พร้อมก่อน redirect
            // ป้องกันปัญหาจอดำตอน restart (Shell ยังไม่ render → GoToAsync cancel)
            Loaded += OnShellLoaded;
        }

        private async void OnShellLoaded(object? sender, EventArgs e)
        {
            if (_initialized) return;
            _initialized = true;
            await InitializeAppAsync();
        }

        /// <summary>
        /// ลงทะเบียน relative routes กับ Shell
        /// เพิ่มเรื่อยๆ เมื่อสร้าง page ใหม่ (Phase 4-7)
        /// </summary>
        private static void RegisterRoutes()
        {
            // Phase 3
            Routing.RegisterRoute(AppConstants.RouteCharacter, typeof(CharacterPage));
            Routing.RegisterRoute(AppConstants.RouteSettings, typeof(SettingsPage));

            // Phase 4
            Routing.RegisterRoute(AppConstants.RouteIntroStory, typeof(IntroStoryPage));
            Routing.RegisterRoute(AppConstants.RouteStory, typeof(StoryPage));
            Routing.RegisterRoute(AppConstants.RouteEnding, typeof(EndingPage));

            // Phase 5
            Routing.RegisterRoute(AppConstants.RouteInventory, typeof(InventoryPage));
            Routing.RegisterRoute(AppConstants.RouteJournal, typeof(JournalPage));
            Routing.RegisterRoute(AppConstants.RouteJournalEditor, typeof(JournalEditorPage));

            // Phase 6
            Routing.RegisterRoute(AppConstants.RouteSaveLoad, typeof(SaveLoadPage));
            Routing.RegisterRoute(AppConstants.RouteBackup, typeof(BackupPage));
        }

        /// <summary>
        /// 1. Init database + seed master data (Items)
        /// 2. ถ้า user login อยู่ → redirect ไป //main
        /// 3. ถ้าไม่ login → อยู่ที่ //login (default)
        /// </summary>
        private async Task InitializeAppAsync()
        {
            try
            {
                // Resolve services จาก DI container
                if (App.Services is null) return;

                var seedService = App.Services.GetService<SeedDataService>();
                if (seedService != null)
                {
                    await seedService.SeedAllAsync();
                }

                // ตรวจ session — ถ้า login อยู่ redirect ไป main
                if (!await SessionManager.IsLoggedInAsync()) return;

                // Navigate ใน UI thread + retry ถ้า fail (Shell อาจยังไม่ ready 100%)
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    for (int attempt = 0; attempt < 3; attempt++)
                    {
                        try
                        {
                            await Task.Delay(150 * (attempt + 1));
                            await Shell.Current.GoToAsync(AppConstants.RouteMainMenu);
                            return;
                        }
                        catch (Exception navEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"[AppShell.Navigate retry {attempt}] {navEx.Message}");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AppShell.InitializeApp] {ex}");
            }
        }
    }
}

