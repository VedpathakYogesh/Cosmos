using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CosmosDbDemo.Models
{
    public class Employee
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public float Salary { get; set; }
        public DateTime JoinDate { get; set; }
    }
}