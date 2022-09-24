using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SpiceDataAccess.Repository.IRepository;
using SpicesModels.Models;
using SpicesModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpicesApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly ICategoryRepository _catRepo;
        [TempData]
        public string StatusMessage { get; set; }
        public SubCategoryController(ISubCategoryRepository subCategoryRepository,ICategoryRepository catRepo)
        {
            _subCategoryRepository = subCategoryRepository;
            _catRepo = catRepo;
        }
        public IActionResult Index()
        {
            return View(  _subCategoryRepository.GetAll(includeProperties: "Category"));
        }
        [HttpGet]
        public IActionResult Create ()
        {
            
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = _catRepo.GetAll(),
                SubCategory = new SubCategory(),
                SubCategoryList = _subCategoryRepository.GetAll(orderBy:x=>x.OrderBy(y=>y.Name)).Select(x=>x.Name).Distinct().ToList()
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExists = _subCategoryRepository
                  .GetAll(includeProperties: "Category" ,filter:x=>x.Name ==model.SubCategory.Name && x.Category.Id==model.SubCategory.CategoryId); 

                if (doesSubCategoryExists.Count() > 0)
                {
                    //Error
                    StatusMessage = "Error : Sub Category exists under " + doesSubCategoryExists.First().Category.Name + " category. Please use another name.";
                }
                else
                {
                    _subCategoryRepository.Add(model.SubCategory);
              await  _subCategoryRepository.Save();
                    return RedirectToAction(nameof(Index));
                }
            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = _catRepo.GetAll(),
                SubCategory = model.SubCategory,
                SubCategoryList = _subCategoryRepository.GetAll(orderBy: x => x.OrderBy(y => y.Name)).Select(x => x.Name).Distinct().ToList(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);
        }
        [ActionName("GetSubCategory")]
        public IActionResult GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories =  _subCategoryRepository.GetAll(filter:x=>x.Category.Id==id).ToList();
            return Json(new SelectList(subCategories, "Id", "Name"));
        }


        //GET - EDIT
        public  IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory =  _subCategoryRepository.FirstOrDefault(filter:x=>x.Id==id);

            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = _catRepo.GetAll(),
                SubCategory = subCategory,
                SubCategoryList = _subCategoryRepository.GetAll(orderBy: x => x.OrderBy(y => y.Name)).Select(x => x.Name).Distinct().ToList()
            };

            return View(model);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Edit(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExists = _subCategoryRepository.GetAll(includeProperties: "Category", filter: s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);
              

                if (doesSubCategoryExists.Count() > 0)
                {
                    //Error
                    StatusMessage = "Error : Sub Category exists under " + doesSubCategoryExists.First().Category.Name + " category. Please use another name.";
                }
                else
                {
                    var subCatFromDb = _subCategoryRepository.Find(model.SubCategory.Id);
                    subCatFromDb.Name = model.SubCategory.Name;
                    subCatFromDb.CategoryId = model.SubCategory.CategoryId;

                    _subCategoryRepository.Save();
                    return RedirectToAction(nameof(Index));
                }
            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = _catRepo.GetAll(),
                SubCategory = model.SubCategory,
                SubCategoryList = _subCategoryRepository.GetAll(orderBy: x => x.OrderBy(y => y.Name)).Select(x => x.Name).Distinct().ToList(),
                StatusMessage = StatusMessage
            };
            //modelVM.SubCategory.Id = id;
            return View(modelVM);
        }

        public  IActionResult Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory =  _subCategoryRepository.FirstOrDefault(filter:x=>x.Id==id , includeProperties: "Category");
            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        ////GET Delete
        //public  IActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    var subCategory = _subCategoryRepository.FirstOrDefault(filter: x => x.Id == id, includeProperties: "Category");
        //    if (subCategory == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(subCategory);
        //}

        //POST Delete
        [HttpGet, ActionName("Delete")]
  
        public async  Task<IActionResult> DeleteConfirmed(int id)
        {
            var subCategory = _subCategoryRepository.FirstOrDefault(filter: x => x.Id == id, includeProperties: "Category");
            _subCategoryRepository.Remove(subCategory);
          await  _subCategoryRepository.Save();
            return RedirectToAction(nameof(Index));

        }



    }

}

