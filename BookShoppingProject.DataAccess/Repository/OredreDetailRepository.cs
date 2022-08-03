using BookShoppingProject.DataAccess.Repository.IRepository;
using BookShoppingProject.Models;
using BookShoppingProject_15.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingProject.DataAccess.Repository
{
    public class OredreDetailRepository : Repository<OrderDetails>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _context;
        public OredreDetailRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public void Update(OrderDetails orderDetails)
        {
            _context.Update(orderDetails);
        }
    }
}
