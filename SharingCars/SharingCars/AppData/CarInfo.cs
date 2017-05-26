using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SharingCars.AppData
{
    class CarInfoData
    {
        public PhotoInfo photo;
        public string name;
        public enum CarType { Scooter, Car };
        public CarType type;
        //public enum CarAge {New,Medium,Old };
        //public CarAge age;
        public int age;
    }
    class CarInfo : InfoPrototype<CarInfoData>
    {
        static string Name = "car-info";
        public async Task<CarInfoData> GetData() { return await GetData(Name); }
        public static async Task<CarInfo> New(CarInfoData data) { return new CarInfo { Id = await InfoPrototype<CarInfoData>.New(Name, data) }; }
    }
}
