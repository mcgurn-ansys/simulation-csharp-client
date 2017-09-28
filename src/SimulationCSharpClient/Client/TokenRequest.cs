using Newtonsoft.Json;

namespace SimulationCSharpClient.Client
{
    public class TokenRequest
    {
        public TokenRequest()
        {
            this.GrantType = "client_credentials";
        }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        [JsonProperty("audience")]
        public string Audience { get; set; }

        [JsonProperty("grant_type")]
        public string GrantType { get; set; }
    }
}
