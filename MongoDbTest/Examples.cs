using MongoDbTest.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest
{
    public class Examples
    {
        private readonly UsersRepository _usersRepo;

        public Examples()
        {
            _usersRepo = new UsersRepository("connString");
        }

        /// <summary>
        /// Adds a new field in an existing document
        /// Example: AddNewFieldInAnExistingDocument("Nikola","address", "test address");
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        private async void AddNewFieldInAnExistingDocument(string userName, string fieldName, string fieldValue)
        {
            var users = await _usersRepo.GetUsersByFieldAsync("name", userName);

            var user = users.FirstOrDefault();

            await _usersRepo.UpdateUserAsync(user.Id, fieldName, fieldValue);
        }
    }
}
