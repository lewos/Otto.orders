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
            //_queueTasks.Enqueue(_mOrdersService.ProcesarOrden(dto));

            //await _mOrdersService.ProcesarOrden(dto); // funciona

            //var q = await _mOrdersService.ProcesarOrden(dto);

            _queueTasks.Enqueue(_mOrdersService.ProcesarOrden(dto));

            //_mOrdersService.ProcesarOrden(dto); // no se , no creo

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
