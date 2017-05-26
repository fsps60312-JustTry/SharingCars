using System;
using System.Threading.Tasks;
using Plugin.Geolocator;

namespace SharingCars.AppData
{
    class DeviceLocationInfo
    {
        public bool IsEnabled = false;
        public DateTime LastUpdateTime;
        public double Longitude, Latitude;
        public async Task Update()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;
            var position = await locator.GetPositionAsync(10000);
            Latitude = position.Latitude;
            Longitude = position.Longitude;
            LastUpdateTime = new DateTime(position.Timestamp.Ticks);
            IsEnabled = true;
        }
    }
}
