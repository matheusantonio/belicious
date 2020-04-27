using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace belicious.Models
{
    public class Bookmark
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string bookmarkId {get; set;}
        
        [Display(Name="URL")]
        public string urlLink {get; set;}

        public string name {get; set;}

        public void setName()
        {
            if(name != null) return;
            string n = Regex.Replace(urlLink, @"^((https:[/]*|http:[/]*)(www)*|(www.))[.]*", "");
            string f = Regex.Replace(n, @"[.].*$", "");

            name = f;
        }
    }
} 