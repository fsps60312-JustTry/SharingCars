using System;
using System.Threading.Tasks;
using System.IO;

namespace SharingCars.AppData.Utils
{
    //class PhotoManager
    //{
    //    public static async Task<ulong> UploadPhotoAsync(Stream photoStream)
    //    {
    //        ulong ans = SharingCars.Utils.Numbers.GetRandomNumber();
    //        var blockBlob = await Methods.GetBlockBlobAsync("photos",ans.ToString("X16").ToLower());
    //        await blockBlob.UploadFromStreamAsync(photoStream);
    //        return ans;
    //    }
    //    public static async Task<Stream> DownloadPhotoAsync(ulong photoId)
    //    {
    //        var blockBlob = await Methods.GetBlockBlobAsync("photos", photoId.ToString("X16").ToLower());
    //        Stream photoStream = new MemoryStream();
    //        await blockBlob.DownloadToStreamAsync(photoStream);
    //        photoStream.Position = 0;
    //        return photoStream;
    //    }
    //}
}
