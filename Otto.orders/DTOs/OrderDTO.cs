namespace Otto.orders.DTOs
{
    // TODO poner una base clase
    public class OrderDTO
    {

        //id, vendedor, id_item, item_description, quantity ,id_carrito, sku, , shipping_status
        public int Id { get; set; }
        //vendedor
        public string? UserId { get; set; }
        public long? MUserId { get; set; }
        public long? MOrderId { get; set; }
        public long? BusinessId { get; set; }
        public string ItemId { get; set; } = null!;
        public string ItemDescription { get; set; } = null!;
        public int Quantity { get; set; }
        public string? PackId { get; set; }
        public string? SKU { get; set; }
        public string ShippingStatus { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public string? State { get; set; }
        public bool? InProgress { get; set; }
        public string? UserIdInProgress { get; set; }

        public DateTime? InProgressDateTimeTaken { get; set; }
        public DateTime? InProgressDateTimeModified { get; set; }
    }
}
