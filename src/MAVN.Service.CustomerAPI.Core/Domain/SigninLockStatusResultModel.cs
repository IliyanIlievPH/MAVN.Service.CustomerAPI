namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class SigninLockStatusResultModel
    {
        public bool IsLocked { get; set; }
        
        public int RetryPeriodInMinutesWhenLocked { get; set; }
        
        public string Message { get; set; }

        public static SigninLockStatusResultModel Locked(int lockPeriodInMinutes)
        {
            return new SigninLockStatusResultModel
            {
                IsLocked = true,
                RetryPeriodInMinutesWhenLocked = lockPeriodInMinutes,
                Message = $"Your account has been locked. Try again in {lockPeriodInMinutes} min."
            };
        }

        public static SigninLockStatusResultModel NotLocked()
        {
            return new SigninLockStatusResultModel {IsLocked = false, RetryPeriodInMinutesWhenLocked = 0};
        }
    }
}
