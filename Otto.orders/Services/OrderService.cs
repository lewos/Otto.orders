using Microsoft.EntityFrameworkCore;
using Otto.orders.DTOs;
using Otto.orders.Mapper;
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
                order.ShippingStatus = State.Pendiente;
                order.State = State.Pendiente;
                order.InProgress = false;

                await db.Orders.AddAsync(order);
                var rowsAffected = await db.SaveChangesAsync();


                return new Tuple<Order, int>(order, rowsAffected);
            }
        }

        public async Task<Tuple<Order, int>> UpdateOrderInProgressAsync(int id, string UserIdInProgress)
        {
            //TODO check con el servicio de usuario con el id UserIdInProgress exista

            using (var db = new OrderDb())
            {
                var order = await db.Orders.Where(t => t.Id == id && t.InProgress == false).FirstOrDefaultAsync();
                if (order != null)
                {
                    UpdateOrderInProgressProperties(UserIdInProgress, order);
                    UpdateDateTimeKindForPostgress(order);
                }

                db.Entry(order).State = EntityState.Modified;
                var rowsAffected = await db.SaveChangesAsync();
                return new Tuple<Order, int>(order, rowsAffected);
            }
        }

        private void UpdateOrderInProgressProperties(string UserIdInProgress, Order order)
        {
            var utcNow = DateTime.UtcNow;
            order.Modified = utcNow;
            order.State = State.Tomada;
            order.InProgress = true;
            order.UserIdInProgress = UserIdInProgress;
            order.InProgressDateTimeTaken = utcNow;
            order.InProgressDateTimeModified = utcNow;
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

        public async Task<Tuple<Order, int>> UpdateOrderTableAsync(int id, Order newOrder)
        {
            using (var db = new OrderDb())
            {
                // Si ya existe un token con ese mismo usuario, hago el update
                var order = await db.Orders.Where(t => t.Id == id).FirstOrDefaultAsync();
                if (order != null)
                {

                    var utcNow = DateTime.UtcNow;
                    order.Modified = DateTime.UtcNow;

                    UpdateOrderAllProperties(newOrder, order);
                    UpdateDateTimeKindForPostgress(order);
                }

                db.Entry(order).State = EntityState.Modified;
                var rowsAffected = await db.SaveChangesAsync();
                return new Tuple<Order, int>(order, rowsAffected);
            }
        }

        private void UpdateOrderAllProperties(Order newOrder, Order order)
        {
            var utcNow = DateTime.UtcNow;
            order.UserId = newOrder.UserId;
            order.MUserId = newOrder.MUserId;
            order.MOrderId = newOrder.MOrderId;
            order.BusinessId = newOrder.BusinessId;
            order.ItemId = newOrder.ItemId;
            order.ItemDescription = newOrder.ItemDescription;
            order.Quantity = newOrder.Quantity;
            order.PackId = newOrder.PackId;
            order.SKU = newOrder.SKU;
            order.ShippingStatus = newOrder.ShippingStatus;
            order.Created = newOrder.Created;
            order.Modified = utcNow;
            order.State = newOrder.State;
            order.InProgress = newOrder.InProgress;
            order.UserIdInProgress = newOrder.UserIdInProgress;
            order.InProgressDateTimeTaken = newOrder.InProgressDateTimeTaken;
            order.InProgressDateTimeModified = newOrder.InProgressDateTimeModified;
        }

        private void UpdateOrderProperties(Order newOrder, Order order)
        {
            var utcNow = DateTime.UtcNow;
            order.Modified = DateTime.UtcNow;
        }

        private void UpdateDateTimeKindForPostgress(Order order)
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
