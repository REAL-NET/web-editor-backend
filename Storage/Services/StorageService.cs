using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Storage.Data;
using Storage.Models;
using Storage.Models.Database;
using Storage.Requests;

namespace Storage.Services
{
    public class StorageService
    {
        private readonly StorageDbContext _storageDb;
        
        public StorageService(StorageDbContext storageDbContext)
        {
            _storageDb = storageDbContext;
        }

        public void Save(string username, string? info, DateTime? lastModified, string token)
        {
            var maxId = _storageDb.StorageDb.AsEnumerable().Select(r => r.Id).DefaultIfEmpty(0).Max();
            var saveId = username + (maxId + 1);
            if (lastModified == null)
            {
                lastModified = DateTime.Now;
            }
            var newRecord = new DbRecord
            {
                User = username,
                SaveId = saveId,
                LastModified = lastModified.Value,
                Info = info
            };
            try
            {
                _storageDb.Add(newRecord);
                _storageDb.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new StorageException("Adding failed to database", e);
            }
            
            RepoRequest.SaveRepo(saveId, token);
        }

        public List<Record> GetRecords()
        {
            return _storageDb.StorageDb.AsEnumerable()
                .Select(r => (Record) r)
                .ToList();
        }

        public List<Record> GetRecords(string username)
        {
            return _storageDb.StorageDb.AsEnumerable()
                .Where(r => r.User == username)
                .Select(r => (Record) r)
                .ToList();
        }

        public void DeleteRecord(Record record)
        {
            var toRemove = _storageDb.StorageDb.AsEnumerable().Where(r => r.Equals(record));
            foreach (var r in toRemove)
            {
                _storageDb.StorageDb.Remove(r);
            }
            try
            {
                _storageDb.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new StorageException("Deleting failed to database", e);
            }
            // TODO: add deleting in repo and request
        }

        public void LoadRepo(string saveId, string token)
        {
            try
            {
                var toLoad = _storageDb.StorageDb.AsEnumerable().First(r => r.SaveId.Equals(saveId)).SaveId;
                RepoRequest.LoadRepo(toLoad, token);
            }
            catch (InvalidOperationException e)
            {
                throw new StorageException($"Save Id is not found: {saveId}", e);
            }
        }

        public void LoadLastRepo(string user, string token)
        {
            var toLoad = _storageDb.StorageDb.AsEnumerable()
                .Where(r => r.User.Equals(user))
                .OrderBy(r => r.LastModified)
                .Last();
            LoadRepo(toLoad.SaveId, token);
        }
        
        public class StorageException : Exception
        {
            public StorageException(string message) : base(message)
            {
            }
            
            public StorageException(string message, Exception inner) : base(message, inner)
            {
            }
        }
    }
}