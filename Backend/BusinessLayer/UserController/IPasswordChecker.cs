namespace IntroSE.Kanban.Backend.BusinessLayer.UserController
{
    /// <summary>
    /// Interface <c>IPasswordChecker</c> is used to define laws regarding passwords
    /// new users register with
    /// </summary>
    internal interface IPasswordChecker
    {
        /// <summary>
        /// This method checks the given password is valid and meets the laws specified
        /// </summary>
        /// <param name="password">The password of the new user</param>
        /// <returns>true if the password is valid, false if exists a rule
        /// it doesn't meet</returns>
        public bool CheckPassword(string password);
    }
}
