using System.Net.Http.Headers;
using System.Text;
using Logger;

namespace CheckinReminderService
{
    public static class ApiService
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<string> CallApiAsync(ApiSettings apiSettings)
        {
            try
            {
                Logger.Logger.Write("API Call Start", $"Calling {apiSettings.Method} {apiSettings.EndpointUrl}", "", Logger.Logger.LogType.InformationLog);

                HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(apiSettings.Method), apiSettings.EndpointUrl);

                // Bearer token auth
                if (!string.IsNullOrEmpty(apiSettings.BearerToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiSettings.BearerToken);
                }
                // API key auth
                else if (!string.IsNullOrEmpty(apiSettings.ApiKey) && !string.IsNullOrEmpty(apiSettings.ApiKeyHeaderName))
                {
                    request.Headers.Add(apiSettings.ApiKeyHeaderName, apiSettings.ApiKey);
                }

                // Extra headers
                if (apiSettings.Headers != null && apiSettings.Headers.Count > 0)
                {
                    foreach (var header in apiSettings.Headers)
                        request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                // Request body for POST/PUT/PATCH
                if (!string.IsNullOrEmpty(apiSettings.RequestBody) &&
                    (apiSettings.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
                     apiSettings.Method.Equals("PUT", StringComparison.OrdinalIgnoreCase) ||
                     apiSettings.Method.Equals("PATCH", StringComparison.OrdinalIgnoreCase)))
                {
                    request.Content = new StringContent(apiSettings.RequestBody, Encoding.UTF8, apiSettings.ContentType ?? "application/json");
                }
                else if (apiSettings.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
                {
                    // POST with empty body so the server doesn't reject it
                    request.Content = new StringContent("{}", Encoding.UTF8, "application/json");
                }

                if (apiSettings.TimeoutSeconds > 0)
                    httpClient.Timeout = TimeSpan.FromSeconds(apiSettings.TimeoutSeconds);

                HttpResponseMessage response = await httpClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Logger.Logger.Write("API Call Success", $"Status: {(int)response.StatusCode} {response.StatusCode}", responseContent, Logger.Logger.LogType.InformationLog);
                    return responseContent;
                }
                else
                {
                    string error = $"API call failed — HTTP {(int)response.StatusCode} {response.StatusCode}: {responseContent}";
                    Logger.Logger.Write("API Call Failed", error, "", Logger.Logger.LogType.ErrorLog);
                    throw new HttpRequestException(error);
                }
            }
            catch (Exception ex)
            {
                Logger.Logger.Write("API Call Error", "Exception during API call", ex.Message + " | StackTrace: " + ex.StackTrace, Logger.Logger.LogType.ErrorLog);
                throw;
            }
        }
    }
}
