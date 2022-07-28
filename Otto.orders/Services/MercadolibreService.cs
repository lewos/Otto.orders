using Otto.orders.DTOs;
using Otto.orders.Models;
using Otto.orders.Models.Responses;
using System.Text.Json;

namespace Otto.orders.Services
{
    public class MercadolibreService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MercadolibreService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<MOrderResponse> GetOrder(long MUserId, string Resource, string AccessToken)
        {
            try
            {
                //TODO poner en una variable de entorno
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


                    return new MOrderResponse(Response.OK, $"{Response.OK}", mOrder);

                }

                //TODO si no lo encontro, verificar en donde leo la respuesta del servicio
                return new MOrderResponse(Response.WARNING, $"No existe la orden {Resource} del usuario {MUserId}", null);


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la orden {Resource} del usuario {MUserId}. Ex : {ex}");
                //TODO verificar en donde leo la respuesta del servicio
                return new MOrderResponse(Response.ERROR, $"Error al obtener la orden {Resource} del usuario {MUserId}. Ex : {ex}", null);

            }
        }

        public async Task<MItemResponse> GetItem(long MUserId, string Resource, string AccessToken)
        {
            try
            {
                //TODO poner en una variable de entorno
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

                //TODO si no lo encontro, verificar en donde leo la respuesta del servicio
                return new MItemResponse(Response.WARNING, $"No existe la orden {Resource} del usuario {MUserId}", null);


            }
            catch (Exception ex)
            {
                //TODO verificar en donde leo la respuesta del servicio
                return new MItemResponse(Response.ERROR, $"Error al obtener la orden {Resource} del usuario {MUserId}. Ex : {ex}", null);

            }


        }
    }
}
