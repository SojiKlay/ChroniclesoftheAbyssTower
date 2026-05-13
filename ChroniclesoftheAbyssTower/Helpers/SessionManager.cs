namespace ChroniclesoftheAbyssTower.Helpers
{
    /// <summary>
    /// จัดการ session ของผู้ใช้ที่ login อยู่
    /// เก็บข้อมูลใน SecureStorage ของ MAUI (เข้ารหัสโดยระบบ)
    /// </summary>
    public static class SessionManager
    {
        // คีย์ที่ใช้เก็บใน SecureStorage
        private const string KeyUserId = "abyss_user_id";
        private const string KeyUsername = "abyss_username";
        private const string KeyActivePlayerId = "abyss_active_player_id";

        /// <summary>
        /// บันทึก session หลัง login สำเร็จ
        /// </summary>
        public static async Task SetSessionAsync(int userId, string username)
        {
            await SecureStorage.Default.SetAsync(KeyUserId, userId.ToString());
            await SecureStorage.Default.SetAsync(KeyUsername, username);
        }

        /// <summary>
        /// ดึง UserId ของ user ที่ login อยู่ (return null ถ้ายังไม่ได้ login)
        /// </summary>
        public static async Task<int?> GetUserIdAsync()
        {
            var raw = await SecureStorage.Default.GetAsync(KeyUserId);
            if (string.IsNullOrWhiteSpace(raw)) return null;
            return int.TryParse(raw, out var id) ? id : null;
        }

        /// <summary>
        /// ดึง Username ของ user ที่ login อยู่
        /// </summary>
        public static async Task<string?> GetUsernameAsync()
        {
            return await SecureStorage.Default.GetAsync(KeyUsername);
        }

        /// <summary>
        /// บันทึก PlayerId ที่กำลังเล่นอยู่ (player ปัจจุบันของ user คนนี้)
        /// </summary>
        public static async Task SetActivePlayerIdAsync(int playerId)
        {
            await SecureStorage.Default.SetAsync(KeyActivePlayerId, playerId.ToString());
        }

        /// <summary>
        /// ดึง PlayerId ปัจจุบัน
        /// </summary>
        public static async Task<int?> GetActivePlayerIdAsync()
        {
            var raw = await SecureStorage.Default.GetAsync(KeyActivePlayerId);
            if (string.IsNullOrWhiteSpace(raw)) return null;
            return int.TryParse(raw, out var id) ? id : null;
        }

        /// <summary>
        /// ล้าง active player (ใช้ตอนลบ player / จบเกม)
        /// คงข้อมูล User session ไว้ (ผู้ใช้ยังอยู่ในระบบ)
        /// </summary>
        public static void ClearActivePlayer()
        {
            SecureStorage.Default.Remove(KeyActivePlayerId);
        }

        /// <summary>
        /// ตรวจว่า user login อยู่หรือไม่
        /// </summary>
        public static async Task<bool> IsLoggedInAsync()
        {
            var id = await GetUserIdAsync();
            return id.HasValue;
        }

        /// <summary>
        /// Logout - ล้าง session ทั้งหมด
        /// </summary>
        public static void ClearSession()
        {
            // SecureStorage.RemoveAll() ลบทุก key ทำให้ปลอดภัยที่สุด
            SecureStorage.Default.Remove(KeyUserId);
            SecureStorage.Default.Remove(KeyUsername);
            SecureStorage.Default.Remove(KeyActivePlayerId);
        }
    }
}
