using System;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Xamarin.Forms;
using SharingCars.Utils.ActionSheets;
using SharingCars.Utils.Alerts;

namespace SharingCars.AppData
{
    class DeviceLocationInfo
    {
        public bool IsEnabled = false;
        public DateTime LastUpdateTime;
        public double Longitude, Latitude;
        public async Task<bool> Update()
        {
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;
                Plugin.Geolocator.Abstractions.Position position = null;
                indexRetry:;
                try
                {
                    position = await locator.GetPositionAsync(10000);
                }
                catch(TaskCanceledException error)
                {
                    if (error.Message != "A task was canceled.")
                    {
                        await new ErrorAlert("取得裝置位置失敗",error).Show();
                    }
                }
                catch (Exception error)
                {
                    await new ErrorAlert("取得裝置位置失敗",error).Show();
                }
                if (position == null)
                {
                    var selection = await new DeviceLocationUpdateFailed().Show();
                    switch (selection)
                    {
                        case null:
                        case DeviceLocationUpdateFailed.Cancel:
                            return false;
                        case DeviceLocationUpdateFailed.Button1:
                            goto indexRetry;
                        case DeviceLocationUpdateFailed.Button2:
                            try
                            {
                                await new TodoAlert().Show();
                                //position=await CrossGeolocator.Current.GetLastKnownLocationAsync();
                                return false;
                            }
                            catch (Exception error)
                            {
                                await new ErrorAlert("取得裝置位置失敗: 未知的錯誤", error).Show();
                            }
                            break;
                        default:
                            await new ErrorAlert($"未知的選項：{selection}",new Exception()).Show();
                            break;
                    }
                }
                Latitude = position.Latitude;
                Longitude = position.Longitude;
                LastUpdateTime = new DateTime(position.Timestamp.Ticks);
                IsEnabled = true;
                return true;
            }
            catch(Exception error)
            {
                await new ErrorAlert("取得裝置位置失敗", error).Show();
                return false;
            }
        }
    }
}
