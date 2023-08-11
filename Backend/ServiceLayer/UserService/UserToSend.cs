namespace IntroSE.Kanban.Backend.ServiceLayer.UserService
{
    /// <summary>
    /// Class <c>UserToSend</c> represents the user data which returns to the Presentation Layer
    /// </summary>
    internal class UserToSend
    {
        /// <summary>
        /// Constant that represent the user's identifier
        /// </summary>
        public readonly string EMAIL;
        /// <summary>
        /// Initializes a new instance of the <see cref="UserToSend"/> class.
        /// </summary>
        ///<param name="email"> the user's <c>email</c></param>
        public UserToSend(string email)
        {
            this.EMAIL = email;
        }
    }
}
