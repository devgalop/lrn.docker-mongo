using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using GraphQL.AspNet.Interfaces.Controllers;
using lrn.devgalop.dockermongo.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace lrn.devgalop.dockermongo.Webapi.Controllers
{
    public class ProductController : GraphController
    {
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        [Query]
        public ProductResponse GetProduct(int id)
        {
            return new()
            {
                Name = "MockProduct",
                UnitPrice = 5.50,
            };
        }

        [Mutation("createProduct")]
        public ProductResponse InsertProductAsync(ProductResponse product)
        {
            try
            {
                Console.WriteLine("Hello");
                return new()
                {
                    Name = "MockProduct",
                    UnitPrice = 5.50,
                };
            }
            catch (Exception)
            {
                return new()
                {
                    Name = "MockProduct",
                    UnitPrice = 5.50,
                };
            }
        }
    }
}