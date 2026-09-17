namespace CarolinaCaresPortal.Data
{
    public class Customer
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public AddressRecord Address { get; set; }

        public string PhoneNumber { get; set; }

        public int FamilySize { get; set; }
    }
}
