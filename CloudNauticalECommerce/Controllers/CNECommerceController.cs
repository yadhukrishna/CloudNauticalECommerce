using CloudNauticalECommerce.DataModel;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace CloudNauticalECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CNECommerceController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public CNECommerceController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        [HttpPost("GetOrderDetails")]
        public IActionResult GetOrderDetails(RequestCustomer req)
        {

            string connectionString = configuration.GetConnectionString("MyDB");
            if (!string.IsNullOrEmpty(connectionString))
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var customer = connection.QuerySingleOrDefault<Customer>(
                        "SELECT FIRSTNAME, LASTNAME FROM CUSTOMERS WHERE EMAIL = @Email AND CUSTOMERID = @CustomerId",
                        new { Email = req.UserName, CustomerId = req.CustomerId });

                    if (customer == null)
                    {
                        return BadRequest("Invalid customer email or ID.");
                    }

                    var order = connection.QuerySingleOrDefault<Order>(
                        @"SELECT TOP 1 O.ORDERID AS OrderNumber, O.ORDERDATE, O.DELIVERYEXPECTED, 
                             C.HOUSENO + ' ' + C.STREET + ', ' + C.TOWN + ', ' + C.POSTCODE AS DeliveryAddress,o.CONTAINSGIFT
                      FROM ORDERS O
                      JOIN CUSTOMERS C ON O.CUSTOMERID = C.CUSTOMERID
                      WHERE C.CUSTOMERID = @CustomerId
                      ORDER BY O.ORDERDATE DESC", new { CustomerId = req.CustomerId });

                    if (order == null)
                    {
                        return Ok(new { customer, order = (object)null });
                    }

                    var orderItems = connection.Query<OrderItem>(
                        @"SELECT P.PRODUCTNAME, OI.QUANTITY, (OI.PRICE/OI.QUANTITY) as priceEach
                      FROM ORDERITEMS OI
                      JOIN PRODUCTS P ON OI.PRODUCTID = P.PRODUCTID
                      WHERE OI.ORDERID = @OrderId",
                        new { OrderId = order.OrderNumber }).ToList();


                    if (order.ContainsGift)
                    {
                        orderItems.ForEach(item => item.ProductName = "Gift");
                    }

                    order.OrderItems = orderItems;

                    var response = new
                    {
                        customer,
                        order
                    };

                    return Ok(response);
                }
            }
            else
            {
                throw new("Connectiion string is empty");
            }
        }

    }
}
