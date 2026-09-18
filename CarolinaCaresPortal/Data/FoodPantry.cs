using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarolinaCaresPortal.Data
{
    public class FoodPantry
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public string Name { get; set; }

        public AddressRecord Address { get; set; }

        public string PhoneNumber { get; set; }

        public string DayOfWeek { get; set; }

        public string TimeOfDay { get; set; }
    }
}
