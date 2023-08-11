using System;
using IntroSE.Kanban.Backend.DataAccessLayer.UserDataManager;
using log4net;

namespace IntroSE.Kanban.Backend.BusinessLayer.UserController
{
    // <summary>
    /// this class represents the User in the BL
    /// </summary>
    internal class User
    {
        /// <summary>
        /// the email field of the user
        /// </summary>
        private string email;
        /// <summary>
        /// the password field parameter
        /// </summary>
        private string password;
        /// <summary>
        /// the field that says if the user is logged in
        /// </summary>
        private bool loggedIn;

        /// <summary>
        /// Field <c>userDTO</c> represents the User as a DTO in the DataAccessLayer
        /// </summary>
        private UserDTO userDTO;

        /// <summary>
        /// The constractor of this class. creates a new User from an email and a password
        /// </summary>
        /// <param name="email">the email of the new User</param>
        /// <param name="password">the password of the new User</param>
        public User(string email, string password)
        {
            this.email = email;
            this.password = password;
            this.loggedIn = true;

            this.userDTO = new UserDTO(email, password);
            this.userDTO.AddToDatabase();
        }

        /// <summary>
        /// Constructor of <c>User</c>. Initializes its fields from the received
        /// DTO. The user won't be logged in
        /// This constructor should be used after loading data from the Data Access Layer,
        /// as it doesn't insert the user
        /// </summary>
        /// <param name="userDTO"><c>UserDTO</c> that was received from loading the data</param>
        public User(UserDTO userDTO)
        {
            this.email = userDTO.Email;
            this.password = userDTO.Password;
            this.loggedIn = false;
            this.userDTO = userDTO;
        }

        /// <summary>
        /// the email field getter and setter
        /// </summary>
        public string Email { get => this.email;
            set 
            {
                if (this.userDTO.SetEmail(value))
                {
                    this.email = value;
                }
                else
                {
                    throw new Exception("Failed to update the email");
                }
            } 
        }
        /// <summary>
        /// the password field setter
        /// </summary>
        public string Password { set 
            { 
                if (this.userDTO.SetPassword(value))
                {
                    this.password = value;
                }
                else
                {
                    throw new Exception("Failed to update the password");
                }
            } 
        }
        /// <summary>
        /// the getter of the LoggedIn variable
        /// </summary>
        public bool LoggedIn { get => loggedIn; }
        /// <summary>
        /// changes loggedIn to the opposite status of its current state
        /// </summary>
        public void ChangeLogInStatus() { 
            loggedIn = !loggedIn;
        }
        /// <summary>
        /// logs in this user if the right password is given
        /// </summary>
        /// <param name="password">the password that is given</param>
        /// <returns> true if login was sucssesfull and false otherwise</returns>
        /// <exception cref="Exception">throws an exeption if user is currently logged in</exception>
        public bool Login(string password) {
            if(loggedIn)
            {
                throw new Exception("already looged in");
            }
            if (password == this.password)
                ChangeLogInStatus();
            return loggedIn;
        }
        
    }
}
