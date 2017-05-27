using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SharingCars.AppData
{
    class UserInfoData
    {
        //public PhotoInfo Photo;
        public string PhotoSource;
        public string Name;
        public string FacebookId;
    }
    class UserInfo:InfoPrototype<UserInfoData>
    {
        static string Name = "user-info";
        public async Task<UserInfoData> GetData() { return await GetData(Name); }
        public static async Task<UserInfo> New(UserInfoData data) { return new UserInfo { Id = await InfoPrototype<UserInfoData>.New(Name, data) }; }
    }
}
