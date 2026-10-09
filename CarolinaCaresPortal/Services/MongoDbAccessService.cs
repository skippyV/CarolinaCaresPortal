using CarolinaCaresPortal.Data;
using CarolinaCaresPortal.Shared;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Misc;
using Serilog;

namespace CarolinaCaresPortal.Services
{
    public class MongoDbAccessService : IMongoDbAccessService
    {
        private MongoClient? mongoClient;
        private IMongoDatabase? iMongoDatabase;

        public MongoDbAccessService(MongoDbConfig config)
        {
            try
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
            catch(Exception ex)
            {
                string errorMessage = "Exception in constructor of MongoDb service!Ensure MongoDb is running.";
                Log.Error(errorMessage);
                //throw new InvalidOperationException(errorMessage, ex);
                throw new InvalidOperationException(errorMessage);
            }

            
        }

        public ResponseStatus CreateCustomer(Customer newCustomer)
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

        public FoodPantry? GetFoodPantryById(string pantryId)
        {
            try
            {
                IMongoCollection<FoodPantry> PantriesCollection = iMongoDatabase!.GetCollection<FoodPantry>
                    (MongoDbAccessServiceConstants.FoodPantriesCollectionName);

                FilterDefinition<FoodPantry> filter = Builders<FoodPantry>.Filter.Eq(e => e.Id, pantryId);
                List<FoodPantry> findResults = PantriesCollection.Find(filter).ToList();

                if (findResults.Count == 1)
                {
                    return findResults.First();
                }
                else
                {
                    Log.Warning($"{nameof(MongoDbAccessService.GetFoodPantryById)} returned {findResults.Count}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace!);
            }

            return null;
        }

        public ResponseStatus DeletePantryRecord(string pantryId)
        {
            ResponseStatus responseStatus = new(); // defaults to Success=false, DbActionStatus=DbInfoDetails.notApplicable

            try
            {
                IMongoCollection<FoodPantry> PantriesCollection = iMongoDatabase!.GetCollection<FoodPantry>
                    (MongoDbAccessServiceConstants.FoodPantriesCollectionName);

                FilterDefinition<FoodPantry> findFoodPantryFilter = Builders<FoodPantry>.Filter.Eq(e => e.Id, pantryId);

                DeleteResult deletePantryRecordResult = PantriesCollection.DeleteOne(findFoodPantryFilter);

                if (deletePantryRecordResult.DeletedCount == 1)
                {
                    responseStatus.StatusMessage += " Pantry record was deleted.";
                    responseStatus.Success = true;
                }
                else
                {
                    responseStatus.StatusMessage = $"Program Error No records matched in {nameof(MongoDbAccessService.DeletePantryRecord)}";
                    responseStatus.DbActionStatus = DbInfoDetails.errorOccurred;
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

        public Customer GetCustomerById(string customerId)
        {
            try
            {
                IMongoCollection<Customer> CustomersCollection = iMongoDatabase!.GetCollection<Customer>
                    (MongoDbAccessServiceConstants.CustomersCollectionName);

                FilterDefinition<Customer> filter = Builders<Customer>.Filter.Eq(e => e.Id, customerId);
                List<Customer> findResults = CustomersCollection.Find(filter).ToList();

                if (findResults.Count == 1)
                {
                    return findResults.First();
                }
                else
                {
                    Log.Warning($"{nameof(MongoDbAccessService.GetCustomerById)} returned {findResults.Count}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace!);
            }

            return null;
        }

        public ResponseStatus DeleteCustomer(string customerId)
        {
            ResponseStatus responseStatus = new(); // defaults to Success=false, DbActionStatus=DbInfoDetails.notApplicable

            try
            {
                IMongoCollection<Customer> CustomersCollection = iMongoDatabase!.GetCollection<Customer>
                    (MongoDbAccessServiceConstants.CustomersCollectionName);

                FilterDefinition<Customer> findCustomerFilter = Builders<Customer>.Filter.Eq(e => e.Id, customerId);

                DeleteResult deleteCustomerResult = CustomersCollection.DeleteOne(findCustomerFilter); 

                if (deleteCustomerResult.DeletedCount == 1)
                {
                    responseStatus.StatusMessage += " Customer record was deleted.";
                    responseStatus.Success = true;
                }
                else
                {
                    responseStatus.StatusMessage = $"Program Error No records matched in {nameof(MongoDbAccessService.DeleteCustomer)}";
                    responseStatus.DbActionStatus = DbInfoDetails.errorOccurred;
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

        public ResponseStatus UpdateCustomer(Customer customer)
        {
            ResponseStatus responseStatus = new(); // defaults to Success=false, DbActionStatus=DbInfoDetails.notApplicable

            try
            {
                IMongoCollection<Customer> CustomersCollection = iMongoDatabase!.GetCollection<Customer>
                    (MongoDbAccessServiceConstants.CustomersCollectionName);

                FilterDefinition<Customer> findCustomerFilter = Builders<Customer>.Filter.Eq(e => e.Id, customer.Id);

                ReplaceOneResult replaceResult = CustomersCollection.ReplaceOne(d => d.Id == customer.Id, customer);

                if(replaceResult.ModifiedCount > 0)
                {
                    responseStatus.Success = true;
                    responseStatus.StatusMessage = $"Customer {customer.FirstName} {customer.LastName} was updated";
                }     
                else
                {
                    responseStatus.StatusMessage = "Warning - no modifications made for UpdateCustomer()";
                    responseStatus.DbActionStatus= DbInfoDetails.noChangesMade;
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

        public ResponseStatus UpdatePantry(FoodPantry pantry)
        {
            ResponseStatus responseStatus = new(); // defaults to Success=false, DbActionStatus=DbInfoDetails.notApplicable

            try
            {
                IMongoCollection<FoodPantry> PantriesCollection = iMongoDatabase!.GetCollection<FoodPantry>
                    (MongoDbAccessServiceConstants.FoodPantriesCollectionName);

                FilterDefinition<Customer> findCustomerFilter = Builders<Customer>.Filter.Eq(e => e.Id, pantry.Id);

                ReplaceOneResult replaceResult = PantriesCollection.ReplaceOne(d => d.Id == pantry.Id, pantry);

                if (replaceResult.ModifiedCount > 0)
                {
                    responseStatus.Success = true;
                    responseStatus.StatusMessage = $"Pantry {pantry.Name} was updated";
                }
                else
                {
                    responseStatus.StatusMessage = "Warning - no modifications made for UpdatePantry()";
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

        public ResponseStatus IsMongoDbConnectionActive()
        {
            ResponseStatus responseStatus = new();
            try
            {
                using IAsyncCursor<string> returnedCursor = mongoClient!.ListDatabaseNames();
                bool returnedAny = returnedCursor.Any();
                if (returnedAny)
                {
                    responseStatus.Success = true;
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
