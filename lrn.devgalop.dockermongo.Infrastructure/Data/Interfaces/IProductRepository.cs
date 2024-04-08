using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lrn.devgalop.dockermongo.Infrastructure.Data.Models;

namespace lrn.devgalop.dockermongo.Infrastructure.Data.Interfaces
{
    public interface IProductRepository
    {
        Task<BaseResponse> CreateProductAsync(Product product, CancellationToken ct = default);
        Task<ProductResponse> GetProductAsync(string uniqueId, CancellationToken ct = default);
        Task<ProductsResponse> GetProductsByNameAsync(string name, CancellationToken ct = default);
        Task<ProductsResponse> GetProductsByStatusAsync(bool status = true, CancellationToken ct = default);
        Task<BaseResponse> UpdateProductAsync (string uniqueId, UpdateProductRequest request, CancellationToken ct = default);
    }
}