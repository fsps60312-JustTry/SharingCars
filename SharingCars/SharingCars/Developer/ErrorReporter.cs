using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Xamarin.Forms;
using Newtonsoft.Json;
using SharingCars.Utils.Alerts;
using System.Diagnostics;

namespace SharingCars
{
    class ErrorReporter
    {
        public static void Assert(bool condition)
        {
            try
            {
                Trace.Assert(condition);
            }
            catch(Exception error)
            {
                ReportError("Assertion failed",error);
            }
        }
        public static void Report(string s)
        {
            try
            {
                throw new Exception(s);
            }
            catch(Exception error)
            {
                ReportError("錯誤訊息",error);
            }
        }
        public static async void ReportError(string message,Exception error) { await ReportErrorAsync(message,error); }
        public static async Task ReportErrorAsync(string message, Exception error)
        {
            await ReportErrorAsync(new Exception(message, error));
        }
        private static async Task ReportErrorAsync(Exception error)
        {
            await ReportErrorAsync(JsonConvert.SerializeObject(error));
        }
        public static async Task ReportErrorAsync(string s)
        {
            var directory = await GetBlobDirectoryAsync();
            await Azure.UploadTextAsync(directory.GetBlockBlobReference(DateTime.Now.Ticks.ToString()), s);
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
                foreach (var blob in resultSegment.Results)
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
                    var blockBlob = (blob.Parent as CloudBlobDirectory).GetBlockBlobReference(name) as CloudBlockBlob;
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
