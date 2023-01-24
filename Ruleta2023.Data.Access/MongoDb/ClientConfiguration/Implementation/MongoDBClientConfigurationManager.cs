using Ruleta2023.Data.Access.MongoDb.ClientConfiguration.Contract;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ruleta2023.Domain.Data.Users;
using Serilog;

namespace Ruleta2023.Data.Access.MongoDb.ClientConfiguration.Implementation
{
    public class MongoDBClientConfigurationManager : IClientConfigurationManager
    {
        private MongoClient client;
        private string collectionName;
        private IMongoDatabase database;

        public MongoDBClientConfigurationManager(MongoClient client, string dbName, string collectionName)
        {
            this.client = client;
            this.collectionName = collectionName;
            this.database = client.GetDatabase(dbName);
        }

        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<ClientClass> GetClient(string id)
        {

            try
            {
                IMongoCollection<ClientClass> collection = database.GetCollection<ClientClass>(collectionName);
                var builder = Builders<ClientClass>.Filter;
                var filter = builder.And(builder.Eq(row => row.Id, id));
                return (await collection.FindAsync(filter)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Prueba entrada" + ex.Message);
                return null;
            }
        }


        public async Task Save(ClientClass entity)
        {

            try
            {
                IMongoCollection<ClientClass> collection = database.GetCollection<ClientClass>(collectionName);
                await collection.InsertOneAsync(entity);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }

        }

        public async Task Update(ClientClass entity)
        {

            try
            {
                var collection = database.GetCollection<ClientClass>(collectionName);
                var filter = Builders<ClientClass>.Filter.Eq(row => row.Id, entity.Id);
                var result = await collection.ReplaceOneAsync(filter, entity);
                if (result.ModifiedCount <= 0)
                    throw new Exception("Updated 0 documents");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }

        }
    }
}
