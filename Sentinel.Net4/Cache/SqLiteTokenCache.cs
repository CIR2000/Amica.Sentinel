using System;
using System.IO;
using System.Threading.Tasks;
using SQLite;

// ReSharper disable once CheckNamespace
namespace Sentinel.Cache
{
    class SqLiteTokenCache : ITokenCache
    {
        private static readonly SQLiteAsyncConnection Connection;

        static SqLiteTokenCache()
        {
            Connection = new SQLiteAsyncConnection(DatabasePath);
            Connection.CreateTableAsync<Token>();
        }

        public async Task<Token> FetchAsync(string username)
        {
            var token =  await Connection.FindAsync<Token>(username);
            if (token != null && !token.Expired)
                return token;
            // if token is expired we return null as if it was missing.
            // we don't delete it, however.
            return null;
        }

        public async Task UpsertAsync(Token token)
        {
            if (await Connection.FindAsync<Token>(token.Username) == null)
                await Connection.InsertAsync(token);
            else
                await Connection.UpdateAsync(token);
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
