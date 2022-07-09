using RestSharp;
using NLog;

namespace Ensek.Helpers
{
    public static class RestSharApiHelper
    {
        internal static async Task<IRestResponse> SendRequest(string api,
                                                                Method method,
                                                                string? serlializedObject = null,
                                                                string? token = null)
        {
            var client = new RestClient(api);
            client.Timeout = 30000;
            var request = new RestRequest(method);
            
            if (token != null)
            {
               request.AddHeader("Authorization", "Bearer " + token);
            }
            if (method == Method.POST || method == Method.PUT)
            {
                request.AddHeader("accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", serlializedObject ?? String.Empty, ParameterType.RequestBody);
            }
            IRestResponse response = await client.ExecuteAsync(request);
            return response;
        }

    }
}
