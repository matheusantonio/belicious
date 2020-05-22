using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using System;
using System.Text;
using HtmlAgilityPack;

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

            string newname;

            var web = new HtmlAgilityPack.HtmlWeb();
            HtmlDocument doc = web.Load(this._urlLink);

            var h1 = doc.DocumentNode.SelectSingleNode("//h1");
            var title = doc.DocumentNode.SelectSingleNode("//title");
            if(h1 != null && !h1.HasChildNodes)
            {
                newname = h1.InnerHtml.Trim();
            }
            else if(title != null && !title.HasChildNodes)
            {
                    newname = title.InnerHtml.Trim();
            }else
            {
                string n = Regex.Replace(urlLink, @"^((https:[/]*|http:[/]*)(www)*|(www.))[.]*", "");
                string f = Regex.Replace(n, @"[.].*$", "");
                newname = f;

            }

            name = newname;
        }
    }
} 