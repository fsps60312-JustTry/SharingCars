using System;

namespace SharingCars.Utils
{
    class Numbers
    {
        public static ulong GetRandomNumber()
        {
            while (true)
            {
                var ans = BitConverter.ToUInt64(Guid.NewGuid().ToByteArray(), 0);
                if (ans != 0) return ans;
            }
        }
    }
}
