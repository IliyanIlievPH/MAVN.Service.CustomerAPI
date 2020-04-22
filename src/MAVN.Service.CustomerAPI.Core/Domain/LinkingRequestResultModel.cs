namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class LinkingRequestResultModel
    {
        public string LinkCode { get; set; }

        public LinkingError Error { get; set; }
    }
}
