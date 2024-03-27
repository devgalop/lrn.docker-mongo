using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using lrn.devgalop.dockermongo.Infrastructure.Data.Interfaces;
using lrn.devgalop.dockermongo.Infrastructure.Data.Models;
using MongoDB.Driver;

namespace lrn.devgalop.dockermongo.Infrastructure.Data.Repositories
{
    public class MongoDbRepository : IRepository
    {
        private readonly IMongoDatabase _database;

        public MongoDbRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<BaseResponse> InsertAsync(User user, CancellationToken ct = default)
        {
            try
            {
                List<ValidationResult> validationResults = new();
                if(user is null) throw new Exception("User model cannot be null or empty");
                if(!Validator.TryValidateObject(user,new ValidationContext(user), validationResults,true))
                {
                    string errors = string.Join(",",validationResults.Select(r => r.ErrorMessage));
                    throw new Exception($"Invalid model. Error: {errors}");
                }
                var usersCollection = _database.GetCollection<User>("users");
                await usersCollection.InsertOneAsync(user, new InsertOneOptions(), ct);
                return new()
                {
                    IsSucceed = true
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    IsSucceed = false,
                    ErrorMessage = ex.Message,
                    ErrorDescription = ex.ToString()
                };
            }
        }

        public async Task<UserResponse> GetUserAsync(string username, CancellationToken ct = default)
        {
            try
            {
                if(string.IsNullOrEmpty(username))throw new ArgumentNullException("Username cannot be null or empty");
                var usersCollection = _database.GetCollection<User>("users");
                var user = await usersCollection.Find(u => u.Username == username).FirstOrDefaultAsync();
                return new()
                {
                    IsSucceed = true,
                    Result = user
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    IsSucceed = false,
                    ErrorMessage = ex.Message,
                    ErrorDescription = ex.ToString()
                };
            }
        }
    }
}