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
        private string _tokenUrl = "/oauth/token";

        public async Task<BearerAuthenticator> GetBearerAuthenticator(bool ignoreCache = false)
        {
            var token = await GetBearerToken(ignoreCache);
            return token == null ? null : new BearerAuthenticator(token.AccessToken);
        }
        public async Task<Token> GetBearerToken(bool ignoreCache=false)
        {
            Validate();
            
            // TODO is a token cached for Username/ClientId combo?
            
            // TODO if not cached or ignore_cache, perform token request to remote service, store it to cache and then return it.

            // else, retrieve token from cache and return it.
            HttpResponse = await PerformTokenRequest();
            if (HttpResponse.StatusCode != HttpStatusCode.OK)
                return null;

            var json = await HttpResponse.Content.ReadAsStringAsync();
            Token = JsonConvert.DeserializeObject<Token>(json);
            Token.Username = Username;
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
        public string GrantType { get { return "password"; } }
        public Token Token { get; internal set; }
        public string TokenUrl
        {
            get { return _tokenUrl; }
            set { _tokenUrl = value; }
        }
    }
}