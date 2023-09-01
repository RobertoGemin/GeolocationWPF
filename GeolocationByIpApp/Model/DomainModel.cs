using SQLite;

namespace GeolocationApp.Model
{
    public class DomainModel
    {
        [PrimaryKey] 
        public string Name { get; set; }

        public string IpAdressId { get; set; }
    }
}