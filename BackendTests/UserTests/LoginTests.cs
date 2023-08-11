namespace BackendTests.UserTests
{
    /// <summary>
    /// Class <c>LoginTests</c> manages tests regarding user logging.
    /// </summary>
    internal class LoginTests : AbstractUserTests
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginTests"/> class.
        /// </summary>
        public LoginTests() : base()
        {
            TypeOfTests = "Login";
        }

        /// <summary>
        /// Tests Requirements #1 and #8
        /// </summary>
        public override void RunTests()
        {
            /// Logins expecting failures:

            // Try to Login with a non existing Email - expect failure: User doesn't exist.
            UserTestExpectsFailure(userServ.Login(FULL_EMAIL, VALID_PASS));

            // Register and then try to Login with same email and password - expect failure: Already logged in.
            userServ.Register(FULL_EMAIL, VALID_PASS);
            UserTestExpectsFailure(userServ.Login(FULL_EMAIL, VALID_PASS));

            // Try to Login to an existing logged-out user with the wrong Password- expect failure: Wrong Password.
            userServ.Logout(FULL_EMAIL);
            UserTestExpectsFailure(userServ.Login(FULL_EMAIL, PASS_TOO_SHORT));

            // Try to Login with a different email and same password - expected failure: User shouldn't exist.
            UserTestExpectsFailure(userServ.Login(FULL_EMAIL_2, VALID_PASS));


            /// Logins expecting success:

            // Try to Login to an existing logged-out user with the right Password- expect success;
            UserTestExpectsSuccess(userServ.Login(FULL_EMAIL, VALID_PASS));

            // Prints whether all the tests in this class have passed or not.
            printAllTestsResult();
        }
    }
}
