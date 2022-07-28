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
            return await _db.Orders.ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _db.Orders.FindAsync(id);
        }

        public async Task<Tuple<Order, int>> CreateAsync(Order order) 
        {
            var utcNow = DateTime.UtcNow;

            order.Created = utcNow;
            order.Modified = utcNow;
            order.ShippingStatus = "pending";


            await _db.Orders.AddAsync(order);
            var rowsAffected = await _db.SaveChangesAsync();


            return new Tuple<Order, int>(order, rowsAffected);
        }

        public async Task<int> UpdateAsync(int id, Order newOrder)
        {
            // Si ya existe un token con ese mismo usuario, hago el update
            var order = await _db.Orders.Where(t => t.Id == id).FirstOrDefaultAsync();
            if (order != null)
            {
                UpdateOrderProperties(newOrder, order);
                UpdateDateTimeKindForPostgress(order);
            }

            _db.Entry(order).State = EntityState.Modified;
            return await _db.SaveChangesAsync();

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
            var rowsAffected = 0;
            var order = await _db.Orders.FindAsync(id);
            if (order != null)
            {
                _db.Orders.Remove(delOrder);
                rowsAffected = await _db.SaveChangesAsync();
            }
           


            return rowsAffected;
        }
}
}
