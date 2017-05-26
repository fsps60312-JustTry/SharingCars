using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SharingCars.Developer
{
    class DeveloperPage : ContentPage
    {
        ScrollView SVmain;
        StackLayout SLmain;
        Button BTNerrorList,BTNpushNotification;
        public DeveloperPage()
        {
            InitializeViews();
            RegisterEvents();
        }
        private void RegisterEvents()
        {
            BTNerrorList.Clicked += async delegate
              {
                  BTNerrorList.IsEnabled = false;
                  await Navigation.PushAsync(new ErrorListPage());
                  BTNerrorList.IsEnabled = true;
              };
            BTNpushNotification.Clicked += async delegate
              {
                  BTNpushNotification.IsEnabled = false;
                  await Navigation.PushAsync(new PushNotificationPage());
                  BTNpushNotification.IsEnabled = true;
              };
        }
        private void InitializeViews()
        {
            {
                SVmain = new ScrollView();
                {
                    SLmain = new StackLayout();
                    {
                        BTNerrorList = new Button
                        {
                            Text = "Error List"
                        };
                        SLmain.Children.Add(BTNerrorList);
                    }
                    {
                        BTNpushNotification = new Button
                        {
                            Text = "Push Notification"
                        };
                        SLmain.Children.Add(BTNpushNotification);
                    }
                    SVmain.Content = SLmain;
                }
                this.Content = SVmain;
            }
        }
    }
}
