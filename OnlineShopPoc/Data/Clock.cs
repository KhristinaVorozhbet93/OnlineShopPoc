using OnlineShopPoc.Interfaces;

namespace OnlineShopPoc.Data
{
    public class Clock : IClock
    {
        public DateTime GetTimeUtc()
        {
            return DateTime.UtcNow;
        }
    }
}
