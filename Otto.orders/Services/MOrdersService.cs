using Otto.orders.DTOs;
using Otto.orders.Models;
using Otto.orders.Models.Responses;

namespace Otto.orders.Services
{
    public class MOrdersService
    {
        private readonly AccessTokenService _accessTokenService;
        private readonly MercadolibreService _mercadolibreService;
        private readonly OrderService _orderService;
        private readonly UserService _userService;

        public MOrdersService(AccessTokenService accessTokenService, MercadolibreService mercadolibreService, OrderService orderService, UserService userService)
        {
            _accessTokenService = accessTokenService;
            _mercadolibreService = mercadolibreService;
            _orderService = orderService;
            _userService = userService;
        }

        public async Task<int> ProcesarOrden(MOrderNotificationDTO dto)
        {
            //Ver si el topico es el que necesito
            if (!string.IsNullOrEmpty(dto.Topic) && dto.Topic.Contains("orders_v2"))
            {
                //TODO si ya guarde la orden y mercadolibre sigue notificando, descartar notificacion
                // Guardar en un cache en memoria, algunas ordenes asi no consulto la base constantemente
                if (await isNewOrder(dto))
                {
                    return await CreateOrder(dto);
                }
                else
                {
                    return await UpdateOrder(dto);
                }
                return 0;
            }
            return 0;
        }

        private async Task<int> CreateOrder(MOrderNotificationDTO dto)
        {
            var mOrder = await GetMOrder(dto);
            if (mOrder != null)
            {
                //TODO SACAR comentarios

                //Si la orden pertenece a un carrito
                //El campo "pack_id" muestra el número de paquete al cual pertenece la orden.
                //if (mOrder.PackId == null)
                //Hay un solo producto en ese paquete                    
                //Guardar la orden
                //var a = await CreateOrder(mOrder);
                //return a;
                //Hay mas de un producto en ese paquete                    
                //Guardar un registro por cada producto
                //var a = await CreateOrder(mOrder);
                //return a;

                //Ver como llega la orden. LLega un item/producto por orden, por lo tanto tengo que guardar la orden y se va a agrupar por el pack id
                //Si no tiene pack_id, la orden contiene un solo producto
                var a = await CreateOrder(mOrder);
                return a;
            }
            else
            {
                //No obtuve la orden, mercadolibre va a mandar otra notificacion o
                // voy a leer cuando reinicie la aplicacion las notificaciones no leidas
                Console.WriteLine($"No se pudo obtener la orden");
                return 0;
            }

        //TODO Ver si el producto o item de la orden esta dentro del deposito o es una venta/orden que no esta en el deposito            
        }


        private async Task<int> UpdateOrder(MOrderNotificationDTO dto)
        {
            var mOrder = await GetMOrder(dto);
            if (mOrder != null)
            {
                //TODO Ver que campos hay que actualizar


                //Obtener la orden vieja, orden de la base

                //comparar con la orden vieja

                // Es una orden que tiene un nuevo claim o mediacion // ej cancelada
                // 

                //update

                //UpdateOrderTable     

                var a = await UpdateOrderTable(mOrder);
                return a;
            }
            else
            {
                //No obtuve la orden, mercadolibre va a mandar otra notificacion o
                // voy a leer cuando reinicie la aplicacion las notificaciones no leidas
                Console.WriteLine($"No se pudo obtener la orden");
                return 0;
            }
        }
        private async Task<int> UpdateOrderTable(MOrderDTO order)
        {
            //TODO verificar que campos hay que actualizar, lo mas seguro sea el estado si esta cancelado


            // Buscar ese usuario id
            var user = await GetUser(order.Seller.Id);

            var newOrder = new Order
            {
                UserId = user?.Id,
                MUserId = order.Seller.Id,
                MOrderId = order.Id,
                //BusinessId
                ItemId = order.OrderItems[0].Item.Id,
                ItemDescription = order.OrderItems[0].Item.Title,
                Quantity = order.OrderItems[0].Quantity,
                PackId = order.PackId.ToString(),
                SKU = order.OrderItems[0].Item.SellerSku,
                State = State.Pendiente,
                ShippingStatus = State.Pendiente,
                Created = DateTime.UtcNow
            };

            var algo = await _orderService.UpdateOrderTableByMIdAsync((long)newOrder.MOrderId, newOrder);
            Console.WriteLine($"Cantidad de filas afectadas {algo.Item2}");
            return algo.Item2;
        }
        private async Task<MOrderDTO> GetMOrder(MOrderNotificationDTO dto)
        {
            //Buscar el accessToken de ese usuario
            MTokenDTO accessToken = await GetAccessToken(dto);

            var orderResponse = new MOrderResponse(Response.ERROR, "", new MOrderDTO());

            if (accessToken != null)
            {
                orderResponse = await _mercadolibreService.GetMOrderAsync((long)dto.MUserId, dto.Resource, accessToken.AccessToken);
            }

            return orderResponse.res == Response.OK
                ? orderResponse.mOrder
                : null;

        }
        private async Task<UserDTO> GetUser(long MUserId)
        {
            var userResponse = await _userService.GetUserByMIdCacheAsync(MUserId);
            return userResponse.res == Response.OK
                ? userResponse.user
                : null;

        }
        private async Task<bool> isNewOrder(MOrderNotificationDTO dto)
        {
            try
            {
                var resource = long.Parse(dto.Resource.Split("/")[2]);

                var order = await _orderService.GetByMOrderIdWithoutPackIdAsync(resource);

                //si la orden es null, no lo encontro
                return order == null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private async Task<bool> isNewOrder(long resource)
        {
            try
            {
                var order = await _orderService.GetByMOrderIdWithoutPackIdAsync(resource);

                //si la orden es null, no lo encontro
                return order == null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<int> CreateOrder(MOrderDTO order)
        {
            // Buscar ese usuario id
            var user = await GetUser(order.Seller.Id);

            var utcNow = DateTime.UtcNow;
            var newOrder = new Order
            {
                UserId = user?.Id == null ? null : user?.Id,
                MUserId = order?.Seller?.Id == null ? null : order.Seller.Id,
                MOrderId = order?.Id == null ? null : order.Id,
                //BusinessId
                ItemId = order?.OrderItems[0].Item.Id == null ? null : order.OrderItems[0].Item.Id,
                ItemDescription = order?.OrderItems[0].Item.Title == null ? null : order.OrderItems[0].Item.Title,
                Quantity = order.OrderItems[0].Quantity,
                PackId = order.PackId == null ? null : order.PackId.ToString(),
                SKU = order.OrderItems[0].Item.SellerSku == null ? null : order.OrderItems[0].Item.SellerSku,
                State = State.Pendiente,
                ShippingStatus = State.Pendiente,
                Created = utcNow,
                Modified = utcNow,
                InProgress = false,
            };

            //Ver que no exista una orden con esos datos para no duplicar
            if (await isNewOrder(order.Id))
            {
                var algo = await _orderService.CreateAsync(newOrder);
                Console.WriteLine($"Cantidad de filas afectadas {algo.Item2}");
                return algo.Item2;
            }
            else
            {
                return 0;
            }
        }
        private async Task<MTokenDTO> GetAccessToken(MOrderNotificationDTO dto)
        {
            var res = await _accessTokenService.GetTokenCacheAsync((long)dto.MUserId);

            if (hasTokenExpired(res.token))
                res = await _accessTokenService.GetTokenAfterRefresh((long)dto.MUserId);
            return res.token;
        }
        private async Task<MTokenDTO> GetAccessToken(long mUserId)
        {
            var res = await _accessTokenService.GetTokenCacheAsync(mUserId);

            if (hasTokenExpired(res.token))
                res = await _accessTokenService.GetTokenAfterRefresh(mUserId);
            return res.token;
        }
        private bool hasTokenExpired(MTokenDTO token)
        {
            var utcNow = DateTime.UtcNow;
            // Si expiro o si esta a punto de expirar
            if (token.ExpiresAt < utcNow + TimeSpan.FromMinutes(10))
                return true;
            return false;
        }
        public async Task<MissedFeedsDTO> GetMUnreadNotificationsAsync()
        {
            //este el es id del usario dueño de la aplicacion en mercadolibre
            long mUserId = long.Parse(Environment.GetEnvironmentVariable("APP_MUSER_ID_OWNER"));
            string appId = Environment.GetEnvironmentVariable("APP_ID");

            //Buscar el accessToken de ese usuario
            MTokenDTO accessToken = await GetAccessToken(mUserId);

            var mUnreadNotificationsResponse = new MUnreadNotificationsResponse(Response.ERROR, "", new MissedFeedsDTO());

            if (accessToken != null)
            {
                mUnreadNotificationsResponse = await _mercadolibreService.GetUnreadNotificationsAsync(mUserId, appId, accessToken.AccessToken);
            }

            return mUnreadNotificationsResponse.res == Response.OK
                ? mUnreadNotificationsResponse.missedFeeds
                : null;
        }
    }
}
