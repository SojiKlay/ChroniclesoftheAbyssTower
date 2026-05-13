using ChroniclesoftheAbyssTower.Helpers;
using ChroniclesoftheAbyssTower.Models;

namespace ChroniclesoftheAbyssTower.Services
{
    /// <summary>
    /// ผลลัพธ์ของการ Login/Register
    /// </summary>
    public record AuthResult(bool Success, string? ErrorMessage = null, User? User = null);

    /// <summary>
    /// Service จัดการ Login / Register / Logout
    /// ใช้ PBKDF2 hash + Salt และเก็บ session ผ่าน SessionManager
    /// </summary>
    public class AuthService
    {
        private readonly DatabaseService _databaseService;
        private readonly ValidationService _validationService;

        public AuthService(DatabaseService databaseService, ValidationService validationService)
        {
            _databaseService = databaseService;
            _validationService = validationService;
        }

        /// <summary>
        /// ลงทะเบียนผู้ใช้ใหม่
        /// 1. validate input
        /// 2. ตรวจ username ซ้ำ
        /// 3. hash password + salt
        /// 4. insert user + เริ่ม session
        /// </summary>
        public async Task<AuthResult> RegisterAsync(string username, string password, string confirmPassword)
        {
            // ----- Validate -----
            var usernameCheck = _validationService.ValidateUsername(username);
            if (!usernameCheck.IsValid)
                return new AuthResult(false, usernameCheck.ErrorMessage);

            var passwordCheck = _validationService.ValidatePassword(password);
            if (!passwordCheck.IsValid)
                return new AuthResult(false, passwordCheck.ErrorMessage);

            var confirmCheck = _validationService.ValidateConfirmPassword(password, confirmPassword);
            if (!confirmCheck.IsValid)
                return new AuthResult(false, confirmCheck.ErrorMessage);

            // ----- ตรวจ username ซ้ำ -----
            var conn = await _databaseService.GetConnectionAsync();
            var existing = await conn.Table<User>()
                .Where(u => u.Username == username)
                .FirstOrDefaultAsync();

            if (existing != null)
                return new AuthResult(false, "ชื่อผู้ใช้นี้ถูกใช้แล้ว กรุณาเลือกชื่ออื่น");

            // ----- Hash + Insert -----
            try
            {
                var salt = PasswordHasher.GenerateSalt();
                var hash = PasswordHasher.HashPassword(password, salt);

                var user = new User
                {
                    Username = username,
                    PasswordHash = hash,
                    Salt = salt,
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow
                };

                await conn.InsertAsync(user);

                // เริ่ม session ทันที (ผู้ใช้เข้าใหม่ login อัตโนมัติ)
                await SessionManager.SetSessionAsync(user.UserId, user.Username);

                return new AuthResult(true, null, user);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AuthService.Register] {ex}");
                return new AuthResult(false, "เกิดข้อผิดพลาดในการสร้างบัญชี กรุณาลองใหม่");
            }
        }

        /// <summary>
        /// Login ด้วย username + password
        /// 1. หา user ตาม username
        /// 2. verify password ด้วย salt + hash ที่เก็บไว้
        /// 3. update LastLoginAt + เริ่ม session
        /// </summary>
        public async Task<AuthResult> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                return new AuthResult(false, "กรุณากรอกชื่อผู้ใช้");

            if (string.IsNullOrEmpty(password))
                return new AuthResult(false, "กรุณากรอกรหัสผ่าน");

            try
            {
                var conn = await _databaseService.GetConnectionAsync();
                var user = await conn.Table<User>()
                    .Where(u => u.Username == username)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    // ใช้ข้อความเดียวกันกับ password ผิด เพื่อไม่ให้ leak ว่าชื่อมีจริงหรือไม่
                    return new AuthResult(false, "ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง");
                }

                if (!PasswordHasher.VerifyPassword(password, user.Salt, user.PasswordHash))
                {
                    return new AuthResult(false, "ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง");
                }

                // อัปเดต LastLoginAt
                user.LastLoginAt = DateTime.UtcNow;
                await conn.UpdateAsync(user);

                // เริ่ม session
                await SessionManager.SetSessionAsync(user.UserId, user.Username);

                return new AuthResult(true, null, user);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AuthService.Login] {ex}");
                return new AuthResult(false, "เกิดข้อผิดพลาดในการเข้าสู่ระบบ กรุณาลองใหม่");
            }
        }

        /// <summary>
        /// ออกจากระบบ - ล้าง session ทั้งหมด
        /// </summary>
        public Task LogoutAsync()
        {
            SessionManager.ClearSession();
            return Task.CompletedTask;
        }

        /// <summary>
        /// ดึง User ปัจจุบันจาก session
        /// </summary>
        public async Task<User?> GetCurrentUserAsync()
        {
            var userId = await SessionManager.GetUserIdAsync();
            if (!userId.HasValue) return null;

            return await _databaseService.GetAsync<User>(userId.Value);
        }
    }
}
