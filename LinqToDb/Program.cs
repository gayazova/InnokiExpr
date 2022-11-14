using LinqToDB;
using LinqToDB.Configuration;

namespace LinqToDb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var connectionString = @"Host=localhost;Database=LinqToDb;Port=5433;Username=postgres;Password=postgres;Trust Server Certificate=True";

                // create options builder
                var builder = new LinqToDbConnectionOptionsBuilder();
                builder.UsePostgreSQL(connectionString);
                builder.UseMappingSchema(LinqToDbMapper.GetSchema());
                var opt = new LinqToDbConnectionOptions<DbContext>(builder);
                using (var context = new DbContext(opt))
                {
                    //var list = context.City.ToList();
                    //foreach (var item in list)
                    //{
                    //    Console.WriteLine($"{item.Id} {item.Name} {item.Wf?.Id}");
                    //}

                    //var wc = context.WeatherForecast.Join(context.City, SqlJoinType.Left)
                    //foreach (var weatherForecast in wc)
                    //{
                    //    Console.WriteLine($"{weatherForecast.Id} {weatherForecast.Date} {weatherForecast.CreatedByUser?.Id}");
                    //}
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.ReadKey();
        }
    }
}
