using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SharingCars.Developer
{
    class PushNotificationPage:ContentPage
    {
        Grid GDmain;
        Button BTNsend;
        Editor EDtitle, EDmessage;
        public PushNotificationPage()
        {
            InitializeViews();
            RegisterEvents();;
        }
        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async() =>
            {
                if (await DisplayAlert("", "Do you want to quit?", "Yes", "No"))
                {
                    await Navigation.PopAsync();
                }
            });
            return true;
        }
        private async Task PushNotification(bool bePublic)
        {
            await NotificationManager.Send.OfficialNotification(EDtitle.Text, EDmessage.Text, bePublic);
        }
        private void RegisterEvents()
        {
            BTNsend.Clicked += async delegate
              {
                  BTNsend.IsEnabled = false;
                  if(await DisplayAlert("Push Notification","Are you sure?","Yes","No"))
                  {
                      await PushNotification(await DisplayAlert("", "Are you want to make this notification be public?", "Yes", "No"));
                      if(await DisplayAlert("", "Notification sent successfully!\r\nDo you want to go back to previous page?", "Yes","No"))
                      {
                          await Navigation.PopAsync();
                      }
                  }
                  BTNsend.IsEnabled = true;
              };
        }
        private void InitializeViews()
        {
            NavigationPage.SetHasBackButton(this, false);
            {
                GDmain = new Grid();
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                {
                    GDmain.Children.Add(new Label { Text = "Title" }, 0, 0);
                }
                {
                    EDtitle = new Editor
                    {
                    };
                    GDmain.Children.Add(EDtitle, 0, 1);
                }
                {
                    GDmain.Children.Add(new Label { Text = "Message" }, 0, 2);
                }
                {
                    EDmessage = new Editor
                    {
                    };
                    GDmain.Children.Add(EDmessage, 0, 3);
                }
                {
                    BTNsend = new Button
                    {
                        Text = "Send"
                    };
                    GDmain.Children.Add(BTNsend, 0, 4);
                }
                this.Content = GDmain;
            }
        }
    }
}
