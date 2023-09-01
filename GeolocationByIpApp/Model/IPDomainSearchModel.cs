using static GeolocationApp.Model.Enums;

namespace GeolocationApp.Model
{
    public class IpDomainSearchModel
    {
        public IpDomainSearchModel()
        {
            EntryDataType = EntryType.None;
            Id = string.Empty;
        }

        private IpDomainSearchModel(EntryType entryDataType, string id)
        {
            EntryDataType = entryDataType;
            Id = id;
        }

        public EntryType EntryDataType { get; private set; }
        public string Id { get; private set; }

        public static IpDomainSearchModel Success(EntryType entryType, string id = "")
        {
            return new IpDomainSearchModel(entryType, id);
        }

        public static IpDomainSearchModel Empty()
        {
            return new IpDomainSearchModel();
        }
    }
}