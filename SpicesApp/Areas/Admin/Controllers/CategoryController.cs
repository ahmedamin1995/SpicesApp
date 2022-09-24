using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Spice_Utility;
using SpiceDataAccess.Repository;
using SpiceDataAccess.Repository.IRepository;
using SpicesModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpicesApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _catRepo;
        private readonly INotyfService _notyf;
        public CategoryController(ICategoryRepository catRepo, INotyfService notyf)
        {
            _catRepo = catRepo;
            _notyf = notyf;
        }
        public IActionResult Index()
        {
            return View(_catRepo.GetAll());
        }

        public  IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<IActionResult> Create(Category obj)
        {
            if(ModelState.IsValid)
            {
                _catRepo.Add(obj);
                await _catRepo.Save();
                //TempData[WC.Success] = "Category created successfully";
                _notyf.Success("Category created successfully");
                return RedirectToAction("Index");

            }
            // TempData[WC.Error] = "Error while creating category";
            _notyf.Success("Error while creating category");

            return View(obj);

        }

        [HttpGet]
        public  IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _catRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [HttpGet]
        public IActionResult Detail(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _catRepo.Find(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< IActionResult> Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _catRepo.Update(obj);
              await  _catRepo.Save();
                TempData[WC.Success] = "Action completed successfully";
                return RedirectToAction("Index");
            }
            return View(obj);

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int ?id)
        {
            if(id>0)
            {
                var obj = _catRepo.Find(id.GetValueOrDefault());
                if (obj != null)
                {
                    _catRepo.Remove(obj);
                   await _catRepo.Save();
                }
               
            }
            return RedirectToAction("Index");
        }



    }
}
