
namespace SharingCars.AppData
{
    class CarInfo
    {
        public ulong pictureId = 0;
        public string name;
        public enum CarType { Scooter, Car };
        public CarType type;
        //public enum CarAge {New,Medium,Old };
        //public CarAge age;
        public int age;
    }
}
