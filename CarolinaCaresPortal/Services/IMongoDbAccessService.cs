using CarolinaCaresPortal.Data;
using CarolinaCaresPortal.Shared;

namespace CarolinaCaresPortal.Services
{
    public interface IMongoDbAccessService
    {
        ResponseStatus CreateCustomerRecord(Customer newCustomer);

        ResponseStatus CreatePantryRecord(FoodPantry newPantry);

        List<FoodPantry> GetFoodPantries();

        FoodPantry GetFoodPantryById(string pantryId);

        ResponseStatus DeletePantryRecord(string pantryId);

        List<Customer> GetCustomers();

    }
}
