﻿using System.Text.Json.Serialization;

namespace Otto.orders.DTOs
{

    public class MItemDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("site_id")]
        public string SiteId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("subtitle")]
        public object Subtitle { get; set; }

        [JsonPropertyName("seller_id")]
        public int SellerId { get; set; }

        [JsonPropertyName("category_id")]
        public string CategoryId { get; set; }

        [JsonPropertyName("official_store_id")]
        public object OfficialStoreId { get; set; }

        [JsonPropertyName("price")]
        public int Price { get; set; }

        [JsonPropertyName("base_price")]
        public int BasePrice { get; set; }

        [JsonPropertyName("original_price")]
        public object OriginalPrice { get; set; }

        [JsonPropertyName("inventory_id")]
        public object InventoryId { get; set; }

        [JsonPropertyName("currency_id")]
        public string CurrencyId { get; set; }

        [JsonPropertyName("initial_quantity")]
        public int InitialQuantity { get; set; }

        [JsonPropertyName("available_quantity")]
        public int AvailableQuantity { get; set; }

        [JsonPropertyName("sold_quantity")]
        public int SoldQuantity { get; set; }

        [JsonPropertyName("sale_terms")]
        public List<SaleTerm> SaleTerms { get; set; }

        [JsonPropertyName("buying_mode")]
        public string BuyingMode { get; set; }

        [JsonPropertyName("listing_type_id")]
        public string ListingTypeId { get; set; }

        [JsonPropertyName("start_time")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("stop_time")]
        public DateTime StopTime { get; set; }

        [JsonPropertyName("end_time")]
        public DateTime EndTime { get; set; }

        [JsonPropertyName("expiration_time")]
        public object ExpirationTime { get; set; }

        [JsonPropertyName("condition")]
        public string Condition { get; set; }

        [JsonPropertyName("permalink")]
        public string Permalink { get; set; }

        [JsonPropertyName("thumbnail_id")]
        public string ThumbnailId { get; set; }

        [JsonPropertyName("thumbnail")]
        public string Thumbnail { get; set; }

        [JsonPropertyName("secure_thumbnail")]
        public string SecureThumbnail { get; set; }

        [JsonPropertyName("pictures")]
        public List<Picture> Pictures { get; set; }

        [JsonPropertyName("video_id")]
        public object VideoId { get; set; }

        [JsonPropertyName("descriptions")]
        public List<object> Descriptions { get; set; }

        [JsonPropertyName("accepts_mercadopago")]
        public bool AcceptsMercadopago { get; set; }

        [JsonPropertyName("non_mercado_pago_payment_methods")]
        public List<object> NonMercadoPagoPaymentMethods { get; set; }

        [JsonPropertyName("shipping")]
        public ItemShipping Shipping { get; set; }

        [JsonPropertyName("international_delivery_mode")]
        public string InternationalDeliveryMode { get; set; }

        [JsonPropertyName("seller_address")]
        public SellerAddress SellerAddress { get; set; }

        [JsonPropertyName("seller_contact")]
        public object SellerContact { get; set; }

        [JsonPropertyName("location")]
        public Location Location { get; set; }

        [JsonPropertyName("coverage_areas")]
        public List<object> CoverageAreas { get; set; }

        [JsonPropertyName("attributes")]
        public List<Attribute> Attributes { get; set; }

        [JsonPropertyName("warnings")]
        public List<object> Warnings { get; set; }

        [JsonPropertyName("listing_source")]
        public string ListingSource { get; set; }

        [JsonPropertyName("variations")]
        public List<object> Variations { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("sub_status")]
        public List<object> SubStatus { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }

        [JsonPropertyName("warranty")]
        public string Warranty { get; set; }

        [JsonPropertyName("catalog_product_id")]
        public object CatalogProductId { get; set; }

        [JsonPropertyName("domain_id")]
        public string DomainId { get; set; }

        [JsonPropertyName("seller_custom_field")]
        public object SellerCustomField { get; set; }

        [JsonPropertyName("parent_item_id")]
        public object ParentItemId { get; set; }

        [JsonPropertyName("differential_pricing")]
        public object DifferentialPricing { get; set; }

        [JsonPropertyName("deal_ids")]
        public List<object> DealIds { get; set; }

        [JsonPropertyName("automatic_relist")]
        public bool AutomaticRelist { get; set; }

        [JsonPropertyName("date_created")]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("last_updated")]
        public DateTime LastUpdated { get; set; }

        [JsonPropertyName("health")]
        public double Health { get; set; }

        [JsonPropertyName("catalog_listing")]
        public bool CatalogListing { get; set; }

        [JsonPropertyName("item_relations")]
        public List<object> ItemRelations { get; set; }

        [JsonPropertyName("channels")]
        public List<string> Channels { get; set; }
    }

    public class Attribute
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value_id")]
        public string ValueId { get; set; }

        [JsonPropertyName("value_name")]
        public string ValueName { get; set; }

        [JsonPropertyName("value_struct")]
        public object ValueStruct { get; set; }

        [JsonPropertyName("values")]
        public List<Value> Values { get; set; }

        [JsonPropertyName("attribute_group_id")]
        public string AttributeGroupId { get; set; }

        [JsonPropertyName("attribute_group_name")]
        public string AttributeGroupName { get; set; }
    }

    public class Location
    {
    }

    public class Picture
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("secure_url")]
        public string SecureUrl { get; set; }

        [JsonPropertyName("size")]
        public string Size { get; set; }

        [JsonPropertyName("max_size")]
        public string MaxSize { get; set; }

        [JsonPropertyName("quality")]
        public string Quality { get; set; }
    }

    public class SaleTerm
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value_id")]
        public string ValueId { get; set; }

        [JsonPropertyName("value_name")]
        public string ValueName { get; set; }

        [JsonPropertyName("value_struct")]
        public object ValueStruct { get; set; }

        [JsonPropertyName("values")]
        public List<Value> Values { get; set; }
    }

    public class SellerAddress
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class ItemShipping
    {
        [JsonPropertyName("mode")]
        public string Mode { get; set; }

        [JsonPropertyName("methods")]
        public List<object> Methods { get; set; }

        [JsonPropertyName("tags")]
        public List<object> Tags { get; set; }

        [JsonPropertyName("dimensions")]
        public object Dimensions { get; set; }

        [JsonPropertyName("local_pick_up")]
        public bool LocalPickUp { get; set; }

        [JsonPropertyName("free_shipping")]
        public bool FreeShipping { get; set; }

        [JsonPropertyName("logistic_type")]
        public string LogisticType { get; set; }

        [JsonPropertyName("store_pick_up")]
        public bool StorePickUp { get; set; }
    }

    public class Value
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("struct")]
        public object Struct { get; set; }
    }


}
