using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spice_Utility;
using SpiceDataAccess.Repository.IRepository;
using SpicesModels.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SpicesApp.Areas.Admin.Controllers
{
    [Area("Admin")]
  //  [Authorize(Roles = SD.ManagerUser)]
    public class CouponController : Controller
    {
        private readonly ICouponRepository _couponRep;
        public CouponController(ICouponRepository couponRep)
        {
            _couponRep = couponRep;
        }
        public IActionResult Index()
        {
            return View(_couponRep.GetAll());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupons)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    coupons.Picture = p1;
                }
                _couponRep.Add(coupons);
                await _couponRep.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(coupons);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var coupon = _couponRep.FirstOrDefault(filter: x => x.Id == id);

            return View(coupon);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Coupon coupons)
        {
            if (coupons.Id == 0)
            {
                return NotFound();
            }

            var couponFromDb = _couponRep.FirstOrDefault(filter: x => x.Id == coupons.Id);

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    couponFromDb.Picture = p1;
                }
                couponFromDb.MinimumAmount = coupons.MinimumAmount;
                couponFromDb.Name = coupons.Name;
                couponFromDb.Discount = coupons.Discount;
                couponFromDb.CouponType = coupons.CouponType;
                couponFromDb.IsActive = coupons.IsActive;

                await _couponRep.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(coupons);
        }

        public  IActionResult Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = _couponRep.FirstOrDefault(filter: x => x.Id == id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }


    }
}
