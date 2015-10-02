using System.Threading.Tasks;

namespace Sentinel.Cache
{
    public interface ITokenCache
    {
        Task<Token> FetchAsync(string username);
        Task UpsertAsync(Token token);
   }
}