using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using belicious.Models;
using belicious.Models.Persistence;
using belicious.Models.ViewModels;

namespace belicious.Controllers
{

    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BookmarksContext _context;

        public HomeController(ILogger<HomeController> logger, BookmarksContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            TopBookmarkViewModel indexBookmarks = new TopBookmarkViewModel();

            indexBookmarks.topBookmarks = new List<Bookmark>();

            var topBookmarks = (from tb in (from ub in _context.UserBookmarks
                                group ub by ub.bookmarkId into oub
                                select new { key = oub.Key, cnt = oub.Count()})
                                orderby tb.cnt
                                select tb).Take(10);

            foreach(var item in topBookmarks.ToList())
            {
                string bookmarkId = item.key;

                indexBookmarks.topBookmarks.Add(
                    _context.Bookmarks.Find(bookmarkId)
                );
            }


            indexBookmarks.recentlyAdded = new List<Bookmark>();

            var recentBookmarks = (from ub in _context.UserBookmarks
                                  orderby ub.addedTime
                                  select ub.bookmarkId).Take(10);

            foreach(var bookmarkId in recentBookmarks.ToList())
            {
                indexBookmarks.recentlyAdded.Add(
                    _context.Bookmarks.Find(bookmarkId)
                );
            }

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
