using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingProject.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ListCount { get; set; }
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<ShoppingCart> ListCart { get; set; }
    }
}
