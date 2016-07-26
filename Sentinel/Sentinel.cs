using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Eve.Authenticators;
using SimpleObjectCache;

namespace Amica.vNext
{

    // TODO IDisposable.
    public class Sentinel : ISentinel
    {
        public Sentinel()
        {
            BaseAddress = new Uri("https://10.0.2.2:8000");
        }

		/// <summary>
        /// Override this to implement platform-specific handlers,
        /// such as HttpModernClient on iOS/Android.
        /// </summary>
        /// <returns></returns>
        protected virtual HttpClient GetHttpClient()
        {
            return new HttpClient();
        }

        public async Task InvalidateUser(string username)
        {
            await LocalCache.Invalidate<Token>(username).ConfigureAwait(false);
        }
        public async Task<BearerAuthenticator> GetBearerAuthenticator(bool forceRefresh = false)
        {
            var token = await GetBearerToken(forceRefresh).ConfigureAwait(false);
            return token != null ? new BearerAuthenticator(token.AccessToken) : null;
        }
        public async Task<Token> GetBearerToken(bool forceRefresh = false)
        {
            Validate();

            HttpResponse = null;
            Token = null;

            if (!forceRefresh)
            {
                try
                {
                    Token = await LocalCache.Get<Token>(Username).ConfigureAwait(false);
                    if (!Token.Expired)
                        return Token;
                }
                catch (KeyNotFoundException) { }
            }

            using (var client = GetHttpClient())
            {
                client.BaseAddress = BaseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("client_id", ClientId),
                    new KeyValuePair<string, string>("password", Password),
                    new KeyValuePair<string, string>("username", Username),
                    new KeyValuePair<string, string>("grant_type", GrantType)
                });

                HttpResponse = await client.PostAsync(TokenUrl, content).ConfigureAwait(false);
                if (HttpResponse.StatusCode != HttpStatusCode.OK) return Token;

                var json = await HttpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                Token = JsonConvert.DeserializeObject<Token>(json);
                Token.Username = Username;

                await LocalCache.Insert(Username, Token, Token.ExpiresAt).ConfigureAwait(false);
            }

            return Token;
        }

        private void Validate()
        {
            if (LocalCache == null)
                throw new ArgumentNullException(nameof(LocalCache));
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
        public Uri BaseAddress { get; set; }
        public HttpResponseMessage HttpResponse { get; set; }
        public string GrantType => "password";
        public Token Token { get; internal set; }
        public string TokenUrl { get; set; } = "/oauth/token";
        public IBulkObjectCache LocalCache { get; set; }
    }
}