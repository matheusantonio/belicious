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
        public List<string> tags {get; set;}
    }
}