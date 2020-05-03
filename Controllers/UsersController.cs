using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using belicious.Models;
using belicious.Models.Persistence;
using belicious.Models.ViewModels;

namespace belicious.Controllers
{
    public class UsersController : Controller
    {

        private readonly BookmarksContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UsersController(BookmarksContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var currentUser = _userManager.GetUserId(User);

            var username = _userManager.GetUserName(User);

            var userBookmarks = from ub in _context.UserBookmarks
                                from bm in _context.Bookmarks
                                where ub.userId == currentUser
                                && bm.bookmarkId == ub.bookmarkId
                                select new {bookmarks = bm, name = ub.userDefinedName};

            List<Tuple<Bookmark, List<string>, string>> bookmarks = new List<Tuple<Bookmark, List<string>, string>>();

            foreach(var item in userBookmarks)
            {
                Bookmark bookmark = item.bookmarks;

                var tags = (from t in _context.Tags
                           from tb in _context.TagBookmarks
                           where t.tagId == tb.tagId
                           && tb.bookmarkId == bookmark.bookmarkId
                           select t.tag).ToList();
                
                bookmarks.Add(new Tuple<Bookmark, List<string>, string>(bookmark,
                                                                        tags,
                                                                        item.name));
            }

            UserBookmarkViewModel userBookmark = new UserBookmarkViewModel
            {
                userName = username,
                bookmarks = bookmarks
            };

            return View(userBookmark);
        }

        public IActionResult New()
        {
            NewBookmarkViewModel userBookmark = new NewBookmarkViewModel
            {
                userName = _userManager.GetUserName(User),
                tags = new List<string>()
            };

            return View(userBookmark);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(NewBookmarkViewModel newUserBookmark)
        {
            
            var checkBookmark = (from bm in _context.Bookmarks
                                where bm.urlLink == newUserBookmark.url
                                select bm).FirstOrDefault();
            
            string bookmarkId;
            
            if(checkBookmark != null)
            {
                bookmarkId = checkBookmark.bookmarkId;

            } else
            {
                Bookmark bookmark = new Bookmark{
                    urlLink = newUserBookmark.url
                };
                bookmark.setName();

                _context.Bookmarks.Add(bookmark);
                await _context.SaveChangesAsync();

                bookmarkId = (from bm in _context.Bookmarks
                            where bm.urlLink == bookmark.urlLink
                            && bm.name == bookmark.name
                            select bm.bookmarkId)
                            .FirstOrDefault();
            }
            
            UserBookmark userBookmark = new UserBookmark
            {
                bookmarkId = bookmarkId,
                userId = _userManager.GetUserId(User),
                addedTime = DateTime.Now,
                userDefinedName = newUserBookmark.bookmarkName
            };

            _context.UserBookmarks.Add(userBookmark);
            await _context.SaveChangesAsync();

            foreach(string tag in newUserBookmark.tags)
            {
                Tag newTag = new Tag()
                {
                    tag = tag
                };
                _context.Tags.Add(newTag);
                await _context.SaveChangesAsync();

                var tagId = (from t in _context.Tags
                             where t.tag == tag
                             select t.tagId)
                             .FirstOrDefault();

                _context.TagBookmarks.Add(new TagBookmark
                {
                    tagId = tagId,
                    bookmarkId = bookmarkId
                });

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(string bookmarkId)
        {
            var url = from b in _context.Bookmarks
                      where b.bookmarkId == bookmarkId
                      select b.urlLink;

            var addedDate = from bu in _context.UserBookmarks
                            where bu.bookmarkId == bookmarkId
                            && bu.userId == _userManager.GetUserId(User)
                            select bu.addedTime;

            RemoveBookmarkViewModel bookmarkViewModel = new RemoveBookmarkViewModel
            {
                bookmarkId = bookmarkId,
                userName = _userManager.GetUserName(User),
                url = url.FirstOrDefault(),
                addedDate = addedDate.FirstOrDefault()
            };

            return View(bookmarkViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string bookmarkId)
        {
            UserBookmark assosciation = (from ub in _context.UserBookmarks
                                        where ub.bookmarkId == bookmarkId
                                        && ub.userId == _userManager.GetUserId(User)
                                        select ub).FirstOrDefault();

            _context.UserBookmarks.Remove(assosciation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}