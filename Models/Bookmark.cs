using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using System;

namespace belicious.Models
{
    public class Bookmark
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string bookmarkId {get; set;}
        
        public string _urlLink;

        [Display(Name="URL")]
        public string urlLink {
            get
            {
                return _urlLink;
            } 
            set
            {
                if(Regex.IsMatch(value, @"^https{0,1}:[/]{2}"))
                {
                    _urlLink = value;
                }
                else
                {
                    _urlLink = "https://" + value;
                }   
            }
        }

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