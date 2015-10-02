using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Eve.Authenticators;

namespace Sentinel
{
    public class SentinelClient
    {
        public async Task<BearerAuthenticator> GetBearerAuthenticator(bool forceRefresh = false)
        {
            var token = await GetBearerToken(forceRefresh);
            return token == null ? null : new BearerAuthenticator(token.AccessToken);
        }
        public async Task<Token> GetBearerToken(bool forceRefresh=false)
        {
            Validate();

	    // TODO support for handling a different cache type (maybe get cache instance passwed as a proprierty)
	    var cache = new Cache.SqLiteTokenCache();
            if (!forceRefresh) {
                Token = await cache.FetchAsync(Username);
                if (Token != null) return Token;
            }
            
            HttpResponse = await PerformTokenRequest();
            if (HttpResponse.StatusCode != HttpStatusCode.OK)
                return null;

            var json = await HttpResponse.Content.ReadAsStringAsync();
            Token = JsonConvert.DeserializeObject<Token>(json);
            Token.Username = Username;

	    await cache.UpsertAsync(Token);

            return Token;
        }

        private async Task<HttpResponseMessage> PerformTokenRequest()
        {
            using (var client = new HttpClient {BaseAddress = BaseAddress})
            {
                client.DefaultRequestHeaders.Accept.Clear ();
                client.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));

                var content = new FormUrlEncodedContent(new [] {
                    new KeyValuePair<string, string>("client_id", ClientId),
                    new KeyValuePair<string, string>("password", Password),
                    new KeyValuePair<string, string>("username", Username),
                    new KeyValuePair<string, string>("grant_type", GrantType)
                });

                var r = await client.PostAsync(TokenUrl, content).ConfigureAwait(false);
                return r;
            }

        }
        private void Validate()
        {
            if (BaseAddress == null)
                throw new ArgumentNullException("BaseAddress");
            if (TokenUrl == null)
                throw new ArgumentNullException("TokenUrl");
            if (Username == null)
                throw new ArgumentNullException("Username");
            if (Password == null)
                throw new ArgumentNullException("Password");
            if (ClientId == null)
                throw new ArgumentNullException("ClientId");
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public Uri BaseAddress { get; set; }
        public HttpResponseMessage HttpResponse { get; set; }
        public string GrantType => "password";
        public Token Token { get; internal set; }
        public string TokenUrl { get; set; } = "/oauth/token";
    }
}