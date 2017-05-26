using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Plugin.DeviceInfo;

namespace SharingCars.AppData
{
    static class AppData
    {
        public enum DataType { FacebookProfile, CarInfo,DeviceLocation };
        //public static string tmp;
        public static string FacebookAppId { get { return "330515237367117"; } }
        public static string StorageAccountKey { get { return "GTH65N27gS8mOOTyQpl52kVzTW1m3/cG0Afi9HjjHhc27wuVHuKQ7eC7XpNa90UWJ7mh7Ilne1npFyHtNaiO/A=="; } }
        public static string StorageAccountName { get { return "sharingcars"; } }
        public static string DeviceId { get { return CrossDeviceInfo.Current.Id; } }
        public static FacebookProfile userFacebookProfile;
        public static List<CarInfo> cars = new List<CarInfo>();
        public static DeviceLocationInfo deviceLocation = new DeviceLocationInfo();
        public static async Task Upload(DataType dataType)
        {
            string s = null;
            switch (dataType)
            {
                case DataType.FacebookProfile:
                    s = JsonConvert.SerializeObject(userFacebookProfile);
                    break;
                case DataType.CarInfo:
                    s = JsonConvert.SerializeObject(cars);
                    break;
                case DataType.DeviceLocation:
                    s = JsonConvert.SerializeObject(deviceLocation);
                    break;
                default:
                    string msg = $"Upload DataType \"{dataType}\" haven't been implemented!";
                    Debug.WriteLine(msg);
                    await App.Current.MainPage.DisplayAlert("Error", msg, "OK");
                    return;
            }
            Trace.Assert(s != null);
            var blockBlob = await GetBlockBlobAsync(dataType.ToString().ToLower());
            //container.GetBlockBlobReference(dataType.ToString().ToLower());
            await blockBlob.UploadTextAsync(s);
        }
        public static async Task Download(DataType dataType)
        {
            var blockBlob = await GetBlockBlobAsync(dataType.ToString().ToLower());
            if (!await blockBlob.ExistsAsync())
            {
                await App.Current.MainPage.DisplayAlert("", $"You haven't save any \"{dataType}\" data on cloud!", "OK");
                return;
            }
            string s = await blockBlob.DownloadTextAsync();
            switch (dataType)
            {
                case DataType.FacebookProfile:
                    userFacebookProfile = JsonConvert.DeserializeObject<FacebookProfile>(s);
                    break;
                case DataType.CarInfo:
                    cars = JsonConvert.DeserializeObject<List<CarInfo>>(s);
                    break;
                case DataType.DeviceLocation:
                    deviceLocation = JsonConvert.DeserializeObject<DeviceLocationInfo>(s);
                    break;
                default:
                    string msg = $"Download DataType \"{dataType}\" haven't been implemented!";
                    Debug.WriteLine(msg);
                    await App.Current.MainPage.DisplayAlert("Error", msg, "OK");
                    return;
            }
        }
        public static string StringMapToLowerCase(string s)
        {
            s = s.Replace("0", "00");
            for(char c='A';c<='Z';c++)
            {
                s = s.Replace($"{c}", $"0{c}".ToLower());
            }
            foreach(char c in s) Trace.Assert('a' <= c && c <= 'z');
            return s;
        }
        public static CloudBlobClient blobClient
        {
            get
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                    "DefaultEndpointsProtocol=https;" +
                    $"AccountName={AppData.StorageAccountName};" +
                    $"AccountKey={AppData.StorageAccountKey};" +
                    "EndpointSuffix=core.windows.net"
                );
                return storageAccount.CreateCloudBlobClient();
            }
        }
        public static CloudBlobContainer container
        {
            get
            {
                return blobClient.GetContainerReference(StringMapToLowerCase(DeviceId));
            }
        }
        public static async Task<CloudBlockBlob> GetBlockBlobAsync(string name)
        {
            var container = AppData.container;
            await container.CreateIfNotExistsAsync();
            return container.GetBlockBlobReference(name);
        }
    }
}
