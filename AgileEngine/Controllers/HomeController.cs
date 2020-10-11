using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AgileEngine.Models;
using AgileEngineBEServices;
using AgileEngine_BE.Extensions;
using AgileEngineBEServices.DTOs;
using AgileEngine_BE.Models;
using Microsoft.Extensions.Options;

namespace AgileEngine.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private PhotosServices _photosServices;
        private readonly IOptions<Keys> _appKeySettings;

        public HomeController(ILogger<HomeController> logger, PhotosServices photosServices, IOptions<Keys> _appKeySettings)
        {
            _logger = logger;
            this._photosServices = photosServices;
            this._appKeySettings = _appKeySettings;
        }

        public IActionResult Index()
        {
            this.SetToken();
            return View();
        }

        [HttpGet]
        public IActionResult Images()
        {
            var images = HttpContext.Session.GetObject<List<PictureDto>>("1");
            try
            {
                images = images == null ? this._photosServices.GetImages(1, HttpContext.Session.GetObject<string>("Token")) : images;
            }
            catch(Exception e)
            {
                this.SetToken();
                images = images == null ? this._photosServices.GetImages(1, HttpContext.Session.GetObject<string>("Token")) : images;

            }
            

            HttpContext.Session.SetObject("1", images);

            return View(new ImagesViewModel() { PicturesList = images });
        }

        [HttpGet]
        public IActionResult GetImages(int page)
        {
            var images = HttpContext.Session.GetObject<List<PictureDto>>(page.ToString());
            try
            {
                images = images == null ? this._photosServices.GetImages(page, HttpContext.Session.GetObject<string>("Token")) : images;
            }
            catch(Exception e)
            {
                this.SetToken();
                images = images == null ? this._photosServices.GetImages(page, HttpContext.Session.GetObject<string>("Token")) : images;
            }

            HttpContext.Session.SetObject(page.ToString(), images);

            return PartialView("_TableImages", images);
        }

        [HttpGet]
        public IActionResult ImageDetails(string id)
        {

            var pictureDetailsList = HttpContext.Session.GetObject<List<PictureDetailsDto>>("PictureDetailsList");
            PictureDetailsDto imageDetails;
            if (pictureDetailsList != null && pictureDetailsList.Any(p => p.id == id))
                imageDetails = pictureDetailsList.Single(p => p.id == id);

            else
            {
                try
                {
                    pictureDetailsList = new List<PictureDetailsDto>();
                    imageDetails = this._photosServices.GetImageDetail(id, HttpContext.Session.GetObject<string>("Token"));
                }
                catch(Exception e)
                {
                    this.SetToken();
                    pictureDetailsList = new List<PictureDetailsDto>();
                    imageDetails = this._photosServices.GetImageDetail(id, HttpContext.Session.GetObject<string>("Token"));
                }
            }

            pictureDetailsList.Add(imageDetails);

            HttpContext.Session.SetObject("PictureDetailsList", pictureDetailsList);

            return PartialView("ImageDetails", imageDetails);
        }

        [HttpGet]
        public IActionResult Search(ImagesViewModel model)
        {
            var pictureDetailsList = HttpContext.Session.GetObject<List<PictureDetailsDto>>("PictureDetailsList");

            if (pictureDetailsList == null)
                return PartialView("_TableImages", new List<PictureDto>());
            else
            {
                var result = pictureDetailsList.Where(i => (String.IsNullOrWhiteSpace(model.Search.cropped_picture) ? true : i.cropped_picture == model.Search.cropped_picture)
                                                            && (String.IsNullOrWhiteSpace(model.Search.author) ? true : i.author == model.Search.author)
                                                            && (String.IsNullOrWhiteSpace(model.Search.camera) ? true : i.camera == model.Search.camera)
                                                            && (String.IsNullOrWhiteSpace(model.Search.tags) ? true : i.tags == model.Search.tags)
                                                            && (String.IsNullOrWhiteSpace(model.Search.id) ? true : i.id == model.Search.id)
                                                            && (String.IsNullOrWhiteSpace(model.Search.full_picture) ? true : i.full_picture == model.Search.full_picture));

                if (result != null)
                    return PartialView("_TableImages", result.Select(r => new PictureDto() { id = r.id, cropped_picture = r.cropped_picture }).ToList());

                return PartialView("_TableImages", new List<PictureDto>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void SetToken()
        {
            var token = this._photosServices.GetToken(this._appKeySettings.Value.ApiKey);
            HttpContext.Session.SetObject("Token", token);
        }
    }
}
