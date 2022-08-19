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

        public async Task<Order> GetByMOrderIdAsync(string id)
        {
            using (var db = new OrderDb())
            {
                var order = await db.Orders.Where(t => t.MOrderId == long.Parse(id)).FirstOrDefaultAsync();
                return order;
            }
        }
        public async Task<List<Order>> GetOrderByPackId(string id)
        {
            using (var db = new OrderDb())
            {
                var orders = await db.Orders.Where(t => t.PackId == id).ToListAsync();
                return orders;
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


        public async Task<List<PackDTO>> GetPendingAsync()
        {
            using(var db = new OrderDb())
            {
                var orders = await db.Orders.Where(t => t.State == State.Pendiente).ToListAsync();

                List<OrderDTO> result = GetListOrderDTO(orders);

                List<PackDTO> otroResult = GetListPackDTO(result);

                return otroResult;
            }
        }

        private List<PackDTO> GetListPackDTO(List<OrderDTO> orders)
        {
            //agrupar por packid

            var result = new List<PackDTO>();

            var groupByPackIdQuery =
                                    from order in orders
                                    group order by order.PackId into newGroup
                                    orderby newGroup.Key
                                    select newGroup;

            foreach (var nameGroup in groupByPackIdQuery)
            {
                var items = new List<OrderDTO>();
                //ordenes sin pack id
                if (string.IsNullOrEmpty(nameGroup.Key)) 
                {
                    foreach (var order in nameGroup)
                    {
                        items = new List<OrderDTO>();
                        items.Add(order);
                        result.Add(new PackDTO(order.MOrderId.ToString(), "",  items));
                    }
                }
                else 
                {
                    foreach (var order in nameGroup)
                    {
                        items.Add(order);
                    }
                    result.Add(new PackDTO("", nameGroup.Key, items));
                }
            }
            return result;
        }

        private static List<OrderDTO> GetListOrderDTO(List<Order> orders)
        {
            var result = new List<OrderDTO>();
            orders.ForEach(order => result.Add(OrderMapper.GetOrderDTO(order)));
            return result;
        }

        public async Task<Tuple<Order, int>> CreateAsync(Order order)
        {
            try
            {
                using (var db = new OrderDb())
                {
                    await db.Orders.AddAsync(order);
                    var rowsAffected = await db.SaveChangesAsync();


                    return new Tuple<Order, int>(order, rowsAffected);
                }
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                Console.WriteLine($"Ya existe una orden con ese mOrdenId, se descarta el alta");
                return new Tuple<Order, int>(order, 1);
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrio un error al dar de alta la orden {ex}");
                throw;
            }            
        }
        public async Task<Tuple<OrderDTO, int>> UpdateOrderInProgressByMOrderIdAsync(string id, string UserIdInProgress)
        {
            //TODO check con el servicio de usuario con el id UserIdInProgress exista

            using (var db = new OrderDb())
            {
                var order = await db.Orders.Where(t => t.MOrderId == long.Parse(id) && t.InProgress == false).FirstOrDefaultAsync();
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

        public async Task<Tuple<PackDTO, int>> UpdateOrderInProgressByPackIdAsync(string id, string UserIdInProgress)
        {
            //TODO check con el servicio de usuario con el id UserIdInProgress exista

            using (var db = new OrderDb())
            {
                int rowsAffected = 0;

                var orders = await db.Orders.Where(t => t.PackId == id && t.InProgress == false).ToListAsync();
                if (orders != null)
                {
                    var items = new List<OrderDTO>();
                    foreach (var order in orders) 
                    {
                        UpdateOrderInProgressProperties(UserIdInProgress, order);
                        UpdateDateTimeKindForPostgress(order);
                        db.Entry(order).State = EntityState.Modified;
                        rowsAffected = + await db.SaveChangesAsync();
                        var dto = OrderMapper.GetOrderDTO(order);
                        items.Add(dto);
                    }

                    return new Tuple<PackDTO, int>(new PackDTO("", id, items), rowsAffected);
                }
                else 
                {
                    return new Tuple<PackDTO, int>(null, 0);
                }                

            }
        }

        public async Task<Tuple<OrderDTO, int>> UpdateOrderStopInProgressByMOrderIdAsync(string id, string UserIdInProgress)
        {
            //TODO check con el servicio de usuario con el id UserIdInProgress exista

            using (var db = new OrderDb())
            {
                //quien esta cancelando sea quien la tomo
                var order = await db.Orders.Where(t => t.MOrderId == long.Parse(id) &&
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



        public async Task<Tuple<PackDTO, int>> UpdateOrderStopInProgressByPackIdAsync(string id, string UserIdInProgress)
        {
            //TODO check con el servicio de usuario con el id UserIdInProgress exista

            using (var db = new OrderDb())
            {
                int rowsAffected = 0;
                //quien esta cancelando sea quien la tomo
                var orders = await db.Orders.Where(t => t.PackId == id && 
                                                       t.InProgress == true && 
                                                       t.UserIdInProgress == UserIdInProgress &&
                                                       t.State != State.Finalizada &&
                                                       t.State != State.Enviada
                                                       ).ToListAsync();
                if (orders != null)
                {
                    var items = new List<OrderDTO>();
                    foreach (var order in orders)
                    {
                        UpdateOrderInProgressProperties(UserIdInProgress, order, true);
                        UpdateDateTimeKindForPostgress(order);
                        db.Entry(order).State = EntityState.Modified;
                        rowsAffected = +await db.SaveChangesAsync();
                        var dto = OrderMapper.GetOrderDTO(order);
                        items.Add(dto);
                    }
                    return new Tuple<PackDTO, int>(new PackDTO("", id, items), rowsAffected);
                }
                else
                {
                    return new Tuple<PackDTO, int>(null, 0);
                }
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
            if(newOrder.InProgress != null)                 
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
