using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineMarketplace.Models
{
    public class Store
    {
        public int StoreId { get; set; }

        public string Title { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }

        public virtual ICollection<Product> Products { get; set; } 
    }
}