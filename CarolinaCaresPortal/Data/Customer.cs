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

        public List<DateOnly> Attendance = [];

        public List<DateOnly> GetAttendanceRecords()
        {
            return Attendance;
        }

        public DateOnly? GetLastAttendanceRecord()
        {
            if (Attendance.Count > 0)
            {
                // return Attendance[Attendance.Count - 1];
                return Attendance[^1]; // this is the Index operator https://blog.ndepend.com/c-index-and-range-operators-explained/
            }
            else
            {
                return null;
            }
        }

        public DateOnly? GetNextToLastAttendanceRecord()
        {
            if (Attendance.Count > 1)
            {
                //return Attendance[Attendance.Count - 2]; 
                return Attendance[^2]; // this is the Index operator https://blog.ndepend.com/c-index-and-range-operators-explained/
            }
            else
            {
                return null;
            }
        }

        public void AddAttendanceRecord(DateOnly value)
        {
            if(!Attendance.Contains(value))
            {  
                Attendance.Add(value); 
            }
        }
    }
}
