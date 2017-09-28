using Newtonsoft.Json;

namespace SimulationCSharpClient.Client
{
    public class UserTokenRequest
    {
        public UserTokenRequest()
        {
            this.Scope = "openid email user_metadata app_metadata picture";
        }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("connection")]
        public string Connection { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }
    }
}
