

namespace Frontend.Model.User
{
    // <summary>
    /// Class <c>UserModel</c> represnts a user in the Model Layer
    /// with its email and password
    /// </summary>
    public class UserModel
    {

        /// <summary>
        /// Field <c>Email</c> represents the email of the user
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Field <c>Password</c> represents the password of the user
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// <c>UserModel</c> constructor
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <param name="password">The password of the user</param>
        public UserModel(string email, string password) { 
            
            Email = email;
            Password = password;
        }

    }
}
