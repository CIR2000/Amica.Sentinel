using System;
using System.Threading.Tasks;
using Eve.Authenticators;

namespace Amica.vNext
{
    public interface ISentinel
    {
        Task<BearerAuthenticator> GetBearerAuthenticator(bool forceRefresh = false);
        Task<Token> GetBearerToken(bool forceRefresh = false);
		string Username { get; set; }
		string Password { get; set; }
		string ClientId { get; set; }
		string TokenUrl { get; set; }
		Uri  BaseAddress { get; set; }
		SqliteObjectCacheBase Cache { get; set; }
    }
}
