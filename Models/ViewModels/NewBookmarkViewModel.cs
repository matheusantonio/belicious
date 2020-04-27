using System.Collections.Generic;

namespace belicious.Models.ViewModels
{
    public class NewBookmarkViewModel
    {
        public string userName {get; set;}

        public string url {get; set;}

        public List<string> tags {get; set;}
    }
}