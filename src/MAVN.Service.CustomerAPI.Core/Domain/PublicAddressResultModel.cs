namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class PublicAddressResultModel
    {
        public string PublicAddress { get; set; }
        public PublicAddressError Error { get; set; }
        
        public PublicAddressStatus Status { get; set; }
    }
}
