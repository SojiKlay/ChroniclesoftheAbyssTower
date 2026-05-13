using ChroniclesoftheAbyssTower.Helpers;

namespace ChroniclesoftheAbyssTower.Services
{
    /// <summary>
    /// Service จัดการการตั้งค่าของแอป (BGM/SFX volume, ฯลฯ)
    /// เก็บใน Preferences API ของ MAUI (key-value store ใน internal storage)
    /// </summary>
    public class SettingsService
    {
        // ============== BGM ==============

        /// <summary>
        /// เปิด/ปิด BGM
        /// </summary>
        public bool IsBgmEnabled
        {
            get => Preferences.Default.Get(AppConstants.PrefBgmEnabled, true);
            set => Preferences.Default.Set(AppConstants.PrefBgmEnabled, value);
        }

        /// <summary>
        /// ระดับความดัง BGM (0.0 - 1.0)
        /// </summary>
        public double BgmVolume
        {
            get => Preferences.Default.Get(AppConstants.PrefBgmVolume, 0.6);
            set => Preferences.Default.Set(AppConstants.PrefBgmVolume, Math.Clamp(value, 0.0, 1.0));
        }

        // ============== SFX ==============

        /// <summary>
        /// เปิด/ปิด SFX
        /// </summary>
        public bool IsSfxEnabled
        {
            get => Preferences.Default.Get(AppConstants.PrefSfxEnabled, true);
            set => Preferences.Default.Set(AppConstants.PrefSfxEnabled, value);
        }

        /// <summary>
        /// ระดับความดัง SFX (0.0 - 1.0)
        /// </summary>
        public double SfxVolume
        {
            get => Preferences.Default.Get(AppConstants.PrefSfxVolume, 0.8);
            set => Preferences.Default.Set(AppConstants.PrefSfxVolume, Math.Clamp(value, 0.0, 1.0));
        }

        /// <summary>
        /// คืนค่า default ทั้งหมด
        /// </summary>
        public void ResetToDefaults()
        {
            IsBgmEnabled = true;
            BgmVolume = 0.6;
            IsSfxEnabled = true;
            SfxVolume = 0.8;
        }
    }
}
