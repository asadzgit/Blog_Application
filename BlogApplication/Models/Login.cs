using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BlogApplication.Models
{
    //model class for login
    public class Login
    {
        //username property. can  not be left blank
        [Required(ErrorMessage = "Please enter Username")] 
        public string Username { get; set; }

        //password property. can  not be left blank
        [Required(ErrorMessage = "Please enter Password")] 
        public string Password { get; set; }
        //property to print any error in the login 
        public string Error { get; set; }
    }
}
