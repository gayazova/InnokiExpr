using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;

namespace LinqToDbT
{
    public class DbContext : DataConnection
    {
        public DbContext() : base("Host=localhost;Database=innokit.07;Port=5433;Username=postgres;Password=postgres;Trust Server Certificate=True")
        {
        }

        public DbContext(LinqToDbConnectionOptions<DbContext> options) :
            base(options)
        {
         
        }

        public ITable<City> City => GetTable<City>();

        public ITable<WeatherForecast> WeatherForecast => GetTable<WeatherForecast>();

        public ITable<User> User => GetTable<User>();

        public ITable<BaseEntity> BaseEntity => GetTable<BaseEntity>();
    }
}
