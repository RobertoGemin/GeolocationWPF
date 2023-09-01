using System;
using System.Net;
using System.Text.RegularExpressions;
using GeolocationApp.Model;
using static GeolocationApp.Model.Enums;

namespace GeolocationApp.Methods
{
    public class Validate
    {
        public static string GetResponseStatusCodes(HttpStatusCode httpStatusCode)
        {
            var number = (int)httpStatusCode;
            if (number <= 299) return ".";
            if (number >= 300 && number <= 399) return $"Redirection messages. Code:{number} Name:{httpStatusCode}";
            if (number >= 400 && number <= 499) return $"Client error responses. Code:{number} Name:{httpStatusCode}";
            if (number >= 500 && number <= 599)
                return $"Server error responses. Code:{number} Name:{httpStatusCode}";
            return "Unknown errors.";
        }

      
        public static IpDomainSearchModel ValidateText(string value)
        {
            var id = string.Empty;
            var typeOfParameter = EntryType.None;

            var updateText = removeNoise(value);
            if (!string.IsNullOrEmpty(updateText))
            {
                var validText = getIPAddress(updateText);
                if (!string.IsNullOrEmpty(validText))
                {
                    id = validText;
                    typeOfParameter = EntryType.IPAddress;
                    return IpDomainSearchModel.Success(typeOfParameter, id);
                }

                if (isValidURL(updateText))
                {
                    id = updateText;
                    typeOfParameter = EntryType.DomainName;
                    return IpDomainSearchModel.Success(typeOfParameter, id);
                }
            }

            return IpDomainSearchModel.Empty();
        }

        public static string removeNoise(string searchText)
        {
            searchText = searchText.Replace(" ", string.Empty).Replace(",", ".").Replace("..", ".").TrimEnd('.')
                .ToLower();
            return searchText;
        }

        public static string getIPAddress(string searchText)
        {
            var result = string.Empty;
            if (IPAddress.TryParse(searchText, out var ipaddress))
                result = ipaddress.ToString().Replace("\t", string.Empty);
            return result;
        }


        public static bool isValidURL(string cleanUrl)
        {
            if (TopDomainIsNumeric(cleanUrl)) return false;

            var removeSubDomein = cleanUrl.Replace("www.", string.Empty);
            var Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
            var Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return Rgx.IsMatch(removeSubDomein);
        }

        public static bool TopDomainIsNumeric(string value)
        {
            var segments = value.Split('.');
            var lastSegment = segments[segments.Length - 1];
            return int.TryParse(lastSegment, out _);
        }
    }
}