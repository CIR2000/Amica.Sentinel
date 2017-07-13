using Newtonsoft.Json;
using System;

namespace Amica.Sentinel
{
    public class Token
    {
        public Token()
        {
            CreatedAt = DateTime.Now;
        }
        public string Username { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt => CreatedAt.AddSeconds(ExpiresIn);

        public bool Expired => ExpiresAt <= DateTime.Now;
        public string Scope { get; set; }
    }
}
