namespace IntroSE.Kanban.Backend.DataAccessLayer.UserDataManager
{
    /// <summary>
    /// Class <c>UserDTO</c> represents a user in <c>Users</c> table of the database,
    /// namely a row in the table
    /// </summary>
    internal class UserDTO
    {
        /// <summary>
        /// Constants representing columns related to users
        /// </summary>
        private const string EMAIL_COLUMN = "EMAIL";
        private const string PASSWORD_COLUMN = "PASSWORD";

        /// <value>
        /// Field <c>userDataManager</c> is used to communicate with the database
        /// </value>
        private UserDataManager userDataManager;

        /// <value>
        /// <c>Email</c> represents the email of the user
        /// </value>
        public string Email { get; private set; }

        /// <value>
        /// <c>Password</c> represents the password of the user
        /// </value>
        public string Password { get; private set; }

        /// <summary>
        /// <c>UserDTO</c> constructor, initialising the email and password
        /// </summary>
        /// <param name="email">The email of the user stored in the database</param>
        /// <param name="password">The password of the user stored in the database</param>
        public UserDTO(string email, string password) 
        { 
            this.Email = email;
            this.Password = password;
            this.userDataManager = new UserDataManager();
        }

        /// <summary>
        /// This method inserts this <c>UserDTO</c> to the database
        /// </summary>
        /// <returns></returns>
        public bool AddToDatabase()
        {
            return this.userDataManager.InsertDTO(this);
        }

        /// <summary>
        /// Setter of the user email. Also updates the user's email in the database
        /// </summary>
        /// <param name="email">The new user email</param>
        /// <returns>true if the update has succeeded, else false</returns>
        public bool SetEmail(string email)
        {
            bool updateResult = this.userDataManager.UpdateDTO(EMAIL_COLUMN, this.Email, EMAIL_COLUMN, email);
            if (updateResult) 
            {
                this.Email = email;
            }

            return updateResult;
        }

        /// <summary>
        /// Setter of the user password. Also updates the user's password in the database
        /// </summary>
        /// <param name="password">The new user password password</param>
        /// <returns>true if the update succeeded, else false</returns>
        public bool SetPassword(string password) 
        {
            bool updateResult = this.userDataManager.UpdateDTO(EMAIL_COLUMN, this.Email, PASSWORD_COLUMN, password);
            if (updateResult) 
            {
                this.Password = password;
            }

            return updateResult;
        }
    }
}
