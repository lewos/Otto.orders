using Otto.orders.DTOs;

namespace Otto.orders.Models.Responses
{
    public class MUnreadNotificationsResponse
    {
        public MUnreadNotificationsResponse(Response r, string v, MissedFeedsDTO missedFeedsDTO)
        {
            res = r;
            msg = v;
            missedFeeds = missedFeedsDTO;
        }

        public Response res { get; set; }

        public string msg { get; set; }

        public MissedFeedsDTO missedFeeds { get; set; }

    }
}
