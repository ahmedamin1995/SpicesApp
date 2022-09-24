using Microsoft.AspNetCore.Mvc;
using SpiceDataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpicesApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.ManagerUser)]
    public class UserController : Controller
    {
        private IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            return View( _userRepository.GetAll(u => u.Id != claim.Value));
        }


        public async Task<IActionResult> Lock(string id)
        {
            if (id == null)
                return NotFound(); 
            var user = _userRepository.FirstOrDefault(x => x.Id == id);
            if (user == null)
                return NotFound();
            user.LockoutEnd = DateTime.Now.AddYears(1000);
           await _userRepository.Save();
            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> UnLock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = _userRepository.FirstOrDefault(x => x.Id == id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            applicationUser.LockoutEnd = DateTime.Now;

            await _userRepository.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
