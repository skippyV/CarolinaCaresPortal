using CarolinaCaresPortal.Data;
using CarolinaCaresPortal.Shared;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using Serilog;

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

                FilterDefinitionBuilder<Customer> filterBuilder = Builders<Customer>.Filter;

                FilterDefinition<Customer> filter = filterBuilder.Eq(g => g.LastName, newCustomer.LastName)
                                                    & filterBuilder.Eq(g => g.FirstName, newCustomer.FirstName)
                                                    & filterBuilder.Eq(g => g.PhoneNumber, newCustomer.PhoneNumber);

                List<Customer> results = customers.Find(filter).ToList();

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

        public ResponseStatus CreatePantryRecord(FoodPantry newPantry)
        {
            ResponseStatus responseStatus = new(); // defaults to Success=false, DbActionStatus=DbInfoDetails.notApplicable

            try
            {
                IMongoCollection<FoodPantry> pantries = iMongoDatabase!.GetCollection<FoodPantry>(MongoDbAccessServiceConstants.FoodPantriesCollectionName);

                FilterDefinitionBuilder<FoodPantry> filterBuilder = Builders<FoodPantry>.Filter;

                FilterDefinition<FoodPantry> filter = filterBuilder.Eq(g => g.Name, newPantry.Name)
                                                    & filterBuilder.Eq(g => g.PhoneNumber, newPantry.PhoneNumber);

                List<FoodPantry> results = pantries.Find(filter).ToList();

                if (results.Count == 0) // no record found so create one
                {
                    pantries.InsertOne(newPantry);
                    responseStatus.StatusMessage = $"Food Pantry {newPantry.Name} was created!";
                    responseStatus.DbActionStatus = DbInfoDetails.recordUpdated;

                    Log.Information(responseStatus.StatusMessage);
                }
                else
                {
                    responseStatus.StatusMessage = $"Food Pantry {newPantry.Name} already exists! No changes made.";
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

        public List<Customer> GetCustomers()
        {
            IMongoCollection<Customer> customers = iMongoDatabase!.GetCollection<Customer>(MongoDbAccessServiceConstants.CustomersCollectionName);

            try
            {
                List<Customer> documents = customers.Find(Builders<Customer>.Filter.Empty).ToList();
                return documents;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + ":::" + ex.StackTrace);
                return [];
            }
        }

        public List<FoodPantry> GetFoodPantries()
        {
            IMongoCollection<FoodPantry> pantries = iMongoDatabase!.GetCollection<FoodPantry>(MongoDbAccessServiceConstants.FoodPantriesCollectionName);

            try
            {
                List<FoodPantry> documents = pantries.Find(Builders<FoodPantry>.Filter.Empty).ToList();
                return documents;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + ":::" + ex.StackTrace);
                return [];
            }
        }
    }

    public static class MongoDbAccessServiceConstants
    {
        public const string CustomersCollectionName = "Customers";
        public const string FoodPantriesCollectionName = "FoodPantries";
    }
}
