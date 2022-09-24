using SpiceDataAccess.Data;
using SpiceDataAccess.Repository.IRepository;
using SpicesModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceDataAccess.Repository
{
    public class MenuItemRepository : Repository<MenuItem>,IMenuItemRepository
    {
        private readonly ApplicationDbContext _db;
        public MenuItemRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public async Task Update(MenuItem menuItem)
        {
            
            _db.Entry(menuItem).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _db.SaveChangesAsync();
        }
    }
}
