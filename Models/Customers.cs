using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintShopManagement.Models
{
    public class Customers
    {
        public Customers(string firstName, string lastName, string company, string email, string phone, string address)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.company = company;
            this.email = email;
            this.phone = phone;
            this.address = address;
        }

        public Customers()
        {
            // default constructor
        }

        [Key]
        public int customerId { get; set; }

        [Required]
        public string firstName { get; set; }

        [Required]
        public string lastName { get; set; }

        public string company { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email format!")]
        public string email { get; set; }

        [Required]
        public string phone { get; set; }

        public string address { get; set; }

    }
}
