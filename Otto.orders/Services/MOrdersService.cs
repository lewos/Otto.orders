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
                    //return await UpdateOrder(dto);

                    return 0;
                }
                return 0;
            }
            return 0;
        }

        private async Task<int> CreateOrder(MOrderNotificationDTO dto)
        {
            //Buscar el accessToken de ese usuario
            MTokenDTO accessToken = await GetAccessToken(dto);

            if (accessToken != null)
            {
                // TODO get order
                var orderResponse = await _mercadolibreService.GetMOrderCacheAsync((long)dto.MUserId, dto.Resource, accessToken.AccessToken);

                if (orderResponse.res == Response.OK)
                {
                    var mOrder = await GetMOrder(dto);

                    //TODO fijarme si la orden pertenece a un carrito
                    //El campo "pack_id" muestra el número de paquete al cual pertenece la orden.
                    if (mOrder.PackId == null)
                    {
                        //Hay un solo producto en ese paquete
                        //TODO Ver la orden -- no es necesario, depende de los datos a guardar

                        //Guardar la orden
                        //Verificar que la orden ya no exista

                        //
                        //
                        //TODO ver bajo que campo tengo que obtener la orden para ver si existe _orderService.GetByIdAsync
                        var a = await CreateOrder(mOrder);
                        return a;

                    }
                    else
                    {
                        //TODO get items from order
                        //var itemsOrder = await _mercadolibreService.GetItem((long)dto.MUserId, dto.Resource, accessToken.AccessToken);
                        Console.WriteLine($"Tiene pack id, por ahora no estoy haciendo nada");
                        return 0;
                    }

                }
                else
                {
                    //TODO...... no obtuve la orden
                    Console.WriteLine($"No se pudo obtener la orden");
                    return 0;
                }

                //TODO Ver si el producto o item de la orden esta dentro del deposito o es una venta/orden que no esta en el deposito
            }
            else
            {
                //TODO...... no obtuve el token
                Console.WriteLine($"No se pudo obtener el token");
                return 0;
            }
        }


        //private async Task<int> UpdateOrder(MOrderNotificationDTO dto)
        //{
        //    var mOrder = await GetMOrder(dto);
        //    //TODO Ver que campos hay que actualizar


        //    //Obtener la orden vieja, orden de la base

        //    //comparar con la orden vieja

        //      // Es una orden que tiene un nuevo claim o mediacion // ej cancelada
        //      // 

        //    //update

        //    //UpdateOrderTable            

        //}
        private async Task<int> UpdateOrderTable(MOrderDTO order)
        {
            var newOrder = new Order
            {

                UserId = "alo",
                MUserId = order.Seller.Id,
                MOrderId = order.Id,
                //BusinessId
                ItemId = order.OrderItems[0].Item.Id,
                ItemDescription = order.OrderItems[0].Item.Title,
                Quantity = order.OrderItems[0].Quantity,
                //PackId
                SKU = order.OrderItems[0].Item.SellerSku,
            };
            var oldOrder = await _orderService.GetByMOrderIdAsync(order.Id);

            var algo = await _orderService.UpdateAsync(oldOrder.Id, newOrder);
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
                // TODO get order
                orderResponse = await _mercadolibreService.GetMOrderCacheAsync((long)dto.MUserId, dto.Resource, accessToken.AccessToken);
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

                var order = await _orderService.GetByMOrderIdAsync(resource);

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

            var newOrder = new Order
            {
                UserId = user?.Id,
                MUserId = order.Seller.Id,
                MOrderId = order.Id,
                //BusinessId
                ItemId = order.OrderItems[0].Item.Id,
                ItemDescription = order.OrderItems[0].Item.Title,
                Quantity = order.OrderItems[0].Quantity,
                //PackId
                SKU = order.OrderItems[0].Item.SellerSku,
                State = State.Pendiente,
                ShippingStatus = State.Pendiente,
                Created = DateTime.UtcNow
            };
            var algo = await _orderService.CreateAsync(newOrder);
            Console.WriteLine($"Cantidad de filas afectadas {algo.Item2}");
            return algo.Item2;
        }

        private async Task<MTokenDTO> GetAccessToken(MOrderNotificationDTO dto)
        {
            var res = await _accessTokenService.GetTokenCacheAsync((long)dto.MUserId);

            if (hasTokenExpired(res.token))
                res = await _accessTokenService.GetTokenAfterRefresh((long)dto.MUserId);
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
    }
}
