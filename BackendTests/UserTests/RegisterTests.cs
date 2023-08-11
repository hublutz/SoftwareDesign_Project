namespace BackendTests.UserTests
{
    /// <summary>
    /// Class <c>RegisterTests</c> manages tests regarding user registration.
    /// </summary>
    internal class RegisterTests : AbstractUserTests
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterTests"/> class.
        /// </summary>
        public RegisterTests() : base()
        {
            TypeOfTests = "Register";
        }

        /// <summary>
        /// Tests Requirements #1, #2, #3 and #7 
        /// </summary>
        public override void RunTests()
        {
            // Try to register with valid email and password - expect success
            UserTestExpectsSuccess(userServ.Register(FULL_EMAIL, VALID_PASS));

            // Try to register again with same Email to test registration of existing user - expect failure
            UserTestExpectsFailure(userServ.Register(FULL_EMAIL, VALID_PASS_NO_SYM));

            // Try to register a different user with a different email and same password - expected success
            UserTestExpectsSuccess(userServ.Register(FULL_EMAIL_2, VALID_PASS));

            // logout and try to register with the same email again - expected failure.
            userServ.Logout(FULL_EMAIL);
            UserTestExpectsFailure(userServ.Register(FULL_EMAIL, VALID_PASS));

            // Try to register again with a new email and a valid password
            UserTestExpectsSuccess(userServ.Register(NO_USERNAME_EMAIL, VALID_PASS));


            /// Try to register with invalid passwords:

            // Expected Error - Password must contain at least <<1 lowercase>>, 1 uppercase and 1 number;
            UserTestExpectsFailure(userServ.Register(NO_TOP_DOMAIN_EMAIL, PASS_NO_LOWER));

            // Expected Error - Password must contain at least 1 lowercase, <<1 uppercase>> and 1 number;
            UserTestExpectsFailure(userServ.Register(NO_TOP_DOMAIN_EMAIL, PASS_NO_UPPER));

            // Expected Error - Password must contain at least 1 lowercase, 1 uppercase and <<1 number>>;
            UserTestExpectsFailure(userServ.Register(NO_TOP_DOMAIN_EMAIL, PASS_NO_NUMBER));

            // Expected Error - Password must be 6 or more characters;
            UserTestExpectsFailure(userServ.Register(NO_TOP_DOMAIN_EMAIL, PASS_TOO_SHORT));

            // Expected Error - Password must be 20 or less characters;
            UserTestExpectsFailure(userServ.Register(NO_TOP_DOMAIN_EMAIL, PASS_TOO_LONG));

            // Prints whether all the tests in this class have passed or not.
            printAllTestsResult();
        }
    }
}
