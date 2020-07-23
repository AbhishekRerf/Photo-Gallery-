
using SliderMvc.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
namespace Project.Controllers
{
    
    [Authorize]

    public class AdminController : Controller
    {
        PhotoGalleryEntities db0 = new PhotoGalleryEntities();
        PhotoGalleryEntities1 db = new PhotoGalleryEntities1();
        PhotoGalleryEntities2 db2 = new PhotoGalleryEntities2();
        // GET: Admin
        public ActionResult Index()
        {
            var data = db.Image_Admin_.ToList();
            return View(data);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Image_Admin_ image)
        {
            if(image.Categories!=null)
            {
                if (ModelState.IsValid == true)
                {
                    string filename = Path.GetFileNameWithoutExtension(image.ImageFile.FileName);
                    string extension = Path.GetExtension(image.ImageFile.FileName);
                    HttpPostedFileBase postedFile = image.ImageFile;
                    int length = postedFile.ContentLength;


                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                    {
                        if (length <= 7000000)
                        {
                            filename = filename + extension;
                            image.ImagePath = "~/Images/" + filename;
                            filename = Path.Combine(Server.MapPath("~/Images/"), filename);
                            image.ImageFile.SaveAs(filename);
                            db.Image_Admin_.Add(image);
                            
                            int a = db.SaveChanges();
                            if (a > 0)
                            {
                                TempData["CreateMessage"] = "<script>alert('Image Inserted Successfully')</script>";
                                ModelState.Clear();
                                return RedirectToAction("Create", "Admin");
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
                TempData["RequiredMessage"] = "<script>alert('Please Select Category Or Select Image')</script>";
            }
            
            return View();
        }

        public ActionResult edit( int id)
        {
            var row = db.Image_Admin_.Where(x => x.Id == id).FirstOrDefault();
            Session["Image"] = row.ImagePath;
            return View(row);
        }

        [HttpPost]
        public ActionResult edit(Image_Admin_ image)
        {
          if(ModelState.IsValid==true)
            {
                if(image.ImageFile!=null)
                {
                    string filename = Path.GetFileNameWithoutExtension(image.ImageFile.FileName);
                    string extension = Path.GetExtension(image.ImageFile.FileName);
                    HttpPostedFileBase postedFile = image.ImageFile;
                    int length = postedFile.ContentLength;


                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                    {
                        if (length <= 3000000)
                        {
                            filename = filename + extension;
                            image.ImagePath = "~/Images/" + filename;
                            filename = Path.Combine(Server.MapPath("~/Images/"), filename);
                            image.ImageFile.SaveAs(filename);
                            db.Entry(image).State = EntityState.Modified;

                            int a = db.SaveChanges();
                            if (a > 0)
                            {
                                string ImagePath = Request.MapPath(Session["Image"].ToString());
                                if (System.IO.File.Exists(ImagePath))
                                {
                                    System.IO.File.Delete(ImagePath);
                                }
                                TempData["EditMessage"] = "<script>alert('Update Successfully')</script>";
                                ModelState.Clear();
                                return RedirectToAction("Index", "Admin");
                            }
                            else
                            {
                                TempData["EditMessageFailed"] = "<script>alert('Image Inserted Failed')</script>";
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
                else
                {
                    db.Entry(image).State = EntityState.Modified;

                    int a = db.SaveChanges();
                    if (a > 0)
                    {
                        image.ImagePath = Session["Image"].ToString();
                        TempData["UpdateMessage"] = "<script>alert('Update Successfully')</script>";
                        ModelState.Clear();
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        TempData["UpdateMessage"] = "<script>alert('Image Inserted Failed')</script>";
                    }
                }
            }
            return View();
        }
        public ActionResult Delete(int id)
        {
            if(id>0)
            {
                var data = db.Image_Admin_.Where(x => x.Id == id).FirstOrDefault();
                if(data!=null)
                {
                    db.Entry(data).State = EntityState.Deleted;
                    int a=db.SaveChanges();
                    if (a > 0)
                    {
                        TempData["DeleteMessage"]= "<script>alert('Image Deleted Successfully')</script>";
                        string ImagePath = Request.MapPath(data.ImagePath.ToString());
                        if(System.IO.File.Exists(ImagePath))
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
            return RedirectToAction("Index","Admin");
        }
        public ActionResult ManageContestImage()
        {
            var data = db2.ContestImages.ToList();
            return View(data);
        }
        [HttpPost]
        public ActionResult SearchUserDetails(int id)
        {
            var data = db0.UserDetails.Where(x => x.id == id).ToList();
            return View(data);
        }

        public ActionResult Gmail()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Gmail(gmail gm)
        {
            MailMessage mm = new MailMessage(gm.From, gm.To);
            mm.Subject = gm.Subject;
            mm.Body = gm.Body;
            mm.IsBodyHtml = false;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            NetworkCredential nc = new NetworkCredential("abhishaw051@gmail.com", "Redminote8");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = nc;
            smtp.Send(mm);
            ViewBag.Message = "<script>alert('Mail Has Been Sent Sucessfully')</script>";
            ModelState.Clear();
            return View(); ;
            
        }





    }
}