using Xamarin.Forms;

namespace SharingCars.UserInfo
{
    class ReportUserPage : ContentPage
    {
        public ReportUserPage()
        {
            this.Title = "Report User";
            DoConstructionActions();
        }
        private async void DoConstructionActions()
        {
            await DisplayAlert("User reported", "", "OK");
            await Navigation.PopAsync();
        }
    }
}
