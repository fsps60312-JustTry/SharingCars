using System;
using Xamarin.Forms;

namespace SharingCars.MainTabbedPage.TabPages
{
    class ProfilePage : ScrollView
    {
        StackLayout SLmain;
        Button BTNprofile, BTNtravelCredit, BTNlistYourSpace, BTNsettings, BTNhelp, BTNgiveUsFeedback;
        public ProfilePage()
        {
            InitializeViews();
            RegisterEvents();
        }
        private void RegisterEvents()
        {
            BTNprofile.Clicked += async delegate (object sender, EventArgs e)
            {
                (sender as Button).IsEnabled = false;
                await Navigation.PushAsync(new UserInfo.UserInfoPage());
                (sender as Button).IsEnabled = true;
            };
        }
        private void InitializeViews()
        {
            //this.Text = "Profile";
            {
                SLmain = new StackLayout();
                {
                    BTNprofile = new Button()
                    {
                        Text = "Profile"
                    };
                    SLmain.Children.Add(BTNprofile);
                }
                {
                    BTNtravelCredit = new Button()
                    {
                        Text = "Travel Credit"
                    };
                    SLmain.Children.Add(BTNtravelCredit);
                }
                {
                    BTNlistYourSpace = new Button()
                    {
                        Text = "List your space"
                    };
                    SLmain.Children.Add(BTNlistYourSpace);
                }
                {
                    BTNsettings = new Button()
                    {
                        Text = "Settings"
                    };
                    SLmain.Children.Add(BTNsettings);
                }
                {
                    BTNhelp = new Button()
                    {
                        Text = "Help"
                    };
                    SLmain.Children.Add(BTNhelp);
                }
                {
                    BTNgiveUsFeedback = new Button()
                    {
                        Text = "Give us feedback"
                    };
                    SLmain.Children.Add(BTNgiveUsFeedback);
                }
                this.Content = SLmain;
            }
        }
    }
}
