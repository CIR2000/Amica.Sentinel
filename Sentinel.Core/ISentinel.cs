using System.Threading.Tasks;
using Eve.Authenticators;

namespace Amica.vNext
{
    public interface ISentinel
    {
        Task<BearerAuthenticator> GetBearerAuthenticator(bool forceRefresh = false);
        Task<Token> GetBearerToken(bool forceRefresh = false);
    }
}
