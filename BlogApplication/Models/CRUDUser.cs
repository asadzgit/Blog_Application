using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace BlogApplication.Models
{
    public class CRUDUser
    {
        //connection string for database connection
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=blogland;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=FalseData Source=(localdb)\MSSQLLocalDB;Initial Catalog=blogland;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        //database connection, command executer and reader objects
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader dr;

        public CRUDUser()
        {
            //establishing the connection with database
            con = new SqlConnection(connectionString);
            //inserting the record of admin in newly created database for the first time
            /*
             * Credentials of Admin
             * username = admin
             * password= admin
             */
            InsertAdminRecord();
        }

        //tells whether a user record exists in the databse
        public bool IsUserExist(string uname, string pswd)
        {
            try
            {
                String query = $"select * from users where username = @u and password=@p";
                cmd = new SqlCommand(query, con);
                cmd.Parameters.Add(new SqlParameter("@u", uname.Trim()));
                cmd.Parameters.Add(new SqlParameter("@p", pswd.Trim()));
                con.Open();
                dr = cmd.ExecuteReader();
                bool flag = dr.HasRows;
                dr.Close();
                con.Close();
                return flag;
            }
            catch (Exception )
            {
                return false;
            }

        }

        //insert the record of admin for the first time newly created databse
        private void InsertAdminRecord()
        {
            if (!IsUserExist("admin", "admin"))
            {
                InsertUser("admin", "admin", "admin@gmail.com","/Images/default.jpg");
            }
        }

        //tells whether a username exists in the databse by checking the usernmae provided
        public bool IsUsernameExist(string uname)
        {
            try
            {
                String query = $"select * from users where username = @u";
                cmd = new SqlCommand(query, con);
                cmd.Parameters.Add(new SqlParameter("@u", uname.Trim()));
                con.Open();
                dr = cmd.ExecuteReader();
                bool flag = dr.HasRows;
                dr.Close();
                con.Close();
                return flag;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //return a user record from the databse by looking at the username and password provided
        public  User GetUser(string uname, string pswd)
        {
            try
            {
                User user = null;
                con.Close();
                con = new SqlConnection(connectionString);
                con.Open();
                String query = $"select * from users where username = @u and password=@p";
                cmd = new SqlCommand(query, con);
                cmd.Parameters.Add(new SqlParameter("@u", uname.Trim()));
                cmd.Parameters.Add(new SqlParameter("@p", pswd));
               
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    //initializing theuser instance based on  the  row returned
                    dr.Read();
                    user = new User { Id = dr.GetInt32(0), Username = dr.GetString(1).Trim(), Password = dr.GetString(2).Trim(), Email = dr.GetString(3).Trim(),Photo=dr.GetString(4).Trim() };
                }
                dr.Close();
                con.Close();
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //update the fields of a particular user in the databse
        public bool UpdateUser(User user)
        {
            try
            {
                String query = $"update users set username = @u,  password = @p, email = @em, photo = @ab where userid = @uid";
                con.Open();
                cmd = new SqlCommand(query, con);
                cmd.Parameters.Add(new SqlParameter("@u", user.Username.Trim()));
                cmd.Parameters.Add(new SqlParameter("@p", user.Password.Trim()));
                cmd.Parameters.Add(new SqlParameter("@em", user.Email.Trim()));
                cmd.Parameters.Add(new SqlParameter("@ab", user.Photo.Trim()));
                cmd.Parameters.Add(new SqlParameter("@uid", user.Id));
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();
                if (rowsAffected > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception )
            {
                return false;
            }
        }

        //return a user from the database  by looking at the user id provided
        public User GetUser(int uid)
        {
            try
            {
                con.Close();
                con = new SqlConnection(connectionString);
                con.Open();
                User user = null;
                String query = $"select * from users where userid = @uid";
                cmd = new SqlCommand(query, con);
                cmd.Parameters.Add(new SqlParameter("@uid", uid));
                
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    //initializing the user object based on the results got from databse
                    dr.Read();
                    user = new User { Id = dr.GetInt32(0), Username = dr.GetString(1).Trim(), Password = dr.GetString(2).Trim(), Email = dr.GetString(3).Trim(), Photo = dr.GetString(4).Trim() };
                }
                dr.Close();
                con.Close();
                return user;
            }
            catch (Exception )
            {
                con.Close();
                return null;
            }
        }

        //indicates whether a particular email record is present in the database
        public bool IsEmailExist(string email)
        {
            try
            {
                String query = $"select * from users where email = @em";
                cmd = new SqlCommand(query, con);
                cmd.Parameters.Add(new SqlParameter("@em", email.Trim()));
                con.Open();
                dr = cmd.ExecuteReader();
                bool flag = dr.HasRows;
                dr.Close();
                con.Close();
                return flag;
            }
            catch (Exception )
            {
                return false;
            }
        }

        //retrieve all the records of the users present in the database
        public List<User> GetAllUsers()
        {
            try
            {
                //making a list of all the users in the database
                List<User> list = new List<User>();
                String query = "select * from users";
                con = new SqlConnection(connectionString);
                con.Open();
                cmd = new SqlCommand(query, con);
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        //populating the list
                        if (dr.GetString(1).Trim() != "admin")
                        {
                            list.Add(new User { Id = dr.GetInt32(0), Username = dr.GetString(1).Trim(), Password = dr.GetString(2).Trim(), Email = dr.GetString(3).Trim(), Photo = dr.GetString(4).Trim() });
                        }
                    }
                }
                con.Close();

                return list;
            }
            catch (Exception)
            {
                con.Close();
                return null;
            }
        }

        //delete the user record
        public bool DeleteUser(int uid)
        {
            try
            {
                String query = $"delete from users where userid = @uid";
                con.Open();
                cmd = new SqlCommand(query, con);
                cmd.Parameters.Add(new SqlParameter("@uid", uid));
                int rowsDeleted = cmd.ExecuteNonQuery();
                con.Close();
                return rowsDeleted > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        //inserts a new user record in the database
        public bool InsertUser(string uname, string pswd, string email,string pic)
        {
            try
            {
                if (IsUsernameExist(uname))
                {
                    return false;
                }
                string query = $"insert into users(username,password,email,photo) values(@u, @p, @em, @pi)";
                con.Open();
                cmd = new SqlCommand(query, con);
                cmd.Parameters.Add(new SqlParameter("@u", uname.Trim()));
                cmd.Parameters.Add(new SqlParameter("@p", pswd.Trim()));
                cmd.Parameters.Add(new SqlParameter("@em", email.Trim()));
                cmd.Parameters.Add(new SqlParameter("@pi", pic.Trim() ));
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();
                if (rowsAffected > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception )
            {
                return false;
            }
        }

        public string GetPassword(string uname, string email)
        {
            try
            {
                String query = $"select * from users where username = @u and email = @em";
                String pswd = null;
                cmd = new SqlCommand(query, con);
                cmd.Parameters.Add(new SqlParameter("@u", uname.Trim()));
                cmd.Parameters.Add(new SqlParameter("@em", email.Trim()));
                con.Open();
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();
                    pswd = dr.GetString(2).Trim();
                }
                dr.Close();
                con.Close();
                return pswd;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool UpdateUsername(User user)
        {
            try
            {
                String query = $"update users set username = @u where userid = @uid";
                con.Open();
                cmd = new SqlCommand(query, con);
                cmd.Parameters.Add(new SqlParameter("@u", user.Username.Trim()));
                cmd.Parameters.Add(new SqlParameter("@uid", user.Id));
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();
                if (rowsAffected > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception )
            {
                return false;
            }
        }

    }

}
