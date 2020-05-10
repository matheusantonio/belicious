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
                                select new {bookmarks = bm, name = ub.userDefinedName, pv = ub.isPrivate};

            List<Tuple<Bookmark, List<string>, string, bool>> bookmarks = new List<Tuple<Bookmark, List<string>, string, bool>>();

            foreach(var item in userBookmarks)
            {
                Bookmark bookmark = item.bookmarks;

                var tags = (from t in _context.Tags
                           from tb in _context.TagBookmarks
                           where t.tagId == tb.tagId
                           && tb.bookmarkId == bookmark.bookmarkId
                           && tb.userId == currentUser
                           select t.tag).ToList();
                
                bookmarks.Add(new Tuple<Bookmark, List<string>, string, bool>(bookmark,
                                                                        tags,
                                                                        item.name,
                                                                        item.pv));
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
                userDefinedName = newUserBookmark.bookmarkName,
                isPrivate = newUserBookmark.isPrivate
            };

            _context.UserBookmarks.Add(userBookmark);
            await _context.SaveChangesAsync();

            foreach(string tag in newUserBookmark.tags)
            {
                var checkTag = (from t in _context.Tags
                               where t.tag == tag
                               select t).FirstOrDefault();

                string tagId;

                if(checkTag != null)
                {
                    tagId = checkTag.tagId;

                } else
                {
                    Tag newTag = new Tag()
                    {
                        tag = tag
                    };
                    _context.Tags.Add(newTag);
                    await _context.SaveChangesAsync();

                    tagId = (from t in _context.Tags
                            where t.tag == tag
                            select t.tagId)
                            .FirstOrDefault();

                }

                _context.TagBookmarks.Add(new TagBookmark
                {
                    tagId = tagId,
                    bookmarkId = bookmarkId,
                    userId = _userManager.GetUserId(User)
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
            
            var tagAssoc = (from tb in _context.TagBookmarks
                                    where tb.bookmarkId == bookmarkId
                                    && tb.userId == _userManager.GetUserId(User)
                                    select tb).ToList();

            _context.UserBookmarks.Remove(assosciation);
            _context.TagBookmarks.RemoveRange(tagAssoc);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    
        public IActionResult Edit(string bookmarkId)
        {
            Bookmark bookmark = _context.Bookmarks.Find(bookmarkId);

            var tags = (from tb in _context.TagBookmarks
                       from t in _context.Tags
                       where tb.bookmarkId == bookmarkId
                       && tb.tagId == t.tagId
                       && tb.userId == _userManager.GetUserId(User)
                       select t.tag).ToList();

            var name_pv = (from ub in _context.UserBookmarks
                          where ub.bookmarkId == bookmarkId
                          && ub.userId == _userManager.GetUserId(User)
                          select new{ name = ub.userDefinedName, pv = ub.isPrivate}).FirstOrDefault();
            
            var editModelView = new EditBookmarkViewModel
            {
                url = bookmark.urlLink,
                tags = tags.Any() ? tags : new List<string>(),
                name = name_pv.name,
                bookmarkId = bookmarkId,
                userName = _userManager.GetUserName(User),
                isPrivate = name_pv.pv
            };

            return View(editModelView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditBookmarkViewModel editBookmark)
        {
            //1 verificar se o nome mudou e alterá-lo (salvar a bookmark após isso)
            var userBookmark = (from ub in _context.UserBookmarks
                            where ub.bookmarkId == editBookmark.bookmarkId
                            && ub.userId == _userManager.GetUserId(User)
                            select ub).FirstOrDefault();

            if(userBookmark.userDefinedName != editBookmark.name
                || userBookmark.isPrivate != editBookmark.isPrivate)
            {
                userBookmark.userDefinedName = editBookmark.name;
                userBookmark.isPrivate = editBookmark.isPrivate;

                _context.UserBookmarks.Update(userBookmark);
                await _context.SaveChangesAsync();
            }

            var tagAssoc = (from tb in _context.TagBookmarks
                            where tb.bookmarkId == editBookmark.bookmarkId
                            && tb.userId == _userManager.GetUserId(User)
                            select tb).ToList();

            _context.TagBookmarks.RemoveRange(tagAssoc);
            await _context.SaveChangesAsync();

            var newTagBookmarks = new List<TagBookmark>();

            if(editBookmark.tags != null){
                foreach(string tag in editBookmark.tags)
                {
                    var checkTag = (from t in _context.Tags
                                where t.tag == tag
                                select t).FirstOrDefault();

                    string tagId;

                    if(checkTag != null)
                    {
                        tagId = checkTag.tagId;

                    } else
                    {
                        Tag newTag = new Tag{
                            tag = tag
                        };

                        _context.Tags.Add(newTag);
                        await _context.SaveChangesAsync();

                        tagId = (from t in _context.Tags
                                 where t.tag == tag
                                 select t.tagId).First();
                        
                    }

                    TagBookmark newTagBookmark = new TagBookmark{
                        userId = _userManager.GetUserId(User),
                        bookmarkId = editBookmark.bookmarkId,
                        tagId = tagId
                    };

                    newTagBookmarks.Add(newTagBookmark);
                }

                await _context.TagBookmarks.AddRangeAsync(newTagBookmarks);
                await _context.SaveChangesAsync();

            }
            

            return RedirectToAction(nameof(Index));
        }
    }
}