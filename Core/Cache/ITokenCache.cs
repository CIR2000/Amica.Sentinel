using System.Threading.Tasks;

namespace Sentinel.Cache
{
    public interface ITokenCache
    {
        Task<Token> GetAsync(string username);
        Task UpsertAsync(Token token);
   }
}