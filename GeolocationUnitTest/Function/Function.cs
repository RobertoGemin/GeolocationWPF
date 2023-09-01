using System.Collections.Generic;
using GeolocationApp.Model;
using static GeolocationApp.Model.APIIpifyModel;

namespace GeolocationUnitTest.Function
{
    public class Function
    {
        public static List<DomainModel> GetWPDomainList(string ipAddress, params string[] domainNames)
        {
            var domainList = new List<DomainModel>();

            if (domainNames == null || domainNames.Length == 0) return null;

            foreach (var domainName in domainNames)
                domainList.Add(new DomainModel { Name = domainName, IpAdressId = ipAddress });

            return domainList;
        }

        public static IpAdressModel GetIpAdress(string validIPAddress, bool returnEmpty = false)
        {
            if (returnEmpty || string.IsNullOrEmpty(validIPAddress)) return null;
            return new IpAdressModel
            {
                Id = validIPAddress,
                City = "Ursynów",
                Region = "Mazovia",
                Country = "PL",
                Latitude = "52.1540",
                Longitude = "21.0514"
            };
        }

        public static DomainModel GetDomain(string validIPAddress, string validDomain)
        {
            return new DomainModel
            {
                Name = validDomain,
                IpAdressId = validIPAddress
            };
        }

        public static APIIpifyModel GetAPIIpifyModel(string validIPAddress, string domainName)
        {
            return new APIIpifyModel
            {
                ip = validIPAddress,
                domainName = domainName,
                location = new Location
                {
                    country = "PL",
                    region = "Mazovia",
                    city = "Ursynów",
                    lat = 52.1540d,
                    lng = 52.1540d
                }
            };
        }

        public static string getErrorMessageFromSearchModelIPadress(IpDomainSearchModel searchModel)
        {
            return $"{searchModel.EntryDataType.ToString()} \"{searchModel.Id}\" Not exist";
        }
    }
}