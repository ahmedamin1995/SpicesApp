using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Spice_Utility;
using SpiceDataAccess.Repository.IRepository;
using SpicesModels.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SpicesApp.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class MenuItemController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IMenuItemRepository _MenuItemRepo;
        private readonly ICategoryRepository _catRepo;
        private readonly ISubCategoryRepository _subCategoryRepository;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }
        public MenuItemController(IWebHostEnvironment hostingEnvironment, IMenuItemRepository MenuItemRepo, ICategoryRepository catRepo,
            ISubCategoryRepository subCategoryRepository)
        {
            _hostingEnvironment = hostingEnvironment;
            _MenuItemRepo = MenuItemRepo;
            _subCategoryRepository = subCategoryRepository;
            _catRepo = catRepo;
            MenuItemVM = new MenuItemViewModel
            {
                Category = _catRepo.GetAll(),
                MenuItem = new SpicesModels.Models.MenuItem()


            };
        }
        public IActionResult Index()
        {
            return View(_MenuItemRepo.GetAll(includeProperties: "Category,SubCategory"));
        }
        //GET - CREATE
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
                return View(MenuItemVM);
            _MenuItemRepo.Add(MenuItemVM.MenuItem);
            await _MenuItemRepo.Save();

            //Work on the image saving section
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var menuItemFromDb = _MenuItemRepo.Find(MenuItemVM.MenuItem.Id);

            if (files.Count > 0)
            {
                //files has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;
            }
            else
            {
                //no file was uploaded, so use default
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".png");
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".png";
            }
            await _MenuItemRepo.Save();
            return RedirectToAction(nameof(Index));

        }
        //GET - EDIT
        public  IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = _MenuItemRepo.FirstOrDefault(filter:x=>x.Id==id,includeProperties: "Category,SubCategory");
            MenuItemVM.SubCategory = _subCategoryRepository.GetAll(filter:x=>x.CategoryId==MenuItemVM.MenuItem.CategoryId);

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }
            return View(MenuItemVM);
        }


        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                MenuItemVM.SubCategory = _subCategoryRepository.GetAll(filter: x => x.CategoryId == MenuItemVM.MenuItem.CategoryId);
                return View(MenuItemVM);
            }

            //Work on the image saving section

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = _MenuItemRepo.FirstOrDefault(filter: x => x.Id == id, includeProperties: "Category,SubCategory");

            if (files.Count > 0)
            {
                //New Image has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension_new = Path.GetExtension(files[0].FileName);

                //Delete the original file
                if (!string.IsNullOrEmpty(menuItemFromDb.Image))
                {
                    var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));

                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);
                    
                }
                //we will upload the new file
                using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension_new;
            }

            menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
            menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
            menuItemFromDb.Price = MenuItemVM.MenuItem.Price;
            menuItemFromDb.Spicyness = MenuItemVM.MenuItem.Spicyness;
            menuItemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
            menuItemFromDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;


            await _MenuItemRepo.Save();

            return RedirectToAction(nameof(Index));
        }

        //GET : Details MenuItem
        public  IActionResult Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem =  _MenuItemRepo.FirstOrDefault(filter: x => x.Id == id, includeProperties: "Category,SubCategory");

            if (MenuItemVM.MenuItem == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(MenuItemVM);
        }

    }
}
