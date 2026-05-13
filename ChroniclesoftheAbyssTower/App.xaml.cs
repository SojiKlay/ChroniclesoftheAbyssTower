using Microsoft.Extensions.DependencyInjection;

namespace ChroniclesoftheAbyssTower
{
    /// <summary>
    /// Entry point ของแอป
    /// ติดตั้ง global exception handler ตรงนี้เพื่อจับ error ทั้งหมดของแอป
    /// </summary>
    public partial class App : Application
    {
        // เก็บ ServiceProvider เพื่อให้ส่วนอื่นเรียกใช้ได้แบบ static (กรณีจำเป็น)
        public static IServiceProvider? Services { get; private set; }

        public App(IServiceProvider services)
        {
            InitializeComponent();
            Services = services;

            // ติดตั้ง Global Exception Handlers
            // ไม่ให้ user เห็น crash โดยตรง - log ไว้ก่อนแล้วแสดง dialog (ในอนาคต)
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell())
            {
                Title = "Chronicles of the Abyss Tower"
            };
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // log ผ่าน System.Diagnostics
            var ex = e.ExceptionObject as Exception;
            System.Diagnostics.Debug.WriteLine($"[FATAL] {ex?.Message}\n{ex?.StackTrace}");
        }

        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[TASK ERROR] {e.Exception.Message}\n{e.Exception.StackTrace}");
            e.SetObserved();
        }
    }
}
