using System.Security.Cryptography;
using System.Text;

namespace ChroniclesoftheAbyssTower.Helpers
{
    /// <summary>
    /// ตัวช่วย hash รหัสผ่านด้วย PBKDF2 + Salt แบบสุ่ม
    /// ใช้ความปลอดภัยตามมาตรฐาน NIST (SP 800-63B)
    /// </summary>
    public static class PasswordHasher
    {
        // จำนวนรอบของ PBKDF2 - มากกว่าจะปลอดภัยกว่าแต่ช้ากว่า
        // 100_000 เป็นค่า recommended สำหรับ SHA-256 ในปี 2023+
        private const int Iterations = 100_000;

        // ขนาดของ Salt และ Hash output (byte)
        private const int SaltSize = 16;
        private const int HashSize = 32;

        /// <summary>
        /// สร้าง Salt แบบสุ่ม (Base64 encoded)
        /// </summary>
        public static string GenerateSalt()
        {
            var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
            return Convert.ToBase64String(saltBytes);
        }

        /// <summary>
        /// Hash รหัสผ่านด้วย PBKDF2-SHA256
        /// </summary>
        /// <param name="password">รหัสผ่านที่ผู้ใช้กรอก</param>
        /// <param name="saltBase64">Salt ในรูปแบบ Base64</param>
        /// <returns>Hash ในรูปแบบ Base64</returns>
        public static string HashPassword(string password, string saltBase64)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password ห้ามเป็นค่าว่าง", nameof(password));
            if (string.IsNullOrEmpty(saltBase64))
                throw new ArgumentException("Salt ห้ามเป็นค่าว่าง", nameof(saltBase64));

            var salt = Convert.FromBase64String(saltBase64);
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                passwordBytes,
                salt,
                Iterations,
                HashAlgorithmName.SHA256);

            var hashBytes = pbkdf2.GetBytes(HashSize);
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// ตรวจสอบรหัสผ่านว่าตรงกับ hash ที่เก็บไว้หรือไม่
        /// ใช้ FixedTimeEquals เพื่อป้องกัน timing attack
        /// </summary>
        public static bool VerifyPassword(string password, string saltBase64, string expectedHashBase64)
        {
            if (string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(saltBase64) ||
                string.IsNullOrEmpty(expectedHashBase64))
            {
                return false;
            }

            try
            {
                var actualHash = HashPassword(password, saltBase64);
                var actualBytes = Convert.FromBase64String(actualHash);
                var expectedBytes = Convert.FromBase64String(expectedHashBase64);

                // ใช้ FixedTimeEquals ป้องกัน timing attack
                return CryptographicOperations.FixedTimeEquals(actualBytes, expectedBytes);
            }
            catch
            {
                return false;
            }
        }
    }
}
