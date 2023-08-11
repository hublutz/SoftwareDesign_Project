using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.UserService;
using System.Text.Json;

namespace BackendTests.UserTests
{
    /// <summary>
    /// <c>abstract class</c> for all user tests to inherit from.
    /// </summary>
    internal abstract class AbstractUserTests
    {
        /// <value>
        /// Used to connect to the Service Layer.
        /// </value>
        protected UserService userServ;
        /// <value>
        /// Signifies whether any test has failed or not.
        /// </value>
        protected bool PassedAllTests = true;
        /// <summary>
        /// What kind of tests are done (Register/Login/Logout); 
        /// Inheriting classes should set data to this field in their constructor.
        /// </summary>
        protected string TypeOfTests;

        // Email constants for the various user tests.
        protected const string FULL_EMAIL = "JohnDoe88@bengu.com";
        protected const string FULL_EMAIL_2 = "Alice53@cl0se.craft";
        protected const string NO_USERNAME_EMAIL = "@GoliathSir.com";
        protected const string NO_DOMAIN_EMAIL = "B0bby@.com";
        protected const string NO_TOP_DOMAIN_EMAIL = "admin@lol7";

        // Password constants for the various user tests.
        protected const string VALID_PASS = "HelloM4Reader!";
        protected const string VALID_PASS_NO_SYM = "All4One";
        protected const string PASS_NO_LOWER = "ALL4ONE";
        protected const string PASS_NO_UPPER = "all4one";
        protected const string PASS_NO_NUMBER = "ALLfourONE";
        protected const string PASS_TOO_SHORT = "Ab8";
        protected const string PASS_TOO_LONG = "OnceUpon4TimeIn4LandFarFarAway";

        /// <summary>
        /// Initializes a new instance of an inherited class.
        /// </summary>
        public AbstractUserTests()
        {
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.DeleteData();
            userServ = serviceManager.UserService;
        }

        /// <summary>
        /// Runs user tests
        /// </summary>
        public abstract void RunTests();

        /// <summary>
        /// Tests a <see cref="UserService"/> operation while expecting it to succeed;
        /// </summary>
        /// <param name="resp">The Response from trying to use a <see cref="UserService"/> operation;</param>
        protected void UserTestExpectsSuccess(string resp)
        {
            Response respToCheck = JsonSerializer.Deserialize<Response>(resp);

            if (respToCheck.Code == ResponseCode.OperationSucceededCode)
            {
                printTestSucceeded(TypeOfTests + " success - test succeeded");
            }
            else
            {
                PassedAllTests = false;
                printTestFailed(TypeOfTests + " fail - test failed; " + respToCheck.Message);
            }
        }

        /// <summary>
        /// Tests a <see cref="UserService"/> operation while expecting it to fail;
        /// </summary>
        /// <param name="resp">The Response from trying to use a <see cref="UserService"/> operation;</param>
        protected void UserTestExpectsFailure(string resp)
        {
            Response respToCheck = JsonSerializer.Deserialize<Response>(resp);

            if (respToCheck.Code == ResponseCode.OperationSucceededCode)
            {
                PassedAllTests = false;
                printTestFailed(TypeOfTests + " success - test failed");
            }
            else
            {
                printTestSucceeded(TypeOfTests + " fail - test succeeded; " + respToCheck.Message);
            }

        }

        /// <summary>
        /// This method prints a test has succeeded
        /// </summary>
        /// <param name="message">A message indicating the test's purpose</param>
        protected void printTestSucceeded(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// This method prints a test has failed
        /// </summary>
        /// <param name="message">A message indicating the test's purpose</param>
        protected void printTestFailed(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("Test failed: " + message);

            Console.ResetColor();
        }

        protected void printAllTestsResult()
        {
            if (PassedAllTests)
                printTestSucceeded($"All the {TypeOfTests} tests have passed succefully.");

            else
                printTestFailed($"Some of the {TypeOfTests} tests have failed!");
        }
    }
}
