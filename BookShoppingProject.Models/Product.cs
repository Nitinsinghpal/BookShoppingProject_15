using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingProject.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public String Title { get; set; }
        public String Description { get; set; }
        public String ISBN { get; set; }
        public String Author { get; set; }
        [Required]
        [Range(1,10000)]
        public Double ListPrice { get; set; }
        [Required]
        [Range(1,10000)]
        public Double Price50 { get; set; }
        [Required]
        [Range(1,10000)]
        public Double Price100 { get; set; }
        [Required]
        [Range(1, 10000)]
        public Double Price { get; set; }
        [Display(Name ="Image Url")]
        public String ImageUrl { get; set; }
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        [Display(Name ="Cover Type")]
        public int CoverTypeId { get; set; }
        [ForeignKey("CoverTypeId")]
        public CoverType CoverType { get; set; }



    }
}
