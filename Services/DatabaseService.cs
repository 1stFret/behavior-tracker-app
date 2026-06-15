using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SQLite;
using BehaviorTracker.Models;

namespace BehaviorTracker.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        public async Task Init()
        {
            if (_database != null)
                return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "behaviortracker.db");
            _database = new SQLiteAsyncConnection(dbPath);
            await _database.CreateTableAsync<Child>();
            await _database.CreateTableAsync<BehaviorEntry>();
            await _database.CreateTableAsync<BehaviorType>();
        }
        public async Task<int> SaveChildAsync(Child child)
        {
            await Init();
            return await _database.InsertAsync(child);
        }

        public async Task<List<Child>> GetChildrenAsync()
        {
            await Init();
            return await _database.Table<Child>().ToListAsync();
        }

        public async Task<int> DeleteChildAsync(Child child)
        {
            await Init();
            return await _database.DeleteAsync(child);
        }

        public async Task<int> SaveBehaviorEntryAsync(BehaviorEntry entry)
        {
            await Init();
            return await _database.InsertAsync(entry);
        }

        public async Task<List<BehaviorEntry>> GetEntriesForChildAsync(int childId)
        {
            await Init();
            return await _database.Table<BehaviorEntry>()
                                  .Where(e => e.ChildId == childId)
                                  .OrderByDescending(e => e.LoggedAt)
                                  .ToListAsync();
        }

        public async Task<int> SaveBehaviorTypeAsync(BehaviorType behaviorType)
        {
            await Init();
            return await _database.InsertAsync(behaviorType);
        }

        public async Task<List<BehaviorType>> GetBehaviorTypesForChildAsync(int childId)
        {
            await Init();
            return await _database.Table<BehaviorType>()
                                  .Where(b => b.ChildId == childId)
                                  .ToListAsync();
        }

        public async Task<int> DeleteBehaviorTypeAsync(BehaviorType behaviorType)
        {
            await Init();
            return await _database.DeleteAsync(behaviorType);
        }

        public async Task<List<BehaviorEntry>> GetAllEntriesAsync()
        {
            await Init();
            return await _database.Table<BehaviorEntry>()
                                  .OrderByDescending(e => e.LoggedAt)
                                  .ToListAsync();
        }

        public async Task<int> DeleteBehaviorEntryAsync(BehaviorEntry entry)
        {
            await Init();
            return await _database.DeleteAsync(entry);
        }

        public async Task<int> UpdateBehaviorEntryAsync(BehaviorEntry entry)
        {
            await Init();
            return await _database.UpdateAsync(entry);
        }
    }
}
