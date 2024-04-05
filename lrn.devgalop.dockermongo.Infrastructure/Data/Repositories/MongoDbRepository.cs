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

        public async Task<BaseResponse> UpdateAuthAsync(string username, UserAuth auth, CancellationToken ct = default)
        {
            try
            {
                if(string.IsNullOrEmpty(username))throw new ArgumentNullException("Username cannot be null or empty");
                if(auth is null) throw new ArgumentNullException("Auth model cannot be null");
                var usersCollection = _database.GetCollection<User>("users");
                var userFiltered = Builders<User>.Filter.Eq(user=> user.Username, username);
                var userUpdated = Builders<User>.Update.Set(user => user.Auth, auth)
                                                        .Set(user => user.UpdateDate, DateTime.UtcNow);
                var response = await usersCollection.UpdateOneAsync(userFiltered, userUpdated);
                if(response.ModifiedCount <= 0) throw new Exception($"Cannot update user.");
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

        public async Task<BaseResponse> RevokeAuthAsync(string username, CancellationToken ct = default)
        {
            try
            {
                if(string.IsNullOrEmpty(username))throw new ArgumentNullException("Username cannot be null or empty");
                var usersCollection = _database.GetCollection<User>("users");
                var userFiltered = Builders<User>.Filter.Eq(user=> user.Username, username);
                var userUpdated = Builders<User>.Update.Set(user => user.Auth, null)
                                                        .Set(user => user.UpdateDate, DateTime.UtcNow);
                var response = await usersCollection.UpdateOneAsync(userFiltered, userUpdated);
                if(response.ModifiedCount <= 0) throw new Exception($"Cannot update user.");
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

        public async Task<BaseResponse> DisableUserAsync(string username, CancellationToken ct = default)
        {
            try
            {
                if(string.IsNullOrEmpty(username))throw new ArgumentNullException("Username cannot be null or empty");
                var usersCollection = _database.GetCollection<User>("users");
                var userFiltered = Builders<User>.Filter.Eq(user=> user.Username, username);
                var userUpdated = Builders<User>.Update.Set(user => user.Status, false)
                                                        .Set(user => user.UpdateDate, DateTime.UtcNow);
                var response = await usersCollection.UpdateOneAsync(userFiltered, userUpdated);
                if(response.ModifiedCount <= 0) throw new Exception($"Cannot update user.");
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

        public async Task<BaseResponse> ChangePassword(string username, string passwordHashed, CancellationToken ct = default)
        {
            try
            {
                if(string.IsNullOrEmpty(username))throw new ArgumentNullException("Username cannot be null or empty");
                var usersCollection = _database.GetCollection<User>("users");
                var userFiltered = Builders<User>.Filter.Eq(user=> user.Username, username);
                var userUpdated = Builders<User>.Update.Set(user => user.Password, passwordHashed)
                                                        .Set(user => user.Auth, null)
                                                        .Set(user => user.UpdateDate, DateTime.UtcNow);
                var response = await usersCollection.UpdateOneAsync(userFiltered, userUpdated);
                if(response.ModifiedCount <= 0) throw new Exception($"Cannot update user.");
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
    
        public async Task<BaseResponse> CreateProductAsync(Product product, CancellationToken ct = default)
        {
            try
            {
                List<ValidationResult> validationResults = new();
                if(product is null) throw new Exception("Product model cannot be null or empty");
                if(!Validator.TryValidateObject(product,new ValidationContext(product), validationResults,true))
                {
                    string errors = string.Join(",",validationResults.Select(r => r.ErrorMessage));
                    throw new Exception($"Invalid model. Error: {errors}");
                }
                var productsCollection = _database.GetCollection<Product>("products");
                await productsCollection.InsertOneAsync(product, new InsertOneOptions(), ct);
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

        public async Task<ProductResponse> GetProductAsync(string name, CancellationToken ct = default)
        {
            try
            {
                if(string.IsNullOrEmpty(name)) throw new Exception("Product unique id cannot be null or empty");
                var productsCollection = _database.GetCollection<Product>("products");
                var product = await productsCollection.Find(p => p.UUID == name).FirstOrDefaultAsync();
                return new()
                {
                    IsSucceed = true,
                    Result = product
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

        public async Task<ProductsResponse> GetProductsByNameAsync(string name, CancellationToken ct = default)
        {
            try
            {
                if(string.IsNullOrEmpty(name)) throw new Exception("Product unique id cannot be null or empty");
                var productsCollection = _database.GetCollection<Product>("products");
                var product = await productsCollection.Find(p => p.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase)).ToListAsync();
                return new()
                {
                    IsSucceed = true,
                    Result = product
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

        public async Task<ProductsResponse> GetProductsByStatusAsync(bool status = true, CancellationToken ct = default)
        {
            try
            {
                var productsCollection = _database.GetCollection<Product>("products");
                var product = await productsCollection.Find(p => p.Status == status).ToListAsync();
                return new()
                {
                    IsSucceed = true,
                    Result = product
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

        public async Task<BaseResponse> UpdateProductAsync (string uniqueId, UpdateProductRequest request, CancellationToken ct = default)
        {
            try
            {
                List<ValidationResult> validationResults = new();
                if(request is null) throw new Exception("Product model cannot be null or empty");
                if(!Validator.TryValidateObject(request,new ValidationContext(request), validationResults,true))
                {
                    string errors = string.Join(",",validationResults.Select(r => r.ErrorMessage));
                    throw new Exception($"Invalid model. Error: {errors}");
                }
                var productsCollection = _database.GetCollection<Product>("products");
                var productFiltered = Builders<Product>.Filter.Eq(product=> product.UUID, uniqueId);
                var productUpdated = Builders<Product>.Update.Set(product => product.Name, request.Name)
                                                        .Set(product => product.Description, request.Description)
                                                        .Set(product => product.Status, request.Status)
                                                        .Set(product => product.UnitPrice, request.UnitPrice)
                                                        .Set(product => product.SellUnitPrice, request.SellUnitPrice)
                                                        .Set(product => product.UpdateDate, DateTime.UtcNow);
                var response = await productsCollection.UpdateOneAsync(productFiltered, productUpdated);
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
    }
}