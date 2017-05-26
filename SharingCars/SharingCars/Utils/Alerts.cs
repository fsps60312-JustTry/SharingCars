using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SharingCars.Utils.Alerts
{
    class ErrorAlert : AlertInfo
    {
        public ErrorAlert(Exception error) : base("Error", error.ToString(), "OK")
        {
            ErrorReporter.ReportError(error);
        }
    }
    class DebugAlert : AlertInfo
    {
        public DebugAlert(string msg = "Something should run in the release version, but not yet for now") : base("Debugging...", msg, "OK") { }
    }
    class TodoAlert : AlertInfo
    {
        public TodoAlert() : base("", "抱歉，這個功能目前還在開發當中，將在未來的版本開放！", "OK") { }
    }
    class JustAlert : AlertInfo
    {
        public JustAlert(string msg) : base("", msg, "OK") { }
    }
    class AlertInfo
    {
        public static string Title { get; private set; }
        public static string Message { get; private set; }
        public static string Accept { get; private set; }
        public static string Cancel { get; private set; }
        public AlertInfo(string title, string message, string accept, string cancel = null)
        {
            Title = title;
            Message = message;
            Accept = accept;
            Cancel = cancel;
        }
        public async Task<bool> Show()
        {
            if (Cancel != null) return await Application.Current.MainPage.DisplayAlert(Title, Message, Accept, Cancel);
            else
            {
                await Application.Current.MainPage.DisplayAlert(Title, Message, Accept);
                return false;
            }
        }
    }
}
