using PomfSharpCdn.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PomfSharpCdn.Controllers
{
    public class HomeController : Controller
    {
        [Route("~/")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("{id}")]
        public ActionResult File(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                {
                    ViewBag.ErrorMessage = "Null, empty or whitespace used for id";
                    return View("Index");
                }

                var fileManager = new FileManager();
                var fileDto = fileManager.GetFileData(id);

                if (fileDto.Location.Equals("nope"))
                {
                    ViewBag.ErrorMessage = "Could not find file! >///<";
                    return View("Index");
                }

                if (fileDto.Type.Contains("video"))
                {
                    ViewBag.VideoType = true;
                }

                ViewBag.FileId = fileDto.FileId;
                ViewBag.FileLocation = fileDto.Location;

                return View();
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "An error occured, I'm sorry! >///< ヾ(´･ ･｀｡)ノ”";
                return View("Index");
            }
        }
    }
}