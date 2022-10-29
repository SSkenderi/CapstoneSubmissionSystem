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
                doctypes = DBEntities1.DocTypes.Where(d=>d.Visible==1).ToList();
                documents = DBEntities1.Documents.Where(d=>d.OwnerID ==loggedUser.UserID).ToList();
                homedata.LoggedUser = loggedUser;
                homedata.Students = students;
                homedata.DocTypes = doctypes;

                if (loggedUser.ISAdmin == 1)
                {


                    return View(homedata);
                    //ketu kthehet direkt faqja se adminit si duhen as skedartet as fileuploadet pasi do hape faqet e tij per ti
                    //menaxhuar keto te dhena
                }
                else if (loggedUser.SupervisorID == null)
                {
                    //ketu jane fakultetet sepse ato nuk kane nje id supervizori

                    //per neser ketu duhet te besh queryn qe kap te gjithe studentet e nje fakulteti dhe i shton te lista
                    //students
                    students = DBEntities1.Users.Where((s => s.SupervisorID != null || s.SupervisorID == 0)).ToList();
                }
                else
                {
                    //ketu mbeten studentet

                    //per neser ketu duhet te besh queryn qe kap te gjithe doctypes qe te visible kane vleren 1 dhe i shton 
                    //te lista doctypes
                    doctypes = DBEntities1.DocTypes.Where(s => s.Visible == 1).ToList();


                    //students = DBEntities1.Users.Where(s => s.ISAdmin == 0).ToList();

                    //nje shembull me siper me linq ku te where fut kushtet qe do
                    //var sql = $@"SELECT    * from students";
                    //var data = DBEntities1.Database.Connection.Query<User>(sql).ToList();
                    //nje shembull me sql query





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
                return RedirectToAction("Index", "Login");
            }




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

        [HttpPost]
        public ActionResult AddUser(int uType, int admn, string fName, string lName, string password, string email, string uName, int pickedSupervisorID)
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
            addUser.FacultyID = pickedSupervisorID;
            

            DBEntities1.Users.Add(addUser);
            DBEntities1.SaveChanges();

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue,
                Data = "SUKSES"
            };
        }



        [HttpPost]
        public ActionResult AddFileUpload(string fileUploadName)
        {
            DocType addFU = new DocType();


            //addUser.FileName = files.FileName.Substring(0, index);

            //addFU.TypeID= fUploadID;
            addFU.TypeName = fileUploadName;
           


            DBEntities1.DocTypes.Add(addFU);
            DBEntities1.SaveChanges();

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue,
                Data = "SUKSES"
            };
        }





        public ActionResult DeleteUser(int userID)
        {
            var deletedUser = DBEntities1.Users.Where(u => u.UserID == userID /*per tu pare prape*/).FirstOrDefault();
            DBEntities1.Users.Remove(deletedUser);
            DBEntities1.SaveChanges();
            return Content("success");
        }

        public ActionResult HideFileTypes(int doctypeID, int LoggedUserID)
        {
            var DocTypesToHide = DBEntities1.DocTypes.Where(f => f.TypeID == doctypeID /*per tu pare prape*/).ToList();
            foreach(var dt in DocTypesToHide)
            {
                dt.Visible = 0;
                DBEntities1.SaveChanges();
            }
      
            return Content("success");
        }


        //[HttpPost]
        //public async Task<ActionResult> SubmitDok(string typeID, string docContent, string docName)
        //{
        //    //SKERDIII 21.10
        //    //ketu do behet inserti ne db i dok

        //    //KAP ID E USERIT TE LOGUAR PER TE LIDHUR DOK ME USERIN 
        //    // TE DHENAT E TJERA TE VIJNE SI PARAMETER NGA AJAXI
        //    //NQS TE DUHEN AKOMA ME SH TE DHENA SHTOJI TE AJO PJESA DATA: NE AJAX POR EMRI ATJE DUHET I NJEJTE ME PARAMETRIN KETU

        //    //Fillimisht kerkon per kete user dhe per kete typeid nqs ka nje dok nuk ben insert por update ate qe eshte

        //    //Nese nuk ka nje dok  me kete type id per kete user e ben insert

        //    //NESE BEHET SHTIMI/UPDATE ME SUKSES KTHEN SUKSES
        //    //NE TE KUNDERT BEN NJE RETURN TJETER FALSE OSE SI TE DUASH.



        //    return new JsonResult
        //    {
        //        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
        //        MaxJsonLength = int.MaxValue,
        //        Data = "SUKSES"
        //    };
        //}


        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(file.FileName);
                    string _path = Path.Combine(Server.MapPath("~/FileRepo"), _FileName);
                    file.SaveAs(_path);
                }
                ViewBag.Message = "File Uploaded Successfully.";
                return View();
            }
            catch
            {
                ViewBag.Message = "File upload failed.";
                return View();
            }
        }



        [HttpPost]
        public ActionResult Shto()
        {

           var files=  Request.Files["FileUpload"];
           var userIDStr=  Request.Params["userID"].ToString();
           var typeID =  Request.Params["typeID"].ToString(); 


            //check if doc exists for this user

             byte[] bytes;
            int userID = Convert.ToInt32(userIDStr);
            int DOCtypeID = Convert.ToInt32(typeID);

            //get document for this user if it exists
            var docexists = DBEntities1.Documents.Where(d => d.TypeID == DOCtypeID && d.OwnerID == userID).FirstOrDefault();

            if(docexists != null && docexists.FileContent !=null )
            {

                using (Stream fs = files.InputStream)
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        bytes = br.ReadBytes((Int32)fs.Length);
                    }
                   
                    int index = files.FileName.LastIndexOf(".");
                    string filenm = files.FileName.ToString();


                    docexists.FileName = files.FileName.Substring(0, index);

                    docexists.Extension = files.FileName.Substring(index + 1, 3);
                    docexists.FileContent = bytes;

                    DBEntities1.SaveChanges();


                }
            }
            else {


                using (Stream fs = files.InputStream)
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        bytes = br.ReadBytes((Int32)fs.Length);
                    }
                    Document dok1 = new Document();
                    DateTime rn = DateTime.Now;
                    int index = files.FileName.LastIndexOf(".");
                    string filenm = files.FileName.ToString();


                    dok1.FileName = files.FileName.Substring(0, index);

                    dok1.TypeID = DOCtypeID;
                    dok1.Extension = files.FileName.Substring(index + 1, 3);
                    dok1.OwnerID = userID;
                    dok1.FileContent = bytes;
                    dok1.Visibility = 0;

                    DBEntities1.Documents.Add(dok1);
                    DBEntities1.SaveChanges();

                }
            }
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue,
                Data = "SUKSES"
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

            documents = DBEntities1.Documents.Where(d => d.OwnerID == StudentID).ToList();

            StudentDocumentsDataViewModel studentDocumentsDataViewModel = new StudentDocumentsDataViewModel();
            studentDocumentsDataViewModel.Documents = documents;
            studentDocumentsDataViewModel.StudentUser = student;
            string stname = student.FirstName + " " + student.LastName; 
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue,
                Data = new {
                    docnames = studentDocumentsDataViewModel.Documents.Select(d => d.FileName).ToArray(),
                    docids = studentDocumentsDataViewModel.Documents.Select(d => d.DocumentID).ToArray(),
                    doctypes = studentDocumentsDataViewModel.Documents.Select(d => d.DocType.TypeName).ToArray(),
                    studentname = stname,
                    success = "success"
                }
            };






        }




    }
}