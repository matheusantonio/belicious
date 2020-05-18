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
            return View();
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
    
        public IActionResult Search(string parameter)
        {
            var resultBookmark = (from t in _context.Tags
                                  from tb in _context.TagBookmarks
                                  from b in _context.Bookmarks
                                  where parameter.Contains(t.tag)
                                  && tb.bookmarkId == b.bookmarkId
                                  && tb.tagId == t.tagId
                                  select b).ToList();

            var searchResults = new SearchResultsViewModel()
            {
                resultBookmark = resultBookmark
            };

            return View(searchResults);
        }
    
        public IActionResult Bookmarks()
        {
           return ViewComponent("IndexBookmarks");
        }
        private List<Tuple<Bookmark, List<string>, int>> retrieveTopBookmarks()
        {
            var topBookmarks = new List<Tuple<Bookmark, List<string>, int>>();

            var topBookmarksQuery = (from tb in (from ub in _context.UserBookmarks
                                            where ub.isPrivate == false
                                            group ub by ub.bookmarkId into oub
                                            select new { key = oub.Key, cnt = oub.Count()})
                                orderby tb.cnt descending
                                select tb).Take(10);

            foreach(var item in topBookmarksQuery.ToList())
            {
                string bookmarkId = item.key;
                int qtd = item.cnt;

                var tags = (from tb in _context.TagBookmarks
                           from t in _context.Tags
                           where tb.bookmarkId == bookmarkId
                           && tb.tagId == t.tagId
                           select t.tag).ToList();

                topBookmarks.Add(
                    new Tuple<Bookmark, List<string>, int>(_context.Bookmarks.Find(bookmarkId), tags, qtd)
                );
            }

            return topBookmarks;
        }
    
        private List<Tuple<Bookmark, List<string>, DateTime>> retrieveRecentBookmakrs()
        {
            var recentBookmarks = new List<Tuple<Bookmark, List<string>, DateTime>>();

            var recentBookmarksQuery = (from ub in _context.UserBookmarks
                                  where ub.isPrivate == false
                                  orderby ub.addedTime descending
                                  select new {key = ub.bookmarkId, time = ub.addedTime, user = ub.userId}).Take(10);

            foreach(var bookmarkTime in recentBookmarksQuery.ToList())
            {
                var tags = (from tb in _context.TagBookmarks
                           from t in _context.Tags
                           where tb.bookmarkId == bookmarkTime.key
                           && tb.tagId == t.tagId
                           && tb.userId == bookmarkTime.user
                           select t.tag).ToList();

                recentBookmarks.Add(
                    new Tuple<Bookmark, List<string>, DateTime>(_context.Bookmarks.Find(bookmarkTime.key),
                                                                tags,
                                                                bookmarkTime.time)
                );
            }

            return recentBookmarks;

        }
    

    }
}
