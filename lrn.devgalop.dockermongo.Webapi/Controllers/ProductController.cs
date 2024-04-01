using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.AspNet.Attributes;
using GraphQL.AspNet.Controllers;
using lrn.devgalop.dockermongo.Core.Models;
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
                IsSucceed = true,
                Name = "MockProduct",
                UnitPrice = 5.50,
                Stock = 2
            };
        }
    }
}