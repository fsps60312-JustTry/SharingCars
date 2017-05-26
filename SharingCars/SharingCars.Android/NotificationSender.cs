using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

using Java.IO;
using Java.Net;
using Javax.Crypto;
using Java;
using Java.Lang;
using Javax.Crypto.Spec;
using Java.Text;

using System.Collections.Generic;

namespace SharingCars.Droid
{
    class NotificationSender
    {
        private static String HubFullAccess = new String("Endpoint=sb://sharingcars.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=/W42qL77wo+uOzxpNR9pu++yLo0ye/eR+ioX22r10ys=");
        private static String HubName = new String("SharingCars");
        private static String HubEndpoint, HubSasKeyName, HubSasKeyValue;
        /**
          * Example code from http://msdn.microsoft.com/library/azure/dn495627.aspx
          * to parse the connection string so a SaS authentication token can be
          * constructed.
          *
          * @param connectionString This must be the DefaultFullSharedAccess connection
          *                         string for this example.
          */
        private static void ParseConnectionString(String connectionString)
        {
            String[] parts;
            {
                var t = connectionString.Split(";");
                parts = new String[t.Length];
                for (int i = 0; i < t.Length; i++) parts[i] = new String(t[i]);
            }
            if (parts.Length != 3)
                throw new AndroidRuntimeException("Error parsing connection string: "
                        + connectionString);

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].StartsWith("Endpoint"))
                {
                    HubEndpoint = new String("https" + parts[i].Substring(11));
                }
                else if (parts[i].StartsWith("SharedAccessKeyName"))
                {
                    HubSasKeyName = new String(parts[i].Substring(20));
                }
                else if (parts[i].StartsWith("SharedAccessKey"))
                {
                    HubSasKeyValue = new String(parts[i].Substring(16));
                }
            }
        }
        /**
          * Example code from http://msdn.microsoft.com/library/azure/dn495627.aspx to
          * construct a SaS token from the access key to authenticate a request.
          *
          * @param uri The unencoded resource URI string for this operation. The resource
          *            URI is the full URI of the Service Bus resource to which access is
          *            claimed. For example,
          *            "http://<namespace>.servicebus.windows.net/<hubName>"
          */
        private static String GenerateSasToken(String uri)
        {
            String targetUri;
            String token = null;
            try
            {
                targetUri = new String(URLEncoder
                        .Encode(uri.ToString().ToLower(), "UTF-8")
                        .ToLower());

                long expiresOnDate = JavaSystem.CurrentTimeMillis();
                int expiresInMins = 60; // 1 hour
                expiresOnDate += expiresInMins * 60 * 1000;
                long expires = expiresOnDate / 1000;
                String toSign = new String(targetUri + "\n" + expires);

                // Get an hmac_sha1 key from the raw key bytes
                byte[] keyBytes = HubSasKeyValue.GetBytes("UTF-8");
                SecretKeySpec signingKey = new SecretKeySpec(keyBytes, "HmacSHA256");

                // Get an hmac_sha1 Mac instance and initialize with the signing key
                Mac mac = Mac.GetInstance("HmacSHA256");
                mac.Init(signingKey);

                // Compute the hmac on input data bytes
                byte[] rawHmac = mac.DoFinal(toSign.GetBytes("UTF-8"));

                // Using android.util.Base64 for Android Studio instead of
                // Apache commons codec
                String signature = new String(URLEncoder.Encode(
                        Base64.EncodeToString(rawHmac, Base64Flags.NoWrap).ToString(), "UTF-8"));

                // Construct authorization string
                token = new String("SharedAccessSignature sr=" + targetUri + "&sig="
                        + signature + "&se=" + expires + "&skn=" + HubSasKeyName);
            }
            catch (Exception e)
            {
                MainActivity.CurrentActivity.ToastNotify("Exception Generating SaS : " + e.Message.ToString());
                //if (isVisible)
                //{
                //    ToastNotify("Exception Generating SaS : " + e.getMessage().toString());
                //}
            }
            return token;
        }
        /**
          * Send Notification button click handler. This method parses the
          * DefaultFullSharedAccess connection string and generates a SaS token. The
          * token is added to the Authorization header on the POST request to the
          * notification hub. The text in the editTextNotificationMessage control
          * is added as the JSON body for the request to add a GCM message to the hub.
          *
          * @param v
          */
        public static void SendNotification(string title, string _msg, string type, Dictionary<string, string> intent = null, string tag = null)
        {
            String msg = new String(_msg);
            //EditText notificationText = (EditText)findViewById(R.id.editTextNotificationMessage);
            //String json = "{\"data\":{\"message\":\"" + notificationText.getText().toString() + "\"}}";
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append(
                "{" +
                    "\"data\":" +
                    "{");
            if (intent != null)
            {
                foreach (var p in intent)
                {
                    jsonBuilder.Append(
                            $"\"{p.Key}\":\"{p.Value}\",");
                }
            }
            jsonBuilder.Append(
                        $"\"{NotificationManager.Flags.Title}\":\"" + title + "\"," +
                        $"\"{NotificationManager.Flags.Message}\":\"" + msg + "\"," +
                        $"\"{NotificationManager.Flags.Type}\":\"" + type + "\"," +
                        $"\"{NotificationManager.Flags.SenderId}\":\"" + AppData.AppDataConstants.DeviceId + "\"" +
                    "}" +
                "}");
            String json = new String(jsonBuilder.ToString());
            new Thread(new System.Action(() =>
            {
                try
                {
                    // Based on reference documentation...
                    // http://msdn.microsoft.com/library/azure/dn223273.aspx
                    ParseConnectionString(HubFullAccess);
                    URL url = new URL("" + HubEndpoint + HubName +
                            "/messages/?api-version=2015-01");

                    HttpURLConnection urlConnection = (HttpURLConnection)url.OpenConnection();

                    try
                    {
                        // POST request
                        urlConnection.DoOutput = true;//.SetDoOutput(true);

                        // Authenticate the POST request with the SaS token
                        urlConnection.SetRequestProperty("Authorization",
                                GenerateSasToken(new String(url.ToString())).ToString());

                        // Notification format should be GCM
                        urlConnection.SetRequestProperty("ServiceBusNotification-Format", "gcm");

                        // Include any tags
                        // Example below targets 3 specific tags
                        // Refer to : https://azure.microsoft.com/en-us/documentation/articles/notification-hubs-routing-tag-expressions/
                        // urlConnection.setRequestProperty("ServiceBusNotification-Tags", 
                        //        "tag1 || tag2 || tag3");

                        // Send notification message
                        if (tag != null) urlConnection.SetRequestProperty("ServiceBusNotification-Tags", tag);
                        var jsonBytes = json.GetBytes();
                        urlConnection.SetFixedLengthStreamingMode(jsonBytes.Length);
                        OutputStream bodyStream = new BufferedOutputStream(urlConnection.OutputStream/*.GetOutputStream()*/);
                        bodyStream.Write(jsonBytes);
                        bodyStream.Close();

                        //MainActivity.CurrentActivity.ToastNotify(urlConnection.ToString());
                        // Get reponse
                        urlConnection.Connect();
                        int responseCode = (int)urlConnection.ResponseCode;//.GetResponseCode();
                        if ((responseCode != 200) && (responseCode != 201))
                        {
                            BufferedReader br = new BufferedReader(new InputStreamReader((urlConnection.ErrorStream/*.getErrorStream()*/)));
                            String line;
                            StringBuilder builder = new StringBuilder("Send Notification returned " +
                                    responseCode + " : ");
                            while ((line = new String(br.ReadLine())) != null)
                            {
                                builder.Append(line.ToString());
                            }
                            MainActivity.CurrentActivity.ToastNotify(builder.ToString());
                        }
                    }
                    finally
                    {
                        urlConnection.Disconnect();
                    }
                }
                catch (Exception e)
                {
                    MainActivity.CurrentActivity.ToastNotify("Exception Sending Notification : " + e.Message.ToString());
                    //if (isVisible)
                    //{
                    //    ToastNotify("Exception Sending Notification : " + e.getMessage().toString());
                    //}
                }
            })).Start();
        }
    }
}