using System;
using System.Collections.Generic;
using System.Text;

namespace ErpDemo
{
    public class Models
    {
        public class Customer
        {
            public int Id { get; set; } 
            public string Name { get; set; } = "";
            public string Email { get; set; } = ""; 
            public string Phone { get; set; } = ""; 
            public DateTime CreatedAt { get; set; } 
        }

        public class Order
        {
            public int Id { get; set; } 
            public int CustomerId { get; set; } 
            public string Reference { get; set; } = ""; 
            public decimal Amount { get; set; } 
            public string Status { get; set; } = "Pending"; 
            public DateTime OrderDate { get; set; } 
        }

    }
}
