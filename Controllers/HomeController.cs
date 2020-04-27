using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using belicious.Models;
using belicious.Models.ViewModels;

namespace belicious.Controllers
{

    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Add most updated bookmark recovery

            TopBookmarkViewModel indexBookmarks = new TopBookmarkViewModel();

            //Remove everything below this later
            indexBookmarks.topBookmarks = new List<Bookmark>();

            indexBookmarks.topBookmarks.Add(
                new Bookmark{
                    urlLink = "google.com",
                    name = "Google"
                }
            );
            indexBookmarks.topBookmarks.Add(
                new Bookmark{
                    urlLink = "facebook.com",
                    name= "Facebook"
                }
            );

            indexBookmarks.recentlyAdded = new List<Bookmark>();

            indexBookmarks.recentlyAdded.Add(
                new Bookmark{
                    urlLink = "youtube.com",
                    name = "Youtube"
                }
            );
            indexBookmarks.recentlyAdded.Add(
                new Bookmark{
                    urlLink = "reddit.com",
                    name= "Reddit"
                }
            );
            indexBookmarks.recentlyAdded.Add(
                new Bookmark{
                    urlLink = "quora.com",
                    name= "Quora"
                }
            );
            //*******************


            return View(indexBookmarks);
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
    }
}
