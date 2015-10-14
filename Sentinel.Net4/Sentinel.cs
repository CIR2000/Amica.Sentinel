using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Eve.Authenticators;
using Amica.vNext;

namespace Sentinel
{
    public class Sentinel
    {
        public Sentinel()
        {
            Cache = new SqliteObjectCache() {ApplicationName = "Sentinel"};

	    // TODO set BaseAddress using the appropriate DiscoveryService class method/property.
            BaseAddress = new Uri("https://10.0.2.2:8000");
        }
        public async Task<BearerAuthenticator> GetBearerAuthenticator(bool forceRefresh = false)
        {
            var token = await GetBearerToken(forceRefresh);
            return token == null ? null : new BearerAuthenticator(token.AccessToken);
        }
        public async Task<Token> GetBearerToken(bool forceRefresh=false)
        {
            Validate();

	    HttpResponse = null;
	    Token = null;

            if (!forceRefresh) {
                try
                {
                    Token = await Cache.Get<Token>(Username);
                    if (!Token.Expired)
                        return Token;
                }
                catch (KeyNotFoundException) { }
            }
            
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

                HttpResponse = await client.PostAsync(TokenUrl, content).ConfigureAwait(false);
                if (HttpResponse.StatusCode != HttpStatusCode.OK) return Token;

                var json = await HttpResponse.Content.ReadAsStringAsync();
                Token = JsonConvert.DeserializeObject<Token>(json);
                Token.Username = Username;

                await Cache.Insert(Username, Token, Token.ExpiresAt);
            }
            return Token;
        }

        private void Validate()
        {
            if (TokenUrl == null)
                throw new ArgumentNullException(nameof(TokenUrl));
            if (Username == null)
                throw new ArgumentNullException(nameof(Username));
            if (Password == null)
                throw new ArgumentNullException(nameof(Password));
            if (ClientId == null)
                throw new ArgumentNullException(nameof(ClientId));
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public Uri BaseAddress { get; }
        public HttpResponseMessage HttpResponse { get; set; }
        public string GrantType => "password";
        public Token Token { get; internal set; }
        public string TokenUrl { get; set; } = "/oauth/token";
	public SqliteObjectCache Cache { get; set; }
    }
}