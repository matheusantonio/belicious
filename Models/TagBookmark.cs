using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace belicious.Models
{
    public class TagBookmark
    {
        [Key]
        public string tagId {get; set;}

        [Key]
        public string bookmarkId {get; set;}
    }
}