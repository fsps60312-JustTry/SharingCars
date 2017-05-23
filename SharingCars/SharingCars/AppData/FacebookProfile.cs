using System.Collections.Generic;

namespace SharingCars.AppData
{
    class FacebookProfile
    {
        //{"name":"Burney Yu","picture":{"data":{"is_silhouette":false,"url":"https:\/\/scontent.xx.fbcdn.net\/v\/t1.0-1\/p50x50\/13346500_119843531771439_3069003593491048659_n.jpg?oh=dfb156ff6836f01f16441e1d5a2881c4&oe=59BBAA19"}},"cover":{"id":"119844001771392","offset_y":0,"source":"https:\/\/scontent.xx.fbcdn.net\/v\/t31.0-8\/s720x720\/13340249_119844001771392_3530815290059172243_o.jpg?oh=7a8bcfdcd4e4e377accafc6da2d26ff9&oe=597A939B"},"age_range":{"max":20,"min":18},"devices":[{"os":"Android"}],"gender":"male","is_verified":false,"id":"302579376831186"}
        public string name = "Burney Yu";
        public class pictureClass
        {
            public class dataClass
            {
                public bool is_silhouette = false;
                public string url = "https://scontent.xx.fbcdn.net/v/t1.0-1/p50x50/13346500_119843531771439_3069003593491048659_n.jpg?oh=dfb156ff6836f01f16441e1d5a2881c4&oe=59BBAA19";
            }
            public dataClass data = new dataClass();
        }
        public pictureClass picture = new pictureClass();
        public class coverClass
        {
            public string id = "119844001771392";
            public int offset_y = 0;
            public string source = "https://scontent.xx.fbcdn.net/v/t31.0-8/s720x720/13340249_119844001771392_3530815290059172243_o.jpg?oh=7a8bcfdcd4e4e377accafc6da2d26ff9&oe=597A939B";
        }
        public coverClass cover = new coverClass();
        public class age_rangeClass
        {
            public int max = 20;
            public int min = 18;
        }
        public age_rangeClass age_range = new age_rangeClass();
        public class devicesClass
        {
            public string os = "Android";
        }
        public List<devicesClass> devices = new List<devicesClass> { new devicesClass() };
        public string gender = "male";
        public bool is_verified = false;
        public string id = "302579376831186";
    }
}
