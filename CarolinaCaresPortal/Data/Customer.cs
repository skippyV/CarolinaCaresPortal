using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarolinaCaresPortal.Data
{
    public class Customer
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public AddressRecord Address { get; set; }

        public string PhoneNumber { get; set; }

        public int FamilySize { get; set; }
    }
}
