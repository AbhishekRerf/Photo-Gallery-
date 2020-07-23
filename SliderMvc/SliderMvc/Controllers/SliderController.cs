using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SliderMvc.Models;

namespace SliderMvc.Controllers
{
    public class SliderController : Controller
    {
        // GET: Slider
        public ActionResult Index()
        {
            using (OurDbContext db = new OurDbContext())
            {
                return View(db.gallery.ToList());
            }
            //return View();
        }

        //Add Images in slider

        public ActionResult AddImage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddImage(HttpPostedFileBase ImagePath)
        {
            if (ImagePath != null)
            {
                // You can skip this block, because it is only to force the user to upload specific resolution pics
               

                // Upload your pic
                string pic = System.IO.Path.GetFileName(ImagePath.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/Content/images"), pic);
                ImagePath.SaveAs(path);
                using (OurDbContext db = new OurDbContext())
                {
                    Gallery gallery = new Gallery { ImagePath = "~/Content/images/" + pic };
                    db.gallery.Add(gallery);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        // Delete Multiple Images
        public ActionResult DeleteImages()
        {
            using (OurDbContext db = new OurDbContext())
            {
                return View(db.gallery.ToList());
            }
        }

        [HttpPost]
        public ActionResult DeleteImages(IEnumerable<int> ImagesIDs)
        {
            using (OurDbContext db = new OurDbContext())
            {
                foreach (var id in ImagesIDs)
                {
                    var image = db.gallery.Single(s => s.ID == id);
                    string imgPath = Server.MapPath(image.ImagePath);
                    db.gallery.Remove(image);
                    if (System.IO.File.Exists(imgPath))
                        System.IO.File.Delete(imgPath);
                }
                db.SaveChanges();
            }
            return RedirectToAction("DeleteImages");
        }
    }
}