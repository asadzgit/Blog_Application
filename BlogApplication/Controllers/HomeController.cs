using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlogApplication.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace BlogApplication.Controllers
{
    public class HomeController : Controller
    {
        //variable for holding the logined user
        static User currentUser;
        //object to communicate with CRUDUser model class that manages database
        CRUDUser userDb;
        //readonly member to give information about the environment of web hosting. used for async methods
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController (IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        //method for admin home screen
        public ViewResult AdminHome()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("Admin"))
            {
                return View("Login");
            }
            try
            {
                userDb = new CRUDUser();
                //retrieving all users from the database
                List<User> userList = userDb.GetAllUsers();
                return View("AdminHome", userList);
            }
            catch(Exception e)
            {
                return View("Error",e.Message);
            }
            
        }

        /*
         * GET and Post methods to add a user in database
         */
        [HttpGet]
        public ViewResult AddUser()
        {
            try
            {
                if (!HttpContext.Request.Cookies.ContainsKey("Admin"))
                {
                    return View("Login");
                }
                else
                {
                    return View();
                }
            }
            catch(Exception e)
            {
                return View("Error", e.Message);
            }
            
        }

        [HttpPost]
        public ViewResult AddUser(User user)
        {
            try
            {
                bool IsError = false;
                CRUDUser userdb = new CRUDUser();
                if (ModelState.IsValid)
                {
                    if (userdb.IsUsernameExist(user.Username))//check if a username is already taken
                    {
                        IsError = true;
                        ModelState.AddModelError("Username", "This Username is already taken, try another one");
                    }
                    if (userdb.IsEmailExist(user.Email))//check if a email is already taken
                    {
                        IsError = true;
                        ModelState.AddModelError("Email", "This Email already used by other user, try another one");
                    }
                    if(user.Password == null)
                    {
                        IsError = true;
                        ModelState.AddModelError("Password", "Please enter a password");
                    }
                    if(user.Password != null && user.Password.Length<5)
                    {
                        IsError = true;
                        ModelState.AddModelError("Password","Password must be at least 5 characters long");
                    }
                    if (IsError)//returning to same view if error occurs
                    {
                        return View();
                    }
                    else
                    {   //inserting new user record in datatbase
                        userdb = new CRUDUser();
                        userdb.InsertUser(user.Username, user.Password, user.Email,"/Images/default.jpg");
                        //get all users from db to show on home screen of admin
                        List<User> userList = userdb.GetAllUsers();
                        return View("AdminHome", userList);
                    }
                }
                else
                {
                    return View();
                }
            }
            catch (Exception e)
            {
                return View("Error", e.Message);
            }
        }
        
        /*
         * GET and Post methods for editing a existing user
         */
        [HttpGet]
        public ViewResult EditUser(int id)
        {
            try
            {
                if (!HttpContext.Request.Cookies.ContainsKey("Admin"))
                {
                    return View("Login");
                }
                //retrieve user to edit from DB
                CRUDUser userdb = new CRUDUser();
                User user = userdb.GetUser(id);
                return View("UpdateUser",user);
            }
            catch (Exception e)
            {
                return View("Error", e.Message);
            }
        }
        [HttpPost]
        public ViewResult EditUser(User user)
        {
            try
            {
                userDb = new CRUDUser();
                CRUDPost postdb = new CRUDPost();
                User u = userDb.GetUser(user.Id);
                if(user.Username != null && user.Username != u.Username)//user gave a new usernamae
                {
                    if (userDb.IsUsernameExist(user.Username))
                    {
                        user.Error = "This username is alreaady take, try a new one";
                        return View("UpdateUser", user);
                    }
                    u.Username = user.Username;//change the userbame to change in DB later
                    postdb.updateUsername(u, user.Username);
                }
                if(user.Email != null && user.Email != u.Email)//user gave a new emaial
                {
                    if (userDb.IsEmailExist(user.Email))
                    {
                        user.Error = "This email is already taken, try a new one";
                        return View("UpdateUser",user);
                    }
                    else
                        u.Email = user.Email;                        
                }
                if (u.Password != user.OldPassword)//if current password did not match
                {
                    user.PassErr = "Old Password does not matched"; //Assigning Error
                    return View("UpdateUser", user); 
                }
                if (user.NewPassword != null )
                {
                    if (user.NewPassword.Length < 5) //Cecking is New Password's length less than 5 characters or not
                    {
                        user.NPassErr = "Password must contain at least 5 characters"; //Assigning New Password Error
                        return View("UpdateUser", user); 
                    }
                    else //admin entered valid new password
                    {
                        u.Password = user.NewPassword;
                    }   
                }
                bool isupdated = userDb.UpdateUser(u);
                if (!isupdated)
                {
                    user.Error = "Could not update the user";//could not updated. assigning error
                    return View("UpdateUser",user);
                }
                    
                List<User> userList = userDb.GetAllUsers();
                return View("AdminHome", userList);
                
            }
            catch (Exception e)
            {
                return View("Error", e.Message);
            }
        }

         //method to delete user 
        [HttpGet]
        public ViewResult DeleteUser(int id)
        {
            try
            {
                if (!HttpContext.Request.Cookies.ContainsKey("Admin"))
                {
                    return View("Login");
                }
                else
                {
                    //delete user
                    CRUDUser userdb = new CRUDUser();
                    CRUDPost blogdb = new CRUDPost();
                    userdb.DeleteUser(id);
                    blogdb.DeletePostByUserID(id);
                    //get all users from the DB
                    List<User> userList = userdb.GetAllUsers();
                    HttpContext.Response.Cookies.Append("Admin", "true");
                    return View("AdminHome", userList);
                }
            }
            catch (Exception e)
            {
                return View("Error", e.Message);
            }
        }

        //method to view details of  auser
        [HttpGet]
        public ViewResult UserDetails(int id)
        {
            try
            {
                if (!HttpContext.Request.Cookies.ContainsKey("Admin"))
                {
                    return View("Login");
                }
                CRUDUser userDb = new CRUDUser();
                //get user from db
                User user = userDb.GetUser(id);
                //pass the user into view to see details
                return View(user);
            }
            catch (Exception e)
            {
                return View("Error", e.Message);
            }
        }

        //method to log out for admin and remove its cookies
        [HttpGet]
        public ViewResult AdminLogout()
        {
            try
            {
                currentUser = new User();
                HttpContext.Response.Cookies.Delete("Admin");
                return View("Login");
            }
            catch (Exception e)
            {
                return View("Error", e.Message);
            }

        }

        //checks whether the admin credentials are got
        private bool IsUserAdmin(string username, string password)
        {
            return username == "admin" && password == "admin";
        }

        //GET and POST methods for Signup
        [HttpGet]
        public ViewResult Signup()
        {
            return View();
        }

        //async post method for signup
        [HttpPost]
        public async Task<ViewResult> Signup(Signup user)
        {
            try
            {
                CRUDUser userdb = new CRUDUser();
                bool IsError = false;
                if (ModelState.IsValid)
                {
                    if (userdb.IsUsernameExist(user.Username))//check if a username is already taken
                    {
                        IsError = true;
                        user.Error = "This Username already used by other user, try another one";
                    }
                    if (userdb.IsEmailExist(user.Email))//check if a email is already taken
                    {
                        IsError = true;
                        user.Error = "This Email already used by other user, try another one";
                    }
                    if(user.ConfirmPassword != user.Password)//check if passwords entered are matched
                    {
                        IsError = true;
                        user.Error = "Passwords did not match";
                    }
                    if (IsError)
                    {
                        return View(user);
                    }
                    else
                    {   //upload image to server and assign it to user and blogs
                        string folder = "Images/";
                        folder += Guid.NewGuid().ToString() +"_"+ user.Photo.FileName;
                        string serverfolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                        await user.Photo.CopyToAsync(new FileStream(serverfolder, FileMode.Create));
                        //insert the newly created user in database
                        userdb.InsertUser(user.Username, user.Password, user.Email, "/"+folder);
                        currentUser = userdb.GetUser(user.Username, user.Password);
                        
                        CRUDPost blogdb = new CRUDPost();
                        List<Post> blogList = blogdb.GetAllPosts();
                        HttpContext.Response.Cookies.Append("User", "true");
                        return View("Login");
                    }
                }
                else
                {
                    return View();
                }
            }
            catch (Exception e)
            {
                return View("Error", e.Message);
            }
        }

        /*
         * GET and POST methods for Login
         */
        [HttpGet]
        public ViewResult Login()
        {
            return View("Login");           
        }
        [HttpPost]
        public ViewResult Login(Login login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CRUDUser userdb = new CRUDUser();
                    if (userdb.IsUserExist(login.Username, login.Password))//check if the user exist in DB 
                    {
                        currentUser = userdb.GetUser(login.Username, login.Password);//saving the credentials in current user
                        if (IsUserAdmin(login.Username, login.Password))//the  admin is logging in
                        {
                            List<User> userList = userdb.GetAllUsers();
                            HttpContext.Response.Cookies.Append("Admin", "true");//add cookies for admin
                            return View("AdminHome", userList);
                        }
                        else//the user logs in
                        {
                            //get all posts 
                            CRUDPost blogdb = new CRUDPost();
                            List<Post> blogList = blogdb.GetAllPosts();
                            HttpContext.Response.Cookies.Append("User", "true");//add cookies for user 
                            if (blogList != null)
                            {
                                foreach (Post b in blogList)
                                {
                                    if (b.UserID == currentUser.Id)//mark the posts of current user so that they can be updated
                                        b.permissiion = true;
                                }
                            }
                            
                            return View("UserHome", blogList);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Incorrect Username or Password\nTry Again or Sign up");
                        return View();
                    }
                }
                else
                {
                    return View();
                }
            }
            catch (Exception e)
            {
                return View("Error", e.Message);
            }
        }
        
        //method for user home screen
        public ViewResult UserHome()
        {
            try
            {
                if (!HttpContext.Request.Cookies.ContainsKey("User"))
                {
                    return View("Login");
                }
                CRUDPost blogdb = new CRUDPost();
                //get all posts from the db
                List<Post> bloglist = blogdb.GetAllPosts();
                //mark the posts of current user so that they can be updated
                if (bloglist != null)
                {
                    foreach (Post b in bloglist)
                    {
                        if (b.UserID == currentUser.Id)
                            b.permissiion = true;
                    }
                }
                
                return View("UserHome", bloglist);
            }

            catch (Exception e)
            {
                return View("Error", e.Message);
            }
        }
        
        /*
         * GET and POST methods fro viewing and updating user profile
         */
        [HttpGet]
        public ViewResult Profile()
        {
            try
            {
                if (!HttpContext.Request.Cookies.ContainsKey("User"))
                {
                    return View("Login");
                }
                return View(currentUser);
            }
            catch (Exception e)
            {
                return View("Error", e.Message);
            }
        }
        [HttpPost]
        public async Task<ViewResult> Profile(User user)
        {
            try
            {
                if (!HttpContext.Request.Cookies.ContainsKey("User")) //Checking is User logged in
                    return View("Login"); //Rendering Login Page
                if (!ModelState.IsValid)
                    return View(user);
                userDb = new CRUDUser();
                CRUDPost postdb = new CRUDPost();
                if (user.Username != null && user.Username != currentUser.Username)//user gave a new username
                {
                    if(userDb.IsUsernameExist(user.Username))
                    {
                        user.Error = "This username is already taken, try a new one";
                        return View("Profile", user);
                    }
                    else
                        postdb.updateUsername(currentUser, user.Username); //update the suername
                        currentUser.Username = user.Username;
                }
                if (user.Email != null && user.Email != currentUser.Email)//user gave a new email
                {
                    if (userDb.IsEmailExist(user.Email))
                    {
                        user.Error = "This email is already taken, try a new one";
                        return View("Profile", user);
                    }
                    currentUser.Email = user.Email;
                }
                user.Email = currentUser.Email;

                if ( currentUser.Password != user.OldPassword)
                {
                    user.PassErr = "Old Password does not matched"; //Assigning Error
                    return View("Profile", user); 
                }

                string newpass = user.NewPassword; //Assigning new password
                if (newpass != null) //Checking is User entered new password
                {
                    if (newpass.Length < 5) //Cecking is New Password's length less than 8 characters or not
                    {
                        user.NPassErr = "Password must contain at least 5 characters"; //Assigning New Password Error
                        return View("Profile", user);
                    }
                    else //user entered valid new password
                    {
                        currentUser.Password = newpass;
                    }
                }
                if (user.Image != currentUser.Image && user.Image != null)//if user selected new image
                {
                    //uploading the picture on server and updating the dadtabase
                    string folder = "Images/";
                    folder += Guid.NewGuid().ToString() + "_" + user.Image.FileName;
                    string serverfolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                    await user.Image.CopyToAsync(new FileStream(serverfolder, FileMode.Create));
                    currentUser.Photo = "/" + folder;
                    currentUser.Image = user.Image;
                    //updating images of all the posts of the user
                    postdb.updatePhoto(currentUser.Username, currentUser.Photo);
                }


                if (userDb.UpdateUser(currentUser))//updating user credentials in the DB
                {
                    List<Post> posts = postdb.GetAllPosts(); //Getting posts from database
                    return View("UserHome", posts); 
                }
                else
                    user.Error = "User already exist with same Username or Email"; //Assigning Error
                return View("Profile", user); //Rendering Profile Page
            }
            catch(Exception e)
            {
                return View("Error",e.Message);
            }            
        }


        public ViewResult About()
        {
            return View();
        }
        
        //logout method for user
        [HttpGet]
        public ViewResult Logout()
        {
            try
            {
                currentUser = new User();
                HttpContext.Response.Cookies.Delete("User");
                return View("Login");
            }
            catch (Exception e)
            {
                return View("Error", e.Message);
            }
        }
        
        //method to view the details of post
        [HttpGet]
        public ViewResult PostDetails(int id)
        {
            try
            {
                if (!HttpContext.Request.Cookies.ContainsKey("User"))
                {
                    return View("Login");
                }
                CRUDPost blogdb = new CRUDPost();
                Post post = blogdb.GetPost(id);//get post from DB
                if (post.UserID == currentUser.Id)//mark if the post can be updated
                    post.permissiion = true;
                return View("ViewPost", post);
            }
            catch (Exception e)
            {
                return View("Error", e.Message);
            }
        }
        
        //GET and POST methods for updating the post 
        [HttpGet]
        public ViewResult UpdatePost(int id)
        {
            try
            {
                if (!HttpContext.Request.Cookies.ContainsKey("User")) 
                    return View("Login"); 
                CRUDPost postdb = new CRUDPost();
                Post post = postdb.GetPost(id);
                if (post != null) //Checking is blog successfully recieved
                {
                    return View("UpdatePost", new UpdatePost(id, post.Title.Trim(), post.Content.Trim())); //Rendering to Updatelog Page
                }
                return View("Error", "No blog exists"); //Rendering to Error Page
            }
            catch(Exception e)
            {
                return View("Error", e.Message);
            }
        }


        [HttpPost]
        public ViewResult UpdatePost(UpdatePost blog)
        {
            try
            {
                if (!HttpContext.Request.Cookies.ContainsKey("User"))
                {
                    return View("Login");
                }
                if (ModelState.IsValid)
                {
                    //update the post in DB with new data
                    CRUDPost blogdb = new CRUDPost();
                    blogdb.UpdatePost(new Post(blog.Id,blog.Title,blog.Content,DateTime.Today.Date.ToString("MMMM dd, yyyy") ) );
                    //get all posts
                    List<Post> bloglist = blogdb.GetAllPosts();
                    //render the home page of user
                    return View("UserHome", bloglist);
                }
                else
                {
                    return View(blog);
                }
            }
            catch (Exception e)
            {
                return View("Error", e.Message);
            }
        }
        [HttpGet]
        public ViewResult CreatePost()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("User"))
            {
                return View("Login");
            }
            return View();
        }


        //method to create a ne wpost
        [HttpPost]
        public ViewResult CreatePost(Post blog)
        {
            if (!ModelState.IsValid)
                return View(blog);
            try 
            {
                //initializing the post object with recieved data
                CRUDPost blogdb= new CRUDPost(); 
                Post b = new Post();
                b.Title = blog.Title;
                b.Content = blog.Content;
                b.Date = DateTime.Today.Date.ToString("MMMM dd, yyyy");
                b.UserID = currentUser.Id;
                b.Username = currentUser.Username;
                b.UserImage = currentUser.Photo;
                //insertin the new post in DB
                blogdb.InsertPost(b);
                //get all posts from DB
                List<Post> bloglist= blogdb.GetAllPosts();
                return View("UserHome", bloglist);
            }
            catch (Exception ex)
            {
                blog.Error = ex.Message; //Assigning Error
                return View("CreatePost", blog); //Rendering Create Blog
            }
            
        }
        //method to delete post 
        public ViewResult DeletePost(int id)
        {
            try
            {
                if (!ModelState.IsValid) 
                    return View(); 
                if (!HttpContext.Request.Cookies.ContainsKey("User")) 
                    return View("Login"); 
                CRUDPost postdb = new CRUDPost(); 
                Post b = new Post();
                if (postdb.DeletePost(id))//delete post record form the db
                {
                    List<Post> posts = postdb.GetAllPosts();//get all posts available
                    return View("UserHome", posts); //Rendering Home Page
                }
                return View("Error", "No blog exists"); //Rendering Error Page
            }
            catch (Exception e)
            {
                return View("Error", e.Message); //Rendering Error Page
            }
        }
    }
}
