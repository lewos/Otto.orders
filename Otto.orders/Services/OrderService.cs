using Microsoft.EntityFrameworkCore;
using Otto.orders.Models;

namespace Otto.orders.Services
{
    public class OrderService
    {
        private readonly OrderDb _db;
        public OrderService(OrderDb db)
        {
            _db = db;
        }

        public async Task<List<Order>> GetAsync()
        {
            using (var db = new OrderDb())
            {
                return await db.Orders.ToListAsync();
            }
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            using (var db = new OrderDb()) 
            {
                return await db.Orders.FindAsync(id);
            }
        }

        public async Task<Order> GetByMOrderIdAsync(long id)
        {
            using (var db = new OrderDb())
            {
                var order = await db.Orders.Where(t => t.MOrderId == id).FirstOrDefaultAsync();
                return order;
            }
        }


        public async Task<Tuple<Order, int>> CreateAsync(Order order) 
        {
            using (var db = new OrderDb())
            {
                var utcNow = DateTime.UtcNow;

                order.Created = utcNow;
                order.Modified = utcNow;
                order.ShippingStatus = "pending";


                await db.Orders.AddAsync(order);
                var rowsAffected = await db.SaveChangesAsync();


                return new Tuple<Order, int>(order, rowsAffected);
            }
        }

        public async Task<Tuple<Order, int>> UpdateAsync(int id, Order newOrder)
        {
            using (var db = new OrderDb()) 
            {
                // Si ya existe un token con ese mismo usuario, hago el update
                var order = await db.Orders.Where(t => t.Id == id).FirstOrDefaultAsync();
                if (order != null)
                {
                    UpdateOrderProperties(newOrder, order);
                    UpdateDateTimeKindForPostgress(order);
                }

                db.Entry(order).State = EntityState.Modified;
                var rowsAffected = await db.SaveChangesAsync();
                return new Tuple<Order, int>(order, rowsAffected);
            }

        }

        private static void UpdateOrderProperties(Order newOrder, Order order)
        {
            var utcNow = DateTime.UtcNow;
            order.Modified = DateTime.UtcNow;
        }

        private static void UpdateDateTimeKindForPostgress(Order order)
        {
            order.Modified = DateTime.SpecifyKind((DateTime)order.Modified, DateTimeKind.Utc);        
        }

        public async Task<int> DeleteAsync(int id, Order delOrder)
        {
            using (var db = new OrderDb())
            {
                var rowsAffected = 0;
                var order = await db.Orders.FindAsync(id);
                if (order != null)
                {
                    db.Orders.Remove(delOrder);
                    rowsAffected = await db.SaveChangesAsync();
                }

                return rowsAffected;
            }
             
        }
}
}
