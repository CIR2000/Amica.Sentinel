using System;
using System.Threading.Tasks;
using Sentinel.NET;

// ReSharper disable once CheckNamespace
namespace Sentinel
{
    public class Sentinel
    {
        private string _tokenUrl = "/oauth/token";

        public async Task<Token> GetToken(bool force=false)
        {
            Validate();
            
            // TODO is a token cached for Username/ClientId combo?
            
            // TODO if not cached or force==true, perform token request to remote service, store it to cache and then return it.
            // else, retrieve token from cache and return it.
            return null;
        }

        private void Validate()
        {
            if (BaseAddress == null)
                throw new ArgumentException(BaseAddress);
            if (TokenUrl == null)
                throw new ArgumentException(TokenUrl);
            if (Username == null)
                throw new ArgumentException(Username);
            if (Password == null)
                throw new ArgumentException(Password);
            if (ClientId == null)
                throw new ArgumentException(ClientId);
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string BaseAddress { get; set; }
        public string GrantType { get { return "password"; } }
        public string TokenUrl
        {
            get { return _tokenUrl; }
            set { _tokenUrl = value; }
        }
    }
}