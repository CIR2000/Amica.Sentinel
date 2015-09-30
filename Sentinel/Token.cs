using Newtonsoft.Json;
using System;

// ReSharper disable once CheckNamespace
namespace Sentinel
{
    public class Token
    {
        public Token()
        {
            CreatedAt = DateTime.Now;
        }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        private int ExpiresIn { get; set; }
        private DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt {
            get { return CreatedAt.AddSeconds(ExpiresIn); }}

        public bool Expired
        {
            get { return DateTime.Now > ExpiresAt; }
        }
        public string Scope { get; set; }
    }
}
