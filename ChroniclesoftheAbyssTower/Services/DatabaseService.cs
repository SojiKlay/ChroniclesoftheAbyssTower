using ChroniclesoftheAbyssTower.Helpers;
using ChroniclesoftheAbyssTower.Models;
using SQLite;

namespace ChroniclesoftheAbyssTower.Services
{
    /// <summary>
    /// Service ดูแล SQLite connection + create tables + generic CRUD
    /// ใช้ singleton ทั้งแอป (ไม่เปิด-ปิด connection บ่อยๆ)
    /// </summary>
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _connection;
        private readonly SemaphoreSlim _initLock = new(1, 1);
        private bool _isInitialized;

        /// <summary>
        /// Path เต็มของไฟล์ database (อยู่ใน FileSystem.AppDataDirectory)
        /// </summary>
        public string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, AppConstants.DatabaseFileName);

        /// <summary>
        /// เริ่มต้น connection + สร้าง table ถ้ายังไม่มี
        /// เรียก method นี้ก่อนใช้งาน DB ทุกครั้ง (idempotent)
        /// </summary>
        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            await _initLock.WaitAsync();
            try
            {
                if (_isInitialized) return;

                _connection = new SQLiteAsyncConnection(
                    DatabasePath,
                    SQLiteOpenFlags.ReadWrite |
                    SQLiteOpenFlags.Create |
                    SQLiteOpenFlags.SharedCache);

                // สร้าง table ทั้งหมด (CreateTableAsync เป็น idempotent - ไม่ลบของเดิม)
                await _connection.CreateTableAsync<User>();
                await _connection.CreateTableAsync<Player>();
                await _connection.CreateTableAsync<Item>();
                await EnsureItemThaiNameColumnAsync();
                await _connection.CreateTableAsync<InventoryItem>();
                await _connection.CreateTableAsync<Journal>();
                await _connection.CreateTableAsync<SaveData>();
                await _connection.CreateTableAsync<StoryProgress>();

                _isInitialized = true;
            }
            finally
            {
                _initLock.Release();
            }
        }

        private async Task EnsureItemThaiNameColumnAsync()
        {
            try
            {
                await _connection!.ExecuteAsync("ALTER TABLE Items ADD COLUMN ThaiName varchar(80)");
            }
            catch (SQLiteException ex) when (ex.Message.Contains("duplicate column name", StringComparison.OrdinalIgnoreCase))
            {
                // Column already exists from a previous app run.
            }
        }

        /// <summary>
        /// ดึง raw connection สำหรับ query ที่ซับซ้อน (ใช้ใน Service เฉพาะทาง)
        /// </summary>
        public async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            await InitializeAsync();
            return _connection!;
        }

        // ============== Generic CRUD ==============

        public async Task<int> InsertAsync<T>(T entity) where T : new()
        {
            var conn = await GetConnectionAsync();
            return await conn.InsertAsync(entity);
        }

        public async Task<int> UpdateAsync<T>(T entity) where T : new()
        {
            var conn = await GetConnectionAsync();
            return await conn.UpdateAsync(entity);
        }

        public async Task<int> DeleteAsync<T>(T entity) where T : new()
        {
            var conn = await GetConnectionAsync();
            return await conn.DeleteAsync(entity);
        }

        public async Task<T?> GetAsync<T>(int id) where T : new()
        {
            var conn = await GetConnectionAsync();
            try
            {
                return await conn.GetAsync<T>(id);
            }
            catch (InvalidOperationException)
            {
                // ไม่เจอ row
                return default;
            }
        }

        public async Task<List<T>> GetAllAsync<T>() where T : new()
        {
            var conn = await GetConnectionAsync();
            return await conn.Table<T>().ToListAsync();
        }

        public async Task<AsyncTableQuery<T>> QueryAsync<T>() where T : new()
        {
            var conn = await GetConnectionAsync();
            return conn.Table<T>();
        }

        // ============== Maintenance ==============

        /// <summary>
        /// ลบไฟล์ database ทั้งก้อน (ใช้กับ Restore Backup)
        /// </summary>
        public async Task ResetDatabaseAsync()
        {
            await _initLock.WaitAsync();
            try
            {
                if (_connection != null)
                {
                    await _connection.CloseAsync();
                    _connection = null;
                }

                if (File.Exists(DatabasePath))
                {
                    File.Delete(DatabasePath);
                }

                _isInitialized = false;
                await InitializeAsync();
            }
            finally
            {
                _initLock.Release();
            }
        }
    }
}
