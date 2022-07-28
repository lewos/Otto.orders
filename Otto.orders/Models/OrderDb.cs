using Microsoft.EntityFrameworkCore;

namespace Otto.orders.Models
{
    public class OrderDb : DbContext
    {
        public OrderDb(DbContextOptions options) : base(options) { }
        public DbSet<Order> Orders { get; set; }
    }
}
