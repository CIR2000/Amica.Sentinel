using System;
using System.IO;
using System.Threading.Tasks;
using SQLite;

namespace Sentinel.Cache
{
    class SqLiteTokenCache : ITokenCache
    {
        private readonly SQLiteAsyncConnection _conn;

        public SqLiteTokenCache()
        {
            _conn = new SQLiteAsyncConnection(DatabasePath);
            _conn.CreateTableAsync<Token>();
        }

        public async Task<Token> GetAsync(string username)
        {
            var token = await _conn.GetAsync<Token>(username);
	    // if token is expired we return null as if it was missing.
	    // we don't delete it, however.
            return token != null ? !token.Expired ? token : null : null;
        }

        public async Task UpsertAsync(Token token)
        {
            if (await _conn.FindAsync<Token>(token.Username) == null)
                await _conn.InsertAsync(token);
            else
                await _conn.UpdateAsync(token);
        }
        private static string DatabasePath
        {
            get
            {
                const string sqliteFilename = "sentinel.db3";

		// TODO set the appropriate path depending on the client type
		#if __IOS__
		    string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
		    string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
		    var path = Path.Combine(libraryPath, sqliteFilename);
		#else
		    #if __ANDROID__
			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
			var path = Path.Combine(documentsPath, sqliteFilename);
		    #else
			// Windows
			var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), sqliteFilename);
		    #endif
		#endif
                return path;
            }
        }
    }
}
