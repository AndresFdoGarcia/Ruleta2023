using MongoDB.Driver;
using Ruleta2023.Data.Access.MongoDb.RouletteConfiguration.Contract;
using Ruleta2023.Domain.Data.Ruleta;
using Ruleta2023.Domain.Data.Users;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruleta2023.Data.Access.MongoDb.RouletteConfiguration.Implementation
{
    public class MongoDBRouletteConfigurationManager : IRouletteConfigurationManager
    {
        private MongoClient client;
        private string collectionName;
        private IMongoDatabase database;

        public MongoDBRouletteConfigurationManager(MongoClient client, string dbName, string collectionName)
        {
            this.client = client;
            this.collectionName = collectionName;
            this.database = client.GetDatabase(dbName);
        }

        public Task Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<RouletteClass> GetClient(string id)
        {

            try
            {
                IMongoCollection<RouletteClass> collection = database.GetCollection<RouletteClass>(collectionName);
                var builder = Builders<RouletteClass>.Filter;
                var filter = builder.And(builder.Eq(row => row.Id, id));
                return (await collection.FindAsync(filter)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Prueba entrada" + ex.Message);
                return null;
            }
        }


        public async Task Save(RouletteClass entity)
        {

            try
            {
                IMongoCollection<RouletteClass> collection = database.GetCollection<RouletteClass>(collectionName);
                await collection.InsertOneAsync(entity);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }

        }

        public async Task Update(RouletteClass entity)
        {

            try
            {
                var collection = database.GetCollection<RouletteClass>(collectionName);
                var filter = Builders<RouletteClass>.Filter.Eq(row => row.Id, entity.Id);
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
