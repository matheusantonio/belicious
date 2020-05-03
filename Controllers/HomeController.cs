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

            indexBookmarks.topBookmarks = new List<Tuple<Bookmark, List<string>, int>>();

            var topBookmarks = (from tb in (from ub in _context.UserBookmarks
                                group ub by ub.bookmarkId into oub
                                select new { key = oub.Key, cnt = oub.Count()})
                                orderby tb.cnt descending
                                select tb).Take(10);

            foreach(var item in topBookmarks.ToList())
            {
                string bookmarkId = item.key;
                int qtd = item.cnt;

                var tags = (from tb in _context.TagBookmarks
                           from t in _context.Tags
                           where tb.bookmarkId == bookmarkId
                           && tb.tagId == t.tagId
                           select t.tag).ToList();

                indexBookmarks.topBookmarks.Add(
                    new Tuple<Bookmark, List<string>, int>(_context.Bookmarks.Find(bookmarkId), tags, qtd)
                );
            }
 

            indexBookmarks.recentlyAdded = new List<Tuple<Bookmark, List<string>, DateTime>>();

            var recentBookmarks = (from ub in _context.UserBookmarks
                                  orderby ub.addedTime descending
                                  select new {key = ub.bookmarkId, time = ub.addedTime}).Take(10);

            foreach(var bookmarkTime in recentBookmarks.ToList())
            {
                var tags = (from tb in _context.TagBookmarks
                           from t in _context.Tags
                           where tb.bookmarkId == bookmarkTime.key
                           && tb.tagId == t.tagId
                           select t.tag).ToList();

                indexBookmarks.recentlyAdded.Add(
                    new Tuple<Bookmark, List<string>, DateTime>(_context.Bookmarks.Find(bookmarkTime.key),
                                                                tags,
                                                                bookmarkTime.time)
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
