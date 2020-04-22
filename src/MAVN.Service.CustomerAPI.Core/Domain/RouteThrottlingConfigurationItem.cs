namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class RouteThrottlingConfigurationItem
    {
        public string Verb { get; set; }
        
        public string Route { get; set; }
        
        public decimal FrequencyInSeconds { get; set; }
    }
}
