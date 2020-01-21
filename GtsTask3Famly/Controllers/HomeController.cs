using GtsTask3Famly.DAL;
using GtsTask3Famly.Models;
using GtsTask3Famly.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static GtsTask3Famly.Utilities.SD;
namespace GtsTask3Famly.Controllers
{
    [Authorize]

    public class HomeController : Controller
    {
        private readonly DB db;
        private readonly IHostingEnvironment env;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
         public HomeController(DB dB, IHostingEnvironment environment,UserManager<User>userM,RoleManager<IdentityRole>roleM)
        {
            db = dB;
            env = environment;
            userManager = userM;
            roleManager = roleM;
        }
        public async Task SeedRoles()
        {
            if (!await roleManager.RoleExistsAsync(AdminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(AdminRole));
            }

            if (!await roleManager.RoleExistsAsync(MemberRole))
            {
                await roleManager.CreateAsync(new IdentityRole(MemberRole));
            }
        }
        public async Task<IActionResult> Index()
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            FamilyToUser familyToUser = db.FamilyToUsers.FirstOrDefault(x => x.UserId == user.Id);
            VM vm = new VM
            {
                User = user,
                People = db.People.Include(x=>x.Gender).Include(x=>x.UserToPerson).Where(x => x.FamilyId == familyToUser.FamilyId)

            };
            return View(vm);
        }

        public IActionResult NewPerson()
        {
            Person person = new Person();
            ViewBag.Genders = db.Genders;
            return View(person);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewPerson(Person person)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Genders = db.Genders;
                return View(person);
            }
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            FamilyToUser familyToUser = db.FamilyToUsers.FirstOrDefault(x => x.UserId == user.Id);
          
            person.FamilyId = familyToUser.FamilyId;

         if (person.PhotoFile != null)
            {
                if (!person.PhotoFile.ContentType.Contains("image/"))
                {
                    ViewBag.Genders = db.Genders;
                    ModelState.AddModelError("PhotoFile", "Invalid photo type");
                    return View(person);
                }
                string fileName = Guid.NewGuid().ToString() + person.PhotoFile.FileName;
                string resultpath = Path.Combine(env.WebRootPath, "image", fileName);
                using (FileStream stream = new FileStream(resultpath, FileMode.Create))
                {
                    await person.PhotoFile.CopyToAsync(stream);
                }
                person.Photo = fileName;
            }
            else
            {
                person.Photo = person.GenderId == 1 ? "default1.jpg" : "default2.jpg";
            }
            Person newPerson = new Person
            {
              Birthdate=person.Birthdate,
              Age=DateTime.Now.Year-person.Birthdate.Year,
                Firstname = person.Firstname,
                LastName = person.LastName,
                FamilyId = person.FamilyId,
                GenderId = person.GenderId,
                Photo = person.Photo,

            };
            db.People.Add(newPerson);
            db.SaveChanges();
            TempData["addPerson"] = true;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditPerson(int id)
        {
            User user =await userManager.FindByNameAsync(User.Identity.Name);
            int familyId = Utilities.FamlyMethods.GetFamilyId(db, user);
            Person person = db.People.FirstOrDefault(x => x.Id == id&&x.FamilyId==familyId);
            if (person == null) { return NotFound(); }
            ViewBag.Genders = db.Genders;
            return View(person);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPerson(Person person)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            int familyId = Utilities.FamlyMethods.GetFamilyId(db, user);
            Person _pers = db.People.FirstOrDefault(x => x.Id == person.Id&&x.FamilyId==familyId);
            if (_pers == null)
            {
                return NotFound();
            }
            person.Photo = _pers.Photo;
            if (!ModelState.IsValid)
            {
                ViewBag.Genders = db.Genders;
                return View(person);
            }

            _pers.Firstname = person.Firstname;
            _pers.LastName = person.LastName;
            _pers.Age = person.Age;
            _pers.GenderId = person.GenderId;
            if (person.PhotoFile != null)
            {
                if (!person.PhotoFile.ContentType.Contains("image/"))
                {
                    ViewBag.Genders = db.Genders;
                    ModelState.AddModelError("PhotoFile", "Invalid photo type");
                    return View(person);
                }
                string fileName = Guid.NewGuid().ToString() + person.PhotoFile.FileName;
                string resultpath = Path.Combine(env.WebRootPath, "image", fileName);
                using (FileStream stream = new FileStream(resultpath, FileMode.Create))
                {
                    await person.PhotoFile.CopyToAsync(stream);
                }

                if (System.IO.File.Exists(Path.Combine(env.WebRootPath, "image", person.Photo)))
                {
                    System.IO.File.Delete(Path.Combine(env.WebRootPath, "image", person.Photo));
                }
                _pers.Photo = fileName;
            }

            db.SaveChanges();
            TempData["editPerson"] = true;
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DeletePerson(int id)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            int familyId = Utilities.FamlyMethods.GetFamilyId(db, user);
            Person person = db.People.FirstOrDefault(x => x.Id == id&&x.FamilyId==familyId);
            if (person == null) { return NotFound(); }
            if (db.UserToPeople.Any(x => x.PersonId == person.Id))
            {
                TempData["realPerson"] = true;
                return RedirectToAction(nameof(Index));
            }
            db.Relationships.RemoveRange(db.Relationships.Where(r => r.PersonId == id || r.RelatedUserId == id && r.IsMain == false));
            if (System.IO.File.Exists(Path.Combine(env.WebRootPath, "image", person.Photo)))
            {
                System.IO.File.Delete(Path.Combine(env.WebRootPath, "image", person.Photo));
            }
            db.People.Remove(person);
            db.SaveChanges();
            TempData["deleteRelation"] = true;
            return RedirectToAction("detail","family", new { id = familyId });
        }
        //Person Detail

        public async Task<IActionResult> PersonDetail(int id)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            int familyId = Utilities.FamlyMethods.GetFamilyId(db, user);
             Person person = db.People.Include(x => x.Gender).FirstOrDefault(x => x.Id == id && x.FamilyId == familyId);
            if (person == null)
            {
                return NotFound();
            }

            VM vm = new VM
            {
                Families = db.Families.Include(x => x.Users).Where(x => x.Users.FirstOrDefault(y => y.RelatedUserId == id) != null),
                Person = person,
                //Relationships = db.Relationships.Include(x => x.Person).Include(x => x.Role).Where(x => x.KindredId == id),
                People = db.People.Where(x => x.Id != id),
                RelRoles = db.RelRoles
            };
            return View(vm);
        }
        //public IActionResult DeleteAllRelation()
        //{
        //    db.Relationships.RemoveRange(db.Relationships);
        //    db.SaveChanges();
        //    return Content("All Relation deleted");
        //}

        //public IActionResult Roles()
        //{
        //    return View(db.RelRoles);
        //}
        //public IActionResult AddRole()
        //{
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult AddRole(RelRole role)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(role);
        //    }
        //    if (db.RelRoles.FirstOrDefault(r => r.Name.ToLower() == role.Name.ToLower()) != null)
        //    {
        //        ModelState.AddModelError("Name", "This role alredy exists");
        //        return View(role);
        //    }

        //    db.RelRoles.Add(role);
        //    db.SaveChanges();
        //    TempData["addRole"] = true;
        //    return RedirectToAction(nameof(Roles));
        //}
        
        //public IActionResult RemoveRole(int id)
        //{
        //    RelRole role = db.RelRoles.FirstOrDefault(r => r.Id == id);
        //    if (role == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(role);

        //}

        //public IActionResult DeleteRole(int id)
        //{
        //    RelRole role = db.RelRoles.FirstOrDefault(r => r.Id == id);
        //    if (role == null)
        //    {
        //        return NotFound();
        //    }
        //    db.Relationships.RemoveRange(db.Relationships.Where(x => x.RelRoleId == id));
        //    db.RelRoles.Remove(role);
        //    db.SaveChanges();
        //    TempData["removeRole"] = true;
        //    return RedirectToAction(nameof(Roles));
        //}

        [HttpPost]
        public async Task<IActionResult> LoadRelation(int? id, int? personId)
        {
            if (id == null || personId == null)
            {
                return NotFound();
            }
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            int familyId = Utilities.FamlyMethods.GetFamilyId(db, user);
            Person person = db.People.Include(x => x.Gender).FirstOrDefault(x => x.Id == id && x.FamilyId == familyId);
            Person activePerson = db.People.Include(x => x.Gender).FirstOrDefault(x => x.Id == personId && x.FamilyId == familyId);
            if (activePerson == null)
            {
                return NotFound();
            }
            int MaleId = db.Genders.FirstOrDefault(x => x.Name.ToLower() == "male").Id;
            int grandpaId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "grandpa").Id;
            int grandmaId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "grandma").Id;
            int grandsonId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "grandson").Id;
            int granddaughterId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "granddaughter").Id;
            int wifeId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "wife").Id;
            int husbandId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "husband").Id;
            int noRoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "norole").Id;
            IEnumerable<RelRole> roles = db.RelRoles.Where(x => x.GenderId == person.GenderId &&
            x.Id != ((activePerson.GenderId == MaleId) && (person.GenderId == MaleId) ? husbandId : 0) &&
            x.Id != ((activePerson.GenderId != MaleId) && (person.GenderId != MaleId) ? wifeId : 0) &&
            x.Id != granddaughterId &&
            x.Id != grandsonId &&
            x.Id != grandpaId &&
            x.Id != grandmaId &&
            x.Id != noRoleId
            );
            
            return PartialView("LoadRelationPartial", roles);
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
       
    }
}
