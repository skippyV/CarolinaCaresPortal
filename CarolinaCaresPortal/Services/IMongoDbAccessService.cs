using CarolinaCaresPortal.Data;
using CarolinaCaresPortal.Shared;

namespace CarolinaCaresPortal.Services
{
    public interface IMongoDbAccessService
    {
        ResponseStatus CreateCustomerRecord(Customer newCustomer);

        ResponseStatus CreatePantryRecord(FoodPantry newPantry);

        List<FoodPantry> GetFoodPantries();

        List<Customer> GetCustomers();

    }
}
