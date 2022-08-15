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
        private readonly MOrdersService _mOrdersService;

        public MOrdersController(IHttpContextAccessor httpContextAccessor, QueueTasks queueTasks,
            MOrdersService mOrdersService)
        {
            _httpContextAccessor = httpContextAccessor;
            _queueTasks = queueTasks;
            _mOrdersService = mOrdersService;
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
