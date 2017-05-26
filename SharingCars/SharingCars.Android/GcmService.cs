using Android.App;
using Android.Content;
using Android.Media;
using Android.Support.V4.App;
using Android.Util;
using Gcm.Client;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WindowsAzure.Messaging;

[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]

//GET_ACCOUNTS is needed only for Android versions 4.0.3 and below
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]


namespace SharingCars.Droid
{
    // Receive Notifications: https://docs.microsoft.com/en-us/azure/notification-hubs/xamarin-notification-hubs-push-notifications-android-gcm
    [BroadcastReceiver(Permission = Gcm.Client.Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK },Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_LIBRARY_RETRY },Categories = new string[] { "@PACKAGE_NAME@" })]

    public class MyBroadcastReceiver : GcmBroadcastReceiverBase<PushHandlerService>
    {
        public static string SenderID = "625479233606";
        public static string ListenConnectionString = "Endpoint=sb://sharingcars.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=jRtGS728Qwd406oUcUMBb8YxFITODMI55OU6uNT9EXA=";
        public static string NotificationHubName = "SharingCars";
        public const string TAG = "MyBroadcastReceiver-GCM";
    }
    [Service] // Must use the service tag
    public class PushHandlerService : GcmServiceBase
    {
        private static string RegistrationID { get; set; }
        private NotificationHub Hub { get; set; }

        public PushHandlerService() : base(MyBroadcastReceiver.SenderID)
        {
            Log.Info(MyBroadcastReceiver.TAG, "PushHandlerService() constructor");
        }
        protected override void OnRegistered(Context context, string registrationId)
        {
            Log.Verbose(MyBroadcastReceiver.TAG, "GCM Registered: " + registrationId);
            RegistrationID = registrationId;
            //CreateNotification("PushHandlerService-GCM Registered...","The device has been Registered!");
            Hub = new NotificationHub(MyBroadcastReceiver.NotificationHubName, MyBroadcastReceiver.ListenConnectionString,context);
            try
            {
                Hub.UnregisterAll(registrationId);
            }
            catch (Exception ex)
            {
                Log.Error(MyBroadcastReceiver.TAG, ex.Message);
            }
            //var tags = new List<string>() { "falcons" }; // create tags if you want
            var tags = new List<string>()
            {
                "Android",
                AppData.AppDataConstants.DeviceId
            };
            try
            {
                var hubRegistration = Hub.Register(registrationId, tags.ToArray());
            }
            catch (Exception ex)
            {
                Log.Error(MyBroadcastReceiver.TAG, ex.Message);
            }
        }
        protected override void OnMessage(Context context, Intent intent)
        {
            Log.Info(MyBroadcastReceiver.TAG, "GCM Message Received!");
            Dictionary<string, string> notificationContents = new Dictionary<string, string>();
            if (intent != null && intent.Extras != null)
            {
                foreach (var key in intent.Extras.KeySet())
                    notificationContents[key] = intent.Extras.Get(key).ToString();
            }
            CreateNotification(notificationContents);
        }
        static int id = 1, ie = 0;
        void CreateNotification(Dictionary<string,string>contents)
        {
            var notificationManager = GetSystemService(Context.NotificationService) as Android.App.NotificationManager;
            var uiIntent = new Intent(this, typeof(MainActivity));
            uiIntent.SetFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
            foreach (var f in Enum.GetValues(typeof(NotificationManager.Flags)))
            {
                if (!contents.ContainsKey($"{f}")) contents[$"{f}"] = $"(Unknown {f})";
            }
            foreach (var p in contents) uiIntent.PutExtra(p.Key, p.Value);
            #region Sol1
            //uiIntent.SetAction(Intent.ActionMain);
            //uiIntent.AddCategory(Intent.CategoryLauncher);
            //uiIntent.PutExtra("alert", "hi");
            //uiIntent.AddFlags(ActivityFlags.SingleTop);
            #endregion
            #region Sol2
            //Android.App.TaskStackBuilder tsb = Android.App.TaskStackBuilder.Create(this);
            //tsb.AddParentStack(new MainActivity());
            //tsb.AddNextIntent(uiIntent);
            #endregion
            #region Sol3
            //uiIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            #endregion
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this);
            var notification = builder
                .SetContentIntent(PendingIntent.GetActivity(this, ie++, uiIntent, PendingIntentFlags.UpdateCurrent))
                .SetSmallIcon(Android.Resource.Drawable.SymActionEmail)
                .SetTicker(contents[$"{NotificationManager.Flags.Title}"])
                .SetContentTitle(contents[$"{NotificationManager.Flags.Title}"])
                .SetContentText(contents[$"{NotificationManager.Flags.Message}"])
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                .SetAutoCancel(true)
                //.SetNumber(1)
                //.AddAction(new NotificationCompat.Action(0, "OK", default(PendingIntent)))
                //.AddAction(new NotificationCompat.Action(0, "Dismiss", default(PendingIntent)))
                .Build();
            notificationManager.Notify(id++, notification);
        }
        protected override void OnUnRegistered(Context context, string registrationId)
        {
            Log.Verbose(MyBroadcastReceiver.TAG, "GCM Unregistered: " + registrationId);
            //CreateNotification("GCM Unregistered...", "The device has been unregistered!");
        }
        protected override bool OnRecoverableError(Context context, string errorId)
        {
            Log.Warn(MyBroadcastReceiver.TAG, "Recoverable Error: " + errorId);
            return base.OnRecoverableError(context, errorId);
        }
        protected override void OnError(Context context, string errorId)
        {
            Log.Error(MyBroadcastReceiver.TAG, "GCM Error: " + errorId);
        }
    }
}