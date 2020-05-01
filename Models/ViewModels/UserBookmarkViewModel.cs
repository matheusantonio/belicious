using System.Collections.Generic;

namespace belicious.Models.ViewModels
{
    public class UserBookmarkViewModel
    {
        public string userName {get; set;}

        //public List<Bookmark> bookmarks {get; set;}

        public Dictionary<Bookmark, List<string>> bookmarks {get; set;}
    }
}

