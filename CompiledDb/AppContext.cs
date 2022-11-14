using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Core.Objects;

//using System.Data.Entity;

namespace CompiledDb
{
    public class AppContext : Microsoft.EntityFrameworkCore.DbContext
    {
        private static string connectionString =
            "Host=localhost;Port=5433;Database=innokit.14;Username=postgres;Password=postgres;CommandTimeout=5";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().ToTable("City", "public");

            modelBuilder.Entity<Salary>().ToTable("Salary", "public");
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<City>().ToTable("City", "public");
        //}

        public DbSet<City> City { get; set; }

        public DbSet<Salary> Salary { get; set; }
    }

    //public class AppContext : ObjectContext
    //{
    //    private static string connectionString =
    //        "Host=localhost;Port=5433;Database=innokit.14;Username=postgres;Password=postgres;CommandTimeout=5";


    //    public AppContext() : base(connectionString) { }

    //    protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
    //    {
    //        modelBuilder.Entity<City>().ToTable("City", "public");
    //    }

    //    public System.Data.Entity.DbSet<City> City { get; set; }
    //}
}
