using System.Threading.Tasks;
using System.IO;

namespace SharingCars.AppData
{
    class PhotoInfo : StreamInfoPrototype
    {
        static string Name = "photos";
        public async Task<Stream> GetData() { return await GetData(Name); }
        public static async Task<PhotoInfo> New(Stream data) { return new PhotoInfo { Id = await New(Name, data) }; }
    }
}