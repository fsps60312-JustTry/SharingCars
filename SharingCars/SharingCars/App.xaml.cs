using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace SharingCars
{
	public partial class App : Application
    {
        public delegate void IntentAvailableEventHandler(Dictionary<string, string> intent);
        private static bool IsIntentRequestRegistered = false;
        public App ()
		{
			//InitializeComponent();
			MainPage = new NavigationPage(new SharingCars.MainTabbedPage.MainTabbedPage());
            RegisterIntentRequest();
        }
        private async void RegisterIntentRequest()
        {
            if (IsIntentRequestRegistered) return;
            IsIntentRequestRegistered = true;
            var eventContent = new IntentAvailableEventHandler(async (Dictionary<string, string> intent) =>
              {
                  //StringBuilder s = new StringBuilder();
                  //foreach (var p in intent) s.AppendLine($"{p.Key}: {p.Value}");
                  //await Application.Current.MainPage.DisplayAlert("", s.ToString(), "OK");
                  await NotificationManager.HandleIntent(intent);
              });
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    {
                        Droid.MainActivity.IntentAvailable += eventContent;
                        break;
                    }
                default:
                    {
                        await MainPage.DisplayAlert("", $"Receive notification not supported on platform {Device.RuntimePlatform}", "OK");
                        break;
                    }
            }
        }
        private async void CheckIntent()
        {
            var intent = await GetIntentAsync();
            if (intent == null) return;
            StringBuilder s = new StringBuilder();
            foreach(var p in intent)
            {
                s.AppendLine($"{p.Key}:{p.Value}");
            }
            await MainPage.DisplayAlert("", s.ToString(), "OK");
        }
        private async Task<Dictionary<string, string>> GetIntentAsync()
        {
            var intent = new Dictionary<string, string>();
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    {
                        //if (Droid.MainActivity.CurrentActivity == null) break;
                        foreach (var key in Droid.MainActivity.CurrentActivity.Intent.Extras.KeySet())
                        {
                            intent[key] = Droid.MainActivity.CurrentActivity.Intent.GetStringExtra(key);
                        }
                        break;
                    }
                default:
                    {
                        await MainPage.DisplayAlert("", $"Receive notification not supported on platform {Device.RuntimePlatform}", "OK");
                        break;
                    }
            }
            return intent;
        }
        protected override void OnStart ()
		{
            // Handle when your app starts
        }

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
