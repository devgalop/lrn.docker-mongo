using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using lrn.devgalop.dockermongo.Core.Interfaces;
using lrn.devgalop.dockermongo.Core.Models;
using lrn.devgalop.dockermongo.Infrastructure.Data.Interfaces;

namespace lrn.devgalop.dockermongo.Core.Services
{
    public class ProductManagementService : IProductManagementService
    {
        private readonly IRepository _repository;

        public ProductManagementService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaseResponse> CreateProductAsync(InsertProductRequest request)
        {
            try
            {
                List<ValidationResult> validationResults = new List<ValidationResult>();
                if(!Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true))
                {
                    string errors = string.Join(",", validationResults.Where(r => !string.IsNullOrEmpty(r.ErrorMessage)).Select(r => r.ErrorMessage));
                    throw new Exception($"Invalid product model. Errors: {errors}");
                }

                var now = DateTime.UtcNow;
                var response = await _repository.CreateProductAsync(new()
                {
                    UUID = Guid.NewGuid().ToString(),
                    Name = request.Name,
                    RegistrationDate = now,
                    UpdateDate = now,
                    Status = true,
                    Description = request.Description,
                    UnitPrice = request.UnitPrice,
                    SellUnitPrice = request.SellUnitPrice
                });

                if(!response.IsSucceed) throw new Exception($"Product could not be created. {response.ErrorMessage}");

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

        public async Task<BasicProductResponse> GetProductAsync(string productId)
        {
            try
            {
                if(string.IsNullOrEmpty(productId)) throw new ArgumentNullException($"Product id cannot be null or empty");
                var response = await _repository.GetProductAsync(productId);
                if(!response.IsSucceed || response.Result is null) throw new Exception($"Cannot return the product or product {productId} cannot exist in database. {response.ErrorMessage}");
                var product = response.Result;
                return new()
                {
                    IsSucceed = true,
                    Id = product.UUID,
                    Name = product.Name,
                    Status = product.Status,
                    RegistrationDate = product.RegistrationDate,
                    UnitPrice = product.UnitPrice,
                    SellUnitPrice = product.SellUnitPrice
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

        public async Task<MultipleProductResponse> GetActiveProductsAsync()
        {
            try
            {
                var response = await _repository.GetProductsByStatusAsync();
                if(!response.IsSucceed || response.Result is null) throw new Exception($"There is not active products in database. {response.ErrorMessage}");
                var products = response.Result.Select(p => new ProductResponse()
                {
                    Id = p.UUID,
                    Name = p.Name,
                    RegistrationDate = p.RegistrationDate,
                    Status = p.Status,
                    UnitPrice = p.UnitPrice,
                    SellUnitPrice = p.SellUnitPrice
                });
                return new()
                {
                    IsSucceed = true,
                    Products = products
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