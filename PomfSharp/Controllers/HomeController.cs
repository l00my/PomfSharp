using PomfSharp.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PomfSharp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Uploaded = false;
            ViewBag.Error = false;
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                ViewBag.Error = false;
                ViewBag.Uploaded = false;

                var maxFileSize = 50 * 100000;

                if (file.ContentLength > maxFileSize)
                {
                    ViewBag.Error = true;
                    ViewBag.ErrorMessage = $"File is too large!";
                    return View("Index");
                }

                var uploadedFilePath = @"C:\temp\UploadedFiles";
                var uploadManager = new UploadManager();
                uploadManager.UploadFile(file, uploadedFilePath);

                ViewBag.Uploaded = true;
                ViewBag.Message = $"{file.FileName}";

                return View("Index");
            }
            catch (Exception e)
            {
                ViewBag.Error = true;
                ViewBag.Uploaded = false;
                ViewBag.ErrorMessage = $"Upload failed!";
                return View("Index");
            }
        }
    }
}