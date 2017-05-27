using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SharingCars.UserPage
{
    class UserInfoPage : ContentPage
    {
        StackLayout SLmain;
        Image IMGprofilePicture;
        Label LBname, LBaddress, LBschool, LBemail;
        Button BTNmyCars, BTNreportThisUser;
        public UserInfoPage(ulong userId)
        {
            InitializeViews();
            RegisterEvents();
            LoadData(userId);
        }
        private async void LoadData(ulong userId) { await LoadDataAsync(userId); }
        private async Task LoadDataAsync(ulong userId)
        {
            var userData = await new AppData.UserInfo { Id = userId }.GetData();
            LBname.Text = userData.Name;
            IMGprofilePicture.Source = userData.PhotoSource;
            if (userId != AppData.AppData.user.Id) BTNmyCars.IsVisible = false;
            if (userId == AppData.AppData.user.Id&&userData.FacebookId == "1330822433661758")
            {
                Button btn = new Button
                {
                    Text = "Developer Insight"
                };
                btn.Clicked += async delegate
                    {
                        btn.IsEnabled = false;
                        await Navigation.PushAsync(new Developer.DeveloperPage());
                        btn.IsEnabled = true;
                    };
                SLmain.Children.Add(btn);
            }
        }
        private void RegisterEvents()
        {
            this.ToolbarItems.Add(new ToolbarItem("Edit", null, async () =>
            {
                await DisplayAlert("Edit", "", "OK");
                if(await DisplayAlert("","Generate error?","Yes","No"))
                {
                    if(await DisplayAlert("","Is the error catched?","Yes","No"))
                    {
                        try
                        {
                            throw new Exception();
                        }
                        catch(Exception error)
                        {
                            await new SharingCars.Utils.Alerts.ErrorAlert("測試",error).Show();
                        }
                    }
                    else
                    {
                        System.Diagnostics.Trace.Assert(false);
                        //throw new Exception();
                    }
                }
            }));
            BTNmyCars.Clicked += async delegate (object sender, EventArgs e)
            {
                BTNmyCars.IsEnabled = false;
                await Navigation.PushAsync(new CarPage.CarPage());
                BTNmyCars.IsEnabled = true;
            };
            BTNreportThisUser.Clicked += async delegate (object sender, EventArgs e)
            {
                BTNreportThisUser.IsEnabled = false;
                //await UploadAndDownload();
                await Navigation.PushAsync(new ReportUserPage());
                BTNreportThisUser.IsEnabled = true;
            };
        }
        private void InitializeViews()
        {
            this.Title = "Personal Info";
            {
                SLmain = new StackLayout();
                {
                    IMGprofilePicture = new Image()
                    {
                        Source = "Icon.png"
                    };
                    SLmain.Children.Add(IMGprofilePicture);
                }
                {
                    LBname = new Label()
                    {
                        Text = "Name"
                    };
                    SLmain.Children.Add(LBname);
                }
                {
                    LBaddress = new Label()
                    {
                        Text = "Address"
                    };
                    SLmain.Children.Add(LBaddress);
                }
                {
                    LBschool = new Label()
                    {
                        Text = "School"
                    };
                    SLmain.Children.Add(LBschool);
                }
                {
                    LBemail = new Label()
                    {
                        Text = "Email"
                    };
                    SLmain.Children.Add(LBemail);
                }
                {
                    BTNmyCars = new Button()
                    {
                        Text = "My Cars"
                    };
                    SLmain.Children.Add(BTNmyCars);
                }
                {
                    BTNreportThisUser = new Button()
                    {
                        Text = "Report this user"
                    };
                    SLmain.Children.Add(BTNreportThisUser);
                }
                this.Content = SLmain;
            }
        }
    }
}
