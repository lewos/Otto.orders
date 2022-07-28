using Microsoft.AspNetCore.Mvc;
using Otto.orders.DTOs;
using Otto.orders.Models;
using Otto.orders.Services;
using System.Text.Json;

namespace Otto.orders.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MOrdersController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly QueueTasks _queueTasks;
        private readonly AccessTokenService _accessTokenService;
        private readonly MercadolibreService _mercadolibreService;
        private readonly OrderService _orderService;

        public MOrdersController(IHttpContextAccessor httpContextAccessor, QueueTasks queueTasks,
            AccessTokenService accessTokenService, MercadolibreService mercadolibreService, OrderService orderService)
        {
            _httpContextAccessor = httpContextAccessor;
            _queueTasks = queueTasks;
            _accessTokenService = accessTokenService;
            _mercadolibreService = mercadolibreService;
            _orderService = orderService;
        }

        // POST api/<OrdersController>
        [HttpPost]
        public async Task<IActionResult> Post(MOrderNotificationDTO dto)
        {

            // async procesar notificacion

            Console.WriteLine("Aver si funciona esto");
            string jsonString = JsonSerializer.Serialize(dto);

            Console.WriteLine(jsonString);


            //Console.WriteLine(value);
            Console.WriteLine("Funciono?");



            //Procesar dentro de una cola -- para responder dentro de los 500ms 
            _queueTasks.Enqueue(new Task(async () =>
            {
                //Ver si el topico es el que necesito
                if (!string.IsNullOrEmpty(dto.Topic) && dto.Topic.Contains("orders_v2"))
                {
                    //Buscar el accessToken de ese usuario
                    var accessTokenResponse = await _accessTokenService.GetToken((long)dto.MUserId);

                    MTokenDTO accessToken;

                    if (accessTokenResponse.res == Models.Response.OK)
                    {
                        accessToken = accessTokenResponse.token;


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

                            }
                            else
                            {
                                //TODO get items from order
                                //var itemsOrder = await _mercadolibreService.GetItem((long)dto.MUserId, dto.Resource, accessToken.AccessToken);
                                Console.WriteLine($"Tiene pack id, por ahora no estoy haciendo nada");
                            }

                        }
                        else
                        {
                            //TODO...... no obtuve la orden
                            Console.WriteLine($"No se pudo obtener la orden");
                        }



                        //TODO Ver si el producto o item de la orden esta dentro del deposito o es una venta/orden que no esta en el deposito





                    }
                    else
                    {
                        //TODO...... no obtuve el token
                        Console.WriteLine($"No se pudo obtener el token");
                    }








                    Console.WriteLine("");
                    //

                }
            }));


            return Ok();


        }


        //public GET
        //TODO si viene con GET
        //tngo que tomar el codigo de la url
        //llamar a la api para guardar el token
        //lo tengo que redigir a la pagina 

        // GET api/<OrdersController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var avergamondi = _httpContextAccessor.HttpContext.Request;

            // async procesar notificacion



            Console.WriteLine("Aver si funciona esto");
            //Console.WriteLine(value);
            Console.WriteLine("Funciono?");


            return Ok();


        }



    }
}
