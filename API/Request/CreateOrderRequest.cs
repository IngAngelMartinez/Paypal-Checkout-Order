using System.Collections.Generic;

namespace API.Requests
{
    public class CreateOrderRequest
    {
        public string Value { get; set; }
        public string CurrencyCode { get; set; }
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Quantity { get; set; }
        public string Sku { get; set; }
        public Money Tax { get; set; }
        public Money UnitAmount { get; set; }
    }


    public class Money 
    {
        public string CurrencyCode { get; set; }
        public string Value { get; set; }
    }

}
