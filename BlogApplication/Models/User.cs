using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BlogApplication.Models
{
    public class User
    {
        //property for id of user
        public int Id { get; set; }

        //username property. can  not be left blank
        [Required(ErrorMessage = "Please eneter a Username")]
        [StringLength(50, ErrorMessage = "Username must not be more than 50 character")] //length check
        [RegularExpression(@"^[A-Za-z0-9@\.\+\-_]+$", ErrorMessage = "Username must cotain Letters, digits and @/./+/-/_ only")] //Characters Check
        public string Username { get; set; }

        //email property. can  not be left blank
        [Required(ErrorMessage = "Please provide an email")] 
        [DataType(DataType.EmailAddress)] //data type check
        [EmailAddress(ErrorMessage = "Email Format is invalid")] //format check
        public string Email { get; set; }
        
        //current password property
        public string Password { get; set; }
        
        //old password property
        public string OldPassword { get; set; }
        
        //property holding the address of the photo of user
        public string Photo { get; set; }

        //http request Photo property. can  not be left blank
        public IFormFile Image { get; set; }
        
        //constructors
        public User() { } 
        public User(string username, string email, string password, string photo) 
        {
            Username = username;
            Email = email;
            Password = password;
            Photo = photo;
        }
        public User(int id, string username, string email, string password, string photo)
        {
            Id = id;
            Username = username;
            Email = email;
            Password = password;
            Photo = photo;
        }
        
        
        //following properties required for validation
        public string Error { get; set; }
        public string ExtErr { get; set; }
        public string PassErr { get; set; }
        public string NPassErr { get; set; }
        public string NewPassword { get; set; }
    }
}
