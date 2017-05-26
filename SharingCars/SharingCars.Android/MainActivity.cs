using System;
using System.Text;
using System.Collections.Generic;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Gcm.Client;
using System.IO;
using Android.Content;

namespace SharingCars.Droid
{
	[Activity (Label = "SharingCars", Icon = "@drawable/icon", Theme="@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static MainActivity CurrentActivity { get; private set; }
        protected override void OnCreate (Bundle bundle)
        {
            CurrentActivity = this;

            TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar; 

			base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);
			LoadApplication (new SharingCars.App ());
            RegisterEverything(bundle);
            HandleIntent();
        }
        private void HandleIntent()
        {
            if (Intent.Extras == null) return;
            var intent = new Dictionary<string, string>();
            foreach (var key in Intent.Extras.KeySet())
            {
                intent[key] = Intent.GetStringExtra(key);
            }
            OnIntentAvailable(intent);
        }
        public static App.IntentAvailableEventHandler IntentAvailable;
        private void OnIntentAvailable(Dictionary<string, string> intent) { IntentAvailable?.Invoke(intent); }
        #region Registings
        private void RegisterEverything(Bundle bundle)
        {
            RegisterForErrorReporting();
            RegisterForGoogleMap(bundle);
            RegisterForNotificationReceiving();
        }
        private void RegisterForNotificationReceiving()
        {
            try
            {
                // Check to ensure everything's set up right
                GcmClient.CheckDevice(this);
                GcmClient.CheckManifest(this);

                // Register for push notification
                System.Diagnostics.Debug.WriteLine("Registering...");
                GcmClient.Register(this, MyBroadcastReceiver.SenderID);
            }
            catch (Java.Net.MalformedURLException)
            {
                CreateAndShowDialog("There was an error creating the client. Verify the URL.", "Error");
            }
            catch (Exception e)
            {
                CreateAndShowDialog(e.Message, "Error");
            }
        }
        private void RegisterForGoogleMap(Bundle bundle)
        {
            Xamarin.FormsMaps.Init(this, bundle);
        }
        private void RegisterForErrorReporting()
        {
            AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs e)
            {
                var msg = $"TaskScheduler.UnobservedTaskException\r\n{e}";
                System.Diagnostics.Trace.WriteLine(msg);
                App.Current.MainPage.DisplayAlert("AppDomain.CurrentDomain.UnhandledException", $"{e}", "OK");
                LogUnhandledException(new Exception("AppDomain.CurrentDomain.UnhandledException", e.ExceptionObject as Exception));
            };
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += delegate (object sender, System.Threading.Tasks.UnobservedTaskExceptionEventArgs e)
            {
                var msg = $"TaskScheduler.UnobservedTaskException\r\n{e}";
                System.Diagnostics.Trace.WriteLine(msg);
                App.Current.MainPage.DisplayAlert("TaskScheduler.UnobservedTaskException", $"{e}", "OK");
                LogUnhandledException(new Exception("TaskScheduler.UnobservedTaskException", e.Exception));
            };
            DisplayCrashReport();
        }
        #endregion
        #region Alert & Toast
        private void CreateAndShowDialog(string message, string title)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetMessage(message);
            builder.SetTitle(title);
            builder.Create().Show();
        }
        public void ToastNotify(string msg)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                //new AlertDialog.Builder(this).SetMessage(msg).Show();
                Toast.MakeText(this, msg, ToastLength.Long).Show();
            });
        }
        #endregion
        #region Error handling

        internal static void LogUnhandledException(Exception exception)
        {
            try
            {
                const string errorFileName = "Fatal.log";
                var libraryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // iOS: Environment.SpecialFolder.Resources
                var errorFilePath = Path.Combine(libraryPath, errorFileName);
                var errorMessage = String.Format("Time: {0}\r\nError: Unhandled Exception\r\n{1}",
                DateTime.Now, exception.ToString());
                File.WriteAllText(errorFilePath, errorMessage);

                // Log to Android Device Logging.
                Android.Util.Log.Error("Crash Report", errorMessage);
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }

        /// <summary>
        // If there is an unhandled exception, the exception information is diplayed 
        // on screen the next time the app is started (only in debug configuration)
        /// </summary>
        //[Conditional("DEBUG")]
        private void DisplayCrashReport()
        {
            const string errorFilename = "Fatal.log";
            var libraryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var errorFilePath = Path.Combine(libraryPath, errorFilename);

            if (!File.Exists(errorFilePath))
            {
                Toast.MakeText(this, "Hooray! No error since last launch!", ToastLength.Long).Show();
                return;
            }

            var errorText = File.ReadAllText(errorFilePath);
            new AlertDialog.Builder(this)
            .SetPositiveButton("Clear", (sender, args) =>
            {
                File.Delete(errorFilePath);
            })
            .SetNegativeButton("Close", (sender, args) =>
            {
                // User pressed Close.
            })
            .SetMessage(errorText)
            .SetTitle("Crash Report")
            .Show();
        }
        #endregion
    }
}

