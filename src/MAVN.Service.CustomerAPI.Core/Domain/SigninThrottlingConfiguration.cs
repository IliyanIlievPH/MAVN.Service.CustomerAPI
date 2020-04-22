using System;

namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class SigninThrottlingConfiguration
    {
        public int WarningThreshold { get; set; }
        
        public int LockThreshold { get; set; }
        
        public TimeSpan ThresholdPeriod { get; set; }
        
        public TimeSpan AccountLockPeriod { get; set; }
    }
}
