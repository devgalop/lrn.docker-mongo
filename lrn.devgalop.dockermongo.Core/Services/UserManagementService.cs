using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using lrn.devgalop.dockermongo.Core.Interfaces;
using lrn.devgalop.dockermongo.Core.Models;
using lrn.devgalop.dockermongo.Infrastructure.Data.Interfaces;
using lrn.devgalop.dockermongo.Infrastructure.Data.Models;
using lrn.devgalop.dockermongo.Infrastructure.Security.EncryptDecrypt.Interfaces;
using lrn.devgalop.dockermongo.Infrastructure.Security.EncryptDecrypt.Models;
using lrn.devgalop.dockermongo.Infrastructure.Security.EncryptDecrypt.Services;
using lrn.devgalop.dockermongo.Infrastructure.Security.JWT.Interfaces;
using lrn.devgalop.dockermongo.Infrastructure.Security.JWT.Models;

namespace lrn.devgalop.dockermongo.Core.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IRepository _repository;
        private readonly IAesCryptService _cryptService;
        private readonly AesCryptType _cryptConfig;

        public UserManagementService(
            IRepository repository,
            IAesCryptService cryptService,
            AesCryptType cryptConfig
            )
        {
            _repository = repository;
            _cryptService = cryptService;
            _cryptConfig = cryptConfig;
        }

        public async Task<Models.BaseResponse> InsertUserAsync(InsertUserRequest request)
        {
            try
            {
                var validationResults = new List<ValidationResult>();
                if(!Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true))
                {
                    string errors = string.Join(",", validationResults.Where(r => !string.IsNullOrEmpty(r.ErrorMessage)).Select(r => r.ErrorMessage));
                    throw new Exception($"Invalid user model. Errors: {errors}");
                }
                var cryptResponse = _cryptService.Encrypt(request.Password, _cryptConfig);
                if(!cryptResponse.IsSucceed || string.IsNullOrEmpty(cryptResponse.Text)) throw new Exception($"Password encryption failed. {cryptResponse.ErrorMessage}");
                var passwordHashed = cryptResponse.Text;

                var userFoundResponse = await _repository.GetUserAsync(request.Username);
                if(userFoundResponse.Result is not null) throw new Exception($"Username {request.Username} already exists");

                var insertResponse = await _repository.InsertAsync(new User()
                {
                    Username = request.Username,
                    Password = passwordHashed,
                    RegistrationDate = request.RegistrationDate,
                    UpdateDate = request.UpdateDate,
                    Status = request.Status,
                    RoleId = ERole.BASIC_AUTH
                });
                if(!insertResponse.IsSucceed) throw new Exception($"User cannot be added to database. {insertResponse.ErrorMessage}");
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

        public async Task<BasicUserResponse> GetUserAsync(string username)
        {
            try
            {
                if(string.IsNullOrEmpty(username))throw new ArgumentNullException("Username cannot be null or empty");
                var response = await _repository.GetUserAsync(username);
                if(!response.IsSucceed || response.Result is null) throw new Exception($"Could not find user '{username}' in repository. {response.ErrorMessage}");
                return new()
                {
                    IsSucceed = true,
                    Username = response.Result.Username,
                    RegistrationDate = response.Result.RegistrationDate,
                    Status = response.Result.Status,
                    Role = response.Result.RoleId.ToString()
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

        public async Task<Models.BaseResponse> DisableUserAsync(string username)
        {
            try
            {
                if(string.IsNullOrEmpty(username))throw new ArgumentNullException("Username cannot be null or empty");
                var response = await _repository.GetUserAsync(username);
                if(!response.IsSucceed || response.Result is null) throw new Exception($"User {username} does not exist. {response.ErrorMessage}");
                var disableResponse = await _repository.DisableUserAsync(username);
                if(!disableResponse.IsSucceed) throw new Exception($"An unexpected error ocurred during user deactivation. {disableResponse.ErrorMessage}"); 
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

        public async Task<Models.BaseResponse> RevokeAccessAsync(string username)
        {
            try
            {
                if(string.IsNullOrEmpty(username))throw new ArgumentNullException("Username cannot be null or empty");
                var response = await _repository.GetUserAsync(username);
                if(!response.IsSucceed || response.Result is null) throw new Exception($"User {username} does not exist. {response.ErrorMessage}");
                var revokeResponse = await _repository.RevokeAuthAsync(username);
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

        public async Task<Models.BaseResponse> ModifyPasswordAsync(ModifyPasswordRequest request)
        {
            try
            {
                List<ValidationResult> validationResults = new();
                if(!Validator.TryValidateObject(request,new ValidationContext(request), validationResults, true))
                {
                    string errors = string.Join(",", validationResults.Where(r => !string.IsNullOrEmpty(r.ErrorMessage)).Select(r => r.ErrorMessage));
                    throw new Exception($"Invalid model. {errors}");
                }

                var userResponse = await _repository.GetUserAsync(request.Username);
                if(!userResponse.IsSucceed || userResponse.Result is null) throw new Exception($"Could not find the user '{request.Username}' in database. {userResponse.ErrorMessage}");
                
                var cryptResponse = _cryptService.Encrypt(request.ActualPassword, _cryptConfig);
                if(!cryptResponse.IsSucceed || string.IsNullOrEmpty(cryptResponse.Text)) throw new Exception($"Password encryption failed. {cryptResponse.ErrorMessage}");

                var userFound = userResponse.Result;
                if(userFound.Password != cryptResponse.Text) throw new Exception($"Username or password are incorrect.");

                cryptResponse = _cryptService.Encrypt(request.NewPassword, _cryptConfig);
                if(!cryptResponse.IsSucceed || string.IsNullOrEmpty(cryptResponse.Text)) throw new Exception($"Password encryption failed. {cryptResponse.ErrorMessage}");

                var response = await _repository.ChangePassword(request.Username, cryptResponse.Text);
                if(!response.IsSucceed) throw new Exception($"Password could not be updated. {response.ErrorMessage}");
   
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