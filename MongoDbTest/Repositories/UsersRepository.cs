using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest.Repositories
{
    /// <summary>
    /// Class used to access Mongo DB
    /// </summary>
    public class UsersRepository
    {
        private IMongoClient _client;
        private IMongoDatabase _database;
        private IMongoCollection<User> _usersCollection;

        public UsersRepository(string connectionString)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase("blog");
            _usersCollection = _database.GetCollection<User>("users");
        }

        /// <summary>
        /// Checking if connection to the database was established.
        /// </summary>
        public bool CheckConnection()
        {
            try
            {
                _database.ListCollections();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return false;
            }

            return true;
        }

        #region C 
        //CREATE

        /// <summary>
        /// Inserting passed user into the database
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task InserUserAsync(User user)
        {
            await _usersCollection.InsertOneAsync(user);
        }
        #endregion

        #region R
        //READ

        /// <summary>
        /// Returning all data from 'users' collection.
        /// </summary>
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _usersCollection.Find(new BsonDocument()).ToListAsync();
        }

        /// <summary>
        /// Returning all users with the defined value of defined field
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public async Task<List<User>> GetUsersByFieldAsync(string fieldName, string fieldValue)
        {
            var filter = Builders<User>.Filter.Eq(fieldName, fieldValue);

            return await _usersCollection.Find(filter).ToListAsync();
        }

        /// <summary>
        /// Returning the user with the name specified
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<User> GetUsersByNameAsync(string name)
        {
            return await _usersCollection.Find(rec => rec.Name.Equals(name)).FirstOrDefaultAsync();
        }


        /// <summary>
        /// Returning defined number of users
        /// </summary>
        /// <param name="startingFrom"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<List<User>> GetUsersAsync(int startingFrom, int count)
        {
            var result = await _usersCollection.Find(new BsonDocument())
                                                .Skip(startingFrom)
                                                .Limit(count)
                                                .ToListAsync();

            return result;
        }
        #endregion

        #region U
        //UPDATE

        /// <summary>
        /// Updating user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateFieldName"></param>
        /// <param name="updateStringValue"></param>
        /// <returns>
        /// True - If user is updated.
        /// False - If user is not updated.
        /// </returns>
        public async Task<bool> UpdateUserAsync(ObjectId id, string updateFieldName, string updateStringValue)
        {
            var filter = Builders<User>.Filter.Eq("_id", id);
            var update = Builders<User>.Update.Set(updateFieldName, updateFieldName);

            var result = await _usersCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount != 0;
        }
        #endregion

        #region D
        //DELETE

        /// <summary>
        /// Removing user with defined _id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// True - If user was deleted.
        /// False - If user was not deleted.
        /// </returns>
        public async Task<bool> DeleteUserByIdAsync(ObjectId id)
        {
            var filter = Builders<User>.Filter.Eq("_id", id);
            var result = await _usersCollection.DeleteOneAsync(filter);

            return result.DeletedCount != 0;
        }

        /// <summary>
        /// Removing all data from 'users' collection
        /// </summary>
        /// <returns></returns>
        public async Task<long> DeleteAllUsersAsync()
        {
            var filter = new BsonDocument();
            var result = await _usersCollection.DeleteManyAsync(filter);

            return result.DeletedCount;
        }
        #endregion

        #region Indexes

        /// <summary>
        /// Creates index on defined field.
        /// </summary>
        public async Task CreateIndexOnCollectionAsync(IMongoCollection<BsonDocument> collection, string field)
        {
            /* 
            //Old(obsolete) implementation
            var keys = Builders<BsonDocument>.IndexKeys.Ascending(field);
            await collection.Indexes.CreateOneAsync(keys);
            */

            var keys = Builders<BsonDocument>.IndexKeys.Ascending(field);
            var indexModel = new CreateIndexModel<BsonDocument>(keys);

            await collection.Indexes.CreateOneAsync(indexModel);
        }

        /// <summary>
        /// Creates index on Name field
        /// </summary>
        public async Task CreateIndexOnNameFieldAsync()
        {
            /* 
            //Oldobsolete) implementation
            var keys = Builders<User>.IndexKeys.Ascending(x => x.Name);
            await _usersCollection.Indexes.CreateOneAsync(keys);
            */

            var keys = Builders<User>.IndexKeys.Ascending(x => x.Name);
            var indexModel = new CreateIndexModel<User>(keys);

            await _usersCollection.Indexes.CreateOneAsync(indexModel);
        }
        #endregion

    }
}
