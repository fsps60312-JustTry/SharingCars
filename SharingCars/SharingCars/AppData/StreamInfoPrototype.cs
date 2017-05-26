using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace SharingCars.AppData
{
    class StreamInfoPrototype
    {
        public ulong Id = 0;
        protected async Task<Stream> GetData(string directoryName)
        {
            Stream ans = await Methods.DownloadStreamAsync(directoryName, Id.ToString("X16").ToLower());
            ans.Position = 0;
            return ans;
        }
        protected static async Task<ulong> New(string directoryName, Stream data)
        {
            ulong id = SharingCars.Utils.Numbers.GetRandomNumber();
            await Methods.UploadStreamAsync(directoryName, id.ToString("X16").ToLower(), data);
            return id;
        }
    }
}
