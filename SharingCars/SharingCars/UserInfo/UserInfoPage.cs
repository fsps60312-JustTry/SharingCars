using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SharingCars.UserInfo
{
    class UserInfoPage : ContentPage
    {
        StackLayout SLmain;
        Image IMGprofilePicture;
        Label LBname, LBaddress, LBjoinTime, LBschool, LBemail;
        Button BTNmyCars, BTNreportThisUser;
        public UserInfoPage()
        {
            InitializeViews();
            RegisterEvents();
            LoadData();
        }
        private async void LoadData() { await LoadDataAsync(); }
        private async Task LoadDataAsync()
        {
            LBname.Text = AppData.AppData.userFacebookProfile.name;
            IMGprofilePicture.Source = AppData.AppData.userFacebookProfile.picture.data.url;
            await Task.Delay(1);
        }
        private void RegisterEvents()
        {
            this.ToolbarItems.Add(new ToolbarItem("Edit", null, async () =>
            {
                await DisplayAlert("Edit", "", "OK");
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
                    IMGprofilePicture = new Image();
                    IMGprofilePicture.Source = "Icon.png";
                    SLmain.Children.Add(IMGprofilePicture);
                }
                {
                    LBname = new Label();
                    LBname.Text = "Name";
                    SLmain.Children.Add(LBname);
                }
                {
                    LBaddress = new Label();
                    LBaddress.Text = "Address";
                    SLmain.Children.Add(LBaddress);
                }
                {
                    LBjoinTime = new Label();
                    LBjoinTime.Text = "Join time";
                    SLmain.Children.Add(LBjoinTime);
                }
                {
                    LBschool = new Label();
                    LBschool.Text = "School";
                    SLmain.Children.Add(LBschool);
                }
                {
                    LBemail = new Label();
                    LBemail.Text = "Email";
                    SLmain.Children.Add(LBemail);
                }
                {
                    BTNmyCars = new Button();
                    BTNmyCars.Text = "My Cars";
                    SLmain.Children.Add(BTNmyCars);
                }
                {
                    BTNreportThisUser = new Button();
                    BTNreportThisUser.Text = "Report this user";
                    SLmain.Children.Add(BTNreportThisUser);
                }
                this.Content = SLmain;
            }
        }
    }
}
