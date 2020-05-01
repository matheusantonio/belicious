using System.Collections.Generic;

namespace belicious.Models.ViewModels
{
    public class TopBookmarkViewModel
    {
        public List<Bookmark> topBookmarks {get; set;}

        //public Dictionary<Bookmark, List<string>> topBookmarks {get; set;}

        public List<Bookmark> recentlyAdded {get; set;}

        //public Dictionary<Bookmark, List<string>> recentlyAdded {get; set;}
    }
}