using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SharingCars
{
    class CarView:StackLayout
    {
        StackLayout SLmain;
        Image IMGcar;
        TextCell TCname, TCtype, TCage;
        public CarView(ulong carId)
        {
            {
                SLmain = new StackLayout();
                {
                    IMGcar = new Image();
                    SLmain.Children.Add(IMGcar);
                }
                {
                    {
                        TCname = new TextCell
                        {
                            Text = "名稱"
                        };
                    }
                    {
                        TCtype = new TextCell
                        {
                            Text = "車種"
                        };
                    }
                    {
                        TCage = new TextCell
                        {
                            Text = "車齡"
                        };
                    }
                    SLmain.Children.Add(new TableView
                    {
                        Intent = TableIntent.Form,
                        Root = new TableRoot
                                {
                                    new TableSection
                                    {
                                        TCname,
                                        TCtype,
                                        TCage
                                    }
                                }
                    });
                }
                this.Children.Add(SLmain);
            }
            DoAsyncInitializationTasks(carId);
        }
        private async void DoAsyncInitializationTasks(ulong carId)
        {
            var carData = await new AppData.CarInfo { Id = carId }.GetData();
            TCname.Detail = carData.name;
            TCtype.Detail = carData.type.ToString();
            TCage.Detail = $"{carData.age}年";
            {
                var stream = await carData.photo.GetData();
                IMGcar.Source = ImageSource.FromStream(() => stream);
            }
        }
    }
}
