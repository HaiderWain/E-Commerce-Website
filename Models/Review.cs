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
    public class Review
    {
        public int reviewID { get; set; }
        public string reviewProductName { get; set; }
        public int reviewCustomerID { get; set; }
        public int reviewStars { get; set; }
        public int reviewProductID { get; set; }
        public string reviewDescription { get; set; }


    }
}