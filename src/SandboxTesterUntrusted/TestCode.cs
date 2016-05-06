using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SandboxTesterUntrusted
{
    public class TestCode
    {
        public static bool Test()
        {
            // Uncomment a statement to get an example of SecurityExceptions or TypeLoadExceptions
            // Or add your own code to test

            // Microsoft.Azure.KeyVault
            //var keyVaultClient = new Microsoft.Azure.KeyVault.KeyVaultClient(AuthenticationCallback);

            // Microsoft.IdentityModel.Clients.ActiveDirectory
            //var adalException = new Microsoft.IdentityModel.Clients.ActiveDirectory.AdalException();

            // Microsoft.WindowsAzure.Storage
            //var exception = new Microsoft.WindowsAzure.Storage.StorageException();

            // No calls to localhost allowed
            //var httpClient = new System.Net.Http.HttpClient();
            //var httpClientResponse = httpClient.GetStringAsync("http://localhost").Wait(3000);

            // No web calls other then http and https allowed
            //var webRequest = System.Net.WebRequest.Create("ftp://localhost");
            //var webResponse = webRequest.GetResponse();

            // Serialization to/from private fields
            //var s = JsonConvert.SerializeObject(new SampleClass());
            //var o = JsonConvert.DeserializeObject<SampleClass>("{Title: \"No can't do\"}");

            return true;
        }

        private static async Task<string> AuthenticationCallback(string authority, string resource, string scope)
        {
            return await Task.Factory.StartNew(() => $"{authority},{resource},{scope}");
        }

        [JsonObject]
        public class SampleClass
        {
            public SampleClass()
            {
                this._title = "Sample string";
            }

            [JsonProperty("Title")]
            private string _title;

            public override string ToString()
            {
                return this._title;
            }
        }
    }
}
