using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
using lrn.devgalop.dockermongo.Infrastructure.Data.Interfaces;
using lrn.devgalop.dockermongo.Infrastructure.Data.Repositories;

namespace lrn.devgalop.dockermongo.Infrastructure.Data.Extensions
{
    public static class DataExtensions
    {
        public static void AddMongoDb(this IServiceCollection services)
        {
            string connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") ?? throw new Exception("MONGODB_CONNECTION_STRING variable is required");
            string databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE") ?? throw new Exception("MONGODB_DATABASE variable is required");
            var mongoURL = new MongoUrl(connectionString);
            var client = new MongoClient(mongoURL);
            var database = client.GetDatabase(databaseName) ?? throw new Exception($"Database {databaseName} cannot be found");
            services.AddSingleton(_=> database);
            services.AddTransient<IRepository, MongoDbRepository>();
        }
    }
}