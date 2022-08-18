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

        public async Task<Order> GetByMOrderIdWithoutPackIdAsync(long id)
        {
            using (var db = new OrderDb())
            {
                var order = await db.Orders.Where(t => t.MOrderId == id).FirstOrDefaultAsync();
                return order;
            }
        }


        public async Task<List<OrderDTO>> GetPendingAsync()
        {
            using(var db = new OrderDb())
            {
                var orders = await db.Orders.Where(t => t.State == State.Pendiente).ToListAsync();

                List<OrderDTO> result = GetListOrderDTO(orders);
                return result;
            }
        }

        private static List<OrderDTO> GetListOrderDTO(List<Order> orders)
        {
            var result = new List<OrderDTO>();
            orders.ForEach(order => result.Add(OrderMapper.GetOrderDTO(order)));
            return result;
        }

        public async Task<Tuple<Order, int>> CreateAsync(Order order)
        {
            using (var db = new OrderDb())
            {                
                await db.Orders.AddAsync(order);
                var rowsAffected = await db.SaveChangesAsync();


                return new Tuple<Order, int>(order, rowsAffected);
            }
        }

        public async Task<Tuple<OrderDTO, int>> UpdateOrderInProgressAsync(int id, string UserIdInProgress)
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
                else 
                {
                    return new Tuple<OrderDTO, int>(null, 0);
                }

                db.Entry(order).State = EntityState.Modified;
                var rowsAffected = await db.SaveChangesAsync();
                var dto = OrderMapper.GetOrderDTO(order);

                return new Tuple<OrderDTO, int>(dto, rowsAffected);
            }
        }

        public async Task<Tuple<OrderDTO, int>> UpdateOrderStopInProgressAsync(int id, string UserIdInProgress)
        {
            //TODO check con el servicio de usuario con el id UserIdInProgress exista

            using (var db = new OrderDb())
            {
                //quien esta cancelando sea quien la tomo
                var order = await db.Orders.Where(t => t.Id == id && 
                                                       t.InProgress == true && 
                                                       t.UserIdInProgress == UserIdInProgress &&
                                                       t.State != State.Finalizada &&
                                                       t.State != State.Enviada
                                                       ).FirstOrDefaultAsync();
                if (order != null)
                {
                    UpdateOrderInProgressProperties(UserIdInProgress, order, true);
                    UpdateDateTimeKindForPostgress(order);
                }
                else
                {
                    return new Tuple<OrderDTO, int>(null, 0);
                }

                db.Entry(order).State = EntityState.Modified;
                var rowsAffected = await db.SaveChangesAsync();
                var dto = OrderMapper.GetOrderDTO(order);

                return new Tuple<OrderDTO, int>(dto, rowsAffected);
            }
        }

        


        private void UpdateOrderInProgressProperties(string UserIdInProgress, Order order,bool CancelInProgress = false)
        {
            var utcNow = DateTime.UtcNow;
            if (CancelInProgress)
            {
                order.Modified = utcNow;
                order.State = State.Pendiente;
                order.InProgress = false;
                order.UserIdInProgress = String.Empty;
                order.InProgressDateTimeTaken = utcNow;
                order.InProgressDateTimeModified = utcNow;
            }
            else 
            {
                order.Modified = utcNow;
                order.State = State.Tomada;
                order.InProgress = true;
                order.UserIdInProgress = UserIdInProgress;
                order.InProgressDateTimeTaken = utcNow;
                order.InProgressDateTimeModified = utcNow;
            }

        }

        public async Task<Tuple<Order, int>> UpdateAsync(long id, Order newOrder)
        {
            try
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
            catch (Exception ex )
            {
                var a = ex;
                throw;
            }           
        }

        public async Task<Tuple<Order, int>> UpdateOrderTableByIdAsync(int id, Order newOrder)
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

        public async Task<Tuple<Order, int>> UpdateOrderTableByMIdAsync(long mOrderId, Order newOrder)
        {
            try
            {
                using (var db = new OrderDb())
                {
                    // Si ya existe un token con ese mismo usuario, hago el update
                    var order = await db.Orders.Where(t => t.MOrderId == mOrderId).FirstOrDefaultAsync();
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
            catch (Exception ex )
            {
                var j = ex;
                throw;
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
