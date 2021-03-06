﻿using Android.App;
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
[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]
//GET_ACCOUNTS is only needed for android versions 4.0.3 and below
[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]

namespace SharingCars.Droid
{
    [BroadcastReceiver(Permission = Gcm.Client.Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "@PACKAGE_NAME@" })]
    public class PushHandlerBroadcastReceiver : GcmBroadcastReceiverBase<GcmService>
    {
        public static string SenderID = "625479233606";
        public static string ListenConnectionString = "Endpoint=sb://sharingcars.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=jRtGS728Qwd406oUcUMBb8YxFITODMI55OU6uNT9EXA=";
        public static string NotificationHubName = "SharingCars";
    }
    [Service]
    public class GcmService : GcmServiceBase
    {
        public static string RegistrationToken { get; private set; }
        public GcmService()
            : base(PushHandlerBroadcastReceiver.SenderID) { }
        public static MobileServiceClient MobileService = new MobileServiceClient(@"https://sharingcars.azurewebsites.net");
        protected override void OnRegistered(Context context, string registrationToken)
        {
            Log.Verbose("PushHandlerBroadcastReceiver", "GCM Registered: " + registrationToken);
            RegistrationToken = registrationToken;
            var push = MobileService.GetPush();
            var tags = new List<string>
            {
                "Android",
                AppData.AppData.DeviceId
            };
            MainActivity.CurrentActivity.RunOnUiThread(() => Register(push, tags));
        }
        protected override void OnUnRegistered(Context context, string registrationToken)
        {
            Log.Error("PushHandlerBroadcastReceiver", "Unregistered RegisterationToken: " + registrationToken);
        }
        protected override void OnError(Context context, string errorId)
        {
            Log.Error("PushHandlerBroadcastReceiver", "GCM Error: " + errorId);
        }
        public async void Register(Microsoft.WindowsAzure.MobileServices.Push push, IEnumerable<string> tags)
        {
            try
            {
                const string templateBodyGCM = "{\"data\":{\"message\":\"$(messageParam)\"}}";
                JObject templates = new JObject
                {
                    ["genericMessage"] = new JObject
                    {
                        {"body",templateBodyGCM }
                    }
                };
                await push.RegisterAsync(RegistrationToken, templates);
                Log.Info("Push Installation Id", push.InstallationId.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                Debugger.Break();
            }
        }
        protected override void OnMessage(Context context, Intent intent)
        {
            Log.Info("PushHandlerBroadcastReceiver", "GCM Message Received!");
            var msg = new StringBuilder();
            if (intent != null && intent.Extras != null)
            {
                foreach (var key in intent.Extras.KeySet())
                    msg.AppendLine(key + "=" + intent.Extras.Get(key).ToString());
            }

            //Retrieve the message
            var prefs = GetSharedPreferences(context.PackageName, FileCreationMode.Private);
            var edit = prefs.Edit();
            edit.PutString("last_msg", msg.ToString());
            edit.Commit();

            string message = intent.Extras.GetString("message");
            if (string.IsNullOrEmpty(message)) message = "(null)";
            string title = intent.Extras.GetString("title");
            if (string.IsNullOrEmpty(title)) title = "(null)";
            //if (!string.IsNullOrEmpty(message))
            //{
            CreateNotification(title, message);
            //    return;
            //}
        }
        static int id = 1,ie=0;
        void CreateNotification(string title, string msg)
        {
            Debug.WriteLine($"Received notification: title={title}, desc={msg}");
            var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;
            var uiIntent = new Intent(this, typeof(MainActivity));
            uiIntent.PutExtra("alert", $"hi{id}");
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
                .SetTicker(title)
                .SetContentTitle(title)
                .SetContentText(msg)
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                .SetAutoCancel(true)
                //.SetNumber(1)
                .AddAction(new NotificationCompat.Action(0, "OK", default(PendingIntent)))
                .AddAction(new NotificationCompat.Action(0, "Dismiss", default(PendingIntent)))
                .Build();
            notificationManager.Notify(id++, notification);
        }
    }
}