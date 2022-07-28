using Otto.orders.DTOs;

namespace Otto.orders.Models.Responses
{
    public class MItemResponse
    {
        public MItemResponse(Response r, string v, MItemDTO mItem)
        {
            res = r;
            msg = v;
            mItem = mItem;
        }

        public Response res { get; set; }

        public string msg { get; set; }

        public MItemDTO mItem { get; set; }
    }
}
