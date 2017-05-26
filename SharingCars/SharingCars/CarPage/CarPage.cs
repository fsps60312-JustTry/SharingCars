using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SharingCars.CarPage
{
    class CarPage : ContentPage
    {
        StackLayout SLmain, SLcars;
        ActivityIndicator AImain;
        ToolbarItem TIadd, TIupdate;
        public CarPage()
        {
            InitializeViews();
            RegisterEvents();
            DoConstructionActions();
        }
        private async Task UpdateViews()
        {
            SLcars.Children.Clear();
            for (int carIndex = 0; carIndex < AppData.AppData.cars.Count; carIndex++)
            {
                var car = await AppData.AppData.cars[carIndex].GetData();
                int carIndexNow = carIndex;
                var btn = new Button()
                {
                    Text = car.name
                };
                btn.Clicked += async delegate (object sender, EventArgs e)
                {
                    (sender as Button).IsEnabled = false;
                    await Navigation.PushAsync(new EditCarPage(carIndexNow));
                    (sender as Button).IsEnabled = true;
                };
                SLcars.Children.Add(btn);
            }
        }
        private async Task DownloadDataAsync()
        {
            AImain.IsRunning = AImain.IsVisible = true;
            await AppData.AppData.DownloadAsync(AppData.AppData.DataType.CarInfo);
            AImain.IsRunning = AImain.IsVisible = false;
        }
        private async void DoConstructionActions()
        {
            await DownloadDataAsync();
            await UpdateViews();
            this.Appearing += async delegate
            {
                await UpdateViews();
            };
        }
        private void RegisterEvents()
        {
            TIadd.Clicked += async delegate
            {
                await Navigation.PushAsync(new EditCarPage());
            };
            TIupdate.Clicked += async delegate
            {
                await DownloadDataAsync();
                UpdateViews();
            };
        }
        private void InitializeViews()
        {
            {
                TIadd = new ToolbarItem()
                {
                    Text = "Add"
                };
                this.ToolbarItems.Add(TIadd);
            }
            {
                TIupdate = new ToolbarItem()
                {
                    Text = "Update"
                };
                this.ToolbarItems.Add(TIupdate);
            }
            {
                SLmain = new StackLayout();
                {
                    AImain = new ActivityIndicator()
                    {
                        IsVisible = false
                    };
                    SLmain.Children.Add(AImain);
                }
                {
                    SLcars = new StackLayout();
                    SLmain.Children.Add(SLcars);
                }
                this.Content = SLmain;
            }
        }
    }
}
