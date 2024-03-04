using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PaintShopManagement.Models
{
    public class Inventory
    {
        public Inventory(string itemName, string color, decimal price, int qty, string weight, string manufacture)
        {
            this.itemName = itemName;
            this.color = color;
            this.price = price;
            this.qty = qty;
            Weight = weight;
            this.manufacture = manufacture;
        }

        public Inventory()
        {
            // default constructor
        }

        [Key]
        public int inventoryId { get; set; }

        [Required]
        public string itemName { get; set; }

        [Required]
        public string color { get; set; }

        [Required]
        public decimal price { get; set; }

        [Required]
        public int qty { get; set; }

        [Required]
        public string wt { get; set; }

        [Required]
        public string manufacture { get; set; }
        public string Weight { get; }
    }
}
