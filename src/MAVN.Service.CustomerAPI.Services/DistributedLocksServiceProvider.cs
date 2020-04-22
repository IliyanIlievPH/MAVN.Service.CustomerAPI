using System;
using Autofac.Features.Indexed;
using MAVN.Service.CustomerAPI.Core.Domain;
using MAVN.Service.CustomerAPI.Core.Services;

namespace MAVN.Service.CustomerAPI.Services
{
    public class DistributedLocksServiceProvider : IDistributedLocksServiceProvider
    {
        private readonly IIndex<DistributedLockPurpose, IDistributedLocksService> _distributedLockServices;

        public DistributedLocksServiceProvider(IIndex<DistributedLockPurpose, IDistributedLocksService> distributedLockServices)
        {
            _distributedLockServices = distributedLockServices;
        }

        public IDistributedLocksService Get(DistributedLockPurpose lockPurpose)
        {
            if (!_distributedLockServices.TryGetValue(lockPurpose, out var service))
            {
                throw new InvalidOperationException($"Distributed lock service of type [{lockPurpose.ToString()}] not found");
            }

            return service;
        }
    }
}
