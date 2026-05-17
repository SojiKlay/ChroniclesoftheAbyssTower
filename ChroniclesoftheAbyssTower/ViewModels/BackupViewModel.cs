using System.Collections.ObjectModel;
using ChroniclesoftheAbyssTower.Helpers;
using ChroniclesoftheAbyssTower.Services;
using ChroniclesoftheAbyssTower.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChroniclesoftheAbyssTower.ViewModels
{
    /// <summary>
    /// ViewModel ของหน้า Backup (Phase 6)
    /// - Export: เขียนไฟล์ JSON เก็บใน FileSystem.AppDataDirectory/AbyssTowerBackups/
    /// - Restore: เลือกไฟล์ + restore ข้อมูลเข้า DB
    /// - แสดงรายการ backup ที่มีในเครื่อง
    /// </summary>
    public partial class BackupViewModel : BaseViewModel
    {
        private readonly BackupService _backupService;

        public BackupViewModel(BackupService backupService)
        {
            _backupService = backupService;
            Title = "สำรองข้อมูล";
        }

        // ============== State ==============

        public ObservableCollection<BackupFileVm> BackupFiles { get; } = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasNoBackups))]
        private int backupCount;

        public bool HasNoBackups => BackupCount == 0;

        [ObservableProperty] private string folderPath = string.Empty;

        // ============== Lifecycle ==============

        public Task OnAppearingAsync()
        {
            FolderPath = _backupService.BackupFolderPath;
            ReloadFiles();
            return Task.CompletedTask;
        }

        private void ReloadFiles()
        {
            var files = _backupService.GetBackupFiles();

            BackupFiles.Clear();
            foreach (var f in files)
            {
                var info = new FileInfo(f);
                BackupFiles.Add(new BackupFileVm
                {
                    FullPath = f,
                    FileName = Path.GetFileName(f),
                    SizeDisplay = FormatFileSize(info.Length),
                    DateDisplay = info.LastWriteTime.ToString("dd/MM/yyyy HH:mm"),
                });
            }
            BackupCount = files.Count;
        }

        private static string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} ไบต์";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:0.0} กิโลไบต์";
            return $"{bytes / (1024.0 * 1024.0):0.00} เมกะไบต์";
        }

        // ============== Commands ==============

        [RelayCommand]
        private async Task ExportAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                ClearError();

                var userId = await SessionManager.GetUserIdAsync();
                if (!userId.HasValue)
                {
                    SetError("กรุณาเข้าสู่ระบบก่อน");
                    return;
                }

                var path = await _backupService.ExportToFileAsync(userId.Value);
                ReloadFiles();

                await Shell.Current.DisplayAlert(
                    "✅ สำรองสำเร็จ",
                    $"บันทึกไฟล์ที่:\n{path}\n\nไฟล์อยู่ในโฟลเดอร์ภายในของแอป สามารถ copy ออกผ่าน File Manager เพื่อย้ายไป cloud ได้",
                    "ตกลง");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BackupVM.Export] {ex}");
                SetError("สำรองไม่สำเร็จ: " + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task RestoreAsync(BackupFileVm? file)
        {
            if (file == null || IsBusy) return;

            // Preview ก่อน restore
            var preview = await _backupService.PreviewFileAsync(file.FullPath);
            if (preview == null)
            {
                await Shell.Current.DisplayAlert("ไฟล์เสียหาย", "ไม่สามารถอ่านไฟล์ข้อมูลสำรองได้", "ปิด");
                return;
            }

            var info = $"โครงสร้างข้อมูล: {preview.SchemaVersion}\n" +
                       $"สร้างเมื่อ: {preview.CreatedAt.ToLocalTime():dd/MM/yyyy HH:mm}\n" +
                       $"ตัวละคร: {preview.Players.Count}\n" +
                       $"ไอเท็ม: {preview.InventoryItems.Count}\n" +
                       $"บันทึก: {preview.Journals.Count}\n" +
                       $"ช่องเซฟ: {preview.SaveData.Count}\n\n" +
                       "**คำเตือน**: ข้อมูลปัจจุบันของผู้ใช้นี้จะถูกแทนที่ทั้งหมด ดำเนินการต่อหรือไม่?";

            var confirm = await Shell.Current.DisplayAlert("กู้คืนข้อมูลสำรอง?", info, "กู้คืน", "ยกเลิก");
            if (!confirm) return;

            try
            {
                IsBusy = true;
                var userId = await SessionManager.GetUserIdAsync();
                if (!userId.HasValue) return;

                await _backupService.RestoreAsync(userId.Value, preview);
                await Shell.Current.DisplayAlert("✅ กู้คืนสำเร็จ", "ข้อมูลถูกกู้คืนเรียบร้อย กรุณากลับไปเริ่มเล่นต่อจากเมนูหลัก", "ตกลง");
                await Shell.Current.GoToAsync(AppConstants.RouteMainMenu);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BackupVM.Restore] {ex}");
                await Shell.Current.DisplayAlert("ผิดพลาด", "กู้คืนไม่สำเร็จ: " + ex.Message, "ปิด");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteFileAsync(BackupFileVm? file)
        {
            if (file == null) return;

            var confirm = await Shell.Current.DisplayAlert(
                "ลบไฟล์ข้อมูลสำรอง?",
                $"ต้องการลบ '{file.FileName}' ถาวรหรือไม่?",
                "ลบ", "ยกเลิก");
            if (!confirm) return;

            try
            {
                _backupService.DeleteBackupFile(file.FullPath);
                ReloadFiles();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BackupVM.Delete] {ex}");
            }
        }

        [RelayCommand]
        private async Task ShareFileAsync(BackupFileVm? file)
        {
            if (file == null) return;
            try
            {
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "แชร์ข้อมูลสำรองหอคอยอเวจี",
                    File = new ShareFile(file.FullPath)
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BackupVM.Share] {ex}");
                await Shell.Current.DisplayAlert("ผิดพลาด", "ไม่สามารถแชร์ได้: " + ex.Message, "ปิด");
            }
        }

        [RelayCommand]
        private async Task ImportFromFileAsync()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "เลือกไฟล์ข้อมูลสำรอง",
                });
                if (result == null) return;

                // Copy ไฟล์ไปที่ folder ของเรา
                Directory.CreateDirectory(_backupService.BackupFolderPath);
                var dest = Path.Combine(_backupService.BackupFolderPath, result.FileName);
                using (var src = await result.OpenReadAsync())
                using (var fs = File.Create(dest))
                {
                    await src.CopyToAsync(fs);
                }
                ReloadFiles();
                await Shell.Current.DisplayAlert("นำเข้าสำเร็จ", "ไฟล์ถูกนำเข้ารายการข้อมูลสำรองแล้ว แตะ 'กู้คืน' เพื่อใช้งาน", "ตกลง");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BackupVM.Import] {ex}");
                await Shell.Current.DisplayAlert("ผิดพลาด", "นำเข้าไฟล์ไม่สำเร็จ: " + ex.Message, "ปิด");
            }
        }

        [RelayCommand]
        private async Task BackAsync() => await Shell.Current.GoToAsync("..");
    }

    /// <summary>
    /// ViewModel ย่อยสำหรับแต่ละไฟล์ backup
    /// </summary>
    public partial class BackupFileVm : ObservableObject
    {
        [ObservableProperty] private string fullPath = string.Empty;
        [ObservableProperty] private string fileName = string.Empty;
        [ObservableProperty] private string sizeDisplay = string.Empty;
        [ObservableProperty] private string dateDisplay = string.Empty;
    }
}
