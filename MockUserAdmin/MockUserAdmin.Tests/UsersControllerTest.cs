using MockUserAdmin.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using MockUserAdmin.Models;
using System.Web.Mvc;

namespace MockUserAdmin.Tests
{
    
    
    /// <summary>
    ///This is a test class for UsersControllerTest and is intended
    ///to contain all UsersControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UsersControllerTest
    {      
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Index 
        ///</summary>
        [TestMethod()]
        public void IndexTest()
        {
            //Get a manual mock repository (IUserViewModel instance) 
            //that includes two users, to be injected as a constructor parameter.
            IUserViewModel viewModel = new myUserViewModel();
            UsersController target = new UsersController(viewModel); 

            ViewResult actual = target.Index();
            IUserViewModel result = (IUserViewModel)actual.Model;

            Assert.AreEqual(result.sysUsers.Count, 2);
        }

        /// <summary>
        ///A test for Adding Valid User with good user params:
        ///</summary>
        [TestMethod()]
        public void UserForm_AddUserTest()
        {            
            IUserViewModel viewModel = new myUserViewModel();
            UsersController target = new UsersController(viewModel);
            SysUser sysUser = new MockUserAdmin.Models.SysUser
            {
                EmailAddress = "unique@123.com",
                Password = "unique",
                FirstName = "Unique",
                LastName = "User"
            };

            int UserCountBefore = viewModel.sysUsers.Count;
           
            ViewResult actual = target.UserForm(sysUser);
            IUserViewModel result = (IUserViewModel)actual.Model;

            int UserCountAfter = result.sysUsers.Count;
            //confirm a user was added...
            Assert.AreEqual(UserCountAfter, UserCountBefore + 1);
        }

        /// <summary>
        ///A test for rejecting new User with Bad email address format:
        ///</summary>
        [TestMethod()]
        public void UserForm_AddUserBadEmailTest()
        {
            IUserViewModel viewModel = new myUserViewModel();
            UsersController target = new UsersController(viewModel);
            SysUser sysUser = new MockUserAdmin.Models.SysUser
            {
                EmailAddress = "unique.123.com",
                Password = "unique",
                FirstName = "Unique",
                LastName = "User"
            };

            target.ModelState.AddModelError("EmailAddress", "Please enter valid Email Address");
            int UserCountBefore = viewModel.sysUsers.Count;

            ViewResult actual = target.UserForm(sysUser);
            IList<Models.SysUser> result = (IList<Models.SysUser>)actual.Model;          

            int UserCountAfter = result.Count;
            Assert.AreEqual(UserCountAfter, UserCountBefore );
        }

        /// <summary>
        ///A test for rejecting new User when missing required field:
        ///</summary>
        [TestMethod()]
        public void UserForm_AddUserMissingFieldTest()
        {
            IUserViewModel viewModel = new myUserViewModel();
            UsersController target = new UsersController(viewModel);
            SysUser sysUser = new MockUserAdmin.Models.SysUser
            {
                EmailAddress = "unique@123.com",
                Password = "unique",
                FirstName = "Unique",
                //LastName = "User"
            };
            target.ModelState.AddModelError("LastName", "Please enter Last Name");
            int UserCountBefore = viewModel.sysUsers.Count;
            
            
            ViewResult actual = target.UserForm(sysUser);
            IList<Models.SysUser> result = (IList<Models.SysUser>)actual.Model;

            int UserCountAfter = result.Count;
            Assert.AreEqual(UserCountAfter, UserCountBefore );
        }

        /// <summary>
        ///A test for rejecting new User with duplicate email address:
        ///</summary>
        [TestMethod()]
        public void UserForm_AddUserDuplicateEmailTest()
        {
            IUserViewModel viewModel = new myUserViewModel();
            UsersController target = new UsersController(viewModel);
            SysUser sysUser = new MockUserAdmin.Models.SysUser
            {
                EmailAddress = "user1@abc.com",
                Password = "user1",
                FirstName = "User1",
                LastName = "Test"
            };
            
            int UserCountBefore = viewModel.sysUsers.Count;

            ViewResult actual = target.UserForm(sysUser);
            IUserViewModel result = (IUserViewModel)actual.Model;

            int UserCountAfter = result.sysUsers.Count;
            Assert.AreEqual(UserCountAfter, UserCountBefore );
        }
    }
    ///<summary>
    ///Manual mock
    ///</summary>
    public class myUserViewModel : IUserViewModel
    {
        public IList<SysUser> sysUsers { get; set; }

        //constructor adds two users to the sysUser list
        public myUserViewModel()
        {
            SysUser usr1 = new SysUser
            {
                EmailAddress = "user1@abc.com",
                Password = "user1",
                FirstName = "User1",
                LastName = "Test"
            };
            SysUser usr2 = new SysUser
            {
                EmailAddress = "user2@abc.com",
                Password = "user2",
                FirstName = "User2",
                LastName = "Test2"
            };

            List<SysUser> userList = new List<SysUser>();
            userList.Add(usr1);
            userList.Add(usr2);

            //Attach class property value
            this.sysUsers = userList;
        }
    }
}
