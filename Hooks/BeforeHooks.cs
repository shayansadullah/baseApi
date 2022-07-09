using Ensek.Helpers;
using Ensek.Models;
using Newtonsoft.Json;
using RegressionPack.Helpers;
using RestSharp;

namespace Ensek.Hooks
{
    [Binding]
    public class BeforeHooks
    {
        private static string? _token;

        [BeforeFeature]
        public static async Task BeforeFeature()
        {
            await Login();
        }

        [BeforeScenario]
        public async Task ResetTestData()
        {
            await Reset();
        }

        public static async Task Login()
        {
            var loginDetails = new Models.Login
            {
                username = "test",
                password = "testing"
            };
            var serializedObject = JsonConvert.SerializeObject(loginDetails);

            var retryPolicy = RetryHelper.HandleExceptionByWaitAndRetry(numberOfRetries: 5);
            var attempts = 0;
            IRestResponse response;

            await retryPolicy.ExecuteAsync(async () =>
            {
                attempts++;
                response = await RestSharApiHelper.SendRequest(
                    $"{Constants.RootUrl}/login", 
                    Method.POST, 
                    serializedObject);

                if (!response.IsSuccessful)
                {
                    throw new Exception($"failed with response code: {response?.StatusCode} " +
                                        $"and response content: {response?.Content}");
                }

                var loginResponseObject = JsonConvert.DeserializeObject<LoginResponse>(response.Content);                    
                _token = loginResponseObject?.access_token;
            });
        }

        public async Task Reset()
        {
            if (_token == null)
            {
                return;
            }
            var retryPolicy = RetryHelper.HandleExceptionByWaitAndRetry(numberOfRetries: 5);
            var attempts = 0;
            IRestResponse response;

            await retryPolicy.ExecuteAsync(async () =>
            {
                attempts++;
                response = await RestSharApiHelper.SendRequest(
                    $"{Constants.RootUrl}/reset", 
                    Method.POST, 
                    null, 
                    _token);

                if (!response.IsSuccessful)
                {
                    throw new Exception($"failed with response code: {response?.StatusCode} " +
                                        $"and response content: {response?.Content}");
                }
                               
                var loginResponseObject = JsonConvert.DeserializeObject<LoginResponse>(response.Content);

                _token = loginResponseObject?.access_token;
            });
        }
    }
}
