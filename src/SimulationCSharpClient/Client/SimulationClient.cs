using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SimulationCSharpClient.Client
{
    /// <summary>
    /// SimulationClient adds functionality to the generated SimulationClient
    /// that enables construction with either username / password or client ID / Secret
    /// </summary>
    public partial class SimulationClient
    {
        /// <summary>
        /// Constructor used when access the API via client and secret
        /// </summary>
        /// <param name="baseUrl">URL or API endpoint - no trailing backslash</param>
        /// <param name="auth0TokenURL">Authentication URL endpoing</param>
        /// <param name="client">client key</param>
        /// <param name="secret">client secret</param>
        /// <param name="audience">client audience</param>
        /// <param name="httpClient">injected http client</param>
        /// <param name="maxRetries">http maxRetries.</param>
        /// <param name="handler">http client handler.</param>
        public SimulationClient(string baseUrl, string auth0TokenURL, string client, string secret, string audience, HttpClient httpClient = null, int maxRetries = 6, HttpClientHandler handler = null)
        {
            this.BaseUrl = baseUrl;
            this._httpClient = httpClient;

            if (this._httpClient == null)
            {
                this._httpClient = new HttpClient(new HttpRetryMessageHandler(maxRetries, handler));
            }

            this._settings = new System.Lazy<Newtonsoft.Json.JsonSerializerSettings>(() =>
            {
                var settings = new Newtonsoft.Json.JsonSerializerSettings();
                this.UpdateJsonSerializerSettings(settings);
                return settings;
            });

            this.AccessToken = this.GetClientAuthToken(auth0TokenURL, client, secret, audience);
        }

        /// <summary>
        /// Constructor used when access the API via username and password
        /// </summary>
        /// <param name="baseUrl">URL or API endpoint - no trailing backslash</param>
        /// <param name="auth0TokenURL">Authentication URL endpoing</param>
        /// <param name="client">Client</param>
        /// <param name="userName">username - usually an email address</param>
        /// <param name="password">password</param>
        /// <param name="connection">user database - values can be found here: https://github.com/3DSIM/simulation-api-specification</param>
        /// <param name="httpClient">injected http client</param>
        /// <param name="maxRetries">http maxRetries.</param>
        /// <param name="handler">handler.</param>
        public SimulationClient(string baseUrl, string auth0TokenURL, string client, string userName, string password, string connection, HttpClient httpClient = null, int maxRetries = 6, HttpClientHandler handler = null)
        {
            this.BaseUrl = baseUrl;
            this._httpClient = httpClient;

            if (this._httpClient == null)
            {
                this._httpClient = new HttpClient(new HttpRetryMessageHandler(maxRetries, handler));
            }

            this._settings = new System.Lazy<Newtonsoft.Json.JsonSerializerSettings>(() =>
            {
                var settings = new Newtonsoft.Json.JsonSerializerSettings();
                this.UpdateJsonSerializerSettings(settings);
                return settings;
            });

            this.AccessToken = this.GetUserAuthToken(auth0TokenURL, client, userName, password, connection);
        }

        public string AccessToken { get; private set; }

        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);
        }

        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, System.Text.StringBuilder urlBuilder)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);
        }

        private string GetClientAuthToken(string auth0TokenURL, string client, string secret, string audience)
        {
            string token = string.Empty;

            var tokenRequest = new TokenRequest();
            tokenRequest.ClientId = client;
            tokenRequest.ClientSecret = secret;
            tokenRequest.Audience = audience;

            HttpClient authClient = new HttpClient();

            try
            {
                HttpContent content = new StringContent(JsonConvert.SerializeObject(tokenRequest), Encoding.UTF8, "application/json");
                HttpResponseMessage response = authClient.PostAsync(auth0TokenURL, content).Result;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Authorization Http Response Error: " + response.StatusCode.ToString());
                }

                string responseBody = response.Content.ReadAsStringAsync().Result;
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);

                if (tokenResponse.Error != null && tokenResponse.Error != string.Empty)
                {
                    throw new Exception("Authorization Error: " + tokenResponse.ErrorDescription);
                }
                else
                {
                    return tokenResponse.AccessToken;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Authorization Error", e);
            }
            finally
            {
                authClient.Dispose();
            }
        }

        private string GetUserAuthToken(string auth0TokenURL, string client, string userName, string password, string connection)
        {
            string token = string.Empty;

            var tokenRequest = new UserTokenRequest();
            tokenRequest.UserName = userName;
            tokenRequest.Password = password;
            tokenRequest.Connection = connection;
            tokenRequest.ClientId = client;

            HttpClient authClient = new HttpClient();

            try
            {
                HttpContent content = new StringContent(JsonConvert.SerializeObject(tokenRequest), Encoding.UTF8, "application/json");
                HttpResponseMessage response = authClient.PostAsync(auth0TokenURL, content).Result;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Authorization Http Response Error: " + response.StatusCode.ToString());
                }

                string responseBody = response.Content.ReadAsStringAsync().Result;
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);

                if (tokenResponse.Error != null && tokenResponse.Error != string.Empty)
                {
                    throw new Exception("Authorization Error: " + tokenResponse.ErrorDescription);
                }
                else
                {
                    return tokenResponse.IdToken;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Authorization Error", e);
            }
            finally
            {
                authClient.Dispose();
            }
        }
    }
}
