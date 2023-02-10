using MongoDB.Driver;

namespace Models
{
    public class DataProvider {
        private const string connectionString = "mongodb://127.0.0.1:27017";
        private const string databaseName = "teamMaker";

        public IMongoCollection<T> ConnectToMongo<T>(in string collection)
        {
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(databaseName);

            return db.GetCollection<T>(collection);
        }
    }
}