using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace belicious.Models.ViewModels
{
    public class NewBookmarkViewModel
    {
        public string userName {get; set;}

        [Display(Name="URL")]
        public string url {get; set;}

        [Display(Name="Defined name")]
        public string bookmarkName {get; set;}

        [Display(Name="Tags")]
        public string tags {get; set;}

        [Display(Name="Private")]
        public bool isPrivate {get; set;}
    }
}