using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using MockUserAdmin.Models;
using System.Data;
using System.Xml;
using MockUserAdmin.Domain;

namespace MockUserAdmin.Controllers
{
    public class UsersController : Controller
    {        
        public IUserViewModel viewModelProp {get; set; }

        //Constructor dependency injection...
        public UsersController(IUserViewModel viewModel)
        {
            viewModelProp = viewModel;
        }

        public ViewResult Index()
        {
            if (viewModelProp.sysUsers == null)
            {
                viewModelProp = getUserList();
            }                              
            return View(viewModelProp);
        }

        [HttpGet]
        public ViewResult UserForm()
        {
            return View(viewModelProp.sysUsers);
        }

        [HttpPost]
        public ViewResult UserForm(SysUser sysUser)
        {
            // INSERT SYSUSER...
            //   VALIDATED by ModelState:
            //   * Values exist for each required field.
            //   * EmailAddress is correct format. 
            //   VALIDATED in code, following:
            //   * EmailAddress is unique in the data.
            if (ModelState.IsValid)
            {
                string emailAddress = sysUser.EmailAddress;
                string password = sysUser.Password;
                string firstName = sysUser.FirstName;
                string lastName = sysUser.LastName;                

                if (viewModelProp.sysUsers == null)
                {
                    //Not a test... 
                    viewModelProp = getUserList();
                }
                
                IList<Models.SysUser> userList = viewModelProp.sysUsers;

                //Validate uniqueness of new user's email address...
                var match = from u in userList
                            where u.EmailAddress == emailAddress
                            select u;

                if (match.Count() > 0)
                {
                    // EmailAddress not unique in data.  Return to sender.
                    ViewBag.Message = "You already have a login.  No need to register again.";
                    return View("Index", viewModelProp);
                }
                else
                {
                    //Add user to the list...
                    userList.Add(sysUser);

                    //Sort records by email address, into a new collection...
                    IEnumerable<SysUser> orderedList = from u in userList
                                                    orderby u.EmailAddress
                                                    select u;
                    IList<SysUser> newIList = new List<SysUser>();

                    // Serialize list to XML... 
                    XmlDocument xmlUsers = new XmlDocument();
                    XmlDeclaration dec = xmlUsers.CreateXmlDeclaration("1.0", null, null);
                    xmlUsers.AppendChild(dec);
                    XmlElement root = xmlUsers.CreateElement("Users");
                    xmlUsers.AppendChild(root);
                    
                    foreach (Models.SysUser u in orderedList)
                    {
                        XmlElement nextUser = xmlUsers.CreateElement("User");
                        nextUser.SetAttribute("EmailAddress", u.EmailAddress);
                        nextUser.SetAttribute("Password", u.Password);
                        nextUser.SetAttribute("FirstName", u.FirstName);
                        nextUser.SetAttribute("LastName", u.LastName);

                        root.AppendChild(nextUser);
                        //add sorted member to new collection...
                        newIList.Add((Models.SysUser)u);
                    }
                    // replace unsorted List<> with new sorted one...
                    viewModelProp.sysUsers = newIList;

                    //Persist the updated Model:
                    PersistUsers(xmlUsers);
                }
                return View("Index", viewModelProp);
            }
            else
            {
                //Invalid, based on model field validation
                //performed in MockUserAdmin.Models.sysUsers.
                return View(viewModelProp.sysUsers);
            }
        }

        private void PersistUsers(XmlDocument xmlUsers)
        {
            try
            {
                //Place viewModel in session. 
                Session[SessionEnum.UserViewModel] = viewModelProp;
            }
            catch (Exception e)
            {
                string strMessage = e.Message;
            }
            try
            {
                //Serialize user list to file.  
                xmlUsers.Save(Server.MapPath("~/Domain/Users.xml"));
            }
            catch (Exception e)
            {
                string strMessage = e.Message;
            }
        }

        private IUserViewModel getUserList()
        {

            if (Session[SessionEnum.UserViewModel] != null)
            {
                viewModelProp = (UserViewModel)Session[SessionEnum.UserViewModel];
            }

            IList<Models.SysUser> userList = new List<Models.SysUser>();

            string xmlFile = Server.MapPath("~/Domain/Users.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);

            string emailAddress = string.Empty;
            string password = string.Empty;
            string firstName = string.Empty;
            string lastName = string.Empty;

            Models.SysUser user = null;
            viewModelProp.sysUsers = userList;

            XmlNodeList nodes = doc.GetElementsByTagName("User");

            //Deserialize xml file into IList<Models.SysUser>
            foreach (XmlNode node in nodes)
            {
                //create user and add to users collection...
                emailAddress = node.Attributes["EmailAddress"].Value;
                password = node.Attributes["Password"].Value;
                firstName = node.Attributes["FirstName"].Value;
                lastName = node.Attributes["LastName"].Value;

                user = new Models.SysUser
                {
                    EmailAddress = emailAddress,
                    Password = password,
                    FirstName = firstName,
                    LastName = lastName
                };

                userList.Add(user);
            }
            return viewModelProp;
        }
    }
}
