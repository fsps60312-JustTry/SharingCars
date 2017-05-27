using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace SharingCars.CommunicationPage
{
    class CarOwnerPriceRequestPage:ContentPage
    {
        Grid GDmain,GDcar;
        ActivityIndicator AIcar;
        ScrollView SVmain;
        StackLayout SLmain;
        ActivityIndicator AImain;
        Button BTNcar;
        EntryCell ECprice;
        Button BTNsend;
        Dictionary<string, string> data;
        ulong carSelected = 0;
        public CarOwnerPriceRequestPage(Dictionary<string, string> _data)
        {
            data = _data;
            foreach (var f in Enum.GetValues(typeof(NotificationManager.Flags))) ErrorReporter.Assert(data.ContainsKey($"{f}"));
            foreach (var f in Enum.GetValues(typeof(NotificationManager.CarRequestFlag))) ErrorReporter.Assert(data.ContainsKey($"{f}"));
            InitializeViews();
            RegisterEvents();
        }
        private async Task Send(int price,string senderId)
        {
            await NotificationManager.Send.CarOwnerPriceRequest(price, carSelected, senderId);
        }
        private void RegisterEvents()
        {
            BTNcar.Clicked += async delegate
              {
                  BTNcar.IsEnabled = false;
                  AIcar.IsVisible = AIcar.IsRunning = true;
                  await AppData.AppData.DownloadAsync(AppData.AppData.DataType.Car);
                  AIcar.IsVisible = AIcar.IsRunning = false;
                  if (AppData.AppData.cars.Count==0)
                  {
                      await DisplayAlert("", "很抱歉，您目前沒有登記任何車輛", "OK");
                  }
                  else
                  {
                      AIcar.IsVisible = AIcar.IsRunning = true;
                      List<string> cars = new List<string>();
                      for(int i=0;i<AppData.AppData.cars.Count;i++)
                      {
                          var car = AppData.AppData.cars[i];
                          cars.Add($"{i+1}: {(await car.GetData()).name}");
                      }
                      AIcar.IsVisible = AIcar.IsRunning = false;
                      string s=await DisplayActionSheet("請選擇一台車", "取消", null, cars.ToArray());
                      AIcar.IsVisible = AIcar.IsRunning = true;
                      if (s!=null&&s!="取消")
                      {
                          s = s.Remove(s.IndexOf(':'));
                          ErrorReporter.Assert(int.TryParse(s, out int i));
                          i--;
                          var car = AppData.AppData.cars[i];
                          carSelected = car.Id;
                          BTNcar.Text = (await car.GetData()).name;
                      }
                      AIcar.IsVisible = AIcar.IsRunning = false;
                  }
                  BTNcar.IsEnabled = true;
              };
            BTNsend.Clicked += async delegate
              {
                  BTNsend.IsEnabled = false;
                  if (!int.TryParse(ECprice.Text, out int price))
                  {
                      await DisplayAlert("", $"欄位\"{ECprice.Label}\"的格式不正確", "OK");
                  }
                  else if(carSelected==0)
                  {
                      await DisplayAlert("", "請選擇您的「欲出借車輛」", "OK");
                  }
                  else
                  {
                      if (await DisplayAlert("確認", $"欲出借車輛：{BTNcar.Text}\r\n{ECprice.Label}：{price}元\r\n確定送出?", "Yes", "No"))
                      {
                          AImain.IsVisible = AImain.IsRunning = true;
                          await Send(price, data[$"{NotificationManager.Flags.DeviceId}"]);
                          AImain.IsVisible = AImain.IsRunning = false;
                          await Navigation.PopAsync();
                      }
                  }
                  BTNsend.IsEnabled = true;
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
                        SLmain = new StackLayout();
                        {
                            {
                                GDcar = new Grid();
                                GDcar.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                                GDcar.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                                {
                                    AIcar = new ActivityIndicator
                                    {
                                        IsVisible = false,
                                        IsRunning = false
                                    };
                                    GDcar.Children.Add(AIcar, 0, 0);
                                }
                                {
                                    BTNcar = new Button
                                    {
                                        Text = "欲出借車輛: 請選擇您擁有的一台車"
                                    };
                                    GDcar.Children.Add(BTNcar, 1, 0);
                                }
                                SLmain.Children.Add(GDcar);
                            }
                            {
                                ECprice = new EntryCell()
                                {
                                    Keyboard = Keyboard.Numeric,
                                    Label = "開價 (新台幣)",
                                    Placeholder = "開個價錢吧..."
                                };
                            }
                            SLmain.Children.Add(new TableView
                            {
                                Intent = TableIntent.Form,
                                Root = new TableRoot
                                {
                                    new TableSection
                                    {
                                        ECprice
                                    }
                                }
                            });
                        }
                        SVmain.Content = SLmain;
                    }
                    GDmain.Children.Add(SVmain, 0, 0);
                }
                {
                    AImain = new ActivityIndicator
                    {
                        IsVisible = false,
                        IsRunning = false
                    };
                    GDmain.Children.Add(AImain, 0, 1);
                }
                {
                    BTNsend = new Button
                    {
                        Text = "Send"
                    };
                    GDmain.Children.Add(BTNsend, 0, 2);
                }
                this.Content = GDmain;
            }
        }
    }
}
