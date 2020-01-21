using GtsTask3Famly.DAL;
using GtsTask3Famly.Models;
using GtsTask3Famly.Utilities;
using GtsTask3Famly.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace GtsTask3Famly.Controllers
{
    [Authorize]
    public class FamilyController : Controller
    {
        private readonly DB db;
        private readonly IHostingEnvironment env;
        private readonly UserManager<User> userManager;
        public FamilyController(DB dB, IHostingEnvironment environment,UserManager<User> UserManager)
        {
            db = dB;
            env = environment;
            userManager = UserManager;
        }
        public async Task<IActionResult> Index()
        {
            User user =await userManager.FindByNameAsync(User.Identity.Name);
            int familId= FamlyMethods.GetFamilyId(db, user);
             return View(db.Families.Include(x => x.Users).Where(x=>x.Id==familId));
        }
        //public IActionResult Create()
        //{
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Family family)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(family);
        //    }
        //    if (family.LogoFile == null)
        //    {
        //        ModelState.AddModelError("PhotoFile", "Please enter a picture");
        //        return View(family);
        //    }
        //    if (!family.LogoFile.ContentType.Contains("image/"))
        //    {
        //        ModelState.AddModelError("PhotoFile", "Invalid photo type");
        //        return View(family);
        //    }
        //    string fileName = Guid.NewGuid().ToString() + family.LogoFile.FileName;
        //    string resultpath = Path.Combine(env.WebRootPath, "image", fileName);
        //    using (FileStream stream = new FileStream(resultpath, FileMode.Create))
        //    {
        //        await family.LogoFile.CopyToAsync(stream);
        //    }
        //    family.Logo = fileName;
        //    db.Families.Add(family);
        //    db.SaveChanges();
        //    TempData["add"] = true;
        //    return RedirectToAction(nameof(Index));
        //}

        public async Task<IActionResult> Edit(int id)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            int familId = FamlyMethods.GetFamilyId(db, user);
            Family family = db.Families.FirstOrDefault(x => x.Id == id&&x.Id==familId);
            if (family == null) { return NotFound(); }
            return View(family);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Family family)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            int familId = FamlyMethods.GetFamilyId(db, user);
            Family fm = db.Families.FirstOrDefault(x => x.Id == family.Id&&x.Id==familId);
            if (fm == null)
            {
                return NotFound();
            }
            family.Logo = fm.Logo;
            if (family.LogoFile != null)
            {
                if (!family.LogoFile.ContentType.Contains("image/"))
                {
                    ViewBag.Genders = db.Genders;
                    ModelState.AddModelError("PhotoFile", "Invalid photo type");
                    return View(family);
                }
                string fileName = Guid.NewGuid().ToString() + family.LogoFile.FileName;
                string resultpath = Path.Combine(env.WebRootPath, "image", fileName);
                using (FileStream stream = new FileStream(resultpath, FileMode.Create))
                {

                    await family.LogoFile.CopyToAsync(stream);
                }
                if (fm.Logo != "defaultfamily.png")
                {
                    if (System.IO.File.Exists(Path.Combine(env.WebRootPath, "image", fm.Logo)))
                    {
                        System.IO.File.Delete(Path.Combine(env.WebRootPath, "image", fm.Logo));
                    }
                }
                
                fm.Logo = fileName;
               
            }
            db.SaveChanges();
            TempData["edit"] = true;
            return RedirectToAction(nameof(Index));
        }

        //public IActionResult Remove(int id)
        //{
        //    Family family = db.Families.FirstOrDefault(x => x.Id == id);
        //    if (family == null) { return NotFound(); }
        //    return View(family);
        //}


        //Remeve family


        //public IActionResult Delete(int id)
        //{
        //    Family family = db.Families.FirstOrDefault(x => x.Id == id);
        //    if (family == null) { return NotFound(); }
        //    db.Relationships.RemoveRange(db.Relationships.Where(r => r.FamilyId == id));
        //    if (System.IO.File.Exists(Path.Combine(env.WebRootPath, "image", family.Logo)))
        //    {
        //        System.IO.File.Delete(Path.Combine(env.WebRootPath, "image", family.Logo));
        //    }
        //    db.Families.Remove(family);
        //    db.SaveChanges();
        //    TempData["remove"] = true;
        //    return RedirectToAction(nameof(Index));
        //}

        public async Task<IActionResult> Detail(int id)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            int familId = FamlyMethods.GetFamilyId(db, user);
            Family family = db.Families.Include(x => x.Users).FirstOrDefault(x => x.Id == id&&x.Id==familId);
            if (family == null)
            {
                return NotFound();
            }
            IEnumerable<Relationship> relationships = db.Relationships.Include(x => x.Person).Include(x => x.Role)
            .Where(x => x.FamilyId == id);
            IEnumerable<Person> people = db.People.Where(x=>x.FamilyId==familId).Include(x => x.Gender).Include(x=>x.UserToPerson);
            relationships = relationships.Join(people, r => r.RelatedUserId, p => p.Id, (rel, pr) => new Relationship
            {
                FamilyId = rel.FamilyId,
                Person=rel.Person,
                IsMain = rel.IsMain,
                PersonId = rel.PersonId,
                RelatedUserId = rel.RelatedUserId,
                RelRoleId = rel.RelRoleId,
                RelatedUser = pr,
                Role=rel.Role
            });

            VM vm = new VM
            {
                Family = family,
                Relationships = relationships.GroupBy(x => x.RelatedUserId).Select(x => x.First()),
                RelRoles = db.RelRoles,
                People = people
            };
            string xy = "";
            FamlyMethods.CreatePartial(relationships.Where(x => x.IsMain == true), relationships,ref xy,true);
            ViewBag.familyTree = xy;
            return View(vm);
        }
        
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult FirstPerson(int? FamilyId, int? PersonId)
        //{
        //    if (FamilyId == null || PersonId == null)
        //    {
        //        return NotFound();
        //    }
        //    Person person = db.People.FirstOrDefault(x => x.Id == PersonId);
        //    Family family = db.Families.Include(x => x.Users).FirstOrDefault(x => x.Id == FamilyId);
        //    if (person == null || family == null)
        //    {
        //        return NotFound();
        //    }
        //    if (family.Users.Count() > 0)
        //    {
        //        return NotFound();
        //    }
        //    Relationship relationship = new Relationship
        //    {
        //        IsMain = true,
        //        FamilyId = (int)FamilyId,
        //        RelatedUserId = (int)PersonId,
        //        RelRoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "norole").Id
        //    };
        //    db.Relationships.Add(relationship);
        //    db.SaveChanges();
        //    return RedirectToAction(nameof(PersonDetail), new { id = person.Id, familyId = family.Id });
        //}
        public async Task<IActionResult> PersonDetail(int id, int familyId)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            int familId = FamlyMethods.GetFamilyId(db, user);
            Family family = db.Families.Include(x => x.Users).FirstOrDefault(x => x.Id == familyId&&x.Id==familId);
            if (family == null)
            {
                return NotFound();
            }
            Person person = db.People.Where(x=>x.FamilyId==familId).Include(x => x.Gender).FirstOrDefault(x => x.Id == id);
            if (person == null)
            {
                return NotFound();
            }
            List<Person> FixPeople = new List<Person>();
            FixPeople = db.People.Where(x=>x.FamilyId==familId).ToList();
            foreach (var item in family.Users)
            {
                Person person1 = db.People.Where(x=>x.FamilyId==familId).FirstOrDefault(x => x.Id == item.RelatedUserId);
                FixPeople.Remove(person1);
            }
            VM vm = new VM
            {
                Family = family,
                Person = person,
                Relationships = db.Relationships.Include(x => x.Person).Include(x => x.Role).Where(x => x.RelatedUserId == person.Id && x.FamilyId == familId),
                People = FixPeople.Where(x => x.Id != id),
                RelRoles = db.RelRoles
            };
            return View(vm);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PersonDetail(VM vm)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            int familId = FamlyMethods.GetFamilyId(db, user);
            Family family = db.Families.Include(x => x.Users).Where(x=>x.Id==familId).FirstOrDefault(x => x.Id == vm.Relationship.FamilyId);
            if (family == null)
            {
                return NotFound();
            }
            Person person = db.People.Include(x => x.Gender).Where(x=>x.FamilyId==familId).FirstOrDefault(x => x.Id == vm.Relationship.RelatedUserId);
            if (person == null)
            {
                return NotFound();
            }
            if (family.Id != familId)
            {
                return NotFound();
            }
            
            List<Person> FixPeople = new List<Person>();
            FixPeople = db.People.Where(x=>x.FamilyId==familId).ToList();
            foreach (var item in family.Users)
            {
                Person person1 = db.People.Where(x=>x.FamilyId==familId).FirstOrDefault(x => x.Id == item.RelatedUserId);
                FixPeople.Remove(person1);
            }
            vm.Person = person;
            vm.Relationships = db.Relationships.Include(x => x.Person).Include(x => x.Role).Where(x => x.RelatedUserId == person.Id && x.FamilyId == family.Id);
            vm.People = FixPeople.Where(x => x.Id != person.Id);
            vm.RelRoles = db.RelRoles;

            if (family.Users.FirstOrDefault(x => x.RelatedUserId == vm.Relationship.PersonId) != null)
            {
                ViewBag.Relation = "Relationship already exists";
                return View(vm);
            }

            if (vm.Relationship.PersonId == 0 || vm.Relationship.RelatedUserId == 0 || vm.Relationship.RelRoleId == 0 || vm.Relationship.FamilyId == 0)
            {
                ModelState.AddModelError("", "Select relation property");
                return View(vm);
            }

            Relationship relationship = new Relationship
            {
                PersonId = vm.Relationship.PersonId,
                RelatedUserId = vm.Relationship.RelatedUserId,
                RelRoleId = vm.Relationship.RelRoleId,
                FamilyId = family.Id
            };
            if (db.Relationships.FirstOrDefault(x => x.RelatedUserId == vm.Relationship.RelatedUserId && x.PersonId == vm.Relationship.PersonId && x.FamilyId == family.Id) != null)
            {
                TempData["Alredy"] = true;
                ModelState.AddModelError("Err", "This Relationship alredy exist");
                return View(vm);
            };
            db.Relationships.Add(relationship);
            RelRole role = db.RelRoles.FirstOrDefault(x => x.Id == vm.Relationship.RelRoleId);
            IEnumerable<RelRole> roles = db.RelRoles;
            int RoleId = 0;
            int MaleId = db.Genders.FirstOrDefault(x => x.Name.ToLower() == "male").Id;
            int FemaleId = db.Genders.FirstOrDefault(x => x.Name.ToLower() == "female").Id;
            int grandpaId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "grandpa").Id;
            int grandmaId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "grandma").Id;
            int fatherId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "father").Id;
            int motherId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "mother").Id;
            int grandsonId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "grandson").Id;
            int granddaughterId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "granddaughter").Id;
            int wifeId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "wife").Id;
            int husbandId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "husband").Id;
            int sonId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "son").Id;
            int daughterId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "daughter").Id;
            int brotherId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "brother").Id;
            int sisterId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "sister").Id;

            IEnumerable<Relationship> personRelationship = db.Relationships.Include(x => x.Role).Where(x => x.RelatedUserId == person.Id && x.FamilyId == family.Id);
            IEnumerable<Relationship> autoPersonRelationships = db.Relationships.Include(x => x.Role).Where(x => x.RelatedUserId == vm.Relationship.PersonId && x.FamilyId == family.Id);
            switch (role.Name.ToLower())
            {
                //Spouse
                case "husband":
                    if (personRelationship.Where(x => x.Role.Name.ToLower() == "husband").Count() >= 1)
                    {
                        ViewBag.Relation = "The Person is married";
                        return View(vm);
                    }
                    RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "wife").Id;
                    //Link 
                    //P1.Child=P2.child
                    if (autoPersonRelationships.Where(x => x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter").Count() > 0)
                    {
                        int MotherId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "mother").Id;
                        foreach (var item in autoPersonRelationships.Where(x => x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter"))
                        {
                            FamlyMethods.AddRelation(db, person.Id, (int)item.PersonId, (int)item.RelRoleId, family.Id);
                            if (db.Relationships.FirstOrDefault(x => x.RelatedUserId == item.PersonId && x.RelRoleId == MotherId && x.FamilyId == family.Id) == null)
                            {
                                FamlyMethods.AddRelation(db, (int)item.PersonId, person.Id, MotherId, family.Id);
                            }

                        }
                        //Grandparent

                        if (personRelationship.Any(x => x.RelRoleId == fatherId || x.RelRoleId == motherId))
                        {
                            foreach (var parent in personRelationship.Where(x => x.Role.Name.ToLower() == "father" || x.Role.Name.ToLower() == "mother"))
                            {
                                foreach (var child in autoPersonRelationships.Where(x => x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter"))
                                {
                                    FamlyMethods.AddRelation(db, (int)child.PersonId, (int)parent.PersonId, parent.RelRoleId == fatherId ? grandpaId : grandmaId, family.Id);
                                    FamlyMethods.AddRelation(db, (int)parent.PersonId, (int)child.PersonId, child.Role.GenderId == MaleId ? grandsonId : granddaughterId, family.Id);
                                }
                            }
                        }
                    }
                    if (personRelationship.Where(x => x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter").Count() > 0)
                    {
                        foreach (var item in personRelationship.Where(x => x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter"))
                        {
                            FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)item.PersonId, (int)item.RelRoleId, family.Id);
                            if (db.Relationships.FirstOrDefault(x => x.RelatedUserId == item.PersonId && x.RelRoleId == fatherId && x.FamilyId == family.Id) == null)
                            {
                                FamlyMethods.AddRelation(db, (int)item.PersonId, (int)vm.Relationship.PersonId, fatherId, family.Id);
                            }
                        }
                        //Child+Child
                        if (autoPersonRelationships.Where(x => x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter").Count() > 0)
                        {
                            foreach (var child in personRelationship.Where(x => x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter"))
                            {
                                foreach (var item in autoPersonRelationships.Where(x => x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter"))
                                {
                                    FamlyMethods.AddRelation(db, (int)child.PersonId, (int)item.PersonId, item.Role.GenderId == MaleId ? brotherId : sisterId, family.Id);
                                    FamlyMethods.AddRelation(db, (int)item.PersonId, (int)child.PersonId, child.Role.GenderId == MaleId ? brotherId : sisterId, family.Id);

                                }
                            }
                        }
                        //parent
                        if (autoPersonRelationships.Any(x => x.Role.Name.ToLower() == "father" || x.Role.Name.ToLower() == "mother"))
                        {
                            foreach (var parent in autoPersonRelationships.Where(x => x.Role.Name.ToLower() == "father" || x.Role.Name.ToLower() == "mother"))
                            {
                                foreach (var child in personRelationship.Where(x => x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter"))
                                {
                                    FamlyMethods.AddRelation(db, (int)child.PersonId, (int)parent.PersonId, parent.RelRoleId == fatherId ? grandpaId : grandmaId, family.Id);
                                    FamlyMethods.AddRelation(db, (int)parent.PersonId, (int)child.PersonId, child.Role.GenderId == MaleId ? grandsonId : granddaughterId, family.Id);

                                }
                            }
                        }
                    }
                    //Person's grandchildren are his husband's grandchildren
                    if (personRelationship.Any(x => x.RelRoleId == grandsonId || x.RelRoleId == granddaughterId))
                    {
                        foreach (var GrandChild in personRelationship.Where(x => x.RelRoleId == grandsonId || x.RelRoleId == granddaughterId))
                        {
                            FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)GrandChild.PersonId, (int)GrandChild.RelRoleId, family.Id);
                            FamlyMethods.AddRelation(db, (int)GrandChild.PersonId, (int)vm.Relationship.PersonId, grandpaId, family.Id);
                        }
                    }
                    //Person's wife's grandchildren are his grandchildren
                    if (autoPersonRelationships.Any(x => x.RelRoleId == grandsonId || x.RelRoleId == granddaughterId))
                    {
                        foreach (var GrandChild in autoPersonRelationships.Where(x => x.RelRoleId == grandsonId || x.RelRoleId == granddaughterId))
                        {
                            FamlyMethods.AddRelation(db, person.Id, (int)GrandChild.PersonId, (int)GrandChild.RelRoleId, family.Id);
                            FamlyMethods.AddRelation(db, (int)GrandChild.PersonId, person.Id, grandmaId, family.Id);
                        }
                    }
                    break;
                //Spouse
                case "wife":
                    if (personRelationship.Where(x => x.Role.Name.ToLower() == "wife").Count() >= 1)
                    {
                        ViewBag.Relation = "The Person is married";
                        return View(vm);
                    }
                    RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "husband").Id;
                    //Link
                    //Child -Father

                    if (autoPersonRelationships.Where(x => x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter").Count() > 0)
                    {
                        int FatherId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "father").Id;
                        foreach (var item in autoPersonRelationships.Where(x => x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter"))
                        {
                            FamlyMethods.AddRelation(db, person.Id, (int)item.PersonId, (int)item.RelRoleId, family.Id);
                            if (db.Relationships.FirstOrDefault(x => x.RelatedUserId == item.PersonId && x.RelRoleId == FatherId && x.FamilyId == family.Id) == null)
                            {
                                FamlyMethods.AddRelation(db, (int)item.PersonId, (int)person.Id, FatherId, family.Id);
                            }
                        }
                        if (personRelationship.Any(x => x.Role.Name.ToLower() == "father" || x.Role.Name.ToLower() == "mother"))
                        {
                            foreach (var parent in personRelationship.Where(x => x.Role.Name.ToLower() == "father" || x.Role.Name.ToLower() == "mother"))
                            {
                                foreach (var child in autoPersonRelationships.Where(x => x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter"))
                                {
                                    FamlyMethods.AddRelation(db, (int)child.PersonId, (int)parent.PersonId, parent.RelRoleId == fatherId ? grandpaId : grandmaId, family.Id);
                                    FamlyMethods.AddRelation(db, (int)parent.PersonId, (int)child.PersonId, child.Role.GenderId == MaleId ? grandsonId : granddaughterId, family.Id);

                                }
                            }
                        }
                    }
                    if (personRelationship.Where(x => x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter").Count() > 0)
                    {
                        foreach (var item in personRelationship.Where(x => x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter"))
                        {
                            FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)item.PersonId, (int)item.RelRoleId, family.Id);
                            if (db.Relationships.FirstOrDefault(x => x.RelatedUserId == item.PersonId && x.RelRoleId == motherId && x.FamilyId == family.Id) == null)
                            {
                                FamlyMethods.AddRelation(db, (int)item.PersonId, (int)vm.Relationship.PersonId, motherId, family.Id);
                            }

                        }
                        //Child+Child
                        if (autoPersonRelationships.Where(x => x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter").Count() > 0)
                        {
                            foreach (var child in personRelationship.Where(x => x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter"))
                            {
                                foreach (var item in autoPersonRelationships.Where(x => x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter"))
                                {
                                    FamlyMethods.AddRelation(db, (int)child.PersonId, (int)item.PersonId, item.Role.GenderId == MaleId ? brotherId : sisterId, family.Id);
                                    FamlyMethods.AddRelation(db, (int)item.PersonId, (int)child.PersonId, child.Role.GenderId == MaleId ? brotherId : sisterId, family.Id);

                                }
                            }
                        }
                        //Grandparent
                        if (autoPersonRelationships.Any(x => x.Role.Name.ToLower() == "father" || x.Role.Name.ToLower() == "mother"))
                        {
                            foreach (var parent in autoPersonRelationships.Where(x => x.Role.Name.ToLower() == "father" || x.Role.Name.ToLower() == "mother"))
                            {
                                foreach (var child in personRelationship.Where(x => x.Role.Name.ToLower() == "son" || x.Role.Name.ToLower() == "daughter"))
                                {
                                    FamlyMethods.AddRelation(db, (int)child.PersonId, (int)parent.PersonId, parent.RelRoleId == fatherId ? grandpaId : grandmaId, family.Id);
                                    FamlyMethods.AddRelation(db, (int)parent.PersonId, (int)child.PersonId, child.Role.GenderId == MaleId ? grandsonId : granddaughterId, family.Id);
                                }
                            }
                        }
                    }
                    //Person's grandchildren are his wife's grandchildren
                    if (personRelationship.Any(x => x.RelRoleId == grandsonId || x.RelRoleId == granddaughterId))
                    {
                        foreach (var GrandChild in personRelationship.Where(x => x.RelRoleId == grandsonId || x.RelRoleId == granddaughterId))
                        {
                            FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)GrandChild.PersonId, (int)GrandChild.RelRoleId, family.Id);
                            FamlyMethods.AddRelation(db, (int)GrandChild.PersonId, (int)vm.Relationship.PersonId, grandmaId, family.Id);
                        }
                    }
                    //Person's husband's grandchildren are his grandchildren
                    if (autoPersonRelationships.Any(x => x.RelRoleId == grandsonId || x.RelRoleId == granddaughterId))
                    {
                        foreach (var GrandChild in autoPersonRelationships.Where(x => x.RelRoleId == grandsonId || x.RelRoleId == granddaughterId))
                        {
                            FamlyMethods.AddRelation(db, person.Id, (int)GrandChild.PersonId, (int)GrandChild.RelRoleId, family.Id);
                            FamlyMethods.AddRelation(db, (int)GrandChild.PersonId, person.Id, grandpaId, family.Id);
                        }
                    }
                    break;
                //Father
                case "father":
                    if (personRelationship.Any(x => x.RelRoleId == fatherId))
                    {
                        ViewBag.Relation = "The father must be one";
                        return View(vm);
                    }
                    if (person.Gender.Name.ToLower() == "male")
                    {
                        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "son").Id;
                    }
                    else if (person.Gender.Name.ToLower() == "female")
                    {
                        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "daughter").Id;
                    }
                    //Person's father's wife is his mother
                    if (autoPersonRelationships.Any(x => x.RelRoleId == wifeId))
                    {
                        if (personRelationship.FirstOrDefault(x => x.RelRoleId == motherId) == null)
                        {
                            FamlyMethods.AddRelation(db, person.Id, (int)autoPersonRelationships.FirstOrDefault(x => x.RelRoleId == wifeId).PersonId, motherId, family.Id);
                            if (person.GenderId == MaleId)
                            {
                                FamlyMethods.AddRelation(db, (int)autoPersonRelationships.FirstOrDefault(x => x.RelRoleId == wifeId).PersonId, person.Id, sonId, family.Id);
                            }
                            else
                            {
                                FamlyMethods.AddRelation(db, (int)autoPersonRelationships.FirstOrDefault(x => x.RelRoleId == wifeId).PersonId, person.Id, daughterId, family.Id);
                            }

                        }
                    }
                    //Person's father's children are his brothers and sisters
                    if (autoPersonRelationships.Any(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                    {
                        foreach (var child in autoPersonRelationships.Where(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                        {
                            FamlyMethods.AddRelation(db, person.Id, (int)child.PersonId, child.RelRoleId == sonId ? brotherId : sisterId, family.Id);
                            FamlyMethods.AddRelation(db, (int)child.PersonId, person.Id, person.GenderId == MaleId ? brotherId : sisterId, family.Id);

                        }
                    }
                    //Person's brothers are his father's childrena >| children
                    if (personRelationship.Any(x => x.RelRoleId == brotherId || x.RelRoleId == sisterId))
                    {
                        foreach (var brosis in personRelationship.Where(x => x.RelRoleId == brotherId || x.RelRoleId == sisterId))
                        {
                            FamlyMethods.AddRelation(db, (int)brosis.PersonId, (int)vm.Relationship.PersonId, fatherId, family.Id);
                            if (brosis.RelRoleId == brotherId)
                            {
                                FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)brosis.PersonId, sonId, family.Id);
                            }
                            else
                            {
                                FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)brosis.PersonId, daughterId, family.Id);
                            }
                            //Children
                            if (db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == brosis.PersonId && (x.RelRoleId == sonId || x.RelRoleId == daughterId)))
                            {
                                foreach (var child in db.Relationships.Where(x => x.FamilyId == family.Id && x.RelatedUserId == brosis.PersonId && (x.RelRoleId == sonId || x.RelRoleId == daughterId)))
                                {
                                    FamlyMethods.AddRelation(db, (int)child.PersonId, (int)vm.Relationship.PersonId, grandpaId, family.Id);
                                    if (child.RelRoleId == sonId)
                                    {
                                        FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)child.PersonId, grandsonId, family.Id);
                                    }
                                    else
                                    {
                                        FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)child.PersonId, granddaughterId, family.Id);
                                    }
                                }
                            }

                        }
                    }
                    //Person's mother is his father's wife
                    if (personRelationship.Any(x => x.RelRoleId == motherId) && !autoPersonRelationships.Any(x => x.RelRoleId == wifeId))
                    {
                        int CheckMotherId = personRelationship.FirstOrDefault(y => y.RelRoleId == motherId).Id;
                        if (!db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == CheckMotherId && x.RelRoleId == husbandId))
                        {
                            FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)personRelationship.FirstOrDefault(x => x.RelRoleId == motherId).PersonId, wifeId, family.Id);
                            FamlyMethods.AddRelation(db, (int)personRelationship.FirstOrDefault(x => x.RelRoleId == motherId).PersonId, (int)vm.Relationship.PersonId, husbandId, family.Id);
                        }
                    }
                    //Person's children are his father's grandchildren
                    if (personRelationship.Any(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                    {
                        foreach (var child in personRelationship.Where(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                        {
                            FamlyMethods.AddRelation(db, (int)child.PersonId, (int)vm.Relationship.PersonId, grandpaId, family.Id);
                            if (child.RelRoleId == sonId)
                            {
                                FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)child.PersonId, grandsonId, family.Id);
                            }
                            else
                            {
                                FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)child.PersonId, granddaughterId, family.Id);
                            }
                        }
                    }
                    break;
                //mother
                case "mother":
                    if (personRelationship.Where(x => x.Role.Name.ToLower() == "mother").Count() == 1)
                    {
                        ViewBag.Relation = "The mother must be one";
                        return View(vm);
                    }
                    if (person.Gender.Name.ToLower() == "male")
                    {
                        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "son").Id;
                    }
                    else if (person.Gender.Name.ToLower() == "female")
                    {
                        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "daughter").Id;
                    }
                    //Person's mother's husband is his father
                    if (autoPersonRelationships.Any(x => x.RelRoleId == husbandId))
                    {
                        if (personRelationship.FirstOrDefault(x => x.RelRoleId == fatherId) == null)
                        {
                            FamlyMethods.AddRelation(db, person.Id, (int)autoPersonRelationships.FirstOrDefault(x => x.RelRoleId == husbandId).PersonId, fatherId, family.Id);
                            if (person.GenderId == MaleId)
                            {
                                FamlyMethods.AddRelation(db, (int)autoPersonRelationships.FirstOrDefault(x => x.RelRoleId == husbandId).PersonId, person.Id, sonId, family.Id);
                            }
                            else
                            {
                                FamlyMethods.AddRelation(db, (int)autoPersonRelationships.FirstOrDefault(x => x.RelRoleId == husbandId).PersonId, person.Id, daughterId, family.Id);
                            }

                        }
                    }
                    //Person's father is his mother's husband
                    if (personRelationship.Any(x => x.RelRoleId == fatherId))
                    {
                        if (!autoPersonRelationships.Any(x => x.RelRoleId == husbandId))
                        {
                            int checkid = personRelationship.FirstOrDefault(y => y.RelRoleId == fatherId).Id;
                            if (!db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == checkid && x.RelRoleId == wifeId))
                            {
                                FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)personRelationship.FirstOrDefault(x => x.RelRoleId == fatherId).PersonId, husbandId, family.Id);
                                FamlyMethods.AddRelation(db, (int)personRelationship.FirstOrDefault(x => x.RelRoleId == fatherId).PersonId, (int)vm.Relationship.PersonId, wifeId, family.Id);
                            }
                        }

                    }
                    //Person's mother's children are his brothers and sisters
                    if (autoPersonRelationships.Any(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                    {
                        foreach (var child in autoPersonRelationships.Where(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                        {
                            if (child.RelRoleId == sonId)
                            {
                                FamlyMethods.AddRelation(db, person.Id, (int)child.PersonId, brotherId, family.Id);
                                if (person.GenderId == MaleId)
                                {
                                    FamlyMethods.AddRelation(db, (int)child.PersonId, person.Id, brotherId, family.Id);
                                }
                                else
                                {
                                    FamlyMethods.AddRelation(db, (int)child.PersonId, person.Id, sisterId, family.Id);
                                }
                            }
                            else if (child.RelRoleId == daughterId)
                            {
                                FamlyMethods.AddRelation(db, person.Id, (int)child.PersonId, sisterId, family.Id);
                                if (person.GenderId == MaleId)
                                {
                                    FamlyMethods.AddRelation(db, (int)child.PersonId, person.Id, brotherId, family.Id);
                                }
                                else
                                {
                                    FamlyMethods.AddRelation(db, (int)child.PersonId, person.Id, sisterId, family.Id);
                                };
                            }
                        }
                    }
                    //Person's brothers are his mother's childrena >| children
                    if (personRelationship.Any(x => x.RelRoleId == brotherId || x.RelRoleId == sisterId))
                    {
                        foreach (var brosis in personRelationship.Where(x => x.RelRoleId == brotherId || x.RelRoleId == sisterId))
                        {
                            FamlyMethods.AddRelation(db, (int)brosis.PersonId, (int)vm.Relationship.PersonId, motherId, family.Id);
                            if (brosis.RelRoleId == brotherId)
                            {
                                FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)brosis.PersonId, sonId, family.Id);
                            }
                            else
                            {
                                FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)brosis.PersonId, daughterId, family.Id);
                            }
                            //Children
                            if (db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == brosis.PersonId && (x.RelRoleId == sonId || x.RelRoleId == daughterId)))
                            {
                                foreach (var child in db.Relationships.Where(x => x.FamilyId == family.Id && x.RelatedUserId == brosis.PersonId && (x.RelRoleId == sonId || x.RelRoleId == daughterId)))
                                {
                                    FamlyMethods.AddRelation(db, (int)child.PersonId, (int)vm.Relationship.PersonId, grandmaId, family.Id);
                                    if (child.RelRoleId == sonId)
                                    {
                                        FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)child.PersonId, grandsonId, family.Id);
                                    }
                                    else
                                    {
                                        FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)child.PersonId, granddaughterId, family.Id);
                                    }
                                }
                            }

                        }
                    }
                    //Person's children are his father's grandchildren
                    if (personRelationship.Any(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                    {
                        foreach (var child in personRelationship.Where(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                        {
                            FamlyMethods.AddRelation(db, (int)child.PersonId, (int)vm.Relationship.PersonId, grandmaId, family.Id);
                            if (child.RelRoleId == sonId)
                            {
                                FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)child.PersonId, grandsonId, family.Id);
                            }
                            else
                            {
                                FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)child.PersonId, granddaughterId, family.Id);
                            }
                        }
                    }
                    break;
                ////Granpa
                //case "grandpa":
                //    if (personRelationship.Where(x => x.Role.Name.ToLower() == "grandpa").Count() >= 2)
                //    {
                //        ViewBag.Relation = "a person may have two grandparents";
                //        return View(vm);
                //    }
                //    if (person.Gender.Name.ToLower() == "male")
                //    {
                //        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "grandson").Id;
                //    }
                //    else if (person.Gender.Name.ToLower() == "female")
                //    {
                //        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "granddaughter").Id;
                //    }
                //    break;
                ////Grandpa
                //case "grandma":
                //    if (personRelationship.Where(x => x.Role.Name.ToLower() == "grandma").Count() >= 2)
                //    {
                //        ViewBag.Relation = "a person may have two grandparents";
                //        return View(vm);
                //    }
                //    if (person.Gender.Name.ToLower() == "male")
                //    {
                //        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "grandson").Id;
                //    }
                //    else if (person.Gender.Name.ToLower() == "female")
                //    {
                //        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "granddaughter").Id;
                //    }
                //    break;
                //Brother
                case "brother":
                    if (person.Gender.Name.ToLower() == "male")
                    {
                        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "brother").Id;
                    }
                    else if (person.Gender.Name.ToLower() == "female")
                    {
                        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "sister").Id;
                    }
                    //Person' brother's brother is his brother
                    if (autoPersonRelationships.Any(x => x.RelRoleId == sisterId || x.RelRoleId == brotherId))
                    {
                        foreach (var broAndSis in autoPersonRelationships.Where(x => x.RelRoleId == sisterId || x.RelRoleId == brotherId))
                        {
                            FamlyMethods.AddRelation(db, person.Id, (int)broAndSis.PersonId, (int)broAndSis.RelRoleId, family.Id);
                            if (person.GenderId == MaleId)
                            {
                                FamlyMethods.AddRelation(db, (int)broAndSis.PersonId, person.Id, brotherId, family.Id);
                            }
                            else
                            {
                                FamlyMethods.AddRelation(db, (int)broAndSis.PersonId, person.Id, sisterId, family.Id);
                            }
                        }
                    }
                    //Person'brothers and sisters are his brother's siter and brothers
                    if (personRelationship.Any(x => x.RelRoleId == brotherId || x.RelRoleId == sisterId))
                    {
                        foreach (var BroAndSis in personRelationship.Where(x => x.RelRoleId == sisterId || x.RelRoleId == brotherId))
                        {
                            FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)BroAndSis.PersonId, (int)BroAndSis.RelRoleId, family.Id);
                            FamlyMethods.AddRelation(db, (int)BroAndSis.PersonId, (int)vm.Relationship.PersonId, brotherId, family.Id);
                            if (autoPersonRelationships.Any(x => x.RelRoleId == brotherId || x.RelRoleId == sisterId))
                            {
                                foreach (var AutoBroSis in autoPersonRelationships.Where(x => x.RelRoleId == brotherId || x.RelRoleId == sisterId))
                                {
                                    FamlyMethods.AddRelation(db, (int)AutoBroSis.PersonId, (int)BroAndSis.PersonId, BroAndSis.RelRoleId == brotherId ? brotherId : sisterId, family.Id);
                                    FamlyMethods.AddRelation(db, (int)BroAndSis.PersonId, (int)AutoBroSis.PersonId, AutoBroSis.RelRoleId == brotherId ? brotherId : sisterId, family.Id);

                                }
                            }
                        }
                    }
                    //Person's father(mother) is his brother's(sister) mother's(father) husband(wife)
                    //Person's brother's father(mother) his father
                    if (autoPersonRelationships.Any(x => x.RelRoleId == fatherId || x.RelRoleId == motherId))
                    {

                        foreach (var FatherOrMather in autoPersonRelationships.Where(x => x.RelRoleId == fatherId || x.RelRoleId == motherId))
                        {
                            if (!db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == person.Id && x.RelRoleId == FatherOrMather.RelRoleId))
                            {
                                FamlyMethods.AddRelation(db, person.Id, (int)FatherOrMather.PersonId, (int)FatherOrMather.RelRoleId, family.Id);
                                if (person.GenderId == MaleId)
                                {
                                    FamlyMethods.AddRelation(db, (int)FatherOrMather.PersonId, person.Id, sonId, family.Id);
                                }
                                else
                                {
                                    FamlyMethods.AddRelation(db, (int)FatherOrMather.PersonId, person.Id, daughterId, family.Id);
                                }
                            }
                            //Person's father(mother) is his brother's children's grandpa(grandma)
                            if (personRelationship.Any(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                            {
                                foreach (var child in personRelationship.Where(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                                {
                                    FamlyMethods.AddRelation(db, (int)child.PersonId, (int)FatherOrMather.PersonId, FatherOrMather.RelRoleId == fatherId ? grandpaId : grandmaId, family.Id);
                                    FamlyMethods.AddRelation(db, (int)FatherOrMather.PersonId, (int)child.PersonId, child.RelRoleId == sonId ? grandsonId : granddaughterId, family.Id);

                                }
                            }
                        }
                    }
                    //Person's father(mother) is his brother'(sister) father
                    if (personRelationship.Any(x => x.RelRoleId == fatherId || x.RelRoleId == motherId))
                    {
                        foreach (var FatherOrMather in personRelationship.Where(x => x.RelRoleId == fatherId || x.RelRoleId == motherId))
                        {
                            if (!db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == vm.Relationship.PersonId && x.RelRoleId == FatherOrMather.RelRoleId))
                            {
                                FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)FatherOrMather.PersonId, (int)FatherOrMather.RelRoleId, family.Id);
                                FamlyMethods.AddRelation(db, (int)FatherOrMather.PersonId, (int)vm.Relationship.PersonId, sonId, family.Id);
                            }

                            if (FatherOrMather.RelRoleId == fatherId && !db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == FatherOrMather.PersonId && x.RelRoleId == wifeId))
                            {
                                if (autoPersonRelationships.Any(x => x.RelRoleId == motherId))
                                {
                                    if (!db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == autoPersonRelationships.FirstOrDefault(y => y.RelRoleId == motherId).PersonId && x.RelRoleId == husbandId))
                                    {
                                        FamlyMethods.AddRelation(db, (int)FatherOrMather.PersonId, (int)autoPersonRelationships.FirstOrDefault(x => x.RelRoleId == motherId).PersonId, wifeId, family.Id);
                                        FamlyMethods.AddRelation(db, (int)autoPersonRelationships.FirstOrDefault(x => x.RelRoleId == motherId).PersonId, (int)FatherOrMather.PersonId, husbandId, family.Id);
                                    }
                                }
                            }
                            else if (!db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == FatherOrMather.PersonId && x.RelRoleId == husbandId))
                            {
                                if (autoPersonRelationships.Any(x => x.RelRoleId == fatherId))
                                {
                                    if (!db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == autoPersonRelationships.FirstOrDefault(y => y.RelRoleId == fatherId).PersonId && x.RelRoleId == wifeId))
                                    {
                                        FamlyMethods.AddRelation(db, (int)FatherOrMather.PersonId, (int)autoPersonRelationships.FirstOrDefault(x => x.RelRoleId == fatherId).PersonId, husbandId, family.Id);
                                        FamlyMethods.AddRelation(db, (int)autoPersonRelationships.FirstOrDefault(x => x.RelRoleId == fatherId).PersonId, (int)FatherOrMather.PersonId, wifeId, family.Id);
                                    }
                                }
                            }
                            //Person's father(mother) is his brother's children's grandpa(grandma)
                            if (autoPersonRelationships.Any(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                            {
                                foreach (var child in autoPersonRelationships.Where(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                                {
                                    FamlyMethods.AddRelation(db, (int)child.PersonId, (int)FatherOrMather.PersonId, FatherOrMather.RelRoleId == fatherId ? grandpaId : grandmaId, family.Id);
                                    FamlyMethods.AddRelation(db, (int)FatherOrMather.PersonId, (int)child.PersonId, child.RelRoleId == sonId ? grandsonId : granddaughterId, family.Id);

                                }
                            }
                        }
                    }

                    //Person' brother's other bs chilren are his father's grandchildren
                    // ForEach 1 Check Brother and sister
                    // Foreach 2 Check brother's and sister's child
                    // Foreach 3  Check Person Mother and Father
                    if ((autoPersonRelationships.Any(x => x.RelRoleId == sisterId || x.RelRoleId == brotherId)) && personRelationship.Any(x => x.RelRoleId == motherId || x.RelRoleId == fatherId))
                    {
                        foreach (var broAndSis in autoPersonRelationships.Where(x => x.RelRoleId == sisterId || x.RelRoleId == brotherId))
                        {
                            if (db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == broAndSis.PersonId && (x.RelRoleId == sonId || x.RelRoleId == daughterId)))
                            {
                                foreach (var child in db.Relationships.Where(x => x.FamilyId == family.Id && x.RelatedUserId == broAndSis.PersonId && (x.RelRoleId == sonId || x.RelRoleId == daughterId)))
                                {
                                    foreach (var FatherOrMother in personRelationship.Where(x => x.RelRoleId == motherId || x.RelRoleId == fatherId))
                                    {
                                        FamlyMethods.AddRelation(db, (int)child.PersonId, (int)FatherOrMother.PersonId, FatherOrMother.RelRoleId == fatherId ? grandpaId : grandmaId, family.Id);
                                        FamlyMethods.AddRelation(db, (int)FatherOrMother.PersonId, (int)child.PersonId, child.RelRoleId == sonId ? grandsonId : granddaughterId, family.Id);

                                    }
                                }
                            }
                        }
                    }
                    break;
                //Sister
                case "sister":
                    if (person.Gender.Name.ToLower() == "male")
                    {
                        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "brother").Id;
                    }
                    else if (person.Gender.Name.ToLower() == "female")
                    {
                        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "sister").Id;
                    }
                    //Person' sister's brother is his brother
                    if (autoPersonRelationships.Any(x => x.RelRoleId == sisterId || x.RelRoleId == brotherId))
                    {
                        foreach (var broAndSis in autoPersonRelationships.Where(x => x.RelRoleId == sisterId || x.RelRoleId == brotherId))
                        {
                            FamlyMethods.AddRelation(db, person.Id, (int)broAndSis.PersonId, (int)broAndSis.RelRoleId, family.Id);
                            if (person.GenderId == MaleId)
                            {
                                FamlyMethods.AddRelation(db, (int)broAndSis.PersonId, person.Id, brotherId, family.Id);
                            }
                            else
                            {
                                FamlyMethods.AddRelation(db, (int)broAndSis.PersonId, person.Id, sisterId, family.Id);
                            }
                        }
                    }
                    //Person'brothers and sisters are his sister's sister and brothers
                    if (personRelationship.Any(x => x.RelRoleId == brotherId || x.RelRoleId == sisterId))
                    {
                        foreach (var BroAndSis in personRelationship.Where(x => x.RelRoleId == sisterId || x.RelRoleId == brotherId))
                        {
                            FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)BroAndSis.PersonId, (int)BroAndSis.RelRoleId, family.Id);
                            FamlyMethods.AddRelation(db, (int)BroAndSis.PersonId, (int)vm.Relationship.PersonId, sisterId, family.Id);
                            if (autoPersonRelationships.Any(x => x.RelRoleId == brotherId || x.RelRoleId == sisterId))
                            {
                                foreach (var AutoBroSis in autoPersonRelationships.Where(x => x.RelRoleId == brotherId || x.RelRoleId == sisterId))
                                {
                                    FamlyMethods.AddRelation(db, (int)AutoBroSis.PersonId, (int)BroAndSis.PersonId, BroAndSis.RelRoleId == brotherId ? brotherId : sisterId, family.Id);
                                    FamlyMethods.AddRelation(db, (int)BroAndSis.PersonId, (int)AutoBroSis.PersonId, AutoBroSis.RelRoleId == brotherId ? brotherId : sisterId, family.Id);

                                }
                            }
                        }
                    }
                    //Person's father(mother) is his sister's(sister) mother's(father) husband(wife)
                    //Person's sister's father(mother) his father
                    if (autoPersonRelationships.Any(x => x.RelRoleId == fatherId || x.RelRoleId == motherId))
                    {

                        foreach (var FatherOrMather in autoPersonRelationships.Where(x => x.RelRoleId == fatherId || x.RelRoleId == motherId))
                        {
                            if (!db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == person.Id && x.RelRoleId == FatherOrMather.RelRoleId))
                            {
                                FamlyMethods.AddRelation(db, person.Id, (int)FatherOrMather.PersonId, (int)FatherOrMather.RelRoleId, family.Id);
                                if (person.GenderId == MaleId)
                                {
                                    FamlyMethods.AddRelation(db, (int)FatherOrMather.PersonId, person.Id, sonId, family.Id);
                                }
                                else
                                {
                                    FamlyMethods.AddRelation(db, (int)FatherOrMather.PersonId, person.Id, daughterId, family.Id);
                                }
                            }
                            //Person's father(mother) is his sister's children's grandpa(grandma)
                            if (personRelationship.Any(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                            {
                                foreach (var child in personRelationship.Where(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                                {
                                    FamlyMethods.AddRelation(db, (int)child.PersonId, (int)FatherOrMather.PersonId, FatherOrMather.RelRoleId == fatherId ? grandpaId : grandmaId, family.Id);
                                    FamlyMethods.AddRelation(db, (int)FatherOrMather.PersonId, (int)child.PersonId, child.RelRoleId == sonId ? grandsonId : granddaughterId, family.Id);

                                }
                            }
                        }
                    }
                    //Person's father(mother) is his sister'(sister) father
                    if (personRelationship.Any(x => x.RelRoleId == fatherId || x.RelRoleId == motherId))
                    {
                        foreach (var FatherOrMather in personRelationship.Where(x => x.RelRoleId == fatherId || x.RelRoleId == motherId))
                        {
                            if (!db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == vm.Relationship.PersonId && x.RelRoleId == FatherOrMather.RelRoleId))
                            {
                                FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)FatherOrMather.PersonId, (int)FatherOrMather.RelRoleId, family.Id);
                                FamlyMethods.AddRelation(db, (int)FatherOrMather.PersonId, (int)vm.Relationship.PersonId, daughterId, family.Id);
                            }

                            if (FatherOrMather.RelRoleId == fatherId && !db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == FatherOrMather.PersonId && x.RelRoleId == wifeId))
                            {
                                if (autoPersonRelationships.Any(x => x.RelRoleId == motherId))
                                {
                                    if (!db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == autoPersonRelationships.FirstOrDefault(y => y.RelRoleId == motherId).PersonId && x.RelRoleId == husbandId))
                                    {
                                        FamlyMethods.AddRelation(db, (int)FatherOrMather.PersonId, (int)autoPersonRelationships.FirstOrDefault(x => x.RelRoleId == motherId).PersonId, wifeId, family.Id);
                                        FamlyMethods.AddRelation(db, (int)autoPersonRelationships.FirstOrDefault(x => x.RelRoleId == motherId).PersonId, (int)FatherOrMather.PersonId, husbandId, family.Id);
                                    }
                                }
                            }
                            else if (!db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == FatherOrMather.PersonId && x.RelRoleId == husbandId))
                            {
                                if (autoPersonRelationships.Any(x => x.RelRoleId == fatherId))
                                {
                                    if (!db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == autoPersonRelationships.FirstOrDefault(y => y.RelRoleId == fatherId).PersonId && x.RelRoleId == wifeId))
                                    {
                                        FamlyMethods.AddRelation(db, (int)FatherOrMather.PersonId, (int)autoPersonRelationships.FirstOrDefault(x => x.RelRoleId == fatherId).PersonId, husbandId, family.Id);
                                        FamlyMethods.AddRelation(db, (int)autoPersonRelationships.FirstOrDefault(x => x.RelRoleId == fatherId).PersonId, (int)FatherOrMather.PersonId, wifeId, family.Id);
                                    }
                                }
                            }
                            //Person's father(mother) is his brother's children's grandpa(grandma)
                            if (autoPersonRelationships.Any(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                            {
                                foreach (var child in autoPersonRelationships.Where(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                                {
                                    FamlyMethods.AddRelation(db, (int)child.PersonId, (int)FatherOrMather.PersonId, FatherOrMather.RelRoleId == fatherId ? grandpaId : grandmaId, family.Id);
                                    FamlyMethods.AddRelation(db, (int)FatherOrMather.PersonId, (int)child.PersonId, child.RelRoleId == sonId ? grandsonId : granddaughterId, family.Id);
                                }
                            }
                        }
                    }

                    //Person' sister's other bs children are his father's grandchildren
                    // ForEach 1 Check Brother and sister
                    // Foreach 2 Check brother's and sister's child
                    // Foreach 3  Check Person Mother and Father
                    if ((autoPersonRelationships.Any(x => x.RelRoleId == sisterId || x.RelRoleId == brotherId)) && personRelationship.Any(x => x.RelRoleId == motherId || x.RelRoleId == fatherId))
                    {
                        foreach (var broAndSis in autoPersonRelationships.Where(x => x.RelRoleId == sisterId || x.RelRoleId == brotherId))
                        {
                            if (db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == broAndSis.PersonId && (x.RelRoleId == sonId || x.RelRoleId == daughterId)))
                            {
                                foreach (var child in db.Relationships.Where(x => x.FamilyId == family.Id && x.RelatedUserId == broAndSis.PersonId && (x.RelRoleId == sonId || x.RelRoleId == daughterId)))
                                {
                                    foreach (var FatherOrMother in personRelationship.Where(x => x.RelRoleId == motherId || x.RelRoleId == fatherId))
                                    {
                                        FamlyMethods.AddRelation(db, (int)child.PersonId, (int)FatherOrMother.PersonId, FatherOrMother.RelRoleId == fatherId ? grandpaId : grandmaId, family.Id);
                                        FamlyMethods.AddRelation(db, (int)FatherOrMother.PersonId, (int)child.PersonId, child.RelRoleId == sonId ? grandsonId : granddaughterId, family.Id);
                                    }
                                }
                            }
                        }
                    }
                    break;
                //Uncle
                //case "uncle":
                //    if (person.Gender.Name.ToLower() == "male")
                //    {
                //        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "nephew").Id;
                //    }
                //    else if (person.Gender.Name.ToLower() == "female")
                //    {
                //        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "niece").Id;
                //    }
                //    break;
                //Aunt
                //case "aunt":
                //    if (person.Gender.Name.ToLower() == "male")
                //    {
                //        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "nephew").Id;
                //    }
                //    else if (person.Gender.Name.ToLower() == "female")
                //    {
                //        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "niece").Id;
                //    }
                //    break;
                //Grandson
                //case "grandson":
                //    if (person.Gender.Name.ToLower() == "male")
                //    {
                //        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "grandpa").Id;
                //        if (autoPersonRelationships.Where(x => x.Role.Name.ToLower() == "grandpa").Count() >= 2)
                //        {
                //            ViewBag.Relation = "The relative's may have two grandparents";
                //            return View(vm);
                //        }

                //    }
                //    else if (person.Gender.Name.ToLower() == "female")
                //    {
                //        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "grandma").Id;
                //        if (autoPersonRelationships.Where(x => x.Role.Name.ToLower() == "grandma").Count() >= 2)
                //        {
                //            ViewBag.Relation = "The relative's may have two grandparents";
                //            return View(vm);
                //        }
                //    }
                //    break;
                //Granddaughter
                //case "granddaughter":
                //    if (person.Gender.Name.ToLower() == "male")
                //    {
                //        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "grandpa").Id;
                //        if (autoPersonRelationships.Where(x => x.Role.Name.ToLower() == "grandpa").Count() >= 2)
                //        {
                //            ViewBag.Relation = "The relative's may have two grandparents";
                //            return View(vm);
                //        }
                //    }
                //    else if (person.Gender.Name.ToLower() == "female")
                //    {
                //        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "grandma").Id;
                //        if (autoPersonRelationships.Where(x => x.Role.Name.ToLower() == "grandma").Count() >= 2)
                //        {
                //            ViewBag.Relation = "The relative's may have two grandparents";
                //            return View(vm);
                //        }
                //    }
                //    break;
                //Cousin
                //case "cousin":
                //    RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "cousin").Id;
                //    break;
                //Son
                case "son":
                    if (person.Gender.Name.ToLower() == "male")
                    {
                        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "father").Id;
                        if (autoPersonRelationships.Where(x => x.Role.Name.ToLower() == "father").Count() >= 1)
                        {
                            ViewBag.Relation = "The relative's father must be one";
                            return View(vm);
                        }
                    }
                    else if (person.Gender.Name.ToLower() == "female")
                    {
                        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "mother").Id;
                        if (autoPersonRelationships.Where(x => x.Role.Name.ToLower() == "mother").Count() >= 1)
                        {
                            ViewBag.Relation = "The relative's mother must be one";
                            return View(vm);
                        }
                    }
                    //Person is his son's brother(sister)'s father(mother)
                    if (autoPersonRelationships.Any(x => x.RelRoleId == brotherId || x.RelRoleId == sisterId))
                    {
                        foreach (var BroAndSis in autoPersonRelationships.Where(x => x.RelRoleId == brotherId || x.RelRoleId == sisterId))
                        {
                            if (!db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == BroAndSis.PersonId && x.RelRoleId == (person.GenderId == MaleId ? fatherId : motherId)))
                            {
                                FamlyMethods.AddRelation(db, (int)BroAndSis.PersonId, person.Id, person.GenderId == MaleId ? fatherId : motherId, family.Id);
                                FamlyMethods.AddRelation(db, person.Id, (int)BroAndSis.PersonId, BroAndSis.RelRoleId == brotherId ? sonId : daughterId, family.Id);
                            }
                            //Person's sun's brothers' and sisters' children are his granchild
                            if (db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == BroAndSis.PersonId && (x.RelRoleId == sonId || x.RelRoleId == daughterId)))
                            {
                                foreach (var grandchild in db.Relationships.Where(x => x.FamilyId == family.Id && x.RelatedUserId == BroAndSis.PersonId && (x.RelRoleId == sonId || x.RelRoleId == daughterId)))
                                {
                                    FamlyMethods.AddRelation(db, person.Id, (int)grandchild.PersonId, grandchild.RelRoleId == sonId ? grandsonId : granddaughterId, family.Id);
                                    FamlyMethods.AddRelation(db, (int)grandchild.PersonId, person.Id, person.GenderId == MaleId ? grandpaId : grandmaId, family.Id);
                                }
                                if (personRelationship.Any(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)))
                                {
                                    foreach (var grandChild in db.Relationships.Where(x => x.FamilyId == family.Id && x.RelatedUserId == BroAndSis.PersonId && (x.RelRoleId == sonId || x.RelRoleId == daughterId)))
                                    {
                                        FamlyMethods.AddRelation(db, (int)personRelationship.First(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)).PersonId, (int)grandChild.PersonId, grandChild.RelRoleId == sonId ? grandsonId : granddaughterId, family.Id);
                                        FamlyMethods.AddRelation(db, (int)grandChild.PersonId, (int)personRelationship.First(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)).PersonId, person.GenderId == MaleId ? grandmaId : grandpaId, family.Id);
                                    }
                                }
                            }
                            //Person's Father(mather) is his son(daughter) 's  granpa(grandma)
                            if (personRelationship.Any(x => x.RelRoleId == fatherId || x.RelRoleId == motherId))
                            {
                                foreach (var FatherOrMother in personRelationship.Where(x => x.RelRoleId == fatherId || x.RelRoleId == motherId))
                                {
                                    FamlyMethods.AddRelation(db, (int)FatherOrMother.PersonId, (int)BroAndSis.PersonId, BroAndSis.RelRoleId == brotherId ? grandsonId : granddaughterId, family.Id);
                                    FamlyMethods.AddRelation(db, (int)BroAndSis.PersonId, (int)FatherOrMother.PersonId, FatherOrMother.RelRoleId == fatherId ? grandpaId : grandmaId, family.Id);
                                }
                            }
                        }
                        if (personRelationship.Any(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)))
                        {
                            foreach (var BroAndSis in autoPersonRelationships.Where(x => x.RelRoleId == brotherId || x.RelRoleId == sisterId))
                            {
                                if (!db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == BroAndSis.PersonId && x.RelRoleId == (person.GenderId == MaleId ? fatherId : motherId)))
                                {
                                    FamlyMethods.AddRelation(db, (int)BroAndSis.PersonId, (int)personRelationship.First(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)).PersonId, person.GenderId == MaleId ? motherId : fatherId, family.Id);
                                    FamlyMethods.AddRelation(db, (int)personRelationship.First(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)).PersonId, (int)BroAndSis.PersonId, BroAndSis.RelRoleId == brotherId ? sonId : daughterId, family.Id);
                                }
                            }
                        }
                    }
                    //Person's wife(husband) is his son's(daughter) mother(mather)
                    if (personRelationship.Any(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)))
                    {
                        FamlyMethods.AddRelation(db, (int)personRelationship.First(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)).PersonId, (int)vm.Relationship.PersonId, sonId, family.Id);
                        FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)personRelationship.First(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)).PersonId, (person.GenderId == MaleId ? motherId : fatherId), family.Id);
                    }
                    //Person's sun's mother(father) is his wife(husband)
                    if (!personRelationship.Any(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)))
                    {
                        if (autoPersonRelationships.Any(x => x.RelRoleId == (person.GenderId == MaleId ? motherId : fatherId)))
                        {
                            FamlyMethods.AddRelation(db, person.Id, (int)autoPersonRelationships.First(x => x.RelRoleId == (person.GenderId == MaleId ? motherId : fatherId)).PersonId, person.GenderId == MaleId ? wifeId : husbandId, family.Id);
                            FamlyMethods.AddRelation(db, (int)autoPersonRelationships.First(x => x.RelRoleId == (person.GenderId == MaleId ? motherId : fatherId)).PersonId, person.Id, person.GenderId == MaleId ? husbandId : wifeId, family.Id);
                        }
                    }
                    //Person' son's son(daugter) is his grandson
                    if (autoPersonRelationships.Any(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                    {
                        foreach (var child in autoPersonRelationships.Where(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                        {
                            FamlyMethods.AddRelation(db, (int)child.PersonId, person.Id, person.GenderId == MaleId ? grandpaId : grandmaId, family.Id);
                            FamlyMethods.AddRelation(db, person.Id, (int)child.PersonId, child.RelRoleId == sonId ? grandsonId : granddaughterId, family.Id);
                            if (personRelationship.Any(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)))
                            {
                                FamlyMethods.AddRelation(db, (int)personRelationship.First(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)).PersonId, (int)child.PersonId, child.RelRoleId == sonId ? grandsonId : granddaughterId, family.Id);
                                FamlyMethods.AddRelation(db, (int)child.PersonId, (int)personRelationship.First(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)).PersonId, person.GenderId == MaleId ? grandpaId : grandmaId, family.Id);
                            }
                        }
                    }
                    //Person's father(mather) is his son's grandpa
                    if (personRelationship.Any(x => x.RelRoleId == fatherId || x.RelRoleId == motherId))
                    {
                        foreach (var FatherOrMother in personRelationship.Where(x => x.RelRoleId == fatherId || x.RelRoleId == motherId))
                        {
                            FamlyMethods.AddRelation(db, (int)FatherOrMother.PersonId, (int)vm.Relationship.PersonId, grandsonId, family.Id);
                            FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)FatherOrMother.PersonId, FatherOrMother.RelRoleId == fatherId ? grandpaId : grandmaId, family.Id);
                        }
                    }
                    //Person's son is his children' brother
                    if (personRelationship.Any(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                    {
                        foreach (var Child in personRelationship.Where(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                        {
                            FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)Child.PersonId, Child.RelRoleId == sonId ? brotherId : sisterId, family.Id);
                            FamlyMethods.AddRelation(db, (int)Child.PersonId, (int)vm.Relationship.PersonId, brotherId, family.Id);
                            if (db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == vm.Relationship.PersonId && (x.RelRoleId == brotherId || x.RelRoleId == sisterId)))
                            {
                                foreach (var BroAndSis in db.Relationships.Where(x => x.FamilyId == family.Id && x.RelatedUserId == vm.Relationship.PersonId && (x.RelRoleId == brotherId || x.RelRoleId == sisterId)))
                                {
                                    FamlyMethods.AddRelation(db, (int)Child.PersonId, (int)BroAndSis.PersonId, BroAndSis.RelRoleId == brotherId ? brotherId : sisterId, family.Id);
                                    FamlyMethods.AddRelation(db, (int)BroAndSis.PersonId, (int)Child.PersonId, Child.RelRoleId == sonId ? brotherId : sisterId, family.Id);
                                }
                            }
                        }
                    }

                    break;
                //Daughter
                case "daughter":
                    if (person.Gender.Name.ToLower() == "male")
                    {
                        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "father").Id;
                        if (autoPersonRelationships.Where(x => x.Role.Name.ToLower() == "father").Count() >= 1)
                        {
                            ViewBag.Relation = "The relative's father must be one";
                            return View(vm);
                        }
                    }
                    else if (person.Gender.Name.ToLower() == "female")
                    {
                        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "mother").Id;
                        if (autoPersonRelationships.Where(x => x.Role.Name.ToLower() == "mother").Count() >= 1)
                        {
                            ViewBag.Relation = "The relative's mother must be one";
                            return View(vm);
                        }
                    }
                    //Person is his sister's brother(sister)'s father(mother)
                    if (autoPersonRelationships.Any(x => x.RelRoleId == brotherId || x.RelRoleId == sisterId))
                    {
                        foreach (var BroAndSis in autoPersonRelationships.Where(x => x.RelRoleId == brotherId || x.RelRoleId == sisterId))
                        {
                            if (!db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == BroAndSis.PersonId && x.RelRoleId == (person.GenderId == MaleId ? fatherId : motherId)))
                            {
                                FamlyMethods.AddRelation(db, (int)BroAndSis.PersonId, person.Id, person.GenderId == MaleId ? fatherId : motherId, family.Id);
                                FamlyMethods.AddRelation(db, person.Id, (int)BroAndSis.PersonId, BroAndSis.RelRoleId == brotherId ? sonId : daughterId, family.Id);
                            }
                            //Person's sun's brothers' and sisters' children are his granchild
                            if (db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == BroAndSis.PersonId && (x.RelRoleId == sonId || x.RelRoleId == daughterId)))
                            {
                                foreach (var grandchild in db.Relationships.Where(x => x.FamilyId == family.Id && x.RelatedUserId == BroAndSis.PersonId && (x.RelRoleId == sonId || x.RelRoleId == daughterId)))
                                {
                                    FamlyMethods.AddRelation(db, person.Id, (int)grandchild.PersonId, grandchild.RelRoleId == sonId ? grandsonId : granddaughterId, family.Id);
                                    FamlyMethods.AddRelation(db, (int)grandchild.PersonId, person.Id, person.GenderId == MaleId ? grandpaId : grandmaId, family.Id);
                                }
                                if (personRelationship.Any(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)))
                                {
                                    foreach (var grandChild in db.Relationships.Where(x => x.FamilyId == family.Id && x.RelatedUserId == BroAndSis.PersonId && (x.RelRoleId == sonId || x.RelRoleId == daughterId)))
                                    {
                                        FamlyMethods.AddRelation(db, (int)personRelationship.First(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)).PersonId, (int)grandChild.PersonId, grandChild.RelRoleId == sonId ? grandsonId : granddaughterId, family.Id);
                                        FamlyMethods.AddRelation(db, (int)grandChild.PersonId, (int)personRelationship.First(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)).PersonId, person.GenderId == MaleId ? grandmaId : grandpaId, family.Id);
                                    }
                                }
                            }
                            //Person's Father(mather) is his son(daughter) 's  granpa(grandma)
                            if (personRelationship.Any(x => x.RelRoleId == fatherId || x.RelRoleId == motherId))
                            {
                                foreach (var FatherOrMother in personRelationship.Where(x => x.RelRoleId == fatherId || x.RelRoleId == motherId))
                                {
                                    FamlyMethods.AddRelation(db, (int)FatherOrMother.PersonId, (int)BroAndSis.PersonId, BroAndSis.RelRoleId == brotherId ? grandsonId : granddaughterId, family.Id);
                                    FamlyMethods.AddRelation(db, (int)BroAndSis.PersonId, (int)FatherOrMother.PersonId, FatherOrMother.RelRoleId == fatherId ? grandpaId : grandmaId, family.Id);
                                }
                            }
                        }
                        if (personRelationship.Any(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)))
                        {
                            foreach (var BroAndSis in autoPersonRelationships.Where(x => x.RelRoleId == brotherId || x.RelRoleId == sisterId))
                            {
                                if (!db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == BroAndSis.PersonId && x.RelRoleId == (person.GenderId == MaleId ? fatherId : motherId)))
                                {
                                    FamlyMethods.AddRelation(db, (int)BroAndSis.PersonId, (int)personRelationship.First(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)).PersonId, person.GenderId == MaleId ? motherId : fatherId, family.Id);
                                    FamlyMethods.AddRelation(db, (int)personRelationship.First(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)).PersonId, (int)BroAndSis.PersonId, BroAndSis.RelRoleId == brotherId ? sonId : daughterId, family.Id);
                                }
                            }
                        }
                    }
                    //Person's wife(husband) is his son's(daughter) mother(mather)
                    if (personRelationship.Any(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)))
                    {
                        FamlyMethods.AddRelation(db, (int)personRelationship.First(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)).PersonId, (int)vm.Relationship.PersonId, daughterId, family.Id);
                        FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)personRelationship.First(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)).PersonId, (person.GenderId == MaleId ? motherId : fatherId), family.Id);
                    }
                    //Person's daughter's mother(father) is his wife(husband)
                    if (!personRelationship.Any(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)))
                    {
                        if (autoPersonRelationships.Any(x => x.RelRoleId == (person.GenderId == MaleId ? motherId : fatherId)))
                        {
                            FamlyMethods.AddRelation(db, person.Id, (int)autoPersonRelationships.First(x => x.RelRoleId == (person.GenderId == MaleId ? motherId : fatherId)).PersonId, person.GenderId == MaleId ? wifeId : husbandId, family.Id);
                            FamlyMethods.AddRelation(db, (int)autoPersonRelationships.First(x => x.RelRoleId == (person.GenderId == MaleId ? motherId : fatherId)).PersonId, person.Id, person.GenderId == MaleId ? husbandId : wifeId, family.Id);
                        }
                    }
                    //Person' daughter's son(daugter) is his grandson
                    if (autoPersonRelationships.Any(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                    {
                        foreach (var child in autoPersonRelationships.Where(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                        {
                            FamlyMethods.AddRelation(db, (int)child.PersonId, person.Id, person.GenderId == MaleId ? grandpaId : grandmaId, family.Id);
                            FamlyMethods.AddRelation(db, person.Id, (int)child.PersonId, child.RelRoleId == sonId ? grandsonId : granddaughterId, family.Id);
                            if (personRelationship.Any(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)))
                            {
                                FamlyMethods.AddRelation(db, (int)personRelationship.First(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)).PersonId, (int)child.PersonId, child.RelRoleId == sonId ? grandsonId : granddaughterId, family.Id);
                                FamlyMethods.AddRelation(db, (int)child.PersonId, (int)personRelationship.First(x => x.RelRoleId == (person.GenderId == MaleId ? wifeId : husbandId)).PersonId, person.GenderId == MaleId ? grandpaId : grandmaId, family.Id);
                            }
                        }
                    }
                    //Person's father(mather) is his daughter's grandpa
                    if (personRelationship.Any(x => x.RelRoleId == fatherId || x.RelRoleId == motherId))
                    {
                        foreach (var FatherOrMother in personRelationship.Where(x => x.RelRoleId == fatherId || x.RelRoleId == motherId))
                        {
                            FamlyMethods.AddRelation(db, (int)FatherOrMother.PersonId, (int)vm.Relationship.PersonId, granddaughterId, family.Id);
                            FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)FatherOrMother.PersonId, FatherOrMother.RelRoleId == fatherId ? grandpaId : grandmaId, family.Id);
                        }
                    }
                    //Person's daughter is his children' brother
                    if (personRelationship.Any(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                    {
                        foreach (var Child in personRelationship.Where(x => x.RelRoleId == sonId || x.RelRoleId == daughterId))
                        {
                            FamlyMethods.AddRelation(db, (int)vm.Relationship.PersonId, (int)Child.PersonId, Child.RelRoleId == sonId ? brotherId : sisterId, family.Id);
                            FamlyMethods.AddRelation(db, (int)Child.PersonId, (int)vm.Relationship.PersonId, sisterId, family.Id);
                            if (db.Relationships.Any(x => x.FamilyId == family.Id && x.RelatedUserId == vm.Relationship.PersonId && (x.RelRoleId == brotherId || x.RelRoleId == sisterId)))
                            {
                                foreach (var BroAndSis in db.Relationships.Where(x => x.FamilyId == family.Id && x.RelatedUserId == vm.Relationship.PersonId && (x.RelRoleId == brotherId || x.RelRoleId == sisterId)))
                                {
                                    FamlyMethods.AddRelation(db, (int)Child.PersonId, (int)BroAndSis.PersonId, BroAndSis.RelRoleId == brotherId ? brotherId : sisterId, family.Id);
                                    FamlyMethods.AddRelation(db, (int)BroAndSis.PersonId, (int)Child.PersonId, Child.RelRoleId == sonId ? brotherId : sisterId, family.Id);
                                }
                            }
                        }
                    }
                    break;
                    //Niece
                    //case "niece":
                    //    if (person.Gender.Name.ToLower() == "male")
                    //    {
                    //        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "uncle").Id;
                    //    }
                    //    else if (person.Gender.Name.ToLower() == "female")
                    //    {
                    //        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "aunt").Id;
                    //    }
                    //    break;
                    //Nephew
                    //case "nephew":
                    //    if (person.Gender.Name.ToLower() == "male")
                    //    {
                    //        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "uncle").Id;
                    //    }
                    //    else if (person.Gender.Name.ToLower() == "female")
                    //    {
                    //        RoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "aunt").Id;
                    //    }
                    //    break;
            }
            bool MainRelationship=false;
            if (vm.Relationship.RelRoleId== fatherId)
            {
               Relationship mainRel= personRelationship.FirstOrDefault(x => x.IsMain == true);
                if (mainRel != null)
                {
                    mainRel.IsMain = false;
                    MainRelationship = true;
                }
            }
            if (vm.Relationship.RelRoleId == motherId)
            {
                if(!personRelationship.Any(x => x.RelRoleId == fatherId))
                {
                    Relationship mainRel = personRelationship.FirstOrDefault(x => x.IsMain == true);
                    if (mainRel != null)
                    {
                        mainRel.IsMain = false;
                        MainRelationship = true;
                    }
                }
            }
            Relationship AutoRelationship = new Relationship
            {
                PersonId = vm.Relationship.RelatedUserId,
                RelatedUserId = (int)vm.Relationship.PersonId,
                RelRoleId = RoleId,
                FamilyId = family.Id,
                IsMain=MainRelationship
            };
            db.Relationships.Add(AutoRelationship);
            db.SaveChanges();
            TempData["addRelation"] = true;
            return RedirectToAction(nameof(PersonDetail), new { id = vm.Relationship.RelatedUserId, familyId = family.Id });
        }
        public async Task<IActionResult> DeleteRelation(int id, int familyId, int personId)
        {
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            int familId = FamlyMethods.GetFamilyId(db, user);
            Family family = db.Families.FirstOrDefault(x => x.Id == familyId&&x.Id==familId);
            if (family == null)
            {
                return NotFound();
            }
            Person mainPerson = db.People.Where(x=>x.FamilyId==familId).FirstOrDefault(x => x.Id == personId);
            if (mainPerson == null)
            {
                return NotFound();
            }
            
            Person person = db.People.Where(x=>x.FamilyId==familId).FirstOrDefault(x => x.Id == id);
            IEnumerable<Relationship> relationships = db.Relationships.Where(x => x.FamilyId == familyId && (x.PersonId == id || x.RelatedUserId == id)&&x.IsMain==false);

            if (!relationships.Any())
            {
                return NotFound();
            }
            db.Relationships.RemoveRange(relationships);
            db.SaveChanges();
            TempData["deleteRelation"] = true;
            return RedirectToAction(nameof(PersonDetail), new { id = personId, familyId = familyId });
        }
       
    }
}