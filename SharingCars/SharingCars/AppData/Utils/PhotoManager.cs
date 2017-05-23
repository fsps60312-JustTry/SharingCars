using System;
using System.Threading.Tasks;
using System.IO;

namespace SharingCars.AppData.Utils
{
    class PhotoManager
    {
        private static ulong GetRandomNumber()
        {
            while (true)
            {
                var ans = BitConverter.ToUInt64(Guid.NewGuid().ToByteArray(), 0);
                if (ans != 0) return ans;
            }
        }
        public static async Task<ulong> UploadPhotoAsync(Stream photoStream)
        {
            ulong ans = GetRandomNumber();
            var blockBlob = await AppData.GetBlockBlobAsync(ans.ToString("X16").ToLower());
            await blockBlob.UploadFromStreamAsync(photoStream);
            return ans;
        }
        public static async Task<Stream> DownloadPhotoAsync(ulong photoId)
        {
            var blockBlob = await AppData.GetBlockBlobAsync(photoId.ToString("X16").ToLower());
            Stream photoStream = new MemoryStream();
            await blockBlob.DownloadToStreamAsync(photoStream);
            photoStream.Position = 0;
            return photoStream;
        }
    }
}
