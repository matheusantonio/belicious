using System.Collections.Generic;
using System;

namespace belicious.Models.ViewModels
{
    public class UserBookmarkViewModel
    {
        public string userName {get; set;}

        //public Dictionary<Bookmark, List<string>> bookmarks {get; set;}
        public List<Tuple<Bookmark, List<string>, string>> bookmarks {get; set;}
    }
}