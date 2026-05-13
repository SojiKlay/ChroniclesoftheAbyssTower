namespace ChroniclesoftheAbyssTower.Helpers
{
    /// <summary>
    /// Extension methods สำหรับ animation ที่ใช้ใน MAUI built-in API
    /// (ไม่ต้องพึ่ง 3rd-party library — Plugin.Maui.Audio / Lottie ถูกตัดออก)
    /// </summary>
    public static class AnimationHelper
    {
        /// <summary>
        /// Fade-in element จาก opacity 0 → 1
        /// </summary>
        public static async Task FadeInAsync(this VisualElement element, uint duration = 350)
        {
            element.Opacity = 0;
            await element.FadeTo(1, duration, Easing.CubicOut);
        }

        /// <summary>
        /// Fade-in + slide up เล็กน้อย (entrance ที่ดูมีชีวิตขึ้น)
        /// </summary>
        public static async Task FadeInSlideUpAsync(this VisualElement element, uint duration = 400, double offset = 16)
        {
            element.Opacity = 0;
            element.TranslationY = offset;
            await Task.WhenAll(
                element.FadeTo(1, duration, Easing.CubicOut),
                element.TranslateTo(0, 0, duration, Easing.CubicOut)
            );
        }

        /// <summary>
        /// Scale-in (ปรากฏแบบขยายเล็กน้อย → ปกติ)
        /// </summary>
        public static async Task ScaleInAsync(this VisualElement element, uint duration = 300, double fromScale = 0.92)
        {
            element.Opacity = 0;
            element.Scale = fromScale;
            await Task.WhenAll(
                element.FadeTo(1, duration, Easing.CubicOut),
                element.ScaleTo(1, duration, Easing.CubicOut)
            );
        }

        /// <summary>
        /// Pulse (เด้ง 1 ครั้ง — ใช้กับ feedback ตอนคลิกหรือเตือน)
        /// </summary>
        public static async Task PulseAsync(this VisualElement element, uint duration = 200, double scale = 1.05)
        {
            await element.ScaleTo(scale, duration / 2, Easing.CubicOut);
            await element.ScaleTo(1, duration / 2, Easing.CubicIn);
        }

        /// <summary>
        /// Shake (สั่นเตือน — ใช้กับ error)
        /// </summary>
        public static async Task ShakeAsync(this VisualElement element, uint duration = 300, double offset = 8)
        {
            var quarter = (uint)(duration / 4);
            await element.TranslateTo(-offset, 0, quarter, Easing.Linear);
            await element.TranslateTo(offset, 0, quarter, Easing.Linear);
            await element.TranslateTo(-offset / 2, 0, quarter, Easing.Linear);
            await element.TranslateTo(0, 0, quarter, Easing.Linear);
        }
    }
}
