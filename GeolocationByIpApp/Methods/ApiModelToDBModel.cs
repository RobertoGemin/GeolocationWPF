using GeolocationApp.Model;

namespace GeolocationApp.Methods
{
    public class ApiModelToDbModel
    {
    
        public static void ConvertModel(APIIpifyModel apiInfo, out IpAdressModel ipAdressModel, out DomainModel domain)
        {
            domain = new DomainModel();

            ipAdressModel = new IpAdressModel
            {
                Id = apiInfo.ip,
                City = apiInfo.location.country,
                Region = apiInfo.location.region,
                Country = apiInfo.location.country,
                Latitude = apiInfo.location.lat.ToString(),
                Longitude = apiInfo.location.lng.ToString()
            };
            if (!string.IsNullOrEmpty(apiInfo.domainName))
                domain = new DomainModel
                {
                    Name = apiInfo.domainName,
                    IpAdressId = apiInfo.ip
                };
        }
    }
}