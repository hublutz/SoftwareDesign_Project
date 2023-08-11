using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.BoardService;
using IntroSE.Kanban.Backend.ServiceLayer.TaskServices;
using IntroSE.Kanban.Backend.ServiceLayer.UserService;
using System.Text.Json;

namespace BackendTests.TaskTests
{
    /// <summary>
    /// <c>AbstractTaskTest</c> is an abstract class for tests related to Tasks.
    /// It includes common components for all task tests
    /// </summary>
    internal abstract class AbstractTaskTest
    {
        /// <value>
        /// <c>userService</c> is used for managing User operations in the Service Layer
        /// </value>
        protected UserService userService;
        /// <value>
        /// <c>boardService</c> is used for managing Board operations in the Service Layer
        /// </value>
        protected BoardService boardService;
        /// <value>
        /// <c>taskService</c> is used for managing Task operations in the Service Layer
        /// </value>
        protected TaskService taskService;
        /// <value>
        /// <c>id</c> is the id of the task added using:  "LoginAndAddTask()"
        /// </value>
        private int id;
        /// <value>
        /// <c>idBeenUpdated</c> says if <c>id</c> has been updated
        /// </value>
        private Boolean idBeenUpdated;

        /// <summary>
        ///  <c>id</c> getter
        /// </summary>
        protected int GetId()
        {
            if (idBeenUpdated)
            {
                return id;
            }
            else
            {
                throw new System.ArgumentNullException();
            }
        }

        /// <value>
        /// Constatnts used for adding a user, board or task
        /// </value>
        protected const string EMAIL1 = "testUser1@gmail.com";
        protected const string EMAIL2 = "testUser2@gmail.com";
        protected const string EMAIL3 = "testUser3@gmail.com";
        protected const string EMAIL4 = "testUser4@gmail.com";
        protected const string PASSWORD = "Aa12345678";
        protected const string BOARD_NAME = "testBoard";
        protected const string DUE_DATE = "12/12/2099 00:00:01";


        protected const int FIRST_BOARD_ID= 0;
        protected const int FIRST_COLUMN_ORDINAL_ID = 0;
        protected const int FIRST_TASK_ID = 0;


        /// <summary>
        /// Constructor of <c>AbstractBoardTest</c>. Initialises its three services and resets idBeenUpdated variable. 
        /// </summary>
        protected AbstractTaskTest() {
            ServiceManager serviceManager = new ServiceManager();
            serviceManager.DeleteData();
            userService = serviceManager.UserService;
            boardService = serviceManager.BoardService;
            taskService = serviceManager.TaskService;

            idBeenUpdated = false;

        }

        /// <summary>
        /// This method is used for running all test methods in a certain Task test class
        /// </summary>
        public abstract void RunTests();

        /// <summary>
        /// This method registers and logs in a new user and adds a board and a task.
        /// </summary>
        /// <param name="email">The email of the user to login</param>
        protected void RegisterAndAddTask(string email)
        {
            userService.Register(email, PASSWORD);
            boardService.AddBoard(email, BOARD_NAME);
            taskService.AddTask(email, BOARD_NAME, DUE_DATE, "title", "decription");

            this.id = GetTaskId(email,BOARD_NAME);
            
            idBeenUpdated = true; 
        }


        // <summary>
        /// This method logs out the user that logged in.
        /// </summary>
        /// <param name="email">The email of the user to logout</param>
        protected void Logout(string email) {
            userService.Logout(email);
        }

        /// <summary>
        /// This method gets the id of a newly created task.
        /// </summary>
        /// <param name="email">The email of the user that owns the task</param>
        /// <param name="BOARD_NAME">The boared name in which the task is located in</param>
        /// <returns> the id or throws an exseption if cant tell <returns> 
        protected int GetTaskId(string email, string BOARD_NAME)
        {
            string jsonColunm = boardService.GetColumn(email, BOARD_NAME, 0);
            object messege = JsonSerializer.Deserialize<Response>(jsonColunm).Message;
            TaskToSend[] column = JsonSerializer.Deserialize<TaskToSend[]>(messege.ToString());
            
            if (column.Length != 1)
                throw new Exception("cant tell id becuse more then 1 task in column");
            return column[0].Id;
        }
        /// <summary>
        /// Registers a new user, make him join the board and get assigned to a task
        /// <para>Only call this function once and only after <see cref="RegisterAndAddTask(string)"/></para>
        /// </summary>
        /// <param name="ownerEmail">The email of the board owner</param>
        protected void AssignOtherUserToTask(string ownerEmail)
        {
            userService.Register(EMAIL2, PASSWORD); // Register other user
            boardService.JoinBoard(EMAIL2, FIRST_BOARD_ID); // Other User join Board
            taskService.AssignTask(ownerEmail, BOARD_NAME, FIRST_TASK_ID, EMAIL2);  // Assign Task to other User
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
    }
}
