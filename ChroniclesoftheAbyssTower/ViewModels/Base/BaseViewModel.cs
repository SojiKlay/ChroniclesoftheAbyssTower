using CommunityToolkit.Mvvm.ComponentModel;

namespace ChroniclesoftheAbyssTower.ViewModels.Base
{
    /// <summary>
    /// คลาสฐานสำหรับทุก ViewModel ในโปรเจค
    /// ให้บริการ IsBusy, Title, Error message ที่ใช้ร่วมกัน
    /// </summary>
    public partial class BaseViewModel : ObservableObject
    {
        // ============== IsBusy ==============
        // กำลังโหลด/ทำงานอยู่หรือไม่ - ใช้กับ ActivityIndicator
        // [NotifyPropertyChangedFor(nameof(IsNotBusy))] ทำให้ IsNotBusy รีเฟรชตาม
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool isBusy;

        // ตรงข้ามกับ IsBusy - ใช้ผูกกับ IsEnabled ของปุ่ม (ปุ่มทำงานเมื่อไม่ busy)
        public bool IsNotBusy => !IsBusy;

        // ============== Title ==============
        // หัวข้อหน้าจอ
        [ObservableProperty]
        private string title = string.Empty;

        // ============== Error ==============
        // ข้อความ error สำหรับแสดงในหน้าจอ
        // เมื่อเปลี่ยนค่า จะรีเฟรช HasError ด้วย
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        private string errorMessage = string.Empty;

        // มี error หรือไม่ - ใช้ผูกกับ IsVisible ของ Label error
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        /// <summary>
        /// ล้าง error message
        /// </summary>
        protected void ClearError()
        {
            ErrorMessage = string.Empty;
        }

        /// <summary>
        /// ตั้ง error message พร้อม clear IsBusy ทันที
        /// </summary>
        protected void SetError(string message)
        {
            ErrorMessage = message;
            IsBusy = false;
        }
    }
}

