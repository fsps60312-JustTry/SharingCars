using Xamarin.Forms;

namespace SharingCars.UserPage
{
    class ReportUserPage : ContentPage
    {
        public ReportUserPage()
        {
            this.Title = "Report User";
            DoInitializationTasksAsync();
        }
        private async void DoInitializationTasksAsync()
        {
            await DisplayAlert("User reported", "", "OK");
            await Navigation.PopAsync();
        }
    }
}
