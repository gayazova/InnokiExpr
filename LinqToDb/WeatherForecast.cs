namespace LinqToDb
{
    public class WeatherForecast : BaseEntity
    {
        public DateTime Date { get; set; }

        //public int TEMPERATUREC { get; set; }

        //public double Rating { get; set; }

        //public Summaries Summary { get; set; }

        //public bool IsZaebis { get; set; }

        //public string Commentary { get; set; }
    }

    public enum Summaries
    {
        Freezing,

        Chilly,

        Cool,

        Warm,

        Hot,

        Scorching
    }
}
