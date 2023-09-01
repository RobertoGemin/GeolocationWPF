using GeolocationApp.Model;

namespace GeolocationApp.Interface
{
    public interface IInputValidationService
    {
        IpDomainSearchModel GetIpDomainSearchModel(string input);
    }
}