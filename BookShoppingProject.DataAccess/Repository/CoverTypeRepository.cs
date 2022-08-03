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
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _context;
        public CoverTypeRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public void Update(CoverType coverType)
        {
            _context.Update(coverType);
        }
    }
}
