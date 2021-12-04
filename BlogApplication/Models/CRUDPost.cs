using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace BlogApplication.Models
{
    public class CRUDPost 
    {
        //connection string for database connection
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=blogland;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=FalseData Source=(localdb)\MSSQLLocalDB;Initial Catalog=blogland;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        //database connection, command executer and reader objects
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader dr;

        public CRUDPost()
        {
            //estab;ishing the connection with database
            con = new SqlConnection(connectionString);
        }

        //retrieve all posts in the databse
        public List<Post> GetAllPosts()
        {
            try
            {
                //list of all the posts of database
                List<Post> list = new List<Post>();
                String query = "select * from posts";
                con.Close();
                con = new SqlConnection(connectionString);
                con.Open();
                cmd = new SqlCommand(query, con);
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    //populating the list from the records retrieved from the database
                    while (dr.Read())
                    {
                        list.Add(new Post { Id = dr.GetInt32(0), UserID = dr.GetInt32(1), Username = dr.GetString(2).Trim(), Title = dr.GetString(3).Trim(), Content = dr.GetString(4).Trim(), Date = dr.GetString(5),UserImage=dr.GetString(6).Trim() });
                    }
                }
                con.Close();
                //reversing the order of posts so that latest posts shows at the top
                list.Reverse();
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //dlete a post from the dataabase
        public bool DeletePost(int pid)
        {
            try
            {
                //query to delete post
                String query = $"delete from posts where Blogid = @pid";
                con.Open();
                cmd = new SqlCommand(query, con);
                cmd.Parameters.Add(new SqlParameter("@pid", pid));
                //executing the query
                int rowsDeleted = cmd.ExecuteNonQuery();
                con.Close();
                if (rowsDeleted > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //delete posts of a particular user
        public bool DeletePostByUserID(int uid)
        {
            try
            {
                //query to delete all the posts of a specific user
                String query = $"delete from posts where userid = @uid";
                con.Open();
                cmd = new SqlCommand(query, con);
                cmd.Parameters.Add(new SqlParameter("@uid", uid));
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();
                if (rowsAffected > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //insert a new post in the database written by a user
        public bool InsertPost(Post Blog)
        {
            try
            {
                String query = $"insert into posts (userid, username, title, content, curdatetime,userimage) values(@uid, @un, @t, @c, @dt,@ui)";
                con.Open();
                cmd = new SqlCommand(query, con);
                cmd.Parameters.Add(new SqlParameter("@uid", Blog.UserID));
                cmd.Parameters.Add(new SqlParameter("@un", Blog.Username.Trim()));
                cmd.Parameters.Add(new SqlParameter("@t", Blog.Title.Trim()));
                cmd.Parameters.Add(new SqlParameter("@c", Blog.Content.Trim()));
                cmd.Parameters.Add(new SqlParameter("@dt",Blog.Date.Trim()  )  );
                cmd.Parameters.Add(new SqlParameter("@ui", Blog.UserImage.Trim()));
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();
                if (rowsAffected > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //retrieve a particular posts from the database
        public Post GetPost(int pid)
        {
            try
            {
                con.Close();
                con = new SqlConnection(connectionString);
                con.Open();
                Post post = null;
                String query = $"select * from posts where Blogid = @pid";
                cmd = new SqlCommand(query, con);
                cmd.Parameters.Add(new SqlParameter("@pid", pid));
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    //creating an instance of post and initializing it according to retrieved data
                    dr.Read();
                    post = new Post { Id = dr.GetInt32(0), UserID = dr.GetInt32(1), Username = dr.GetString(2).Trim(), Title = dr.GetString(3).Trim(), Content = dr.GetString(4).Trim(), Date = dr.GetString(5),UserImage=dr.GetString(6).Trim() };
                }
                dr.Close();
                con.Close();
                return post;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //updates a specific post in the dadtabse
        public bool UpdatePost(Post Blog)
        {
            try
            {
                String query = $"update posts set title = @t, content = @c, curdatetime = @dt where Blogid = @pid";
                con.Open();
                cmd = new SqlCommand(query, con);
                cmd.Parameters.Add(new SqlParameter("@t", Blog.Title.Trim()));
                cmd.Parameters.Add(new SqlParameter("@c", Blog.Content.Trim()));
                cmd.Parameters.Add(new SqlParameter("@dt", Blog.Date.Trim()));
                cmd.Parameters.Add(new SqlParameter("@pid", Blog.Id));
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();
                if(rowsAffected > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //updates the photo of all the posts written by a user
        public bool updatePhoto(string uname, string photo)
        {
            try
            {
                String query = $"update posts set userimage = @ph where username = @un";
                con.Open();
                cmd = new SqlCommand(query, con);
                cmd.Parameters.Add(new SqlParameter("@ph", photo.Trim()));
                cmd.Parameters.Add(new SqlParameter("@un", uname.Trim()));
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

        //updates the username of all the posts of a user
        public bool updateUsername(User u,string uname)
        {
            try
            {
                String query = $"update posts set username = @un where userid = @ui";
                con.Open();
                cmd = new SqlCommand(query, con);
                cmd.Parameters.Add(new SqlParameter("@un", uname.Trim()));
                cmd.Parameters.Add(new SqlParameter("@ui", u.Id));
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();
                if (rowsAffected > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

}
