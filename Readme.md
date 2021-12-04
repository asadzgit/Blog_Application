# Blog Application

* A simple Blog Application having two types of users; admin and regular users. 
* Admin can add, delete, and update all the users. 
* Simple users can only view others posts while registered users can create an account and can edit/update/delete posts written by themselves (like we all do on facebook/Instagram). They can also update their profile.

## How to run

(1)Uset the following credentials to login as an Admin
username = admin
password = admin

(2)
(a)--Create the following two tables in your local SQL server databse. just paste the following scripts
(b)--Copy the connection string of your databse (in which you have mad these tables) in the "connectionstring" variable in "CRUDUser" and "CRUDPost" files in Model folder.
------------------------------------------------------------------------
Script for 1st table:
------------------------------------------------------------------------
CREATE TABLE [dbo].[posts] (
    [Blogid]      INT          IDENTITY (1, 1) NOT NULL,
    [userid]      INT          NOT NULL,
    [username]    NCHAR (50)   NOT NULL,
    [title]       NCHAR (50)   NOT NULL,
    [content]     NCHAR (2000) NOT NULL,
    [curdatetime] NCHAR(50)     NOT NULL,
    [userimage]   NCHAR (200)  NOT NULL,
    PRIMARY KEY CLUSTERED ([Blogid] ASC)
);

------------------------------------------------------------------------
Script for 2nd table:
------------------------------------------------------------------------
CREATE TABLE [dbo].[users] (
    [userid]   INT         IDENTITY (1, 1) NOT NULL,
    [username] NCHAR (50)  NOT NULL,
    [password] NCHAR (20)  NOT NULL,
    [email]    NCHAR (50)  NOT NULL,
    [photo]    NCHAR (200) NOT NULL,
    PRIMARY KEY CLUSTERED ([userid] ASC)
);

