using System;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using SharingCars.Utils.Alerts;

namespace SharingCars.MainTabbedPage.TabPages
{
    /*
     * https://developer.xamarin.com/guides/xamarin-forms/user-interface/map/
     * Required Permissions for Xamarin.Forms.Maps.Map:
     * AccessCoarseLocation
     * AccessFineLocation
     * AccessLocationExtraCommands
     * AccessMockLocation
     * AccessNetworkState
     * AccessWifiState
     * Internet
    */
    /*
     * https://developer.xamarin.com/guides/android/platform_features/maps_and_location/maps/obtaining_a_google_maps_api_key/
     * keytool -list -v -keystore "C:\Users\Burney\AppData\Local\Xamarin\Mono for Android\debug.keystore" -alias androiddebugkey -storepass android -keypass android
    */
    class ExplorePage : RelativeLayout
    {
        Map MAP;
        Grid GDmain, GDhere;
        Button BTNfilter, BTNbroadcast, BTNhere;
        ActivityIndicator AIhere;
        public ExplorePage()
        {
            InitializeViews();
            RegisterEvents();
        }
        async Task PushNotificationToNearByCarOwners(MapSpan region)
        {
            await NotificationManager.Send.CarRequest(region);
        }
        private void SetCurrentPosition(Position position)
        {
            MAP.Pins.Clear();
            MAP.Pins.Add(new Pin() { Position = position, Label = "Me", Type = PinType.Generic });
            MAP.MoveToRegion(new MapSpan(position, 0.01, 0.01));
        }
        void RegisterEvents()
        {
            BTNhere.Clicked += async delegate
              {
                  BTNhere.IsEnabled = false;
                  AIhere.IsVisible = AIhere.IsRunning = true;
                  try
                  {
                      if (await AppData.AppData.deviceLocation.Update())
                      {
                          SetCurrentPosition(new Position(AppData.AppData.deviceLocation.Latitude, AppData.AppData.deviceLocation.Longitude));
                          await AppData.AppData.UploadAsync(AppData.AppData.DataType.DeviceLocation);
                      }
                      else
                      {
                          if (await Application.Current.MainPage.DisplayAlert("Can't get your location", $"Choose \"OK\" to use previous location", "OK", "Cancel"))
                          {
                              await AppData.AppData.DownloadAsync(AppData.AppData.DataType.DeviceLocation);
                              if (!AppData.AppData.deviceLocation.IsEnabled)
                              {
                                  await Application.Current.MainPage.DisplayAlert("", "Previous location doesn't exist!", "OK");
                              }
                              else
                              {
                                  SetCurrentPosition(new Position(AppData.AppData.deviceLocation.Latitude, AppData.AppData.deviceLocation.Longitude));
                              }
                          }
                      }
                  }
                  catch(Exception error)
                  {
                      await new ErrorAlert(error).Show();
                  }
                  AIhere.IsVisible = AIhere.IsRunning = false;
                  BTNhere.IsEnabled = true;
              };
            BTNbroadcast.Clicked += async delegate
            {
                BTNbroadcast.IsEnabled = false;
                if (await Application.Current.MainPage.DisplayAlert("確認", "向周圍的車主發送通知？", "OK", "Cancel"))
                {
                    await PushNotificationToNearByCarOwners(MAP.VisibleRegion);
                }
                BTNbroadcast.IsEnabled = true;
            };
        }
        void InitializeViews()
        {
            {
                MAP = new Map();
                SetCurrentPosition(new Position(25.0195948, 121.5418243));
                MAP.Pins.Add(new Pin() { Position = new Position(25.0195948 - 0.01 / 4, 121.5418243 - 0.01 / 4), Label = "Generic", Type = PinType.Generic });
                MAP.Pins.Add(new Pin() { Position = new Position(25.0195948 - 0.01 / 4, 121.5418243 + 0.01 / 4), Label = "Place", Type = PinType.Place });
                MAP.Pins.Add(new Pin() { Position = new Position(25.0195948 + 0.01 / 4, 121.5418243 - 0.01 / 4), Label = "SavedPin", Type = PinType.SavedPin });
                MAP.Pins.Add(new Pin() { Position = new Position(25.0195948 + 0.01 / 4, 121.5418243 + 0.01 / 4), Label = "SearchResult", Type = PinType.SearchResult });
                this.Children.Add(MAP,
                    Constraint.RelativeToParent((parent) => { return 0; }),
                    Constraint.RelativeToParent((parent) => { return 0; }),
                    Constraint.RelativeToParent((parent) => { return parent.Width; }),
                    Constraint.RelativeToParent((parent) => { return parent.Height; }));
            }
            {
                GDmain = new Grid();
                GDmain.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Star) });
                GDmain.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Auto) });
                GDmain.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Star) });
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10.0, GridUnitType.Star) });
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Auto) });
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Auto) });
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Auto) });
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Star) });
                {
                    BTNfilter = new Button()
                    {
                        Text = "Filter",
                        Opacity = 0.5
                    };
                    GDmain.Children.Add(BTNfilter, 1, 1);
                }
                {
                    GDhere = new Grid();
                    GDhere.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Auto) });
                    GDhere.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Auto) });
                    {
                        AIhere = new ActivityIndicator()
                        {
                            IsVisible = false,
                            IsRunning = false
                        };
                        GDhere.Children.Add(AIhere, 0, 0);
                    }
                    {
                        BTNhere = new Button()
                        {
                            Text = "Here",
                            Opacity = 0.5
                        };
                        GDhere.Children.Add(BTNhere, 1, 0);
                    }
                    GDmain.Children.Add(GDhere, 1, 2);
                }
                {
                    BTNbroadcast = new Button()
                    {
                        Text = "Broadcast",
                        Opacity = 0.5
                    };
                    GDmain.Children.Add(BTNbroadcast, 1, 3);
                }
                this.Children.Add(GDmain,
                    Constraint.RelativeToParent((parent) => { return 0; }),
                    Constraint.RelativeToParent((parent) => { return 0; }),
                    Constraint.RelativeToParent((parent) => { return parent.Width; }),
                    Constraint.RelativeToParent((parent) => { return parent.Height; }));
            }
        }
    }
}
