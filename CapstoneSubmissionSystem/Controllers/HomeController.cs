using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapstoneSubmissionSystem.Models;
using CapstoneSubmissionSystem.ViewModels;
using System.Data;
using Dapper;
using System.Dynamic;
using System.Threading.Tasks;
using System.IO;

namespace CapstoneSubmissionSystem.Controllers
{
    public class HomeController : Controller
    {
        CapstoneSubmissionSystemEntities DBEntities1 = new CapstoneSubmissionSystemEntities();


        public ActionResult Index()
        {

            HomePageDataViewModel homedata = new HomePageDataViewModel();
            var loggedUser = new User();
            var students = new List<User>();
            var doctypes = new List<DocType>();
            var documents = new List<Document>();


            //Get logged in user
            if (Session["UserID"] != null)
            {
                var iduseri = Convert.ToInt32(Session["UserID"].ToString());
                loggedUser = DBEntities1.Users.Where(u => u.UserID == iduseri).FirstOrDefault();
            }



            //per te provuar role te ndryshme vendos id 3 per student, id 4 per fakultet dhe 7 per admin
            //loggedUser = DBEntities1.Users.Where(u => u.UserID == 4).FirstOrDefault();

            if (loggedUser != null && loggedUser.UserID != 0)
            {

                students = DBEntities1.Users.ToList();
                doctypes = DBEntities1.DocTypes.Where(d => d.Visible == 1).ToList();
                documents = DBEntities1.Documents.Where(d => d.OwnerID == loggedUser.UserID).ToList();
                homedata.LoggedUser = loggedUser;
                homedata.Students = students;
                homedata.DocTypes = doctypes;

                if (loggedUser.ISAdmin == 1)
                {
                    doctypes = DBEntities1.DocTypes.ToList();

                    homedata.DocTypes = doctypes;

                    return View(homedata);
                    //ketu kthehet direkt faqja se adminit si duhen as skedartet as fileuploadet pasi do hape faqet e tij per ti
                    //menaxhuar keto te dhena
                }
                else if (loggedUser.SupervisorID == null)
                {
                    //ketu jane fakultetet sepse ato nuk kane nje id supervizori

                    var students2 = DBEntities1.Users.Where((s => s.SupervisorID != null && s.SupervisorID == loggedUser.UserID)).ToList();
                    //studentet qe kane supervizor userin e loguar

                    var facultyvisibledocsowners = DBEntities1.Documents.Where(d => d.Visibility == 1).Select(d => d.OwnerID).ToList();
                    //userat, dokumentat e te cileve jane bere visible nga fakulteti

                    var students1 = DBEntities1.Users.Where(u => facultyvisibledocsowners.Any(a => a == u.UserID)).ToList();
                    //studentet qe kane dok te bera visible

                    students = students1.Union(students2).ToList();

                }
                else
                {

                    doctypes = DBEntities1.DocTypes.Where(s => s.Visible == 1).ToList();

                }

                //pasi ke mbushur me siper listat qe duheshin i ruan ato te variabli i viewmodel qe do kthesh te view-ja

                homedata.Students = students;
                homedata.DocTypes = doctypes;
                homedata.Documents = documents;



                //end te dhena test



                //if (loggedUser != null && loggedUser.UserID != 0)
                //{
                //    return View(homedata);
                //}
                //else
                //{
                //    return RedirectToAction("Index", "Login");
                //}

                return View(homedata);
            }
            else
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

        }


        [HttpPost]
        public ActionResult AddUser(int uType, int admn, string fName, string lName, string password, string email, string uName, int? pickedSupervisorID)
        {
            User addUser = new User();


            //addUser.FileName = files.FileName.Substring(0, index);

            addUser.FacultyID = uType;
            addUser.FirstName = fName;
            addUser.LastName = lName;
            addUser.Username = uName;
            addUser.ISAdmin = (byte)admn;
            addUser.Password = password;
            addUser.Email = email;

            
            if (pickedSupervisorID != -1)
            {
                addUser.SupervisorID = pickedSupervisorID;
                //var faculty = DBEntities1.Users.Where(u => u.UserID == pickedSupervisorID).FirstOrDefault().FacultyID;
                //addUser.SupervisorID = faculty;
            }

            else if(pickedSupervisorID == -1)
            {
                addUser.FacultyID = 1;
            }
            DBEntities1.Users.Add(addUser);
            DBEntities1.SaveChanges();

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue,
                Data = "Success"
            };
        }



        [HttpPost]
        public ActionResult AddFileUpload(string fileUploadName)
        {
            DocType addFU = new DocType();


            //addUser.FileName = files.FileName.Substring(0, index);

            //addFU.TypeID= fUploadID;
            addFU.TypeName = fileUploadName;
            addFU.Visible = 1;


            DBEntities1.DocTypes.Add(addFU);
            DBEntities1.SaveChanges();

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue,
                Data = "Success"
            };
        }

        [HttpPost]
        public ActionResult ToggleVisibility(string iddocu, string visbilitynr)
        {
            int intiddoc = 0;
            if (iddocu != null && iddocu != "")
            {

                intiddoc = Convert.ToInt32(iddocu);
            }
            var thisdoc = DBEntities1.Documents.Where(d => d.DocumentID == intiddoc).FirstOrDefault();
            if (visbilitynr == "0")
                thisdoc.Visibility = 1;
            else if (visbilitynr == "1")
                thisdoc.Visibility = 0;
            DBEntities1.SaveChanges();

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue,
                Data = "Success"
            };
        }
        [HttpPost]
        public ActionResult ToggleVisibilityUpload(string doctypeid)
        {
            int intiddoc = 0;
            if (doctypeid != null && doctypeid != "")
            {

                intiddoc = Convert.ToInt32(doctypeid);
            }
            var thisdoc = DBEntities1.DocTypes.Where(d => d.TypeID == intiddoc).FirstOrDefault();
            if (thisdoc.Visible == 0)
                thisdoc.Visible = 1;
            else if (thisdoc.Visible == 1)
                thisdoc.Visible = 0;
            DBEntities1.SaveChanges();

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue,
                Data = "Success"
            };
        }

        public ActionResult DeleteUser(string userids)
        {
            var arrids = userids.Split('-').ToArray();
            try
            {


                foreach (var iduser in arrids)
                {
                    if (iduser != "")
                    {
                        var idint = Convert.ToInt32(iduser);
                        var deletedUser = DBEntities1.Users.Where(u => u.UserID == idint).FirstOrDefault();
                        DBEntities1.Documents.RemoveRange(DBEntities1.Documents.Where(D => D.OwnerID == deletedUser.UserID).ToList());
                        DBEntities1.Users.Remove(deletedUser);
                        DBEntities1.SaveChanges();
                    }
                }
                return new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = int.MaxValue,
                    Data = "success"
                };
            }
            catch (Exception ex)
            {
                return new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = int.MaxValue,
                    Data = "failure"
                };
            }
        }

        [HttpPost]
        public ActionResult Shto()
        {

            var files = Request.Files["FileUpload"];
            var userIDStr = Request.Params["userID"].ToString();
            var typeID = Request.Params["typeID"].ToString();
            string _FileName = "";
            string _path = "";
            if (files.ContentLength > 0)
            {
                _FileName = Path.GetFileName(files.FileName).Replace('`', ' ').Replace('\'', ' ');


                if (Directory.Exists(Server.MapPath("~/FileRepo/" + userIDStr + "-" + typeID + "")))
                {
                    _path = Path.Combine(Server.MapPath("~/FileRepo/" + userIDStr + "-" + typeID + ""), _FileName);

                    //The code will execute if the folder exists
                }
                else
                {
                    //The below code will create a folder if the folder does not exist as prt of the solution.            
                    DirectoryInfo folder = Directory.CreateDirectory(Server.MapPath("~/FileRepo/" + userIDStr + "-" + typeID + ""));
                    _path = Path.Combine(Server.MapPath("~/FileRepo/" + userIDStr + "-" + typeID + ""), _FileName);

                }

                files.SaveAs(_path);
            }

            //check if doc exists for this user
            int userID = Convert.ToInt32(userIDStr);
            int DOCtypeID = Convert.ToInt32(typeID);

            //get document for this user if it exists
            var docexists = DBEntities1.Documents.Where(d => d.TypeID == DOCtypeID && d.OwnerID == userID).FirstOrDefault();

            if (docexists != null)
            {
                docexists.FileName = _FileName;
                docexists.Extension = _FileName.Substring(_FileName.LastIndexOf(".") + 1, _FileName.Length - 1 - _FileName.LastIndexOf("."));
                docexists.FilePath = _path;
                DBEntities1.SaveChanges();
            }

            else
            {
                Document dok1 = new Document();
                DateTime rn = DateTime.Now;
                int index = files.FileName.LastIndexOf(".");
                string filenm = files.FileName.ToString().Replace('`', ' ').Replace('\'', ' ');
                dok1.FileName = filenm;
                dok1.TypeID = DOCtypeID;
                dok1.Extension = _FileName.Substring(_FileName.LastIndexOf(".") + 1, _FileName.Length - 1 - _FileName.LastIndexOf("."));
                dok1.OwnerID = userID;
                dok1.FilePath = _path;
                dok1.Visibility = 0;
                DBEntities1.Documents.Add(dok1);
                DBEntities1.SaveChanges();
            }
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue,
                Data = "Success"
            };

        }


        [HttpPost]
        public ActionResult StudentFolder(int StudentID)
        {

            var student = new User();
            var students = new List<User>();
            var documents = new List<Document>();


            //MARRIM DOK E USERIT

            student = DBEntities1.Users.Where(s => s.UserID == StudentID).FirstOrDefault();

            int iduseri = 0;


            if (Session["UserID"] != null)
            {
                iduseri = Convert.ToInt32(Session["UserID"].ToString());
            }


            //marrim te gjithe dok nqs studenti eshte i fakultetit te loguar

            if (student.SupervisorID == iduseri)
            {
                documents = DBEntities1.Documents.Where(d => d.OwnerID == StudentID).ToList();
            }
            else //folderi i hapur nuk eshte student i userit te loguar por i jane bere doc visible
            {
                documents = DBEntities1.Documents.Where(d => d.OwnerID == StudentID && d.Visibility == 1).ToList();
            }
            StudentDocumentsDataViewModel studentDocumentsDataViewModel = new StudentDocumentsDataViewModel();
            studentDocumentsDataViewModel.Documents = documents;
            studentDocumentsDataViewModel.StudentUser = student;
            string stname = student.FirstName + " " + student.LastName;
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue,
                Data = new
                {
                    docnames = studentDocumentsDataViewModel.Documents.Select(d => d.FileName).ToArray(),
                    docids = studentDocumentsDataViewModel.Documents.Select(d => d.DocumentID).ToArray(),
                    doctypes = studentDocumentsDataViewModel.Documents.Select(d => d.DocType.TypeName).ToArray(),
                    docvisibility = studentDocumentsDataViewModel.Documents.Select(d => d.Visibility).ToArray(),
                    docpaths = studentDocumentsDataViewModel.Documents.Select(d => d.FilePath.Substring(d.FilePath.LastIndexOf("CapstoneSubmissionSystem") + 24)).ToArray(),
                    studentname = stname,
                    success = "success"
                }
            };
        }

    }
}