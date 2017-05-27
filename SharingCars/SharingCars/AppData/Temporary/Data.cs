using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SharingCars.AppData.Temporary
{
    class Data
    {
        public static FacebookProfile facebookProfile { get; private set; }
        public static async Task SetFromFacebookProfile(FacebookProfile fp)
        {
            facebookProfile = fp;
            AppData.user = await UserInfo.New(new UserInfoData { FacebookId = fp.id, Name = fp.name, PhotoSource = fp.picture.data.url });
        }
    }
}
