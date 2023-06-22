namespace OnlineShopPoc
{
    public class Clock : IClock
    {
        public DateTime GetTimeUtc()
        {
            return DateTime.UtcNow; 
        }
    }
}
