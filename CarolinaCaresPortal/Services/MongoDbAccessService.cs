using CarolinaCaresPortal.Data;
using CarolinaCaresPortal.Shared;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using Serilog;
using System.Xml.Linq;

namespace CarolinaCaresPortal.Services
{
    public class MongoDbAccessService : IMongoDbAccessService
    {
        private MongoClient? mongoClient;
        private IMongoDatabase? iMongoDatabase;

        public MongoDbAccessService(MongoDbConfig config)
        {
            using var loggerFactory = LoggerFactory.Create(b =>
            {
                // b.AddConfiguration(configLogger);
                b.AddSerilog();
                //b.AddSimpleConsole();
                b.SetMinimumLevel(LogLevel.Debug);
            });

            MongoClientSettings mongoClientSettings = MongoClientSettings.FromConnectionString(config.MongoDbConnectionString);

            mongoClientSettings.LoggingSettings = new LoggingSettings(loggerFactory);
            mongoClient = new MongoClient(mongoClientSettings);
            iMongoDatabase = mongoClient.GetDatabase(config.CarolinaCaresCustomersDbName);

            iMongoDatabase!.CreateCollection(MongoDbAccessServiceConstants.FoodPantriesCollectionName);

            iMongoDatabase!.CreateCollection(MongoDbAccessServiceConstants.CustomersCollectionName);
        }

        public ResponseStatus CreateCustomerRecord(Customer newCustomer)
        {
            ResponseStatus responseStatus = new(); // defaults to Success=false, DbActionStatus=DbInfoDetails.notApplicable

            try
            {
                IMongoCollection<Customer> customers = iMongoDatabase!.GetCollection<Customer>(MongoDbAccessServiceConstants.CustomersCollectionName);

                var filterBuilder = Builders<Customer>.Filter;
                //var filter = filterBuilder.Eq(g => g.LastName, newCustomer.LastName) & filterBuilder.Eq(h => h.FirstName, newCustomer.FirstName);
                var filter = filterBuilder.Eq(g => g.LastName, newCustomer.LastName) ;
                var results = customers.Find(filter).ToList();

                if (results.Count == 0) // no record found so create one
                {
                    customers.InsertOne(newCustomer);
                    responseStatus.StatusMessage = $"Customer {newCustomer.FirstName} was created!";
                    responseStatus.DbActionStatus = DbInfoDetails.recordUpdated;

                    Log.Information(responseStatus.StatusMessage);
                }
                else
                {
                    responseStatus.StatusMessage = $"Customer {newCustomer.FirstName} {newCustomer.LastName} already exists! No changes made.";
                    responseStatus.DbActionStatus = DbInfoDetails.noChangesMade;
                }
            }
            catch (Exception ex)
            {
                responseStatus.DbActionStatus = DbInfoDetails.errorOccurred;
                responseStatus.StatusMessage = ex.Message + ":::" + ex.StackTrace;
                Log.Error(responseStatus.StatusMessage);
            }

            return responseStatus;
        }
    }

    public static class MongoDbAccessServiceConstants
    {
        public const string CustomersCollectionName = "Customers";
        public const string FoodPantriesCollectionName = "FoodPantries";
    }
}
