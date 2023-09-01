namespace GeolocationApp.Model
{
    public class APIIpifyModel
    {
        public string ip { get; set; }
        public string domainName { get; set; }

        public Location location { get; set; }

        public class Location
        {
            public string country { get; set; }
            public string region { get; set; }
            public string city { get; set; }
            public double lat { get; set; }
            public double lng { get; set; }
        }
    }
}