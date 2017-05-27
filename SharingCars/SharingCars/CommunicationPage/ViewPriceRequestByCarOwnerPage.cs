using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SharingCars.CommunicationPage
{
    class ViewPriceRequestByCarOwnerPage:ContentPage
    {
        Grid GDmain,GDbuttons;
        ScrollView SVmain;
        CarView carView;
        Label LBprice;
        Button BTNaccept, BTNdecline;
        Dictionary<string, string> data;
        public ViewPriceRequestByCarOwnerPage(Dictionary<string, string> _data)
        {
            data = _data;
            foreach (var f in Enum.GetValues(typeof(NotificationManager.Flags))) ErrorReporter.Assert(data.ContainsKey($"{f}"));
            foreach (var f in Enum.GetValues(typeof(NotificationManager.CarOwnerPriceRequestFlag))) ErrorReporter.Assert(data.ContainsKey($"{f}"));
            InitializeViews();
            RegisterEvents();
        }
        private void RegisterEvents()
        {
            BTNaccept.Clicked += async delegate
              {
                  BTNaccept.IsEnabled = false;
                  if (await DisplayAlert("", "您確定要接受這輛車?", "Yes", "No"))
                  {
                      await Navigation.PushAsync(new UserPage.UserInfoPage(ulong.Parse(data[$"{NotificationManager.Flags.UserId}"])));
                      await NotificationManager.Send.CarAccepted(data[$"{NotificationManager.Flags.DeviceId}"]);
                      await App.Current.MainPage.DisplayAlert("交易成功", "這是車主的聯絡資料，趕快和他聯絡並完成交易吧！", "OK");
                  }
                  BTNaccept.IsEnabled = true;
              };
            BTNdecline.Clicked += async delegate
              {
                  BTNdecline.IsEnabled = false;
                  if (await DisplayAlert("", "您確定要拒絕這輛車?", "Yes", "No"))
                  {
                      await Navigation.PopAsync();
                  }
                  BTNdecline.IsEnabled = true;
              };
        }
        private void InitializeViews()
        {
            {
                GDmain = new Grid();
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                {
                    SVmain = new ScrollView();
                    {
                        carView = new CarView(ulong.Parse(data[$"{NotificationManager.CarOwnerPriceRequestFlag.CarId}"]));
                        SVmain.Content = carView;
                    }
                    GDmain.Children.Add(SVmain, 0, 0);
                }
                {
                    LBprice = new Label
                    {
                        Text = $"開價：新台幣{data[$"{NotificationManager.CarOwnerPriceRequestFlag.Price}"]}元"
                    };
                    GDmain.Children.Add(LBprice, 0, 1);
                }
                {
                    GDbuttons = new Grid();
                    GDbuttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    GDbuttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    {
                        BTNaccept = new Button { Text = "接受" };
                        GDbuttons.Children.Add(BTNaccept, 0, 0);
                    }
                    {
                        BTNdecline = new Button { Text = "拒絕" };
                        GDbuttons.Children.Add(BTNdecline, 1, 0);
                    }
                    GDmain.Children.Add(GDbuttons, 0, 2);
                }
                this.Content = GDmain;
            }
        }
    }
}
