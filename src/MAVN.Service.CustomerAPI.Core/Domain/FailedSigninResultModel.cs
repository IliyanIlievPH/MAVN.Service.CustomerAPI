namespace MAVN.Service.CustomerAPI.Core.Domain
{
    public class FailedSigninResultModel
    {
        public FailedSigninEffect Effect { get; set; }
        public int AttemptsLeftBeforeLock { get; set; }
        public int RetryPeriodInMinutesWhenLocked { get; set; }
        public string Message { get; set; }

        public static FailedSigninResultModel SigninLocked(int lockPeriodInMinutes)
        {
            return new FailedSigninResultModel
            {
                Effect = FailedSigninEffect.SigninLocked,
                AttemptsLeftBeforeLock = 0,
                RetryPeriodInMinutesWhenLocked = lockPeriodInMinutes,
                Message = $"Your account has been locked. Try again in {lockPeriodInMinutes} min."
            };
        }

        public static FailedSigninResultModel Warning(int attemptsLeftBeforeLock)
        {
            return new FailedSigninResultModel
            {
                Effect = FailedSigninEffect.Warning,
                AttemptsLeftBeforeLock = attemptsLeftBeforeLock,
                RetryPeriodInMinutesWhenLocked = 0,
                Message = $"You have {attemptsLeftBeforeLock} more attempts to sign in, or your account will be temporarily locked."
            };
        }

        public static FailedSigninResultModel NoImpact(int attemptsLeftBeforeLock)
        {
            return new FailedSigninResultModel
            {
                Effect = FailedSigninEffect.None,
                AttemptsLeftBeforeLock = attemptsLeftBeforeLock,
                RetryPeriodInMinutesWhenLocked = 0,
                Message = $"You have {attemptsLeftBeforeLock} more attempts to sign in, or your account will be temporarily locked."
            };
        }
    }
}
