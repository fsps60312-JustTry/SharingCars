﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms;

namespace SharingCars
{
    class NotificationManager
    {
        public enum Flags {Title,Message,Type,SenderId };
        public enum SendType { CarRequest, CarOwnerPriceRequest,OfficialNotification };
        public enum CarRequestFlag { RequestLatitude, RequestLongitude, RequestRadius, DeviceLatitude, DeviceLongitude };
        public enum CarOwnerPriceRequestFlag { Price};
        public enum OfficialNotificationFlag { };
        public static async Task HandleIntent(Dictionary<string, string> intent)
        {
            foreach (var f in Enum.GetValues(typeof(Flags))) Trace.Assert(intent.ContainsKey($"{f}"));
            string type = intent[$"{Flags.Type}"];
            if (type == $"{SendType.CarRequest}")
            {
                foreach (var f in Enum.GetValues(typeof(CarRequestFlag))) Trace.Assert(intent.ContainsKey($"{f}"));
                if (await Application.Current.MainPage.DisplayAlert(intent[$"{Flags.Message}"], "Accept?\r\n按下「Yes」之後您將需要在下一個頁面開價", "Yes", "No"))
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new CommunicationPage.CarOwnerPriceRequestPage(intent));
                }
            }
            else if (type == $"{SendType.CarOwnerPriceRequest}")
            {
                foreach (var f in Enum.GetValues(typeof(CarOwnerPriceRequestFlag))) Trace.Assert(intent.ContainsKey($"{f}"));
                await Application.Current.MainPage.DisplayAlert("", $"{intent[Flags.SenderId.ToString()]} offer a price of {intent[CarOwnerPriceRequestFlag.Price.ToString()]}", "OK");
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
            public static async Task CarOwnerPriceRequest(int price, string senderId)
            {
                await SendNotification("Sharing Cars", $"{AppData.AppData.userFacebookProfile.name} offered a price", $"{SendType.CarOwnerPriceRequest}",
                    new Dictionary<string, string>
                    {
                        { $"{CarOwnerPriceRequestFlag.Price}", $"{price}" }
                    }, senderId);
            }
            public static async Task CarRequest(Xamarin.Forms.Maps.MapSpan region)
            {
                await SendNotification("Sharing Cars", $"{AppData.AppData.userFacebookProfile.name} need a car", $"{SendType.CarRequest}",
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