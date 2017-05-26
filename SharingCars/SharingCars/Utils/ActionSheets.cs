using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SharingCars.Utils.ActionSheets
{
    class DeviceLocationUpdateFailed : ActionSheetInfo
    {
        public const string Title = "無法取得您的位置，請嘗試找個空曠的地方讓GPS可以連線", Cancel = "取消", Destruction = null;
        public const string Button1 = "重試";
        public const string Button2 = "使用最近一次的位置";
        public override async Task<string> Show() { return await Application.Current.MainPage.DisplayActionSheet(Title, Cancel, Destruction, Button1, Button2); }
    }
    abstract class ActionSheetInfo
    {
        public abstract Task<string> Show();
    }
}
