﻿using LightningDB;
using System;
using System.Threading.Tasks;
using YesSql.Core.Storage;
using YesSql.Core.Services;

namespace YesSql.Storage.LightningDB
{
    public class LightningDocumentStorageFactory : IDocumentStorageFactory, IDisposable
    {
        private readonly string _rootFolder;
        public LightningEnvironment LightningEnvironment { get; }

        public LightningDocumentStorageFactory(string rootFolder) :
            this(rootFolder, 1024 * 1024 * 10)
        {
        }

        public LightningDocumentStorageFactory(string rootFolder, long mapSize)
        {
            _rootFolder = rootFolder;
            LightningEnvironment = new LightningEnvironment(rootFolder);
            LightningEnvironment.MapSize = mapSize;
            LightningEnvironment.MaxDatabases = 2;
            LightningEnvironment.Open();

            using (var txn = LightningEnvironment.BeginTransaction())
            {
                using (txn.OpenDatabase(null, new DatabaseConfiguration { Flags = DatabaseOpenFlags.Create }))
                {
                    txn.Commit();
                }
            }
        }

        public IDocumentStorage CreateDocumentStorage(ISession session, Configuration configuration)
        {
            return new LightningDocumentStorage(LightningEnvironment);
        }

        public void Dispose()
        {
            LightningEnvironment.Dispose();
        }

        public Task InitializeAsync(Configuration configuration)
        {
#if NET451
            return Task.FromResult(0);
#else
            return Task.CompletedTask;
#endif
        }

        public Task InitializeCollectionAsync(Configuration configuration, string collectionName)
        {
            throw new NotImplementedException();
        }
    }
}
