using Microsoft.AspNetCore.Mvc;
using Otto.orders.DTOs;
using Otto.orders.Mapper;
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
        private readonly MOrdersService _mOrdersService;
        private readonly MercadolibreService _mercadolibreService;

        public MOrdersController(IHttpContextAccessor httpContextAccessor, QueueTasks queueTasks,
            MOrdersService mOrdersService, MercadolibreService mercadolibreService)
        {
            _httpContextAccessor = httpContextAccessor;
            _queueTasks = queueTasks;
            _mOrdersService = mOrdersService;

            //Check for unread notifications and queue if necesary
            CheckUnreadNotifications();
        }

        private async void CheckUnreadNotifications()
        {
            var unreadNotifications = await _mOrdersService.GetMUnreadNotificationsAsync();

            var messages = MissedFeedsMapper.GetListMOrderNotificationDTO(unreadNotifications?.Messages);

            foreach (var msj in messages) 
            {
                if(!string.IsNullOrEmpty(msj.Topic) && msj.Topic.Contains("orders_v2"))
                    _queueTasks.Enqueue(_mOrdersService.ProcesarOrden(msj));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(MOrderNotificationDTO dto)
        {
            //string jsonString = JsonSerializer.Serialize(dto);

            //Procesar dentro de una cola -- para responder dentro de los 500ms 
           _queueTasks.Enqueue(_mOrdersService.ProcesarOrden(dto));

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
