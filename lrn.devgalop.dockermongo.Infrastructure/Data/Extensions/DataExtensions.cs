using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;

namespace lrn.devgalop.dockermongo.Infrastructure.Data.Extensions
{
    public static class DataExtensions
    {
        public static void AddMongoDb(this IServiceCollection services)
        {
            string connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") ?? throw new Exception("MONGODB_CONNECTION_STRING variable is required");
            var mongoURL = new MongoUrl(connectionString);
            var client = new MongoClient(mongoURL);
            services.AddSingleton(_=> client);
        }
    }
}