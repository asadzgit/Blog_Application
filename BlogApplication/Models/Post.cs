using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BlogApplication.Models
{
    //model class for creating posts
    public class Post
    {
        public int Id { get; set; }

        //title property. can  not be left blank
        [Required(ErrorMessage = "Please set a title")] //Check is title exist
        [StringLength(50, ErrorMessage = "Title must not be more than 50 character")] //Length Check
        public string Title { get; set; }

        //content property. can  not be left blank
        [Required(ErrorMessage = "Please fill the Content field")] //Check is content exist
        [StringLength(2000, ErrorMessage = "Content must not be more than 2000 character")] //Length Check
        public string Content { get; set; }
        
        //holds the published date of the post
        public string Date { get; set; }
        
        //holds the name of the user that created his post
        public string Username { get; set; }
        
        //holds the image of the user who cretaed the post
        public string UserImage { get; set; }
        
        //holds the id of the user whi creted the post
        public int UserID { get; set; }
        
        //tells whether the posyts ca be updated or deleted in viewing....
        //this is true for the user who creted this post only. for other usersit is false
        public bool permissiion { get; set; }
        
        //non paramterized and parameterized constructirs for the Post class
        public Post() 
        {
            permissiion = false;
        }
        public Post(string title, string content, string date, int uid)
        {
            Title = title;
            Content = content;
            Date = date;
            UserID = uid;
        }
        public Post(int id, string title, string content, string date)
        {
            Id = id;
            Title = title;
            Content = content;
            Date = date;
        }
        public Post(string title, string content, string date, string username, string userImage, int uid, int id) 
        {
            Id = id;
            Title = title;
            Content = content;
            Date = date;
            Username = username;
            UserImage = userImage;
            UserID = uid;
            permissiion = false;
        }
        //string for showing validation error messages
        public string Error { get; set; }    
    }
}
