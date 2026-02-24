namespace CheckinReminderService
{
    public class ApiSettings
    {
        public string EndpointUrl { get; set; } = string.Empty;
        public string Method { get; set; } = "POST";
        public string? RequestBody { get; set; }
        public string? ContentType { get; set; } = "application/json";
        public string? BearerToken { get; set; }
        public string? ApiKey { get; set; }
        public string? ApiKeyHeaderName { get; set; } = "x-api-key";
        public Dictionary<string, string>? Headers { get; set; }
        public int TimeoutSeconds { get; set; } = 60;
    }
}
