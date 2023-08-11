using System.Linq;

namespace IntroSE.Kanban.Backend.BusinessLayer.UserController
{
    /// <summary>
    /// Class <c>NewPasswordChecker</c> validates a password of new user meets the following rules:
    /// <list type="bullet">
    ///     <item>The length should be between 6 to 20 characters</item>
    ///     <item>Should contain at least one upper case letter</item>
    ///     <item>Should contain at least one lower case letter</item>
    ///     <item>Should contain at least one digit</item>
    /// </list>
    /// </summary>
    internal class NewPasswordChecker : IPasswordChecker
    {
        /// <summary>
        /// Represnts the min length of a valid password
        /// </summary>
        private const int MIN_PASSWORD_LENGTH = 6;
        /// <summary>
        /// Represents the max length of a valid password
        /// </summary>
        private const int MAX_PASSWORD_LENGTH = 20;

        /// <summary>
        /// Checks the given password meets the rules specified in <see cref="NewPasswordChecker"/>
        /// </summary>
        /// <param name="password">The password of the new user</param>
        /// <returns>true if the password is valid, false if exists a rule
        /// it doesn't meet</returns>
        public bool CheckPassword(string password)
        {
            if (password == null) 
            {
                return false;
            }
            // check the length
            if (password.Length < MIN_PASSWORD_LENGTH || password.Length > MAX_PASSWORD_LENGTH)
            {
                return false;
            }
            // check the password contains a small letter, a capital letter and a digit
            if (!password.Any(char.IsLower) || !password.Any(char.IsUpper) || 
                !password.Any(char.IsDigit))
            {
                return false;
            }

            return true;
        }
    }
}
