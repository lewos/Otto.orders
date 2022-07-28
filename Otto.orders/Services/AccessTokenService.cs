using Microsoft.Net.Http.Headers;
using Otto.orders.DTOs;
using Otto.orders.Models;
using Otto.orders.Models.Responses;
using System.Text.Json;

namespace Otto.orders.Services
{
    public class AccessTokenService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccessTokenService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<MAccessTokenResponse> GetToken(long MUserId)
        {
            try
            {
                //TODO poner en una variable de entorno
                string baseUrl = "https://ottomtokens.herokuapp.com";
                string endpoint = "api/MTokens/ByMUserId";
                string url = string.Join('/', baseUrl, endpoint, MUserId);


                var httpRequestMessage = new HttpRequestMessage(
                    HttpMethod.Get, url)
                {
                    Headers =
                    {
                        { HeaderNames.Accept, "*/*" },
                    }
                };

                var httpClient = _httpClientFactory.CreateClient();
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream =
                        await httpResponseMessage.Content.ReadAsStreamAsync();

                    var mToken = await JsonSerializer.DeserializeAsync
                        <MTokenDTO>(contentStream);


                    //TODO en el caso que este vencido o este cerca de vencer hacer llamar a la api para hacer el refresh

                    Console.WriteLine(mToken.AccessToken);

                    return new MAccessTokenResponse(Response.OK, $"{Response.OK}", mToken);

                }


                //TODO si no lo encontro, verificar en donde leo la respuesta del servicio
                return new MAccessTokenResponse(Response.WARNING, $"No existe el token del usuario {MUserId}", null);


            }
            catch (Exception ex)
            {
                //TODO verificar en donde leo la respuesta del servicio
                return new MAccessTokenResponse(Response.ERROR, $"Error al obtener el token del usuario {MUserId}. Ex : {ex}", null);

            }

            
        }


    }
}
