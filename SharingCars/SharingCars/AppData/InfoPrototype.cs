using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharingCars.AppData
{
    class InfoPrototype<Data>
    {
        public ulong Id = 0;
        static Dictionary<ulong, Data> cache = new Dictionary<ulong, Data>();
        public async Task<Data> GetData()
        {
            if (cache.ContainsKey(Id)) return cache[Id];
            return cache[Id] = await Methods.DownloadAsync<Data>(Methods.StringMapToLowerCase(typeof(Data).Name), Id.ToString("X16").ToLower());
        }
        public static async Task<ulong> New(Data data)
        {
            ulong id = SharingCars.Utils.Numbers.GetRandomNumber();
            await Methods.UploadAsync(Methods.StringMapToLowerCase(typeof(Data).Name), id.ToString("X16").ToLower(), data);
            cache[id] = data;
            return id;
        }
    }
}
