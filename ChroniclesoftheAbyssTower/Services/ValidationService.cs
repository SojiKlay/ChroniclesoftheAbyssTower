using System.Text.RegularExpressions;

namespace ChroniclesoftheAbyssTower.Services
{
    /// <summary>
    /// ผลลัพธ์ของการ validate
    /// </summary>
    public record ValidationResult(bool IsValid, string? ErrorMessage = null)
    {
        public static ValidationResult Success() => new(true);
        public static ValidationResult Fail(string message) => new(false, message);
    }

    /// <summary>
    /// Service ตรวจสอบความถูกต้องของข้อมูลทั่วทั้งแอป
    /// รวม validation logic ไว้ที่เดียวเพื่อใช้ซ้ำได้
    /// </summary>
    public class ValidationService
    {
        // อนุญาตเฉพาะ a-z, A-Z, 0-9, _ และความยาว 4-20
        private static readonly Regex UsernameRegex =
            new(@"^[a-zA-Z0-9_]{4,20}$", RegexOptions.Compiled);

        /// <summary>
        /// ตรวจ username: 4-20 ตัวอักษร, alphanumeric + underscore
        /// </summary>
        public ValidationResult ValidateUsername(string? username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return ValidationResult.Fail("กรุณากรอกชื่อผู้ใช้");

            if (username.Length < 4)
                return ValidationResult.Fail("ชื่อผู้ใช้ต้องมีอย่างน้อย 4 ตัวอักษร");

            if (username.Length > 20)
                return ValidationResult.Fail("ชื่อผู้ใช้ต้องไม่เกิน 20 ตัวอักษร");

            if (!UsernameRegex.IsMatch(username))
                return ValidationResult.Fail("ชื่อผู้ใช้ใช้ได้เฉพาะ a-z, A-Z, 0-9 และ _");

            return ValidationResult.Success();
        }

        /// <summary>
        /// ตรวจรหัสผ่าน: ขั้นต่ำ 6 ตัวอักษร, ต้องมีตัวอักษรและตัวเลข
        /// </summary>
        public ValidationResult ValidatePassword(string? password)
        {
            if (string.IsNullOrEmpty(password))
                return ValidationResult.Fail("กรุณากรอกรหัสผ่าน");

            if (password.Length < 6)
                return ValidationResult.Fail("รหัสผ่านต้องมีอย่างน้อย 6 ตัวอักษร");

            if (password.Length > 50)
                return ValidationResult.Fail("รหัสผ่านต้องไม่เกิน 50 ตัวอักษร");

            // ต้องมีตัวอักษรอย่างน้อย 1 ตัว
            if (!password.Any(char.IsLetter))
                return ValidationResult.Fail("รหัสผ่านต้องมีตัวอักษรอย่างน้อย 1 ตัว");

            // ต้องมีตัวเลขอย่างน้อย 1 ตัว
            if (!password.Any(char.IsDigit))
                return ValidationResult.Fail("รหัสผ่านต้องมีตัวเลขอย่างน้อย 1 ตัว");

            return ValidationResult.Success();
        }

        /// <summary>
        /// ตรวจ confirm password ว่าตรงกับรหัสผ่านหลัก
        /// </summary>
        public ValidationResult ValidateConfirmPassword(string? password, string? confirmPassword)
        {
            if (string.IsNullOrEmpty(confirmPassword))
                return ValidationResult.Fail("กรุณายืนยันรหัสผ่าน");

            if (password != confirmPassword)
                return ValidationResult.Fail("รหัสผ่านไม่ตรงกัน");

            return ValidationResult.Success();
        }

        /// <summary>
        /// ตรวจชื่อตัวละคร: 1-20 ตัวอักษร
        /// </summary>
        public ValidationResult ValidatePlayerName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ValidationResult.Fail("กรุณากรอกชื่อตัวละคร");

            if (name.Trim().Length > 20)
                return ValidationResult.Fail("ชื่อตัวละครต้องไม่เกิน 20 ตัวอักษร");

            return ValidationResult.Success();
        }

        /// <summary>
        /// ตรวจหัวข้อ Journal: 1-50 ตัวอักษร
        /// </summary>
        public ValidationResult ValidateJournalTitle(string? title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return ValidationResult.Fail("กรุณากรอกหัวข้อ");

            if (title.Trim().Length > 50)
                return ValidationResult.Fail("หัวข้อต้องไม่เกิน 50 ตัวอักษร");

            return ValidationResult.Success();
        }

        /// <summary>
        /// ตรวจเนื้อหา Journal: 1-500 ตัวอักษร
        /// </summary>
        public ValidationResult ValidateJournalContent(string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return ValidationResult.Fail("กรุณากรอกเนื้อหา");

            if (content.Trim().Length > 500)
                return ValidationResult.Fail("เนื้อหาต้องไม่เกิน 500 ตัวอักษร");

            return ValidationResult.Success();
        }
    }
}
