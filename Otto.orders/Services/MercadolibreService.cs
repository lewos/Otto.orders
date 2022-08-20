using Microsoft.Extensions.Caching.Memory;
using Otto.orders.DTOs;
using Otto.orders.Models;
using Otto.orders.Models.Responses;
using System.Text.Json;

namespace Otto.orders.Services
{
    public class MercadolibreService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly MemoryCacheEntryOptions _cacheEntryOptions;

        public MercadolibreService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        
        public async Task<MOrderResponse> GetMOrderAsync(long MUserId, string Resource, string AccessToken)
        {
            try
            {
                //Deberia estar en una variable de entorno
                string baseUrl = "https://api.mercadolibre.com";
                string endpoint = Resource.Substring(1);
                string url = string.Join('/', baseUrl, endpoint);


                var httpRequestMessage = new HttpRequestMessage(
                    HttpMethod.Get, url);

                httpRequestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

                var httpClient = _httpClientFactory.CreateClient();
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream =
                        await httpResponseMessage.Content.ReadAsStreamAsync();

                    var mOrder = await JsonSerializer.DeserializeAsync
                        <MOrderDTO>(contentStream);

                    //TODO comentar
                    string jsonString = JsonSerializer.Serialize(mOrder);
                    Console.WriteLine(jsonString);

                    return new MOrderResponse(Response.OK, $"{Response.OK}", mOrder);

                }
                //si no lo encontro, verificar en donde leo la respuesta del servicio
                return new MOrderResponse(Response.WARNING, $"No existe la orden {Resource} del usuario {MUserId}", null);


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la orden {Resource} del usuario {MUserId}. Ex : {ex}");
                //verificar en donde leo la respuesta del servicio
                return new MOrderResponse(Response.ERROR, $"Error al obtener la orden {Resource} del usuario {MUserId}. Ex : {ex}", null);

            }
        }
        public async Task<MItemResponse> GetMItemAsync(long MUserId, string Resource, string AccessToken)
        {
            try
            {
                //Deberia estar en una variable de entorno
                string baseUrl = "https://api.mercadolibre.com";
                string endpoint = Resource;
                string url = string.Join('/', baseUrl, endpoint);


                var httpRequestMessage = new HttpRequestMessage(
                    HttpMethod.Get, url);

                httpRequestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

                var httpClient = _httpClientFactory.CreateClient();
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream =
                        await httpResponseMessage.Content.ReadAsStreamAsync();

                    var mItem = await JsonSerializer.DeserializeAsync
                        <MItemDTO>(contentStream);


                    return new MItemResponse(Response.OK, $"{Response.OK}", mItem);

                }

                //si no lo encontro, verificar en donde leo la respuesta del servicio
                return new MItemResponse(Response.WARNING, $"No existe la orden {Resource} del usuario {MUserId}", null);


            }
            catch (Exception ex)
            {
                //verificar en donde leo la respuesta del servicio
                return new MItemResponse(Response.ERROR, $"Error al obtener la orden {Resource} del usuario {MUserId}. Ex : {ex}", null);

            }


        }
        public async Task<MUnreadNotificationsResponse> GetUnreadNotificationsAsync(long MUserId, string Resource, string AccessToken) 
        {
            try
            {
                //Deberia estar en una variable de entorno
                string baseUrl = "https://api.mercadolibre.com";
                string endpoint = $"missed_feeds?app_id={Resource}";
                string url = string.Join('/', baseUrl, endpoint);


                var httpRequestMessage = new HttpRequestMessage(
                    HttpMethod.Get, url);

                httpRequestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

                var httpClient = _httpClientFactory.CreateClient();
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream =
                        await httpResponseMessage.Content.ReadAsStreamAsync();

                    var missedFeedsDTO = await JsonSerializer.DeserializeAsync
                        <MissedFeedsDTO>(contentStream);

                    //comentar
                    //string jsonString = JsonSerializer.Serialize(missedFeedsDTO);
                    //Console.WriteLine(jsonString);

                    return new MUnreadNotificationsResponse(Response.OK, $"{Response.OK}", missedFeedsDTO);

                }
                //si no lo encontro, verificar en donde leo la respuesta del servicio
                return new MUnreadNotificationsResponse(Response.WARNING, $"Ocurrio un error al consultar las notificaciones no leidas {Resource} del usuario {MUserId}", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las notificaciones no leidas {Resource} del usuario {MUserId}. Ex : {ex}");
                //verificar en donde leo la respuesta del servicio
                return new MUnreadNotificationsResponse(Response.ERROR, $"Error al obtener las notificaciones no leidas {Resource} del usuario {MUserId}. Ex : {ex}", null);
            }

        }

        public async Task<string> GetPrintOrderAsync(long Resource, string AccessToken, bool pdf = true)
        {

            //Deberia estar en una variable de entorno
            string baseUrl = "https://api.mercadolibre.com";
            //Por ahora solo busco pdf
            string responseType = pdf ? "pdf" : "zpl2";
            string endpoint = $"shipment_labels?shipment_ids={Resource}&response_type={responseType}";
            string url = string.Join('/', baseUrl, endpoint);

            var pegaleAca = $"curl -X GET -H 'Authorization: Bearer {AccessToken}' {url}";
            return pegaleAca;
        }
    }
}
