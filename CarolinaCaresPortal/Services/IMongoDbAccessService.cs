using CarolinaCaresPortal.Data;
using CarolinaCaresPortal.Shared;

namespace CarolinaCaresPortal.Services
{
    public interface IMongoDbAccessService
    {
        ResponseStatus CreateCustomer(Customer newCustomer);

        ResponseStatus CreatePantryRecord(FoodPantry newPantry);

        ResponseStatus UpdateCustomer(Customer customer);

        List<FoodPantry> GetFoodPantries();

        FoodPantry? GetFoodPantryById(string pantryId);

        Customer GetCustomerById(string customerId);

        ResponseStatus DeletePantryRecord(string pantryId);

        ResponseStatus DeleteCustomer(string customerId);

        List<Customer> GetCustomers();

        ResponseStatus IsMongoDbConnectionActive();
    }
}
