using Otto.orders.DTOs;

namespace Otto.orders.Models.Responses
{
    public class UserResponse
    {
        public UserResponse(Response r, string v, UserDTO dto)
        {
            res = r;
            msg = v;
            user = dto;
        }

        public Response res { get; set; }

        public string msg { get; set; }

        public UserDTO user { get; set; }

    }
}
