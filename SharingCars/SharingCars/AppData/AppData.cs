using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Plugin.DeviceInfo;

namespace SharingCars.AppData
{
    static class AppData
    {
        //public static string tmp;
        public static FacebookProfile userFacebookProfile;
        public static List<CarInfo> cars = new List<CarInfo>();
        public static DeviceLocationInfo deviceLocation = new DeviceLocationInfo();
        public enum DataType { FacebookProfile, CarInfo, DeviceLocation };
        public static async Task UploadAsync(DataType dataType)
        {
            string s = null;
            switch (dataType)
            {
                case DataType.FacebookProfile:
                    s = JsonConvert.SerializeObject(AppData.userFacebookProfile);
                    break;
                case DataType.CarInfo:
                    s = JsonConvert.SerializeObject(AppData.cars);
                    break;
                case DataType.DeviceLocation:
                    s = JsonConvert.SerializeObject(AppData.deviceLocation);
                    break;
                default:
                    string msg = $"Upload DataType \"{dataType}\" haven't been implemented!";
                    Debug.WriteLine(msg);
                    await App.Current.MainPage.DisplayAlert("Error", msg, "OK");
                    return;
            }
            Trace.Assert(s != null);
            var blockBlob = await GetBlockBlobForAppDataAsync(dataType);
            //container.GetBlockBlobReference(dataType.ToString().ToLower());
            await blockBlob.UploadTextAsync(s);
        }
        public static async Task DownloadAsync(DataType dataType)
        {
            var blockBlob = await GetBlockBlobForAppDataAsync(dataType);
            if (!await blockBlob.ExistsAsync())
            {
                await App.Current.MainPage.DisplayAlert("", $"You haven't save any \"{dataType}\" data on cloud!", "OK");
                return;
            }
            string s = await blockBlob.DownloadTextAsync();
            switch (dataType)
            {
                case DataType.FacebookProfile:
                    AppData.userFacebookProfile = JsonConvert.DeserializeObject<FacebookProfile>(s);
                    break;
                case DataType.CarInfo:
                    AppData.cars = JsonConvert.DeserializeObject<List<CarInfo>>(s);
                    break;
                case DataType.DeviceLocation:
                    AppData.deviceLocation = JsonConvert.DeserializeObject<DeviceLocationInfo>(s);
                    break;
                default:
                    string msg = $"Download DataType \"{dataType}\" haven't been implemented!";
                    Debug.WriteLine(msg);
                    await App.Current.MainPage.DisplayAlert("Error", msg, "OK");
                    return;
            }
        }
        private static async Task<CloudBlockBlob> GetBlockBlobForAppDataAsync(DataType dataType)
        {
            return (await Methods.GetBlobDirectoryAsync("users")).GetDirectoryReference(Methods.StringMapToLowerCase(AppDataConstants.DeviceId)).GetBlockBlobReference(Methods.StringMapToLowerCase(dataType.ToString()));
        }
    }
    static class Methods
    {
        public static async Task UploadAsync<T>(string directoryName,string name,T data)
        {
            var blockBlob = await GetBlockBlobAsync(directoryName, name);
            await blockBlob.UploadTextAsync(JsonConvert.SerializeObject(data));
        }
        public static async Task<T> DownloadAsync<T>(string directoryName,string name)
        {
            var blockBlob = await GetBlockBlobAsync(directoryName, name);
            return JsonConvert.DeserializeObject<T>(await blockBlob.DownloadTextAsync());
        }
        public static async Task UploadStreamAsync(string directoryName, string name,Stream stream)
        {
            var blockBlob = await GetBlockBlobAsync(directoryName, name);
            await blockBlob.UploadFromStreamAsync(stream);
        }
        public static async Task<Stream> DownloadStreamAsync(string directoryName, string name)
        {
            var blockBlob = await GetBlockBlobAsync(directoryName, name);
            Stream stream = new MemoryStream();
            await blockBlob.DownloadToStreamAsync(stream);
            stream.Position = 0;
            return stream;
        }
        private static CloudBlobClient blobClient
        {
            get
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                    "DefaultEndpointsProtocol=https;" +
                    $"AccountName={AppDataConstants.StorageAccountName};" +
                    $"AccountKey={AppDataConstants.StorageAccountKey};" +
                    "EndpointSuffix=core.windows.net"
                );
                return storageAccount.CreateCloudBlobClient();
            }
        }
        private static CloudBlobContainer container
        {
            get
            {
                return blobClient.GetContainerReference("users-data");
            }
        }
        public static string StringMapToLowerCase(string s)
        {
            s = s.Replace("0", "00");
            for (char c = 'A'; c <= 'Z'; c++)
            {
                s = s.Replace($"{c}", $"0{c}".ToLower());
            }
            foreach (char c in s) Trace.Assert('a' <= c && c <= 'z');
            return s;
        }
        public static async Task<CloudBlobDirectory> GetBlobDirectoryAsync(string directoryName)
        {
            var container = Methods.container;
            await container.CreateIfNotExistsAsync();
            return container.GetDirectoryReference("version-0").GetDirectoryReference(directoryName);
        }
        public static async Task<CloudBlockBlob> GetBlockBlobAsync(string directoryName,string name)
        {
            return (await GetBlobDirectoryAsync("open-data")).GetDirectoryReference(directoryName).GetBlockBlobReference(name);
        }
    }
    static class AppDataConstants
    {
        public static string FacebookAppId { get { return "330515237367117"; } }
        public static string StorageAccountKey { get { return "GTH65N27gS8mOOTyQpl52kVzTW1m3/cG0Afi9HjjHhc27wuVHuKQ7eC7XpNa90UWJ7mh7Ilne1npFyHtNaiO/A=="; } }
        public static string StorageAccountName { get { return "sharingcars"; } }
        public static string DeviceId { get { return CrossDeviceInfo.Current.Id; } }
    }
}
