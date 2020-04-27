using System;
using System.ComponentModel.DataAnnotations;


namespace belicious.Models
{
    public class UserBookmark
    {

        [Key]
        public string userId {get; set;}
        
        [Key]
        public string bookmarkId {get; set;}

        public DateTime addedTime {get; set;}

    }
}

