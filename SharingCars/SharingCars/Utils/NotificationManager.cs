using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms;

namespace SharingCars
{
    class NotificationManager
    {
        public enum Flags {Title,Message,Type,DeviceId,UserId };
        public enum SendType { CarRequest, CarOwnerPriceRequest,CarAccepted,OfficialNotification };
        public enum CarRequestFlag { RequestLatitude, RequestLongitude, RequestRadius, DeviceLatitude, DeviceLongitude };
        public enum CarOwnerPriceRequestFlag { Price,CarId};
        public enum CarAcceptedFlag { };
        public enum OfficialNotificationFlag { };
        public static async Task HandleIntent(Dictionary<string, string> intent)
        {
            foreach (var f in Enum.GetValues(typeof(Flags))) Trace.Assert(intent.ContainsKey($"{f}"));
            string type = intent[$"{Flags.Type}"];
            if (type == $"{SendType.CarRequest}")
            {
                foreach (var f in Enum.GetValues(typeof(CarRequestFlag))) Trace.Assert(intent.ContainsKey($"{f}"));
                if (await Application.Current.MainPage.DisplayAlert(intent[$"{Flags.Message}"], "您要接受這個訂單並開價嗎?", "Yes", "No"))
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new CommunicationPage.CarOwnerPriceRequestPage(intent));
                }
            }
            else if (type == $"{SendType.CarOwnerPriceRequest}")
            {
                foreach (var f in Enum.GetValues(typeof(CarOwnerPriceRequestFlag))) Trace.Assert(intent.ContainsKey($"{f}"));
                if(await Application.Current.MainPage.DisplayAlert(intent[$"{Flags.Message}"],
                    "您要查看詳細資料嗎?", "Yes","No"))
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new CommunicationPage.ViewPriceRequestByCarOwnerPage(intent));
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("", "繼續等待其他車主的回應吧！", "OK");
                }
            }
            else if (type == $"{SendType.CarAccepted}")
            {
                foreach (var f in Enum.GetValues(typeof(CarAcceptedFlag))) Trace.Assert(intent.ContainsKey($"{f}"));
                await App.Current.MainPage.Navigation.PushAsync(new UserPage.UserInfoPage(ulong.Parse(intent[$"{Flags.UserId}"])));
                await App.Current.MainPage.DisplayAlert("", $"這是租車人的資料，趕快和他聯絡並完成交易吧！", "OK");
            }
            else if(type==$"{SendType.OfficialNotification}")
            {
                foreach (var f in Enum.GetValues(typeof(OfficialNotificationFlag))) Trace.Assert(intent.ContainsKey($"{f}"));
                await Application.Current.MainPage.DisplayAlert(intent[$"{Flags.Title}"], intent[$"{Flags.Message}"], "OK");
            }
            else
            {
                StringBuilder s = new StringBuilder();
                foreach (var p in intent)
                {
                    s.AppendLine($"{p.Key}: {p.Value}");
                }
                await Application.Current.MainPage.DisplayAlert("", s.ToString(), "OK");
            }
        }
        public class Send
        {
            public static async Task CarAccepted(string senderId)
            {
                await SendNotification("Sharing Cars",
                    $"{(await AppData.AppData.user.GetData()).Name}接受了您的開價，點擊此訊息以取得聯絡方式", $"{SendType.CarAccepted}",
                    new Dictionary<string, string>
                    {}, senderId);
            }
            public static async Task CarOwnerPriceRequest(int price,ulong carId, string senderId)
            {
                await SendNotification("Sharing Cars",
                    $"{(await AppData.AppData.user.GetData()).Name}開價{price}元且願意出借車輛：{(await new AppData.CarInfo { Id = carId }.GetData()).name}", $"{SendType.CarOwnerPriceRequest}",
                    new Dictionary<string, string>
                    {
                        { $"{CarOwnerPriceRequestFlag.Price}", $"{price}" },
                        { $"{CarOwnerPriceRequestFlag.CarId}", $"{carId}" }
                    }, senderId);
            }
            public static async Task CarRequest(Xamarin.Forms.Maps.MapSpan region)
            {
                await SendNotification("Sharing Cars", $"{(await AppData.AppData.user.GetData()).Name}需要借車", $"{SendType.CarRequest}",
                    new Dictionary<string, string>
                    {
                        { $"{CarRequestFlag.RequestLatitude}", $"{region.Center.Latitude}" },
                        { $"{CarRequestFlag.RequestLongitude}", $"{region.Center.Longitude}" },
                        { $"{CarRequestFlag.RequestRadius}", $"{region.Radius.Meters}" },
                        { $"{CarRequestFlag.DeviceLatitude}", $"{AppData.AppData.deviceLocation.Latitude}" },
                        { $"{CarRequestFlag.DeviceLongitude}", $"{AppData.AppData.deviceLocation.Longitude}" }
                    }, null);
            }
            public static async Task OfficialNotification(string title,string message,bool bePublic)
            {
                await SendNotification(title, message, $"{SendType.OfficialNotification}", null, bePublic ? null : AppData.AppDataConstants.DeviceId);
            }
            private static async Task SendNotification(string title, string msg, string type, Dictionary<string, string> intent = null, string tag = null)
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        {
                            Droid.NotificationSender.SendNotification(title, msg, type, intent, tag);
                            break;
                        }
                    default:
                        {
                            await Application.Current.MainPage.DisplayAlert("", $"Push notification on {Device.RuntimePlatform} not supported!", "OK");
                            break;
                        }
                }
            }
        }
    }
}
