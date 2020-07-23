using SliderMvc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SliderMvc.Controllers
{
    public class HomeController : Controller
    {


        public int AdminValue = 1;
        public int userValue = 0;


        PhotoGalleryEntities db = new PhotoGalleryEntities();
        PhotoGalleryEntities1 db1 = new PhotoGalleryEntities1();
        PhotoGalleryEntities2 db2 = new PhotoGalleryEntities2();
        PhotoGalleryEntities5 db5 = new PhotoGalleryEntities5();
        public ActionResult Index()
        {
            using (OurDbContext db = new OurDbContext())
            {
                return View(db.gallery.ToList());
            }
            //return View();
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

        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(UserDetail userDetail)
        {
            if (ModelState.IsValid == true)
            {
                db.UserDetails.Add(userDetail);
                int a = db.SaveChanges();
                if (a > 0)
                {
                    ViewBag.InsertMessage = "<script>alert('Registration successful !!')</script>";
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.InsertMessage = "<script>alert('Registration Failed !!')</script>";
                }
            }
            return View();
        }
        public ActionResult login()
        {
            return View();
        }
        [HttpPost]

        public ActionResult login(UserDetail userDetail)
        {
            var data = db.UserDetails.Where(x => x.email_id == userDetail.email_id && x.password == userDetail.password).FirstOrDefault();
            {
                if (data != null)
                {
                    UserIdValue userId = new UserIdValue()
                    {
                        UserID = data.id.ToString()
                    };
                    if (data.email_id == "Abhishaw051@gmail.com" || data.email_id == "abhishaw051@gmail.com")
                    {
                        TempData["Flag"] = AdminValue;
                        FormsAuthentication.SetAuthCookie(data.id.ToString(), false);
                        Session["UserId"] = data.id.ToString();
                        Session["email_id"] = data.email_id.ToString();
                        Session["User_Name"] = data.User_Name.ToString();
                        ViewBag.email = data.email_id.ToString();

                        return RedirectToAction("AdminDashBoard");

                    }
                    else
                    {
                        TempData["Flag"] = userValue;
                        FormsAuthentication.SetAuthCookie(data.id.ToString(), false);
                        Session["UserId"] = data.id.ToString();
                        ViewBag.UserID = userId;
                        Session["User_Name"] = data.User_Name.ToString();
                        TempData["SuccessMessage"] = "<script>alert('Log In Successfully')</script>";
                        return RedirectToAction("Dashboard", "Home");

                    }

                }
                else
                {
                    ViewBag.ErrorMessage = "<script>alert('Invalid User Id or Password')</script>";
                    return View();
                }
            }
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "Home");
        }
        [Authorize]
        public ActionResult Dashboard()
        {
            return View();
        }
        [Authorize]
        public ActionResult UserUploadedContestImages()
        {
            UserIdValue userId = new UserIdValue();

            string temp = Session["UserId"].ToString();
            var data = db2.ContestImages.Where(x => x.userid == temp).ToList();
            //var data = db2.ContestImages.ToList();
            return View(data);
        }


        public ActionResult Image(string category)
        {
            if (category != null)
            {
                var data = db1.Image_Admin_.Where(x => x.Categories == category).ToList();
                ViewBag.ImageDetails = category;
                return View(data);
            }
            else
            {
                return View();
            }


        }

        public ActionResult Wallpapers(string category)
        {
            if (category != null)
            {
                var data = db1.Image_Admin_.Where(x => x.Categories == category).ToList();
                ViewBag.ImageDetails = category;
                return View(data);
            }
            else
            {
                return View();
            }


        }


        [Authorize]
        public ActionResult AdminDashBoard()
        {
            int temp = Convert.ToInt32(TempData["Flag"]);

            if (temp == 1)
            {
                return View();
            }

            else
            {

                ViewBag.Logout = "<script>alert('Please Log In Again')</script>";
                return RedirectToAction("Logout");


            }

        }
        public ActionResult Contests()
        {
            return View();
        }
        [HttpPost]

        public ActionResult ParticipateinContest()
        {
            if (Session["UserId"] == null)
            {
                ViewBag.Contest = "<script>alert('Please Log In First')</script>";
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
        }


        [HttpPost]
        public ActionResult Upload(ContestImage contest2)
        {
            ContestImage contest = new ContestImage()
            {
                userid = Session["UserId"].ToString(),
                ImagePath = contest2.ImagePath,
                ImageFile = contest2.ImageFile
            };
            string filename = Path.GetFileNameWithoutExtension(contest.ImageFile.FileName);
            string extension = Path.GetExtension(contest.ImageFile.FileName);
            HttpPostedFileBase postedFile = contest.ImageFile;
            int length = postedFile.ContentLength;
            if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
            {
                if (length <= 3000000)
                {

                    filename = filename + extension;
                    contest.ImagePath = "~/ContestImages/" + filename;
                    filename = Path.Combine(Server.MapPath("~/ContestImages/"), filename);
                    contest.ImageFile.SaveAs(filename);
                    db2.ContestImages.Add(contest);
                    int a = db2.SaveChanges();
                    if (a > 0)
                    {
                        TempData["CreateMessage"] = "<script>alert('Image Inserted Successfully')</script>";
                        ModelState.Clear();
                        return View("~/Views/Home/ParticipateinContest.cshtml");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "<script>alert('Image Inserted Failed')</script>";
                    }
                }
                else
                {
                    TempData["SizeMessage"] = "<script>alert('Image Size Must Be Less Than 3MB')</script>";
                }
            }
            else
            {
                TempData["ExtensionMessage"] = "<script>alert('Format Not Supported')</script>";
            }
            return View();
        }

        public ActionResult EditImage(int id)
        {

            if (id > 0)
            {
                var data = db2.ContestImages.Where(x => x.id == id).FirstOrDefault();
                if (data != null)
                {
                    db2.Entry(data).State = EntityState.Deleted;
                    int a = db2.SaveChanges();
                    if (a > 0)
                    {
                        TempData["DeleteMessage"] = "<script>alert('Image Deleted Successfully')</script>";
                        string ImagePath = Request.MapPath(data.ImagePath.ToString());
                        if (System.IO.File.Exists(ImagePath))
                        {
                            System.IO.File.Delete(ImagePath);
                        }

                    }
                    else
                    {
                        TempData["DeleteMessage"] = "<script>alert('Image Deleted Failed')</script>";
                    }
                }
            }
            return RedirectToAction("UserUploadedContestImages", "Home");
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(UserDetail userDetail)
        {
            if (userDetail.email_id != null && userDetail.password != null && userDetail.confirmpassword != null)
            {
                var data = db.UserDetails.Where(x => x.email_id == userDetail.email_id).FirstOrDefault();
                if (data != null)
                {

                    data.password = userDetail.password;
                    data.confirmpassword = userDetail.confirmpassword;
                    db.Entry(data).State = EntityState.Modified;
                    int a = db.SaveChanges();
                    if (a > 0)
                    {
                        TempData["UpdateMessage"] = "<script>alert('Password Updated Successfully')</script>";
                        ModelState.Clear();
                    }
                    else
                    {
                        TempData["UpdateMessageFailed"] = "<script>alert('Password Updation Failed')</script>";
                    }
                }
                else
                {
                    TempData["UpdateMessageFailed"] = "<script>alert('Email Id And Password Required')</script>";
                }
            }
            else
            {
                TempData["UpdateMessageFailed"] = "<script>alert('All Fields Required')</script>";

            }

            return RedirectToAction("Login", "Home");
        }
        [Authorize]
        public ActionResult UploadYourImage()
        {
            return View();
        }


        public ActionResult Community()
        {
            var data = db5.ImageByUsers.ToList();
            return View(data);
        }
        [HttpPost]
        public ActionResult UploadYourImage(ImageByUser imageBy)
        {

            ImageByUser PictureByUser = new ImageByUser()
            {
                userid = Session["UserId"].ToString(),
                user_name = Session["User_Name"].ToString(),
                ImagePath = imageBy.ImagePath,
                ImageFile = imageBy.ImageFile
            };
            if (PictureByUser.userid != null)
            {
                if (ModelState.IsValid == true)
                {
                    string File = Path.GetFileNameWithoutExtension(PictureByUser.ImageFile.FileName);
                    string extension = Path.GetExtension(PictureByUser.ImageFile.FileName);
                    HttpPostedFileBase fileBase = PictureByUser.ImageFile;
                    int length = fileBase.ContentLength;
                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                    {
                        if (length <= 7000000)
                        {
                            File = File + extension;
                            PictureByUser.ImagePath = "~/ImageByUser/" + File;
                            File = Path.Combine(Server.MapPath("~/ImageByUser/") + File);
                            PictureByUser.ImageFile.SaveAs(File);
                            db5.ImageByUsers.Add(PictureByUser);
                            int a = db5.SaveChanges();
                            if (a > 0)
                            {
                                TempData["CreateMessage"] = "<script>alert('Image Inserted Successfully')</script>";
                                ModelState.Clear();
                                return RedirectToAction("UploadYourImage", "Home");
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "<script>alert('Image Inserted Failed')</script>";
                            }
                        }
                        else
                        {
                            TempData["SizeMessage"] = "<script>alert('Image Size Must Be Less Than 3MB')</script>";
                        }
                    }
                    else
                    {
                        TempData["ExtensionMessage"] = "<script>alert('Format Not Supported')</script>";
                    }

                }
            }
            else
            {
                TempData["requiredmessage"] = "<script>alert('Please Login First')</script>";
            }


            return View();
        }
        [Authorize]
        public ActionResult UserCommunityImage()
        {
            string userid = Session["UserId"].ToString();
            var data = db5.ImageByUsers.Where(x => x.userid == userid).ToList();
            return View(data);
        }
        public ActionResult Privacypolicy()
        {
            return View();
        }
        
        public ActionResult ContestImages()
        {
            var data = db2.ContestImages.ToList();
            return View(data);
        }
    }

}
             
            
            

          





