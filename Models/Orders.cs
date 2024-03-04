using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintShopManagement.Models
{
    public class Orders
    {
        public Orders()
        {
            // Paramerterless Constructor
        }

        // Constructor
        public Orders(int userId, int customerId, int inventoryId, int itemQuantity, DateTimeOffset orderDate)
        {
            this.userId = userId;
            this.customerId = customerId;
            this.inventoryId = inventoryId;
            this.itemQuantity = itemQuantity;
            this.orderDate = orderDate;
        }

        [Key]
        public int orderId { get; set; }

        [Required]
        public int userId { get; set; }

        [Required]
        public int customerId { get; set; }

        [Required]
        public int inventoryId { get; set; }

        public int itemQuantity { get; set; }

        public DateTimeOffset orderDate { get; set; }


        [ForeignKey("userId")]
        public Users Users { get; set; }

        [ForeignKey("customerId")]
        public Customers Customers { get; set; }

        [ForeignKey("inventoryId")]
        public Inventory Inventory { get; set; }
    }
}
