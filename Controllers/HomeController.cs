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

    }
}
