using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SharingCars
{/*
    class ErrorReporter
    {
        public static async Task ReportError(string s)
        {
            var container = AppData.AppData.blobClient.GetContainerReference("e");
            await container.CreateIfNotExistsAsync();
            var directory=container.GetDirectoryReference("error-report");
            //await directory.
            await container.GetBlockBlobReference(DateTime.Now.Ticks.ToString()).UploadTextAsync(s);
        }
        public static async Task<List<string>> DownloadErrorList()
        {
            var container = AppData.AppData.blobClient.GetContainerReference("e");
            await container.CreateIfNotExistsAsync();
            BlobContinuationToken token = null;
            List<string> ans = new List<string>();
            do
            {
                BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(token);
                token = resultSegment.ContinuationToken;
                foreach (IListBlobItem blob in resultSegment.Results)
                {
                    // Blob type will be CloudBlockBlob, CloudPageBlob or CloudBlobDirectory 
                    Console.WriteLine("{0} (type: {1}", blob.Uri, blob.GetType());
                }
            } while (token != null);
            await container.ListBlobsSegmentedAsync()
        }
    }*/
}
