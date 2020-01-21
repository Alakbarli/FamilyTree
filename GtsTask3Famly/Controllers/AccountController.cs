using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using GtsTask3Famly.DAL;
using GtsTask3Famly.Models;
using GtsTask3Famly.Utilities;
using GtsTask3Famly.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GtsTask3Famly.Controllers
{
    public class AccountController : Controller
    {
        private readonly DB db;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;
        private readonly IHostingEnvironment env;
        public AccountController(DB dB, UserManager<User> uManager, IHostingEnvironment hosting, SignInManager<User> signManager, RoleManager<IdentityRole> roleManagerr, IConfiguration con)
        {
            db = dB;
            userManager = uManager;
            signInManager = signManager;
            roleManager = roleManagerr;
            env = hosting;
            configuration = con;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("index","home");
            }
            else
            {
                return RedirectToAction(nameof(SignIn));
            }
        }
        public IActionResult SignIn()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("index", "home");
            }
            return View();
        }
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInVM signVm)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }
            
            User user = await userManager.FindByEmailAsync(signVm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email or password is invalid.");
                return View(signVm);
            }
            Microsoft.AspNetCore.Identity.SignInResult signInResult = await signInManager.PasswordSignInAsync(user, signVm.Password, true, true);
            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email or password is invalid.");
                return View(signVm);
            }
            return RedirectToAction("Index", "Home");
        }
        
        public IActionResult SignUp()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("index", "home");
            }
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpVM newUser)
        {
            if (!ModelState.IsValid)
            {

                return View(newUser);
            }
             var checkEmail= new EmailAddressAttribute();
            if (!checkEmail.IsValid(newUser.Email))
            {
                ModelState.AddModelError("Email", "This email is invalid");
                return View(newUser);
            }
           
            try
            {
                var checkEmail2 = new System.Net.Mail.MailAddress(newUser.Email);
                if (checkEmail2.Address != newUser.Email)
                {
                    ModelState.AddModelError("Email", "This email is invalid");
                    return View(newUser);
                }
            }
            
            catch
            {
                ModelState.AddModelError("Email", "This email is invalid");
                return View(newUser);
            }

            if (DateTime.Today.Year - newUser.BirthDate.Year <= 1) 
            {
                ModelState.AddModelError("Birthday", "The user must be older than 1 year");
                return View(newUser);
            }
            User user = new User
            {
                Firstname = newUser.Firtsname.Trim(),
                Lastname = newUser.Lastname.Trim(),
                Email = newUser.Email.Trim(),
                BirthDate = newUser.BirthDate,
                UserName= newUser.Firtsname.Trim()+ newUser.Lastname.Trim()+Guid.NewGuid().ToString()
            };
            switch (newUser.GenderId)
            {
                case 1:
                    user.GenderId = 1;
                    user.Avatar = "default1.jpg";
                    break;
                case 2:
                    user.GenderId = 2;
                    user.Avatar = "default2.jpg";
                    break;
                default:
                    ModelState.AddModelError("GenderId", "Select valid gender");
                    return View(newUser);
            }
            
            IdentityResult identityResult = await userManager.CreateAsync(user, newUser.Password);
            if (!identityResult.Succeeded)
            {
               foreach (var er in identityResult.Errors)
               {

                    ModelState.AddModelError("", er.Description);
               }

                return View(newUser);
            }
            await userManager.AddToRoleAsync(user,Utilities.SD.MemberRole);
            await userManager.UpdateAsync(user);
            string emailConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

            #region Sending Email Confirmation Message
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(configuration["ConnectionStrings:SmtpClientCredentialEmail"], configuration["ConnectionStrings:SmtpClientCredentialPassword"]);

            MailMessage message = new MailMessage(configuration["ConnectionStrings:SmtpClientCredentialEmail"], newUser.Email);
            message.IsBodyHtml = true;
            message.Subject = "Confirm account";
            message.Body = $"<table style='width:100%;background-color:#fbfbfb;padding:50px'><thead style ='width:100%;display:flex;justify-content:center;'><tr style ='width:100%;display:flex;justify-content:center;'><th style ='width:100%;color:#7e0f9a;font-family:Roboto, sans-serif;font-weight:400;font-size:50px'>Family Tree</th></tr><thead><tbody><tr><td style ='padding:30px 0px;color:#353535;font-family:Roboto Condensed, sans-serif;font-size:20px;'> Dear user, you have successfully signed up. Click the 'Verify Account' button below to verify your account.</td></tr><tr><td style ='font-family:Roboto Condensed, sans-serif;text-align:center;'><a href='https://localhost:44341/account/confirmemail?id={user.Id}&emailConfirmationToken={emailConfirmationToken}' style ='text-decoration:none;padding:10px 30px;border-radius:3px;background-color:#8d11ff;color:black;font-weight:lighter;font-size:20px;cursor:pointer;'>Confirm account</a></td></tr></tbody></table>";

            client.Send(message);
            #endregion
            TempData["signup"] = true;
            await signInManager.SignInAsync(user, true);
            Family family = new Family { Name = $"{user.Firstname}'s family", Logo = "defaultfamily.png" };
            db.Families.Add(family);
            db.SaveChanges();
            FamilyToUser familyToUser = new FamilyToUser { FamilyId = family.Id, UserId = user.Id };
            await db.FamilyToUsers.AddAsync(familyToUser);
            db.SaveChanges();
            Person person = new Person
            {
                Age = DateTime.Today.Year - user.BirthDate.Year,
                Birthdate=user.BirthDate,
                FamilyId = family.Id,
                Firstname = user.Firstname,
                GenderId = user.GenderId,
                LastName = user.Lastname,
                Photo = user.Avatar
            };
            db.People.Add(person);
            db.SaveChanges();
             UserToPerson userToPerson = new UserToPerson { PersonId = person.Id, UserId = user.Id };
            db.UserToPeople.Add(userToPerson);
            db.SaveChanges();
            Relationship relationship = new Relationship
            {
                IsMain = true,
                FamilyId = family.Id,
                RelatedUserId = person.Id,
                RelRoleId = db.RelRoles.FirstOrDefault(x => x.Name.ToLower() == "norole").Id
            };
            db.Relationships.Add(relationship);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Confirmemail(string id,string emailConfirmationToken)
        {
            if (id == null || emailConfirmationToken == null)
            {
                return NotFound();
            }
            User user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                NotFound();
            }
            emailConfirmationToken = emailConfirmationToken.Replace(" ", "+");
            var result = await userManager.ConfirmEmailAsync(user, emailConfirmationToken);
            if (!result.Succeeded)
            {
                return NotFound();
            }
            await userManager.UpdateAsync(user);
            TempData["emailConfirm"] = true;
            await signInManager.SignInAsync(user, true);
            return RedirectToAction("index", "home");
        }
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            User user =await userManager.FindByNameAsync(User.Identity.Name);
            user.Gender = db.Genders.FirstOrDefault(x => x.Id == user.GenderId);
            return View(user);
        }
        [Authorize]
        public async Task<IActionResult> Signout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "account");
        }
        public IActionResult Forgotpassword()
        {
            return View();
        }
        [HttpPost][ValidateAntiForgeryToken]
        public async Task<IActionResult> Forgotpassword(ForgotPasswordVM forgotVm)
        {
            if (!ModelState.IsValid)
            {
                return View(forgotVm);
            }

            User user = await userManager.FindByEmailAsync(forgotVm.Email);

            if (user == null)
            {
                ModelState.AddModelError("Email", "No account found to match the email you specified. Make sure you enter the correct email.");
                return View(forgotVm);
            }
            string passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(user);

            #region Sending Email Account Restoration Message
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(configuration["ConnectionStrings:SmtpClientCredentialEmail"], configuration["ConnectionStrings:SmtpClientCredentialPassword"]);

            MailMessage message = new MailMessage(configuration["ConnectionStrings:SmtpClientCredentialEmail"], forgotVm.Email);
            message.IsBodyHtml = true;
            message.Subject = "Account recovery";
            message.Body = $"<table style='width:100%;background-color:#292C34;padding:50px'><thead style ='width:100%;display:flex;justify-content:center;'><tr style ='width:100%;display:flex;justify-content:center;'><th style ='width:100%;color:#7e0f9a;font-family:Roboto, sans-serif;font-weight:400;font-size:50px'>Family Tree</th></tr><thead><tbody><tr><td style ='padding:30px 0px;color:white;font-family:Roboto Condensed, sans-serif;font-size:20px;text-align:center;'>Dear user, please click on the 'Restore Account' button below to restore your account.</td></tr><tr><td style ='font-family:Roboto Condensed, sans-serif;text-align:center;'><a href='https://localhost:44341/Account/ChangePassword?userId={user.Id}&passwordResetToken={passwordResetToken}' style ='text-decoration:none;padding:10px 30px;border-radius:3px;background-color:#8d11ff;color:black;font-weight:lighter;font-size:20px;cursor:pointer;'>Restore your account</a></td></tr></tbody></table>";
            await client.SendMailAsync(message);
            #endregion
            TempData["ForgotPassword"] = true;
            return View();
            
        }
        public async Task<IActionResult> ChangePassword(string userId, string passwordResetToken)
        {
            if (passwordResetToken == null)
            {
                return NotFound();
            }
            string replacedPasswordResetToken = passwordResetToken.Replace(" ", "+");
            if (replacedPasswordResetToken == null)
            {
                return NotFound();
            }
            User user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            ChangePasswordVM vm = new ChangePasswordVM
            {
                UserId = userId,
                Token = replacedPasswordResetToken
            };
            return View(vm);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Changepassword(ChangePasswordVM vm)
        {
            User user = await userManager.FindByIdAsync(vm.UserId);
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            IdentityResult result = await userManager.ResetPasswordAsync(user, vm.Token, vm.Password);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "The password does not meet requirements.");
                return View(vm);
            }

            await userManager.UpdateAsync(user);
            TempData["update"] = true;
            return RedirectToAction(nameof(SignIn));
        }
        [Authorize]
        public async Task<IActionResult> Invate(int? id,int? familyId)
        {
            if (id == null || familyId == null)
            {
                return View();
            }
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            Person person=db.People.FirstOrDefault(x=>x.Id==id&&x.FamilyId==familyId);
            if (person == null) { return NotFound(); }
            int familId = FamlyMethods.GetFamilyId(db, user);
            if (familId != familyId)
            {
                return NotFound();
            }
            InvateEmail invate = new InvateEmail { FamilyId = (int)familyId, PersonId = (int)id };
             return View(invate);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Invate(InvateEmail invateEmail)
        {
            if (!ModelState.IsValid)
            {
                return View(invateEmail);
            }
            User user = await userManager.FindByNameAsync(User.Identity.Name);
            User invateUser =await userManager.FindByEmailAsync(invateEmail.Email);
            if (invateUser != null)
            {
                ViewBag.Error = "User already exists";
                return View();
            }
            Person person = db.People.Include(x=>x.UserToPerson).FirstOrDefault(x => x.Id == invateEmail.PersonId && x.FamilyId == invateEmail.FamilyId);
            if (person == null) { return NotFound(); }
            if (person.UserToPerson != null)
            {
                ViewBag.Error = "User already exists";
                return View();
            }
            int familId = FamlyMethods.GetFamilyId(db, user);
            if (familId != invateEmail.FamilyId)
            {
                return NotFound();
            }
            try
            {
                PersonToken token = new PersonToken
                {
                    Date = DateTime.Now,
                    PersonId = person.Id,
                    UserId = user.Id,
                    Code = Guid.NewGuid().ToString(),
                    Email=invateEmail.Email
                };
                #region Sending Email Invate Message
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(configuration["ConnectionStrings:SmtpClientCredentialEmail"], configuration["ConnectionStrings:SmtpClientCredentialPassword"]);

                MailMessage message = new MailMessage(configuration["ConnectionStrings:SmtpClientCredentialEmail"], invateEmail.Email);
                message.IsBodyHtml = true;
                message.Subject = "Confirm invate";
                message.Body = $"<table style='width:100%;background-color:#fbfbfb;padding:50px'><thead style ='width:100%;display:flex;justify-content:center;'><tr style ='width:100%;display:flex;justify-content:center;'><th style ='width:100%;color:#7e0f9a;font-family:Roboto, sans-serif;font-weight:400;font-size:50px'>Family Tree</th></tr><thead><tbody><tr><td style ='padding:30px 0px;color:#353535;font-family:Roboto Condensed, sans-serif;font-size:20px;'> Dear user, a friend invited you to his family. Click the 'Verify İnvate' button below to verify your invate.</td></tr><tr><td style ='font-family:Roboto Condensed, sans-serif;text-align:center;'><a href='https://localhost:44341/account/confirminvate?id={person.Id}&token={token.Code}&familyId={familId}' style ='text-decoration:none;padding:10px 30px;border-radius:3px;background-color:#8d11ff;color:black;font-weight:lighter;font-size:20px;cursor:pointer;'>Confirm account</a></td></tr></tbody></table>";

                client.Send(message);

                db.PersonTokens.Add(token);
                db.SaveChanges();
                #endregion
            }
            catch
            {
                ViewBag.Error = "An error occurred";
                return View();
            }
            TempData["invate"] = true;
            return View();
        }
        [AllowAnonymous]
        public async Task<IActionResult> Confirminvate(int? id,string token,int? familyId)
        {
            if (id == null || token == null || familyId == null)
            {
                return NotFound();
            }
            PersonToken personToken = db.PersonTokens.FirstOrDefault(x => x.Code == token);
            if (personToken.Date.AddDays(1) < DateTime.Now)
            {
                return NotFound();
            }
            if (personToken == null)
            {
                return NotFound();
            }

            User user = await userManager.FindByIdAsync(personToken.UserId);
            if (user == null)
            {
                return NotFound();
            }
            ConfirmInvateVM vm = new ConfirmInvateVM
            {
                Id = (int)id,
                Token = token,
                FamilyId = (int)familyId
            };
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirminvate(ConfirmInvateVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            PersonToken personToken = db.PersonTokens.FirstOrDefault(x => x.Code == vm.Token);
            if (personToken == null)
            {
                return NotFound();
            }
            if (personToken.Date.AddDays(1) < DateTime.Now)
            {
                return NotFound();
            }
            User user = await userManager.FindByIdAsync(personToken.UserId);
            if (user == null)
            {
                return NotFound();
            }

            User invateUser = await userManager.FindByEmailAsync(personToken.Email);
            if (invateUser != null)
            {
                return NotFound();
            }
            Person person = db.People.Include(x => x.UserToPerson).FirstOrDefault(x => x.Id == vm.Id && x.FamilyId == vm.FamilyId);
            if (person == null) { return NotFound(); }
            if (person.UserToPerson != null)
            {
                return NotFound();
            }
            int familId = FamlyMethods.GetFamilyId(db, user);
            if (familId != vm.FamilyId)
            {
                return NotFound();
            }

            User newUser = new User
            {
                Firstname = person.Firstname,
                Lastname = person.LastName,
                Email = personToken.Email,
                BirthDate =person.Birthdate,
                UserName = person.Firstname.Trim() + person.LastName.Trim() + Guid.NewGuid().ToString(),
                EmailConfirmed=true,
                Avatar=person.Photo
            };
            switch (person.GenderId)
            {
                case 1:
                    newUser.GenderId = 1;
                    newUser.Avatar = "default1.jpg";
                    break;
                case 2:
                    newUser.GenderId = 2;
                    newUser.Avatar = "default2.jpg";
                    break;
                    
            }
            IdentityResult identityResult = await userManager.CreateAsync(newUser, vm.Password);
            if (!identityResult.Succeeded)
            {
                foreach (var er in identityResult.Errors)
                {

                    ModelState.AddModelError("", er.Description);
                }

                return View(vm);
            }
            await userManager.AddToRoleAsync(newUser, Utilities.SD.MemberRole);
            await userManager.UpdateAsync(newUser);
            await signInManager.SignInAsync(newUser, true);
            FamilyToUser familyToUser = new FamilyToUser { FamilyId = vm.FamilyId, UserId = newUser.Id };
            await db.FamilyToUsers.AddAsync(familyToUser);
            db.SaveChanges();
            UserToPerson userToPerson = new UserToPerson { PersonId = person.Id, UserId =newUser.Id };
            db.UserToPeople.Add(userToPerson);
            db.PersonTokens.Remove(personToken);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
          
        }

    }
}