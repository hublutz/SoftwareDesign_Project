using IntroSE.Kanban.Backend.DataAccessLayer.UserDataManager;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace IntroSE.Kanban.Backend.BusinessLayer.UserController
{    
    /// <summary>
    /// The class that is responsible of all the user managging in the BL
    /// </summary>
    internal class UserController
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// the dictionary that stores all of the users in the system
        /// </summary>
        private Dictionary<string, User> users;
        /// <summary>
        /// the board controller of the system
        /// </summary>
        private BoardController.BoardController boardController;
        /// <summary>
        /// the password checker used to see if new passwords are legal
        /// </summary>
        private IPasswordChecker passwordChecker;
        /// <summary>
        /// the user control constractor
        /// </summary>
        public UserController(BoardController.BoardController boardController) { 
            this.users = new Dictionary<string, User>();
            this.boardController = boardController;
            this.passwordChecker = new NewPasswordChecker();

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
        /// <summary>
        /// the login method that logs in the user
        /// </summary>
        /// <param name="email">the email of the user to log in</param>
        /// <param name="password">the password that is inputed to the user</param>
        /// <exception cref="Exception">throws an exseption if log in is unsucsesfull <see cref="User.Loggin()"/></exception>
        public void Login(string email , string password) {
            if (users.ContainsKey(email)) {
                try
                {
                    if (users[email].Login(password)) {
                        Log.Info("the user \"" + email + "\" was logged in sucssesfuly");
                    }
                    else
                    {
                        throw new Exception("the user \"" + email + "\" had the wrong password");
                    }
      
                }catch (Exception ex)
                {
                    Log.Error("user \"" + email +"\" coulndnt log in exception Massage: " + ex.Message);
                    throw ex;
                }
            }
            else
            {
                Log.Error("user \"" + email + "\" doesnt exist");
                throw new Exception("user \""+email+"\" doesent exist");
            }
        
        }
        /// <summary>
        /// the method that registers a new user
        /// </summary>
        /// <param name="email">the email of the new user</param>
        /// <param name="password">the password of the new user</param>
        /// <exception cref="Exception">throws an exseption if the password is illegal or email is taken</exception>
        public void Register(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                Log.Error("Tried to register a new user but the given email is null or empty");
                throw new Exception("email can't be null or empty");
            }
            if (users.ContainsKey(email))
            {
                Log.Error("Tried to register a new user but the given email already exists");
                throw new Exception("The given Email already exists");
            }
            if (!passwordChecker.CheckPassword(password)) {
                Log.Error("Illegal password while registering");
                throw new Exception("Illegal password while registering");
            }
            
            try {
                User user = new User(email, password);
                users.Add(email, user);
                boardController.AddUserToBoardControllersUserRegistry(user);
                Log.Info("regestration of \""+ email+ "\" has been sucsesfull");
            }
            catch (Exception ex)
            {
                Log.Error("couldnt register exception Massage: " + ex.Message);
                throw ex;
            }
        }
        /// <summary>
        /// this method logs out this user
        /// </summary>
        /// <param name="email">the email of that logs out the user the this email</param>
        /// <exception cref="Exception"> throws an exeption if usre is already loged out or doent exist</exception>
        public void Logout(string email) {
            if(!users.ContainsKey(email)) {
                Log.Error("user \"" + email + "\" doesnt exist");
                throw new Exception("user \"" + email + "\" doesnt exist");
            }
            if (users[email].LoggedIn)
            {
                users[email].ChangeLogInStatus();
                Log.Info("user \"" + email +"\" has been succsessfully loged out");
            }
            else
            {
                Log.Error("user \"" + email + "\" allredy loged out");
                throw new Exception("user \""+email +"\" allredy loged out");
            }
        }

        /// <summary>
        /// loads all user data from DB to RAM
        /// </summary>
        public void LoadData()
        {
            UserDataManager userDataManager = new UserDataManager();

            List<UserDTO> userDTOS = userDataManager.LoadData();

            foreach(UserDTO userDTO in userDTOS)
            {
                User user = new User(userDTO);
                users.Add(user.Email, user);
                boardController.AddUserToBoardControllersUserRegistry(user);
            }
        }
    }
}
    