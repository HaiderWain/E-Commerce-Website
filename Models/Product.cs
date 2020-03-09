using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data;
using Amazonistan.Models;


namespace Amazonistan.Models
{
    public class Product
    {
        public int productID { get; set; }
        public string productName { get; set; }
        public string productCategory { get; set; }
        public int productPrice { get; set; }
    }
}