using CarolinaCaresPortal.Data;
using CarolinaCaresPortal.Shared;

namespace CarolinaCaresPortal.Services
{
    public interface IMongoDbAccessService
    {
        ResponseStatus CreateCustomerRecord(Customer newCustomer);

        ResponseStatus CreatePantryRecord(FoodPantry newPantry);
    }
}
