using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Sentinel;
namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                AsyncContext.Run(() => Test());
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }

        private static async Task Test()
        {
            var r = new SentinelClient
            {
                // Unless a valid SSL certificate is installed, we 
                // need Fiddler running with Decrypt HTTPS option active, 
                // see http://docs.telerik.com/fiddler/Configure-Fiddler/Tasks/DecryptHTTPS
                
                // This is a local Flask-Sentinel server running on OSX.
                // We need this url to get through VBOX sandbox.
                BaseAddress = new Uri("https://10.0.2.2:5000/"),

                // these are test values only valid on my local machine.
                // have fun using them on a live system.
                ClientId = Environment.GetEnvironmentVariable("SentinelClientId"),
                Username = Environment.GetEnvironmentVariable("SentinelUsername"),
                Password = Environment.GetEnvironmentVariable("SentinelPassword")
            };
            var t = await r.GetBearerAuthenticator();
            Console.WriteLine(r.HttpResponse.StatusCode);

        //    using (var client = new HttpClient {BaseAddress = new Uri("http://10.0.2.2:5000/")})
        //    {
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        client.DefaultRequestHeaders.Authorization = t.AuthenticationHeader();


        //        var res = await client.FetchAsync("/countries");

        //    }
        }
    }
}
