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
    public class UserRepository : Repository<ApplicationUser>,IUserRepository
    {
        private readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;
        }

        public void Update(ApplicationUser obj)
        {
            throw new NotImplementedException();
        }
    }
}
