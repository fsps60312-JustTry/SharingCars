using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Plugin.DeviceInfo;
using SharingCars.Utils.Alerts;
using SharingCars.Utils;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SharingCars.AppData
{
    static class AppData
    {
        //public static string tmp;
        //public static FacebookProfile userFacebookProfile;
        public static UserInfo user = null;
        public static List<CarInfo> cars = null;
        public static DeviceLocationInfo deviceLocation = new DeviceLocationInfo();
        public enum DataType { User, Car, DeviceLocation };
        private static string GetJson(DataType dataType,bool createIfNotExist=false)
        {
            string s = null;
            switch (dataType)
            {
                case DataType.User:
                    if (createIfNotExist && AppData.user == null) AppData.user = new UserInfo();
                    s = JsonConvert.SerializeObject(AppData.user);
                    break;
                case DataType.Car:
                    if (createIfNotExist && AppData.cars == null) AppData.cars = new List<CarInfo>();
                    s = JsonConvert.SerializeObject(AppData.cars);
                    break;
                case DataType.DeviceLocation:
                    if (createIfNotExist && AppData.deviceLocation == null) AppData.deviceLocation = new DeviceLocationInfo();
                    s = JsonConvert.SerializeObject(AppData.deviceLocation);
                    break;
                default:
                    ErrorReporter.Report($"Upload DataType \"{dataType}\" haven't been implemented!");
                    return null;
            }
            return s;
        }
        public static async Task UploadAsync(DataType dataType)
        {
            indexRetry:;
            try
            {
                string s = GetJson(dataType);
                ErrorReporter.Assert(s != null);
                var blockBlob = await GetBlockBlobForAppDataAsync(dataType);
                //container.GetBlockBlobReference(dataType.ToString().ToLower());
                await blockBlob.UploadTextAsync(s);
            }
            catch (StorageException error)
            {
                if (error.Message == "An error occurred while sending the request")
                {
                    if (await App.Current.MainPage.DisplayAlert("無法連線", "資料上傳失敗，請將您的裝置連上網路後再重試一次", "重試", "取消"))
                    {
                        goto indexRetry;
                    }
                    else throw new OperationCanceledException();
                }
                else
                {
                    await new ErrorAlert("StorageException: 使用者資料上傳失敗", error).Show();
                    throw new OperationCanceledException();
                }
            }
            catch (Exception error)
            {
                await new ErrorAlert("Exception: 使用者資料上傳失敗", error).Show();
                throw new OperationCanceledException();
            }
        }
        public static async Task DownloadAsync(DataType dataType)
        {
            indexRetry:;
            try
            {
                var blockBlob = await GetBlockBlobForAppDataAsync(dataType);
                string s;
                if (!await blockBlob.ExistsAsync())
                {
                    s = GetJson(dataType, true);
                    if (await App.Current.MainPage.DisplayAlert("", $"You haven't save any \"{dataType}\" data on cloud!\r\nCreate now?", "Yes", "No"))
                    {
                        await UploadAsync(dataType);
                        await new JustAlert($"\"{dataType}\" data created on cloud!").Show();
                    }
                }
                else
                {
                    s = await blockBlob.DownloadTextAsync();
                }
                switch (dataType)
                {
                    case DataType.User:
                        AppData.user = JsonConvert.DeserializeObject<UserInfo>(s);
                        break;
                    case DataType.Car:
                        AppData.cars = JsonConvert.DeserializeObject<List<CarInfo>>(s);
                        break;
                    case DataType.DeviceLocation:
                        AppData.deviceLocation = JsonConvert.DeserializeObject<DeviceLocationInfo>(s);
                        break;
                    default:
                        ErrorReporter.Report($"Download DataType \"{dataType}\" haven't been implemented!");
                        throw new OperationCanceledException();
                }
            }
            catch (Microsoft.WindowsAzure.Storage.StorageException error)
            {
                if (error.Message == "An error occurred while sending the request")
                {
                    if (await App.Current.MainPage.DisplayAlert("無法連線", "資料下載失敗，請將您的裝置連上網路後再重試一次", "重試", "取消"))
                    {
                        goto indexRetry;
                    }
                    else throw new OperationCanceledException();
                }
                else
                {
                    await new ErrorAlert("StorageException: 使用者資料下載失敗", error).Show();
                    throw new OperationCanceledException();
                }
            }
            catch (Exception error)
            {
                await new ErrorAlert("Exception: 使用者資料下載失敗", error).Show();
                throw new OperationCanceledException();
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
            indexRetry:;
            try
            {
                var blockBlob = await GetBlockBlobAsync(directoryName, name);
                await blockBlob.UploadTextAsync(JsonConvert.SerializeObject(data));
            }
            catch (StorageException error)
            {
                if (error.Message == "An error occurred while sending the request")
                {
                    if (await App.Current.MainPage.DisplayAlert("無法連線", "資料上傳失敗，請將您的裝置連上網路後再重試一次", "重試", "取消"))
                    {
                        goto indexRetry;
                    }
                    else throw new OperationCanceledException();
                }
                else
                {
                    await new ErrorAlert("StorageException: 資料上傳失敗", error).Show();
                    throw new OperationCanceledException();
                }
            }
            catch (Exception error)
            {
                await new ErrorAlert("Exception: 資料上傳失敗", error).Show();
                throw new OperationCanceledException();
            }
        }
        public static async Task<T> DownloadAsync<T>(string directoryName,string name)
        {
            indexRetry:;
            try
            {
                var blockBlob = await GetBlockBlobAsync(directoryName, name);
                return JsonConvert.DeserializeObject<T>(await blockBlob.DownloadTextAsync());
            }
            catch (StorageException error)
            {
                if (error.Message == "An error occurred while sending the request")
                {
                    if (await App.Current.MainPage.DisplayAlert("無法連線", "資料下載失敗，請將您的裝置連上網路後再重試一次", "重試", "取消"))
                    {
                        goto indexRetry;
                    }
                    else throw new OperationCanceledException();
                }
                else
                {
                    await new ErrorAlert("StorageException: 資料下載失敗", error).Show();
                    throw new OperationCanceledException();
                }
            }
            catch (Exception error)
            {
                await new ErrorAlert("Exception: 資料下載失敗", error).Show();
                throw new OperationCanceledException();
            }
        }
        public static async Task UploadStreamAsync(string directoryName, string name,Stream stream)
        {
            indexRetry:;
            try
            {
                var blockBlob = await GetBlockBlobAsync(directoryName, name);
                await blockBlob.UploadFromStreamAsync(stream);
            }
            catch (StorageException error)
            {
                if (error.Message == "An error occurred while sending the request")
                {
                    if (await App.Current.MainPage.DisplayAlert("無法連線", "串流上傳失敗，請將您的裝置連上網路後再重試一次", "重試", "取消"))
                    {
                        goto indexRetry;
                    }
                    else throw new OperationCanceledException();
                }
                else
                {
                    await new ErrorAlert("StorageException: 串流上傳失敗", error).Show();
                    throw new OperationCanceledException();
                }
            }
            catch (Exception error)
            {
                await new ErrorAlert("Exception: 串流上傳失敗", error).Show();
                throw new OperationCanceledException();
            }
        }
        public static async Task<Stream> DownloadStreamAsync(string directoryName, string name)
        {
            indexRetry:;
            try
            {
                var blockBlob = await GetBlockBlobAsync(directoryName, name);
                Stream stream = new MemoryStream();
                await blockBlob.DownloadToStreamAsync(stream);
                stream.Position = 0;
                return stream;
            }
            catch (StorageException error)
            {
                if (error.Message == "An error occurred while sending the request")
                {
                    if (await App.Current.MainPage.DisplayAlert("無法連線", "串流下載失敗，請將您的裝置連上網路後再重試一次", "重試", "取消"))
                    {
                        goto indexRetry;
                    }
                    else throw new OperationCanceledException();
                }
                else
                {
                    await new ErrorAlert("StorageException: 串流下載失敗", error).Show();
                    throw new OperationCanceledException();
                }
            }
            catch (Exception error)
            {
                await new ErrorAlert("Exception: 串流下載失敗", error).Show();
                throw new OperationCanceledException();
            }
        }
        public static string StringMapToLowerCase(string s)
        {
            s = s.Replace("0", "00");
            for (char c = 'A'; c <= 'Z'; c++)
            {
                s = s.Replace($"{c}", $"0{c}".ToLower());
            }
            foreach (char c in s) ErrorReporter.Assert('a' <= c && c <= 'z');
            return s;
        }
        public static async Task<CloudBlobDirectory> GetBlobDirectoryAsync(string directoryName)
        {
            indexRetry:;
            try
            {
                var storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(
                    "DefaultEndpointsProtocol=https;" +
                    $"AccountName={AppDataConstants.StorageAccountName};" +
                    $"AccountKey={AppDataConstants.StorageAccountKey};" +
                    "EndpointSuffix=core.windows.net"
                );
                var container = storageAccount.CreateCloudBlobClient().GetContainerReference("users-data");
                await container.CreateIfNotExistsAsync();
                return container.GetDirectoryReference("version-0").GetDirectoryReference(directoryName);
            }
            catch (StorageException error)
            {
                if (error.Message == "An error occurred while sending the request")
                {
                    if (await App.Current.MainPage.DisplayAlert("無法連線", "雲端資料建立失敗，請將您的裝置連上網路後再重試一次", "重試", "取消"))
                    {
                        goto indexRetry;
                    }
                    else throw new OperationCanceledException();
                }
                else
                {
                    await new ErrorAlert("StorageException: 雲端資料建立失敗", error).Show();
                    throw new OperationCanceledException();
                }
            }
            catch (Exception error)
            {
                await new ErrorAlert("Exception: 雲端資料建立失敗", error).Show();
                throw new OperationCanceledException();
            }
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
