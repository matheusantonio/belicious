using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace belicious.Models.ViewModels
{
    public class EditBookmarkViewModel
    {
        [Display(Name="URL")]
        public string url {get; set;}

        [Display(Name="Name")]
        public string name {get; set;}

        [Display(Name="Tags")]
        public List<string> tags {get; set;}

        [Display(Name="Private")]
        public bool isPrivate {get; set;}

        public string bookmarkId {get; set;}

        public string userName {get; set;}
    }
}