using System.Collections.Generic;
using System;

namespace belicious.Models.ViewModels
{
    public class TopBookmarkViewModel
    {
        //public List<Bookmark> topBookmarks {get; set;}

        public List<Tuple<Bookmark, List<string>, int>> topBookmarks {get; set;}

        //public List<Bookmark> recentlyAdded {get; set;}

        public List<Tuple<Bookmark, List<string>, DateTime>> recentlyAdded {get; set;}
    }
}