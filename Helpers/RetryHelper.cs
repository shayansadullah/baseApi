
using Polly;
using Polly.Retry;
using System;

namespace RegressionPack.Helpers
{
    public static class RetryHelper
    {
        public static AsyncRetryPolicy HandleExceptionByWaitAndRetry(int numberOfRetries, int intervalInSeconds=5)
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(numberOfRetries, retryAttempt =>
                TimeSpan.FromSeconds(intervalInSeconds));
            return retryPolicy;
        }
    }
}
