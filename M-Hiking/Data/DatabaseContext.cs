using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace M_Hiking.Data
{
   public class DatabaseContext : IAsyncDisposable
    {
        private const string Dbname = "MHike.db3";
        private static string DbPath => Path.Combine(FileSystem.AppDataDirectory, Dbname);
        private SQLiteAsyncConnection _connection;
        private SQLiteAsyncConnection Database => (_connection??= new SQLiteAsyncConnection(DbPath,
            SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache));
        public async Task CreateTableIfNotExist<TTable>()where TTable : class, new()
        {
            await Database.CreateTableAsync<TTable>();
        }
        public async Task<AsyncTableQuery<TTable>> GetTableAsync<TTable>() where TTable : class, new() 
        {
            await CreateTableIfNotExist<TTable>();
            return Database.Table<TTable>();
        }
        public async Task<IEnumerable<TTable>> GetAllAsync<TTable>() where TTable : class, new()
        {
           var table = await GetTableAsync<TTable>();
            return await table.ToListAsync();
        }

        public async Task<IEnumerable<TTable>> GetFileteredAsync<TTable>(Expression<Func<TTable,bool>>predicate) where TTable : class, new()
        {
            await Database.CreateTableAsync<TTable>();
            return await Database.Table<TTable>().Where(predicate).ToListAsync();
        }
        public async Task<TTable> GetItemByIdAsync<TTable>(object primaryKey) where TTable : class, new()
        {
            await CreateTableIfNotExist<TTable>();
            return await Database.GetAsync<TTable>(primaryKey);
        }
        public async Task<bool> AddItemAsync<TTable>(TTable item) where TTable : class, new()
        {
            await CreateTableIfNotExist<TTable>();
            return await Database.InsertAsync(item) > 0;
        }
        public async Task<bool> UpdateItemAsync<TTable>(TTable item) where TTable : class, new()
        {
            await CreateTableIfNotExist<TTable>();
            return await Database.UpdateAsync(item) > 0;
        }
        public async Task<bool> DeleteItemAsync<TTable>(TTable item) where TTable : class, new()
        {
            await CreateTableIfNotExist<TTable>();
            return await Database.DeleteAsync(item) > 0;
        }
        public async Task<bool> DeleteItemByKeyAsync<TTable>(object primaryKey) where TTable : class, new()
        {
            await CreateTableIfNotExist<TTable>();
            return await Database.DeleteAsync<TTable>(primaryKey) > 0;
        }

        public async ValueTask DisposeAsync() => await _connection?.CloseAsync();
    }
}
