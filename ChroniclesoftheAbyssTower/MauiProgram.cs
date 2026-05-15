using Microsoft.Extensions.Logging;

using Plugin.Maui.Audio;

namespace ChroniclesoftheAbyssTower
{
    /// <summary>
    /// จุดเริ่มต้นของแอป MAUI
    /// ตั้งค่า DI Container, Fonts, Plugins ที่นี่
    /// Service / ViewModel / Page ถูกลงทะเบียนใน partial class
    /// MauiProgram.Registrations.cs
    /// </summary>
    public static partial class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .AddAudio()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // หมายเหตุ: UseMauiCommunityToolkit / UseSkiaSharp / AudioManager
            // ถูกถอดออกชั่วคราวเพื่อรอ packages ที่รองรับ MAUI 10 ใน Phase 6-7

            // ลงทะเบียน Services / ViewModels / Pages (ดู MauiProgram.Registrations.cs)
            builder.RegisterServices();
            builder.RegisterViewModels();
            builder.RegisterPages();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

