using CustomVisionTFJS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace CustomVisionTFJS.Controllers
{
    public class AppController : Controller
    {
        private readonly ILogger<AppController> _logger;

        public AppController(ILogger<AppController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (System.IO.File.Exists("wwwroot/models/model.json")) {

                return RedirectToAction("camera");
            }


            return RedirectToAction("upload");
        }

        public IActionResult Camera()
        {
            if (!System.IO.File.Exists("wwwroot/models/model.json"))
            {
                return RedirectToAction("upload");
            }
            return View();
        }

        public IActionResult Upload()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult UploadModel(IFormFile image)
        {
            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/models");

            if (image != null && Path.GetExtension(image.FileName)==".zip")
            {
                //Set Key Name
                string ImageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                string SavePath = Path.Combine(basePath, ImageName);

                if (Directory.Exists(basePath)) {
                    Directory.Delete(basePath, true);
                }
                
                Directory.CreateDirectory(basePath);

                using (var stream = new FileStream(SavePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
                ZipFile.ExtractToDirectory(SavePath, basePath);

                return RedirectToAction("camera");
            }

            // Clean up the uploaded files if anything went wrong
            if (Directory.Exists(basePath))
            {
                Directory.Delete(basePath, true);
            }
            return View();
        }

    }
}
