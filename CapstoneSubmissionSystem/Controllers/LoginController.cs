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

            if (Session["UserID"]!=null)
            {

                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(User um)
        {
    
            var loggedUser = new User();
            loggedUser = DBEntities1.Users.Where(u => u.Username.Equals(um.Username) && u.Password.Equals(um.Password)).FirstOrDefault();
            //Get logged in user
            //if (Session["UserID"] != null)
            //{
            //    loggedUser=DBEntities1.Users.Where(u=>u.UserID ==  Convert.ToInt32(Session["UserID"].ToString())).FirstOrDefault();
            //}

            if (loggedUser != null && loggedUser.UserID != 0)
            {
                Session["UserID"] = loggedUser.UserID.ToString();
                return RedirectToAction("Index", "Home");
                //return View(loggedUser);
            }
            else
            {
                ModelState.AddModelError("", "Username or password is incorrect.");
                //return RedirectToAction("Index", "Login");
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }





    }
}