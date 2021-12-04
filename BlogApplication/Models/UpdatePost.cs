using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BlogApplication.Models
{
    //model class for update postpage
    public class UpdatePost
    {
        //property holding the id of the posts to be updated
        public int Id { get; set; }

        //title property. can  not be left blank
        [Required(ErrorMessage = "Please set a title")] 
        [StringLength(50, ErrorMessage = "Title must not be more than 50 character")] //length check
        public string Title { get; set; }

        //content property. can  not be left blank
        [Required(ErrorMessage = "Please fill the Content field")] 
        [StringLength(2000, ErrorMessage = "Content must not be more than 2000 character")] //length check
        public string Content { get; set; }
        
        //constructors 
        public UpdatePost() { } 
        public UpdatePost(string title, string content) 
        {
            Title = title;
            Content = content;
        }
        public UpdatePost(int id, string title, string content) 
        {
            Id = id;
            Title = title;
            Content = content;
        }

        //property for error validation
        public string Error { get; set; }
    }
}
