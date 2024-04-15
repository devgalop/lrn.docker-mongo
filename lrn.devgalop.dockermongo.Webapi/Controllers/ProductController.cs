using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using GraphQL.AspNet.Interfaces.Controllers;
using lrn.devgalop.dockermongo.Core.Interfaces;
using lrn.devgalop.dockermongo.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace lrn.devgalop.dockermongo.Webapi.Controllers
{
    public class ProductController : GraphController
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductManagementService _productService;

        public ProductController(
            ILogger<ProductController> logger,
            IProductManagementService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        [Query]
        public async Task<IEnumerable<ProductResponse>?> GetProducts()
        {
            var productsResponse = await _productService.GetActiveProductsAsync();
            return productsResponse.Products;
        }

        [Mutation("createProduct")]
        public async Task<BaseResponse> InsertProductAsync(InsertProductRequest request)
        {
            try
            {
                var response = await _productService.CreateProductAsync(request);
                if(!response.IsSucceed) throw new Exception(response.ErrorMessage);
                return new()
                {
                    IsSucceed = true
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    IsSucceed = true,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}