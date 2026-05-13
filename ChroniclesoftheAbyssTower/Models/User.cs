using SQLite;

namespace ChroniclesoftheAbyssTower.Models
{
    /// <summary>
    /// ผู้ใช้ที่ลงทะเบียนเข้าระบบ
    /// 1 user สามารถมี Player ได้หลายตัว (1-N relationship)
    /// </summary>
    [Table("Users")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int UserId { get; set; }

        [Indexed(Unique = true), MaxLength(20), NotNull]
        public string Username { get; set; } = string.Empty;

        // เก็บเฉพาะ hash ของ password (Base64 ของ PBKDF2-SHA256)
        [NotNull]
        public string PasswordHash { get; set; } = string.Empty;

        // Salt ที่ใช้ตอน hash (Base64)
        [NotNull]
        public string Salt { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }
    }
}
