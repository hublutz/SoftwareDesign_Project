namespace BackendTests.UserTests
{
    /// <summary>
    /// Class <c>LogoutTests</c> manages tests regarding user logging-out.
    /// </summary>
    internal class LogoutTests : AbstractUserTests
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogoutTests"/> class.
        /// </summary>
        public LogoutTests() : base()
        {
            TypeOfTests = "Logout";
        }

        /// <summary>
        /// Tests Requirement #8
        /// </summary>
        public override void RunTests()
        {
            // Try to Logout from a non existing Email - expect failure: User doesn't exist.
            UserTestExpectsFailure(userServ.Logout(FULL_EMAIL));

            // Register and then try to Logout with a different email - expect failure: User shouldn't exist.
            userServ.Register(FULL_EMAIL, VALID_PASS);
            UserTestExpectsFailure(userServ.Logout(FULL_EMAIL_2));

            // Try to Logout from an existing logged-in user with the correct Email- expect success;
            UserTestExpectsSuccess(userServ.Logout(FULL_EMAIL));

            // Try to Logout from an existing logged-OUT user with the correct Email- expect failure: Already logged-out;
            UserTestExpectsFailure(userServ.Logout(FULL_EMAIL));

            // Prints whether all the tests in this class have passed or not.
            printAllTestsResult();
        }
    }
}
