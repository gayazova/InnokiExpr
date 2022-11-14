using LinqToDB.Mapping;

namespace LinqToDb
{
    public class City : Entity<long>
    {
        public string Name { get; set; }

        public long? Population { get; set; }

        [Association(ThisKey = "Wf", OtherKey = "Id", CanBeNull = true, Relationship = LinqToDB.Mapping.Relationship.ManyToOne)]
        public WeatherForecast Wf { get; set; }

        public TimeSpan TimeSpanTest { get; set; }

        public string MenuDisplay => "Города";
    }
}
