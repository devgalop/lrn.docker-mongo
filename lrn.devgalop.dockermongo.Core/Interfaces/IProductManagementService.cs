using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lrn.devgalop.dockermongo.Core.Models;

namespace lrn.devgalop.dockermongo.Core.Interfaces
{
    public interface IProductManagementService
    {
        /// <summary>
        /// Register new product in database
        /// </summary>
        /// <param name="request">Product</param>
        /// <returns></returns>
        Task<BaseResponse> CreateProductAsync(InsertProductRequest request);

        /// <summary>
        /// Get product by unique id
        /// </summary>
        /// <param name="productId">Product unique id</param>
        /// <returns></returns>
        Task<BasicProductResponse> GetProductAsync(string productId);

        /// <summary>
        /// Get all products with active status
        /// </summary>
        /// <returns></returns>
        Task<MultipleProductResponse> GetActiveProductsAsync();

        /// <summary>
        /// Get products that product name contains a string
        /// </summary>
        /// <param name="name">similar string</param>
        /// <returns></returns>
        Task<MultipleProductResponse> GetProductsAsync(string name);
        
        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="request">product</param>
        /// <returns></returns>
        Task<BaseResponse> UpdateProductAsync(UpdateProductRequest request);
    }
}