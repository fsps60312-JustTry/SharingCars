using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Microsoft.WindowsAzure.Storage.Blob;
using SharingCars.Utils.Alerts;
using Newtonsoft.Json;

namespace SharingCars.Developer
{
    class ErrorListPage:ContentPage
    {
        Grid GDmain;
        ActivityIndicator AImain;
        ScrollView SVmain;
        StackLayout SLmain;
        ToolbarItem TIrefresh;
        Dictionary<string, CloudBlockBlob> list = new Dictionary<string, CloudBlockBlob>();
        public ErrorListPage()
        {
            InitializeViews();
            RegisterEvents();
        }
        private void RegisterEvents()
        {
            TIrefresh.Clicked += async delegate
              {
                  AImain.IsVisible = AImain.IsRunning = true;
                  list = await ErrorReporter.DownloadErrorListAsync();
                  SLmain.Children.Clear();
                  foreach(var p in list)
                  {
                      var btn = new Button { Text = p.Key };
                      btn.Clicked += async delegate
                        {
                            btn.IsEnabled = false;
                            try
                            {
                                string text = btn.Text;
                                btn.Text = $"↓{btn.Text}";
                                var s = await Azure.DownloadTextAsync(p.Value);
                                btn.Text = text;
                                await new JustAlert(s).Show();
                            }
                            catch (OperationCanceledException) { }
                            btn.IsEnabled = true;
                        };
                      SLmain.Children.Add(btn);
                  }
                  AImain.IsVisible = AImain.IsRunning = false;
              };
        }
        private void InitializeViews()
        {
            {
                TIrefresh = new ToolbarItem
                {
                    Text = "Refresh"
                };
                this.ToolbarItems.Add(TIrefresh);
            }
            {
                GDmain = new Grid();
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                GDmain.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                {
                    AImain = new ActivityIndicator
                    {
                        IsVisible = false,
                        IsRunning = false
                    };
                    GDmain.Children.Add(AImain, 0, 0);
                }
                {
                    SVmain = new ScrollView();
                    {
                        SLmain = new StackLayout();
                        SVmain.Content = SLmain;
                    }
                    GDmain.Children.Add(SVmain, 0, 1);
                }
                this.Content = GDmain;
            }
        }
    }
}
