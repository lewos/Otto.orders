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

        public MOrdersService(AccessTokenService accessTokenService, MercadolibreService mercadolibreService, OrderService orderService)
        {
            _accessTokenService = accessTokenService;
            _mercadolibreService = mercadolibreService;
            _orderService = orderService;
        }

        public async Task<int> ProcesarOrden(MOrderNotificationDTO dto)
        {
            //Ver si el topico es el que necesito
            if (!string.IsNullOrEmpty(dto.Topic) && dto.Topic.Contains("orders_v2"))
            {
                //Buscar el accessToken de ese usuario
                MTokenDTO accessToken = await GetAccessToken(dto);

                if(accessToken != null)                
                {
                    // TODO get order
                    var orderResponse = await _mercadolibreService.GetOrder((long)dto.MUserId, dto.Resource, accessToken.AccessToken);

                    if (orderResponse.res == Models.Response.OK)
                    {
                        var order = orderResponse.mOrder;

                        //TODO fijarme si la orden pertenece a un carrito
                        //El campo "pack_id" muestra el número de paquete al cual pertenece la orden.
                        if (order.PackId == null)
                        {
                            //Hay un solo producto en ese paquete
                            //TODO Ver la orden -- no es necesario, depende de los datos a guardar

                            //Guardar la orden
                            //Verificar que la orden ya no exista

                            //
                            //
                            //TODO ver bajo que campo tengo que obtener la orden para ver si existe _orderService.GetByIdAsync
                            var a = await CreateOrder(order);
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
            return 0;
        }

        private async Task<int> CreateOrder(MOrderDTO order)
        {
            var newOrder = new Order
            {

                UserId = "alo",
                MUserId = order.Seller.Id,
                //BusinessId
                ItemId = order.OrderItems[0].Item.Id,
                ItemDescription = order.OrderItems[0].Item.Title,
                Quantity = order.OrderItems[0].Quantity,
                //PackId
                SKU = order.OrderItems[0].Item.SellerSku,
            };
            var algo = await _orderService.CreateAsync(newOrder);
            Console.WriteLine($"Cantidad de filas afectadas {algo.Item2}");
            return algo.Item2;
        }

        private async Task<MTokenDTO> GetAccessToken(MOrderNotificationDTO dto)
        {
            var res = await _accessTokenService.GetToken((long)dto.MUserId);
            return res.token;
        }
    }
}
