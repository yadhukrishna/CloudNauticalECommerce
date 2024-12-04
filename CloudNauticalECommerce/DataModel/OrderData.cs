using System.Text.Json.Serialization;

namespace CloudNauticalECommerce.DataModel
{
    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Order
    {
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string DeliveryAddress { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public DateTime DeliveryExpected { get; set; }
        [JsonIgnore]
        public bool ContainsGift { get; set; }
    }

    public class OrderItem
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal PriceEach { get; set; }
    }

}
