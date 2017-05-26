using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharingCars.AppData
{
    abstract class InfoPrototype<Data>
    {
        public ulong Id = 0;
        static Dictionary<ulong, Data> cache = new Dictionary<ulong, Data>();
        protected async Task<Data> GetData(string directoryName)
        {
            if (cache.ContainsKey(Id)) return cache[Id];
            return cache[Id] = await Methods.DownloadAsync<Data>(directoryName, Id.ToString("X16").ToLower());
        }
        protected static async Task<ulong> New(string directoryName, Data data)
        {
            ulong id = SharingCars.Utils.Numbers.GetRandomNumber();
            await Methods.UploadAsync(directoryName, id.ToString("X16").ToLower(), data);
            cache[id] = data;
            return id;
        }
    }
}
