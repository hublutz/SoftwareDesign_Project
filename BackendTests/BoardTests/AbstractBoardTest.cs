using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.BoardService;
using IntroSE.Kanban.Backend.ServiceLayer.UserService;
using IntroSE.Kanban.Backend.ServiceLayer.TaskServices;

using System.Text.Json;

namespace BackendTests.BoardTests
{
    /// <summary>
    /// <c>AbstractBoardTest</c> is an abstract class for tests related to boards.
    /// It includes common components of all board tests
    /// </summary>
    internal abstract class AbstractBoardTest
    {
        /// <value>
        /// <c>userService</c> is used for managing User operations in the Service Layer
        /// </value>
        protected UserService userService;
        /// <summary>
        /// <c>boardService</c> is used for managing Board operations in the Service Layer
        /// </summary>
        protected BoardService boardService;

        protected TaskService taskService;

        /// <summary>
        /// Constatnts used for adding a user or adding a board
        /// </summary>
        protected const string EMAIL1 = "alice@gmail.com";
        protected const string EMAIL2 = "bob@gmail.com";
        protected const string VALID_PASSWORD = "Aa2Aa2Aa2";
        protected const string BOARD_NAME = "board";

        /// <summary>
        /// Constructor of <c>AbstractBoardTest</c>. Initialises its two services.
        /// </summary>
        protected AbstractBoardTest()
        {
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.DeleteData();
            this.userService = serviceManager.UserService;
            this.boardService = serviceManager.BoardService;
            this.taskService = serviceManager.TaskService;
        }

        /// <summary>
        /// This method is used for running all test methods in a certain Board test class
        /// </summary>
        public abstract void RunTests();

        /// <summary>
        /// This method adds a new board using the <c>BoardService</c>
        /// </summary>
        /// <param name="email">The user to add a board to. Must be logged in</param>
        /// <param name="boardName">The new board to add</param>
        /// <returns>Returns the response from the Service Layer (see <see cref="Response"/>)</returns>
        protected Response addBoard(string email, string boardName)
        {
            string responseJson = this.boardService.AddBoard(email, boardName);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return response;
        }

        /// <summary>
        /// This method extracts a board id, assuming the user has only one board
        /// </summary>
        /// <param name="email">The email of the board member</param>
        /// <returns>The is of the board</returns>
        protected int extractBoardId(string email)
        {
            string responseJson = this.boardService.GetUserBoards(email);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            int boardId = -1;
            try
            {
                int[] boardIds = JsonSerializer.Deserialize<int[]>(response.Message.ToString());
                boardId = boardIds[0];
            }
            catch (Exception e)
            {
                printTestFailed("Could not extract board id " + e.Message);
            }
            return boardId;
        }

        /// <summary>
        /// This method prints a test has succeeded
        /// </summary>
        /// <param name="message">A message indicating the test's purpose</param>
        protected void printTestSucceeded(string message) 
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Test succeeded: " + message);
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

        /// <summary>
        /// gets the id of the first board
        /// </summary>
        /// <param name="userName">the user name</param>
        /// <returns>returns the id</returns>
        /// <exception cref="Exception">throws an exeptoin if the there is no boards</exception>
        public int getFirstBoardId(string userName)
        {
            string jsonBoards = this.boardService.GetUserBoards(userName);
            object messege = JsonSerializer.Deserialize<Response>(jsonBoards).Message;
            int[] ids = JsonSerializer.Deserialize<int[]>(messege.ToString());

            if (ids.Length == 0)
                throw new Exception("cant tell id becuse the user doesnt own any boards");
            return ids[0];
        }
    }
}
