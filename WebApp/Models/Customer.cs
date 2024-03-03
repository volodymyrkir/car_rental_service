using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Customer
    {
        [Key]
        public string Email { get; set; }

        [DisplayName("Full Name")]
        public string FullName { get; set; }

    }
}
