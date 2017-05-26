using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Xamarin.Forms;
using Newtonsoft.Json;

namespace SharingCars
{
    class ErrorReporter
    {
        public static async void ReportError(Exception error) { await ReportErrorAsync(error); }
        public static async Task ReportErrorAsync(Exception error)
        {
            await ReportErrorAsync(error.ToString());
        }
        public static async Task ReportErrorAsync(string s)
        {
            var directory = await GetBlobDirectoryAsync();
            await directory.GetBlockBlobReference(DateTime.Now.Ticks.ToString()).UploadTextAsync(s);
        }
        public static async Task<Dictionary<string,CloudBlockBlob>> DownloadErrorListAsync()
        {
            var container = await GetBlobDirectoryAsync();
            BlobContinuationToken token = null;
            var ans = new Dictionary<string, CloudBlockBlob>();
            do
            {
                BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(token);
                token = resultSegment.ContinuationToken;
                foreach (IListBlobItem blob in resultSegment.Results)
                {
                    // Blob type will be CloudBlockBlob, CloudPageBlob or CloudBlobDirectory

                    var uri = blob.Uri.ToString();
                    var name = uri.Substring(uri.LastIndexOf('/') + 1);
                    string key;
                    if (long.TryParse(name, out long tick))
                    {
                        var dateTime = new DateTime(tick);
                        key = $"{ dateTime.ToLongDateString()} {dateTime.ToLongTimeString()} ({name})";
                    }
                    else key = $"Error parsing DateTime: {name}";
                    var blockBlob = blob.Parent.GetBlockBlobReference(name);
                    ans.Add(key, blockBlob);
                    //Console.WriteLine("{0} (type: {1}", blob.Uri, blob.GetType());
                }
            } while (token != null);
            return ans;
        }
        private static async Task<CloudBlobDirectory> GetBlobDirectoryAsync()
        {
            return (await AppData.Methods.GetBlobDirectoryAsync("developer-insights")).GetDirectoryReference("error-reports");
        }
    }
}
