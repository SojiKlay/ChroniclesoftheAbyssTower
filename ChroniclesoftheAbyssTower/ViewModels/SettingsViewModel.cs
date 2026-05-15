using ChroniclesoftheAbyssTower.Helpers;
using ChroniclesoftheAbyssTower.Services;
using ChroniclesoftheAbyssTower.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChroniclesoftheAbyssTower.ViewModels
{
    /// <summary>
    /// ViewModel สำหรับหน้า Settings (BGM/SFX volume + about)
    /// </summary>
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly SettingsService _settingsService;
        private readonly AudioService _audioService;
        private bool _isLoadingSettings;

        // ============== BGM ==============
        [ObservableProperty]
        private bool isBgmEnabled;

        [ObservableProperty]
        private double bgmVolume;

        [ObservableProperty]
        private string bgmVolumeDisplay = "0%";

        // ============== SFX ==============
        [ObservableProperty]
        private bool isSfxEnabled;

        [ObservableProperty]
        private double sfxVolume;

        [ObservableProperty]
        private string sfxVolumeDisplay = "0%";

        // ============== About ==============
        public string AppVersion => "1.0.0";
        public string AppName => "Chronicles of the Abyss Tower";

        public SettingsViewModel(SettingsService settingsService, AudioService audioService)
        {
            _settingsService = settingsService;
            _audioService = audioService;
            Title = "ตั้งค่า";
            LoadSettings();
        }

        private void LoadSettings()
        {
            _isLoadingSettings = true;
            IsBgmEnabled = _settingsService.IsBgmEnabled;
            BgmVolume = _settingsService.BgmVolume;
            BgmVolumeDisplay = $"{(int)(BgmVolume * 100)}%";

            IsSfxEnabled = _settingsService.IsSfxEnabled;
            SfxVolume = _settingsService.SfxVolume;
            SfxVolumeDisplay = $"{(int)(SfxVolume * 100)}%";
            _isLoadingSettings = false;
        }

        // ============== Auto-save ผ่าน partial methods ==============

        partial void OnIsBgmEnabledChanged(bool value)
        {
            _settingsService.IsBgmEnabled = value;
            ApplyAudioSettings();
        }

        partial void OnBgmVolumeChanged(double value)
        {
            _settingsService.BgmVolume = value;
            BgmVolumeDisplay = $"{(int)(value * 100)}%";
            ApplyAudioSettings();
        }

        partial void OnIsSfxEnabledChanged(bool value)
        {
            _settingsService.IsSfxEnabled = value;
        }

        partial void OnSfxVolumeChanged(double value)
        {
            _settingsService.SfxVolume = value;
            SfxVolumeDisplay = $"{(int)(value * 100)}%";
        }

        // ============== Commands ==============

        /// <summary>
        /// คืนค่าเริ่มต้นทั้งหมด
        /// </summary>
        [RelayCommand]
        private async Task ResetDefaultsAsync()
        {
            var confirm = await Shell.Current.DisplayAlert(
                "คืนค่าเริ่มต้น",
                "ต้องการคืนค่าตั้งค่าทั้งหมดเป็นค่าเริ่มต้นหรือไม่?",
                "คืนค่า", "ยกเลิก");
            if (!confirm) return;

            _settingsService.ResetToDefaults();
            LoadSettings();
            await _audioService.ApplySettingsAsync();
        }

        /// <summary>
        /// กลับเมนูหลัก
        /// </summary>
        [RelayCommand]
        private async Task BackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        private void ApplyAudioSettings()
        {
            if (_isLoadingSettings)
                return;

            _ = _audioService.ApplySettingsAsync();
        }
    }
}
