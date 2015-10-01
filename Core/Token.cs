using Newtonsoft.Json;
using System;
using SQLite;

namespace Sentinel
{
    [Table("Tokens")]
    public class Token
    {
        public Token()
        {
            CreatedAt = DateTime.Now;
        }
        [PrimaryKey]
        public string Username { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private int ExpiresIn { get; set; }
        private DateTime CreatedAt { get; }
        public DateTime ExpiresAt {
            get { return CreatedAt.AddSeconds(ExpiresIn); }}

        public bool Expired
        {
            get { return DateTime.Now > ExpiresAt; }
        }
        public string Scope { get; set; }
    }
}
