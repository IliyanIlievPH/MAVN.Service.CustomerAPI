using MAVN.Service.CustomerAPI.Core.Domain;

namespace MAVN.Service.CustomerAPI.Core.Services
{
    public interface IDistributedLocksServiceProvider
    {
        IDistributedLocksService Get(DistributedLockPurpose lockPurpose);
    }
}
