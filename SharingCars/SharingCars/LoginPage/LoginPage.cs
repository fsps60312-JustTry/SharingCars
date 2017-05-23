using System;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.DeviceInfo;
using Newtonsoft.Json;

namespace SharingCars.LoginPage
{
    /*
     * C:\Program Files (x86)\Java\jdk1.8.0_112\bin>keytool -exportcert -alias androiddebugkey -keystore "C:\Users\Burney\AppData\Local\Xamarin\Mono for Android\debug.keystore" | C:\openssl-0.9.8k_WIN32\bin\openssl sha1 -binary | C:\openssl-0.9.8k_WIN32\bin\openssl base64
    */
    public partial class WebViewLogInToFacebook : WebView
    {
        public WebViewLogInToFacebook()
        {
            this.IsVisible = false;
        }
        public async Task<string> GetFacebookProfileJsonAsync()
        {
            this.IsVisible = true;
            this.Source = $"https://www.facebook.com/v2.9/dialog/oauth?client_id={AppData.AppData.FacebookAppId}&redirect_uri=https://www.facebook.com/connect/login_success.html&response_type=token";
            bool done = false;
            string facebookProfileJson = null;
            var eventMethod = new EventHandler<WebNavigatedEventArgs>(async (object sender, WebNavigatedEventArgs e) =>
            {
                //Trace.WriteLine($"url: {e.Url}");
                //await DisplayAlert("", e.Url, "OK");
                //e.Url example: https://www.facebook.com/connect/login_success.html#access_token=EAAI9LEGFCBYBAE6kkcYsFoFsQ51fSvxNgXWZCSxXGTn6ZCkEb9cIAZCQ2gil9iReqbJ4NaelQzmXX8oP2iAd3HyR6m5j7XZBhb4J0Yb1HoG1ulkqZAMjo2MXZCZAzNiL5PnZB0fBuvGeiLZBDByxctD2sXTF0xtFq33gZD&expires_in=5184000
                int index = e.Url.IndexOf("access_token=");
                if (index != -1)
                {
                    index += "access_token=".Length;
                    StringBuilder access_token = new StringBuilder();
                    for (; index < e.Url.Length && (char.IsLetter(e.Url[index]) || char.IsDigit(e.Url[index])); index++) access_token.Append(e.Url[index]);
                    facebookProfileJson = await GetFacebookProfileAsync(access_token.ToString());
                    done = true;
                }
            });
            this.Navigated += eventMethod;
            while (!done) await Task.Delay(500);
            this.Navigated -= eventMethod;
            this.IsVisible = false;
            return facebookProfileJson;
        }
        private async Task<string> GetFacebookProfileAsync(string access_token)
        {
            var requestUrl = $"https://graph.facebook.com/v2.9/me?access_token={access_token}&fields=name,picture,cover,age_range,devices,email,gender,is_verified";
            var httpClient = new System.Net.Http.HttpClient();
            var userJson = await httpClient.GetStringAsync(requestUrl);
            return userJson;
        }
    }
    public partial class LoginPage : ContentPage
    {
        Grid GDmain;
        Button BTNfacebookLogIn, BTNskip;
        Label LBdeviceId;
        WebViewLogInToFacebook WVfacebook;
        public LoginPage()
        {
            InitializeViews();
            RegisterEvents();
        }
        private async Task<bool> LogInToFacebook()
        {
            BTNfacebookLogIn.IsEnabled = false;
            BTNfacebookLogIn.Text = "Logging in to Facebook...";
            string facebookProfileJson = await WVfacebook.GetFacebookProfileJsonAsync();
            if (facebookProfileJson == null)
            {
                await DisplayAlert("", "Failed to get your Facebook info", "OK");
                BTNfacebookLogIn.Text = "Log in to Facebook";
                BTNfacebookLogIn.IsEnabled = true;
                return false;
            }
            else
            {
                facebookProfileJson = facebookProfileJson.Replace("\\/", "/");
                AppData.AppData.userFacebookProfile = (await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<AppData.FacebookProfile>(facebookProfileJson)));
                await Navigation.PopModalAsync();
                return true;
            }
        }
        private void RegisterEvents()
        {
            BTNfacebookLogIn.Clicked += async delegate (object sender, EventArgs e)
            {
                //(sender as Button).IsEnabled = false;
                await LogInToFacebook();
                //(sender as Button).IsEnabled = true;
            };
            BTNskip.Clicked += async delegate
            {
                if (await DisplayAlert("", "Some functions will be restricted", "OK", "Cancel"))
                {
                    AppData.AppData.userFacebookProfile = new AppData.FacebookProfile();
                    await Navigation.PopModalAsync();
                }
            };
        }
        private void InitializeViews()
        {
            {
                GDmain = new Grid();
                GDmain.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                GDmain.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                GDmain.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                {
                    BTNfacebookLogIn = new Button();
                    BTNfacebookLogIn.Text = "Log in with Facebook";
                    GDmain.Children.Add(BTNfacebookLogIn, 1, 1);
                }
                {
                    LBdeviceId = new Label();
                    LBdeviceId.Text = $"此裝置的ID: {CrossDeviceInfo.Current.Id}";
                    GDmain.Children.Add(LBdeviceId, 1, 2);
                }
                {
                    WVfacebook = new WebViewLogInToFacebook();
                    GDmain.Children.Add(WVfacebook, 0, 3);
                    Grid.SetColumnSpan(WVfacebook, 3);
                }
                {
                    BTNskip = new Button();
                    BTNskip.Text = "Skip as Guest User";
                    GDmain.Children.Add(BTNskip, 1, 4);
                }
                this.Content = GDmain;
            }
        }
    }
}
