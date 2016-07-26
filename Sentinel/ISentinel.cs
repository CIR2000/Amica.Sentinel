using System;
using System.Threading.Tasks;
using Eve.Authenticators;
using SimpleObjectCache;

namespace Amica.vNext
{
    public interface ISentinel
    {
        Task<BearerAuthenticator> GetBearerAuthenticator(bool forceRefresh = false);
        Task<Token> GetBearerToken(bool forceRefresh = false);
        Task InvalidateUser(string username);
		string Username { get; set; }
		string Password { get; set; }
		string ClientId { get; set; }
		string TokenUrl { get; set; }
		Uri  BaseAddress { get; set; }
		IBulkObjectCache LocalCache { get; set; }
    }
}
