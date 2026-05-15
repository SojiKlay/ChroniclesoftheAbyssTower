using Plugin.Maui.Audio;

namespace ChroniclesoftheAbyssTower.Services
{
    /// <summary>
    /// Service สำหรับเล่น BGM แบบ 2 layer: เพลงหลัก + เสียงเสริม
    /// </summary>
    public class AudioService
    {
        private readonly IAudioManager _audioManager;
        private readonly SettingsService _settingsService;
        private IAudioPlayer? _mainBgmPlayer;
        private IAudioPlayer? _layerBgmPlayer;
        private string? _requestedMainBgm;
        private string? _requestedLayerBgm;
        private string? _currentMainBgm;
        private string? _currentLayerBgm;

        public AudioService(IAudioManager audioManager, SettingsService settingsService)
        {
            _audioManager = audioManager;
            _settingsService = settingsService;
        }

        public async Task PlayBgmAsync(string mainFile, string? layerFile = null)
        {
            _requestedMainBgm = mainFile;
            _requestedLayerBgm = layerFile;

            if (!_settingsService.IsBgmEnabled)
            {
                StopPlayers(clearRequestedFiles: false);
                return;
            }

            await PlayMainBgmAsync(mainFile);

            if (string.IsNullOrWhiteSpace(layerFile))
            {
                StopLayerBgm();
                return;
            }

            await PlayLayerBgmAsync(layerFile);
        }

        public void StopBgm()
        {
            StopPlayers(clearRequestedFiles: true);
        }

        public async Task ApplySettingsAsync()
        {
            if (!_settingsService.IsBgmEnabled)
            {
                StopPlayers(clearRequestedFiles: false);
                return;
            }

            if (_mainBgmPlayer == null && !string.IsNullOrWhiteSpace(_requestedMainBgm))
            {
                await PlayBgmAsync(_requestedMainBgm, _requestedLayerBgm);
                return;
            }

            ApplyCurrentVolumes();
        }

        private async Task PlayMainBgmAsync(string fileName)
        {
            if (_currentMainBgm == fileName && _mainBgmPlayer != null)
            {
                ApplyCurrentVolumes();
                return;
            }

            StopMainBgm();

            var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
            _mainBgmPlayer = _audioManager.CreatePlayer(stream);
            _mainBgmPlayer.Loop = true;
            _mainBgmPlayer.Volume = _settingsService.BgmVolume;
            _mainBgmPlayer.Play();
            _currentMainBgm = fileName;
        }

        private async Task PlayLayerBgmAsync(string fileName)
        {
            if (_currentLayerBgm == fileName && _layerBgmPlayer != null)
            {
                ApplyCurrentVolumes();
                return;
            }

            StopLayerBgm();

            var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
            _layerBgmPlayer = _audioManager.CreatePlayer(stream);
            _layerBgmPlayer.Loop = true;
            _layerBgmPlayer.Volume = _settingsService.BgmVolume * 0.35;
            _layerBgmPlayer.Play();
            _currentLayerBgm = fileName;
        }

        private void ApplyCurrentVolumes()
        {
            if (_mainBgmPlayer != null)
                _mainBgmPlayer.Volume = _settingsService.BgmVolume;

            if (_layerBgmPlayer != null)
                _layerBgmPlayer.Volume = _settingsService.BgmVolume * 0.35;
        }

        private void StopPlayers(bool clearRequestedFiles)
        {
            StopMainBgm();
            StopLayerBgm();

            if (!clearRequestedFiles)
                return;

            _requestedMainBgm = null;
            _requestedLayerBgm = null;
        }

        private void StopMainBgm()
        {
            _mainBgmPlayer?.Stop();
            _mainBgmPlayer?.Dispose();
            _mainBgmPlayer = null;
            _currentMainBgm = null;
        }

        private void StopLayerBgm()
        {
            _layerBgmPlayer?.Stop();
            _layerBgmPlayer?.Dispose();
            _layerBgmPlayer = null;
            _currentLayerBgm = null;
        }
    }
}
