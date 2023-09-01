using System.Threading.Tasks;
using GeolocationApp.Model;

namespace GeolocationApp.Interface
{
    public interface IApiService
    {
        Task<bool> IsApiValid();
        Task<APIIpifyModel> GetData(IpDomainSearchModel apiIParameter);
    }
}