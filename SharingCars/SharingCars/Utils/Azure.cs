using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SharingCars.Utils.Alerts;

namespace SharingCars
{
    class Azure
    {
        public static async Task UploadTextAsync(CloudBlockBlob blockBlob,string content)
        {
            indexRetry:;
            try
            {
                await blockBlob.UploadTextAsync(content);
            }
            catch (StorageException error)
            {
                if (error.Message == "An error occurred while sending the request")
                {
                    if (await App.Current.MainPage.DisplayAlert("無法連線", "上傳失敗，請將您的裝置連上網路後再重試一次", "重試", "取消"))
                    {
                        goto indexRetry;
                    }
                    else throw new OperationCanceledException();
                }
                else
                {
                    await new ErrorAlert("StorageException: 上傳失敗", error).Show();
                    throw new OperationCanceledException();
                }
            }
            catch (Exception error)
            {
                await new ErrorAlert("Exception: 上傳失敗", error).Show();
                throw new OperationCanceledException();
            }
        }
        public static async Task<string> DownloadTextAsync(CloudBlockBlob blockBlob)
        {
            indexRetry:;
            try
            {
                return await blockBlob.DownloadTextAsync();
            }
            catch (StorageException error)
            {
                if (error.Message == "An error occurred while sending the request")
                {
                    if (await App.Current.MainPage.DisplayAlert("無法連線", "下載失敗，請將您的裝置連上網路後再重試一次", "重試", "取消"))
                    {
                        goto indexRetry;
                    }
                    else throw new OperationCanceledException();
                }
                else
                {
                    await new ErrorAlert("StorageException: 下載失敗", error).Show();
                    throw new OperationCanceledException();
                }
            }
            catch (Exception error)
            {
                await new ErrorAlert("Exception: 下載失敗", error).Show();
                throw new OperationCanceledException();
            }
        }
    }
    //class CloudBlockBlob : Microsoft.WindowsAzure.Storage.Blob.CloudBlockBlob
    //{
    //    private CloudBlockBlob(Uri blobAbsoluteUri) :base(blobAbsoluteUri) { }
    //    public new async Task UploadTextAsync(string text)
    //    {
    //        indexRetry:;
    //        try
    //        {
    //            await base.UploadTextAsync(text);
    //        }
    //        catch (Microsoft.WindowsAzure.Storage.StorageException error)
    //        {
    //            if (error.Message == "An error occurred while sending the request")
    //            {
    //                if (await App.Current.MainPage.DisplayAlert("無法連線", "資料上傳失敗，請將您的裝置連上網路後再重試一次", "重試", "取消"))
    //                {
    //                    goto indexRetry;
    //                }
    //                else throw new OperationCanceledException();
    //            }
    //            else
    //            {
    //                await new ErrorAlert("StorageException: 資料上傳失敗", error).Show();
    //                throw new OperationCanceledException();
    //            }
    //        }
    //        catch (Exception error)
    //        {
    //            await new ErrorAlert("Exception: 資料上傳失敗", error).Show();
    //            throw new OperationCanceledException();
    //        }
    //    }
    //    public new async Task<string>DownloadTextAsync()
    //    {
    //        indexRetry:;
    //        try
    //        {
    //            return await base.DownloadTextAsync();
    //        }
    //        catch (Microsoft.WindowsAzure.Storage.StorageException error)
    //        {
    //            if (error.Message == "An error occurred while sending the request")
    //            {
    //                if (await App.Current.MainPage.DisplayAlert("無法連線", "資料下載失敗，請將您的裝置連上網路後再重試一次", "重試", "取消"))
    //                {
    //                    goto indexRetry;
    //                }
    //                else throw new OperationCanceledException();
    //            }
    //            else
    //            {
    //                await new ErrorAlert("StorageException: 資料下載失敗", error).Show();
    //                throw new OperationCanceledException();
    //            }
    //        }
    //        catch (Exception error)
    //        {
    //            await new ErrorAlert("Exception: 資料下載失敗", error).Show();
    //            throw new OperationCanceledException();
    //        }
    //    }
    //    public override async Task<bool> ExistsAsync()
    //    {
    //        indexRetry:;
    //        try
    //        {
    //            return await base.ExistsAsync();
    //        }
    //        catch (Microsoft.WindowsAzure.Storage.StorageException error)
    //        {
    //            if (error.Message == "An error occurred while sending the request")
    //            {
    //                if (await App.Current.MainPage.DisplayAlert("無法連線", "雲端資料檢查失敗，請將您的裝置連上網路後再重試一次", "重試", "取消"))
    //                {
    //                    goto indexRetry;
    //                }
    //                else throw new OperationCanceledException();
    //            }
    //            else
    //            {
    //                await new ErrorAlert("StorageException: 雲端資料檢查失敗", error).Show();
    //                throw new OperationCanceledException();
    //            }
    //        }
    //        catch (Exception error)
    //        {
    //            await new ErrorAlert("Exception: 雲端資料檢查失敗", error).Show();
    //            throw new OperationCanceledException();
    //        }
    //    }
    //}
    //class CloudBlobContainer : Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer
    //{
    //    public override async Task<bool> CreateIfNotExistsAsync()
    //    {
    //        indexRetry:;
    //        try
    //        {
    //            return await base.CreateIfNotExistsAsync();
    //        }
    //        catch (Microsoft.WindowsAzure.Storage.StorageException error)
    //        {
    //            if (error.Message == "An error occurred while sending the request")
    //            {
    //                if (await App.Current.MainPage.DisplayAlert("無法連線", "雲端資料建立失敗，請將您的裝置連上網路後再重試一次", "重試", "取消"))
    //                {
    //                    goto indexRetry;
    //                }
    //                else throw new OperationCanceledException();
    //            }
    //            else
    //            {
    //                await new ErrorAlert("StorageException: 雲端資料建立失敗", error).Show();
    //                throw new OperationCanceledException();
    //            }
    //        }
    //        catch (Exception error)
    //        {
    //            await new ErrorAlert("Exception: 雲端資料建立失敗", error).Show();
    //            throw new OperationCanceledException();
    //        }
    //    }
    //}
}
