using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
namespace PhysicsDemo.Data.WebHooks
{
    public class WebhookEvent
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string PlayerName { get; set; } = "";
        public Guid UserID { get; set; }
        public Guid GameID { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;
    }
    public class WebHookService
    {
        public event Action<string>? OnWebhookReceived;
        public void RaiseEvent(string json) => OnWebhookReceived?.Invoke(json);
        private readonly HttpClient _http;
        private readonly string _secret;
        private string EndpointUrl => "https://physicsdemo.azurewebsites.net/webhook";
        //private string EndpointUrl => "http://localhost:5218/webhook";
        public WebHookService(HttpClient httpClient, IConfiguration config)
        {
            _http = httpClient;
            _secret = config["Keys:WebHook"] ?? throw new Exception("Signing secret missing");
        }
        public async Task<bool> SendWebhookAsync(WebhookEvent evt)
        {
            string json = JsonSerializer.Serialize(evt);
            string signature = ComputeSignature(json);

            var request = new HttpRequestMessage(HttpMethod.Post, EndpointUrl)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("X-Webhook-Signature", signature);

            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        private string ComputeSignature(string payload)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return Convert.ToHexString(hash);
        }
        public bool ValidateSignature(string payload, string signature)
        {
            /*using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var computed = Convert.ToHexString(hash);*/
            string result = ComputeSignature(payload);
            return result.Equals(signature, StringComparison.OrdinalIgnoreCase);
        }
    }
}