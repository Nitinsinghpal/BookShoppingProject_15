using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingProject.Models
{
    public class Company
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String StreetAddress { get; set; }
        public String City { get; set; }
        public String State { get; set; }
        [Display(Name ="Postal Code")]
        public String PostalCode { get; set; }
        [Display(Name = "Phone Number")]
        public String PhoneNumber { get; set; }
        [Display(Name = "Is Authorised Company")]
        public bool IsAuthorisedCompany { get; set; }
    }
}
