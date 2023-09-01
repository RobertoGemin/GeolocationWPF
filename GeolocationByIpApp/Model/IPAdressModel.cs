using SQLite;

namespace GeolocationApp.Model
{
    public class IpAdressModel
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}