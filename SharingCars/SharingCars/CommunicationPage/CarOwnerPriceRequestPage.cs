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
        Grid GDmain;
        ScrollView SVmain;
        ActivityIndicator AImain;
        EntryCell ECprice;
        Button BTNsend;
        Dictionary<string, string> data;
        public CarOwnerPriceRequestPage(Dictionary<string, string> _data)
        {
            data = _data;
            foreach (var f in Enum.GetValues(typeof(NotificationManager.Flags))) Trace.Assert(data.ContainsKey($"{f}"));
            foreach (var f in Enum.GetValues(typeof(NotificationManager.CarRequestFlag))) Trace.Assert(data.ContainsKey($"{f}"));
            InitializeViews();
            RegisterEvents();
        }
        private async Task Send(int price)
        {
            await NotificationManager.Send.CarOwnerPriceRequest(price);
        }
        private void RegisterEvents()
        {
            BTNsend.Clicked += async delegate
              {
                  BTNsend.IsEnabled = false;
                  if (!int.TryParse(ECprice.Text, out int price))
                  {
                      await DisplayAlert("", $"Format of \"{ECprice.Label}\" is incorrect", "OK");
                  }
                  else
                  {
                      if (await DisplayAlert("Confirm", $"{ECprice.Label}: {price}\r\nAre you sure?", "Yes", "No"))
                      {
                          AImain.IsVisible = AImain.IsRunning = true;
                          await Send(price);
                          AImain.IsVisible = AImain.IsRunning = false;
                          await DisplayAlert("", "Request sent successfully!", "OK");
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
                        ECprice = new EntryCell()
                        {
                            Keyboard = Keyboard.Numeric,
                            Label = "Price",
                            Placeholder = "Enter your price..."
                        };
                        SVmain.Content = new TableView
                        {
                            Intent = TableIntent.Form,
                            Root = new TableRoot
                            {
                                new TableSection
                                {
                                    ECprice
                                }
                            }
                        };
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
