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
                optionsBuilder.UseNpgsql(ConfigHelper.GetConnectionString());

            }
        }
    }
}
