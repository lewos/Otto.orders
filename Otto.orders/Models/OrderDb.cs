using Microsoft.EntityFrameworkCore;

namespace Otto.orders.Models
{
    public class OrderDb : DbContext
    {
        public OrderDb()
        {
            this.OnConfiguring(new DbContextOptionsBuilder());
        }
        public OrderDb(DbContextOptions options) : base(options) { }
        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(ConfigHelper.GetConnectionString("postgres://tpxcdnxwzhzdbi:453c8a7bbdf0e436b4223698a406484bfac7e4d708f2c1ea09ab1f52f7e5cdb2@ec2-44-206-214-233.compute-1.amazonaws.com:5432/d4b2kr0fu7iogt"));

            }
        }
    }
}
