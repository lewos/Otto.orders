using Otto.orders.DTOs;

namespace Otto.orders.Models.Responses
{
    public class MOrderResponse
    {
        public MOrderResponse(Response r, string v, MOrderDTO mOrder)
        {
            res = r;
            msg = v;
            mOrder = mOrder;
        }

        public Response res { get; set; }

        public string msg { get; set; }

        public MOrderDTO mOrder { get; set; }
    }
}
