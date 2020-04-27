using System.ComponentModel.DataAnnotations;

using System;

namespace belicious.Models.ViewModels
{
    public class RemoveBookmarkViewModel
    {
        public string userName {get; set;}

        [Display(Name="Added at")]
        public DateTime addedDate {get; set;}

        [Display(Name="URL")]
        public string url {get; set;}

        public string bookmarkId {get; set;}


    }
}