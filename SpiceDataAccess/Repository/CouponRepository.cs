using Microsoft.EntityFrameworkCore;
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
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {
        private readonly ApplicationDbContext _db;
        public CouponRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(Coupon coupon)
        {
            //var objFromDb = base.FirstOrDefault(u => u.Id == obj.Id);
            //if (objFromDb != null)
            //{
            //    objFromDb.Name = obj.Name;

            //}
            _db.Entry(coupon).State = EntityState.Modified;
        }
    }
}
