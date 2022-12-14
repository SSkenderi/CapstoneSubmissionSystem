using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapstoneSubmissionSystem.Models;

namespace CapstoneSubmissionSystem.Controllers
{
    public class LoginController : Controller
    {
        CapstoneSubmissionSystemEntities DBEntities1 = new CapstoneSubmissionSystemEntities();


        public ActionResult Index()
        {

            if (Session["UserID"] != null)
            {

                return RedirectToAction("Index", "Home", new { area = "" });
            }

            return View();
        }


        public ActionResult Logout()
        {
            Session["UserID"] = null;
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        [HttpPost]
        public ActionResult Index(User um)
        {

            var loggedUser = new User();
            loggedUser = DBEntities1.Users.Where(u => u.Username.Equals(um.Username) && u.Password.Equals(um.Password)).FirstOrDefault();


            if (loggedUser != null && loggedUser.UserID != 0)
            {
                Session["UserID"] = loggedUser.UserID.ToString();
                return RedirectToAction("Index", "Home", new { area = "" });
                //return View(loggedUser);
            }
            else
            {
                ModelState.AddModelError("", "Username or password is incorrect.");
            }

            return View();
        }
    }
}