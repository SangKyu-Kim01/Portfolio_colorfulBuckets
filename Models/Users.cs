using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintShopManagement.Models
{
    public class Users
    {
        [Key]
        public int userId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "UserName must be 3 to 50 length!")]
        [Index(IsUnique = true)]
        public string userName { get; set; }

        [Required(ErrorMessage = "Password required!")]
        public string password { get; set; } 

        [Required(ErrorMessage = "First name required!")]
        public string firstName { get; set; } = "Unknown";

        [Required(ErrorMessage = "Last name required!")]
        public string lastName { get; set; } = "Unknown";

        [EmailAddress(ErrorMessage = "Invalid Email format!")]
        public string email { get; set; }

        public string phone { get; set; }

        [Required(ErrorMessage = "Position required!")]
        public int position { get; set; } = 2;

        [MaxLength]
        public byte[] profilePic { get; set; }

    }
}
