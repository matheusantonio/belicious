using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using belicious.Models.Persistence;
using belicious.Models.ViewModels;
using belicious.Models;

namespace ViewComponents
{
    public class IndexBookmarksViewComponent : ViewComponent
    {
        private readonly BookmarksContext _context;

        public IndexBookmarksViewComponent(BookmarksContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var model = new TopBookmarkViewModel
            {
                topBookmarks = this.retrieveTopBookmarks(),
                recentlyAdded = this.retrieveRecentBookmarks()
            };

            return View(model);
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

        private List<Tuple<Bookmark, List<string>, DateTime>> retrieveRecentBookmarks()
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