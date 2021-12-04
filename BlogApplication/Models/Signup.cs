using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BlogApplication.Models
{
    //model class for Signup page
    public class Signup
    {
        //username property. can  not be left blank
        [Required(ErrorMessage = "Please enter Username")] 
        [StringLength(50, ErrorMessage = "Username must not be more than 50 character")] //Length Check
        [RegularExpression(@"^[A-Za-z0-9@\.\+\-_]+$", ErrorMessage = "Username must cotain Letters, digits and @/./+/-/_ only")] //Characters Check
        public string Username { get; set; }

        //email property. can  not be left blank
        [Required(ErrorMessage = "Please enter email")] 
        [DataType(DataType.EmailAddress)] //Datatype Check
        [EmailAddress(ErrorMessage = "Email Format is invalid")] //format check
        public string Email { get; set; }

        //password property. can  not be left blank
        [Required(ErrorMessage = "Please enter Password")] 
        [MinLengthAttribute(5, ErrorMessage = "Password must have atleast 5 characters")] //min length Check
        [DataType(DataType.Password)] //Datatype Check
        public string Password { get; set; }

        //property to confirm password . can  not be left blank
        [Required(ErrorMessage = "Please enter the field")]
        [DataType(DataType.Password)] //Datatype Check
        public string ConfirmPassword { get; set; }

        //http request Photo property. can  not be left blank
        [Required(ErrorMessage = "Please provide a photo")]
        public IFormFile Photo { get; set; }


        //properties for error validation 
        public string Error { get; set; }
        public string ExtErr { get; set; }
    }
}