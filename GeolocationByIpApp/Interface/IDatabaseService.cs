using System.Collections.Generic;
using System.Threading.Tasks;
using GeolocationApp.Model;

namespace GeolocationApp.Interface
{
    public interface IDatabaseService
    {
        Task<bool> ValidateDatabase();
        Task<bool> CheckTableExist<T>(T model);
        Task<bool> CreateTable<T>(T model);
        Task<bool> Insert(IpAdressModel ipAdressModel, DomainModel domainModel);
        Task<bool> Delete(IpDomainSearchModel searchModel);
        Task<IpAdressModel> GetIpAdress(IpDomainSearchModel searchModel);
        Task<List<DomainModel>> GetDomainList(IpDomainSearchModel searchModel);
    }
}