using IntroSE.Kanban.Backend.BusinessLayer.Board;
using IntroSE.Kanban.Backend.ServiceLayer.TaskServices;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// A class for grading your work <b>ONLY</b>. The methods are not using good SE practices and you should <b>NOT</b> infer any insight on how to write the service layer/business layer. 
    /// <para>
    /// Each of the class' methods should return a JSON string with the following structure (see <see cref="System.Text.Json"/>):
    /// <code>
    /// {
    ///     "ErrorMessage": &lt;string&gt;,
    ///     "ReturnValue": &lt;object&gt;
    /// }
    /// </code>
    /// Where:
    /// <list type="bullet">
    ///     <item>
    ///         <term>ReturnValue</term>
    ///         <description>
    ///             The return value of the function.
    ///             <para>
    ///                 The value may be either a <paramref name="primitive"/>, a <paramref name="Task"/>, or an array of of them. See below for the definition of <paramref name="Task"/>.
    ///             </para>
    ///             <para>If the function does not return a value or an exception has occorred, then the field should be either null or undefined.</para>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>ErrorMessage</term>
    ///         <description>If an exception has occorred, then this field will contain a string of the error message. Otherwise, the field will be null or undefined.</description>
    ///     </item>
    /// </list>
    /// </para>
    /// <para>
    /// An empty response is a response that both fields are either null or undefined.
    /// </para>
    /// <para>
    /// The structure of the JSON of a Task, is:
    /// <code>
    /// {
    ///     "Id": &lt;int&gt;,
    ///     "CreationTime": &lt;DateTime&gt;,
    ///     "Title": &lt;string&gt;,
    ///     "Description": &lt;string&gt;,
    ///     "DueDate": &lt;DateTime&gt;
    /// }
    /// </code>
    /// </para>
    /// </summary>
    public class GradingService
    {
        /// <value>
        /// <c>userService</c> is a member responsible for user related actions in the service layer
        /// (see: <see cref="UserService.UserService")./>
        /// </value>
        private UserService.UserService userService;

        /// <value>
        /// <c>taskService</c> is a member responsible for task related actions in the service layer
        /// (see: <see cref="BTaskServices.TaskService")./>
        /// </value>
        private TaskServices.TaskService taskService;

        /// <value>
        /// <c>boardService</c> is a member responsible for board related actions in the service layer
        /// (see: <see cref="BoardService.BoardService")./>
        /// </value>
        private BoardService.BoardService boardService;

        /// <value>
        /// <c>serviceManager</c> is responsible for initializing all the services
        /// </value>
        private ServiceManager serviceManager;

        public GradingService()
        {
            this.serviceManager = new ServiceManager();
            this.userService = this.serviceManager.UserService;
            this.boardService = this.serviceManager.BoardService;
            this.taskService = this.serviceManager.TaskService;
        }

        /// <summary>
        /// Constants that represent keys for the Json format requested in 
        /// <c>GradingService</c> (see <see cref="GradingService"/>) - 
        /// thus related to <c>GradingService</c> only
        /// </summary>
        private const string ERROR_MESSAGE_KEY = "ErrorMessage";
        private const string RETURN_VALUE_KEY = "ReturnValue";

        /// <summary>
        /// Turns a code and message to a response of the required format for <c>GradingService</c>.
        /// This method is related to <c>GradingService</c> only and used only in that class
        /// </summary>
        /// <param name="code">The code received from the service layer Response (see <see cref="ResponseCode"/>).</param>
        /// <param name="message">The desrialized message from the Response</param>
        /// <returns>A Json of the format requested for <c>GradingService</c> (see <see cref="GradingService"/>):
        /// <code>
        /// {
        ///     "ErrorMessage": &lt;string&gt;,
        ///     "ReturnValue": &lt;object&gt;
        /// }
        /// </code>
        /// </returns>
        private string CreateResponseJson(ResponseCode code, object message)
        {
            Dictionary<string, object> gradingServiceJson = new Dictionary<string, object>();

            switch (code)
            {
                case ResponseCode.OperationSucceededCode:
                    gradingServiceJson[ERROR_MESSAGE_KEY] = null;
                    gradingServiceJson[RETURN_VALUE_KEY] = message;
                    break;
                case ResponseCode.OperationFailedCode:
                    gradingServiceJson[ERROR_MESSAGE_KEY] = message;
                    gradingServiceJson[RETURN_VALUE_KEY] = null;
                    break;
                default:
                    gradingServiceJson[ERROR_MESSAGE_KEY] = "Error: invalid response code received";
                    gradingServiceJson[RETURN_VALUE_KEY] = null;
                    break;
            }
            return JsonSerializer.Serialize(gradingServiceJson);
        }


        /// <summary>
        /// This method registers a new user to the system.
        /// </summary>
        /// <param name="email">The user email address, used as the username for logging the system.</param>
        /// <param name="password">The user password.</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string Register(string email, string password)
        {
            string responseJson = userService.Register(email, password);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return CreateResponseJson(response.Code, response.Message);
        }


        /// <summary>
        ///  This method logs in an existing user.
        /// </summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <returns>A response with the user's email, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string Login(string email, string password)
        {
            string responseJson = userService.Login(email, password);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return CreateResponseJson(response.Code, response.Message);
        }


        /// <summary>
        /// This method logs out a logged in user. 
        /// </summary>
        /// <param name="email">The email of the user to log out</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string Logout(string email)
        {
            string responseJson = userService.Logout(email);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return CreateResponseJson(response.Code, response.Message);
        }

        /// <summary>
        /// This method limits the number of tasks in a specific column.
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string LimitColumn(string email, string boardName, int columnOrdinal, int limit)
        {
            string responseJson = this.boardService.LimitColumn(email, boardName, columnOrdinal, limit);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return CreateResponseJson(response.Code, response.Message);
        }

        /// <summary>
        /// This method gets the limit of a specific column.
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>A response with the column's limit, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string GetColumnLimit(string email, string boardName, int columnOrdinal)
        {
            string responseJson = this.boardService.GetColumnLimit(email, boardName, columnOrdinal);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return CreateResponseJson(response.Code, response.Message);
        }


        /// <summary>
        /// This method gets the name of a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>A response with the column's name, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string GetColumnName(string email, string boardName, int columnOrdinal)
        {
            string responseJson = this.boardService.GetColumnName(email, boardName, columnOrdinal);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return CreateResponseJson(response.Code, response.Message);
        }


        /// <summary>
        /// This method adds a new task.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string AddTask(string email, string boardName, string title, string description, DateTime dueDate)
        {
            string responseJson = this.taskService.AddTask(email, boardName, dueDate.ToString("O"), title, description);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return CreateResponseJson(response.Code, response.Message);
        }


        /// <summary>
        /// This method updates the due date of a task
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="dueDate">The new due date of the column</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string UpdateTaskDueDate(string email, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
        {
            string responseJson = this.taskService.EditDueDate(email, boardName, taskId, dueDate.ToString("O"));
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return CreateResponseJson(response.Code, response.Message);
        }


        /// <summary>
        /// This method updates task title.
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="title">New title for the task</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string UpdateTaskTitle(string email, string boardName, int columnOrdinal, int taskId, string title)
        {
            string responseJson = this.taskService.EditTitle(email, boardName, taskId, title);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return CreateResponseJson(response.Code, response.Message);
        }


        /// <summary>
        /// This method updates the description of a task.
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="description">New description for the task</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string UpdateTaskDescription(string email, string boardName, int columnOrdinal, int taskId, string description)
        {
            string responseJson = this.taskService.EditDescription(email, boardName, taskId, description);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return CreateResponseJson(response.Code, response.Message);
        }


        /// <summary>
        /// This method advances a task to the next column
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string AdvanceTask(string email, string boardName, int columnOrdinal, int taskId)
        {
            string responseJson = this.taskService.MoveTask(email, boardName, taskId);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return CreateResponseJson(response.Code, response.Message);
        }


        /// <summary>
        /// This method returns a column given it's name
        /// </summary>
        /// <param name="email">Email of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>A response with a list of the column's tasks, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string GetColumn(string email, string boardName, int columnOrdinal)
        {
            string responseJson = this.boardService.GetColumn(email, boardName, columnOrdinal);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                TaskToSend[] tasks = response.Message != null ? JsonSerializer.Deserialize<TaskToSend[]>(response.Message.ToString()) : null;
                return CreateResponseJson(response.Code, tasks);
            }
            else
            {
                return CreateResponseJson(response.Code, response.Message);
            }
        }


        /// <summary>
        /// This method creates a board for the given user.
        /// </summary>
        /// <param name="email">Email of the user, must be logged in</param>
        /// <param name="name">The name of the new board</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string CreateBoard(string email, string name)
        {
            string responseJson = this.boardService.AddBoard(email, name);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return CreateResponseJson(response.Code, response.Message);
        }


        /// <summary>
        /// This method deletes a board.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in and an owner of the board.</param>
        /// <param name="name">The name of the board</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string DeleteBoard(string email, string name)
        {
            string responseJson = this.boardService.DeleteBoard(email, name);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return CreateResponseJson(response.Code, response.Message);
        }


        /// <summary>
        /// This method returns all in-progress tasks of a user.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <returns>A response with a list of the in-progress tasks of the user, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string InProgressTasks(string email)
        {
            string responseJson = this.taskService.GetInProgressByUser(email);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                TaskToSend[] tasks = response.Message != null ? JsonSerializer.Deserialize<TaskToSend[]>(response.Message.ToString()) : null;
                return CreateResponseJson(response.Code, tasks);
            }
            else
            {
                return CreateResponseJson(response.Code, response.Message);
            }
        }

        /* FROM HERE: NEW METHODS FOR MILESTONE 2-3 */

        /// <summary>		 
        /// This method returns a list of IDs of all user's boards.		 
        /// </summary>		 
        /// <param name="email">Email of the user. Must be logged in</param>		 
        /// <returns>A response with a list of IDs of all user's boards, unless an error occurs (see <see cref="GradingService"/>)</returns>		 
        public string GetUserBoards(string email)
        {
            string responseJson = this.boardService.GetUserBoards(email);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                int[] boardIds = response.Message != null ? JsonSerializer.Deserialize<int[]>(response.Message.ToString()) : null;
                return CreateResponseJson(response.Code, boardIds);
            }
            return CreateResponseJson(response.Code, response.Message);
        }

        /// <summary>		 
        /// This method adds a user as member to an existing board.		 
        /// </summary>		 
        /// <param name="email">The email of the user that joins the board. Must be logged in</param>		 
        /// <param name="boardID">The board's ID</param>		 
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>		 
        public string JoinBoard(string email, int boardID)
        {
            string responseJson = this.boardService.JoinBoard(email, boardID);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return CreateResponseJson(response.Code, response.Message);
        }

        /// <summary>		 
        /// This method removes a user from the members list of a board.		 
        /// </summary>		 
        /// <param name="email">The email of the user. Must be logged in</param>		 
        /// <param name="boardID">The board's ID</param>		 
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>		 
        public string LeaveBoard(string email, int boardID)
        {
            string responseJson = this.boardService.LeaveBoard(email, boardID);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return CreateResponseJson(response.Code, response.Message);
        }

        /// <summary>		 
        /// This method assigns a task to a user		 
        /// </summary>		 
        /// <param name="email">Email of the user. Must be logged in</param>		 
        /// <param name="boardName">The name of the board</param>		 
        /// <param name="columnOrdinal">The column number. The first column is 0, the number increases by 1 for each column</param>		 
        /// <param name="taskID">The task to be updated identified a task ID</param>        		 
        /// <param name="emailAssignee">Email of the asignee user</param>		 
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>		 
        public string AssignTask(string email, string boardName, int columnOrdinal, int taskID, string emailAssignee)
        {
            string responseJson = this.taskService.AssignTask(email, boardName, taskID, emailAssignee);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return CreateResponseJson(response.Code, response.Message);
        }

        /// <summary>		 
        /// This method returns a board's name		 
        /// </summary>		 
        /// <param name="boardId">The board's ID</param>		 
        /// <returns>A response with the board's name, unless an error occurs (see <see cref="GradingService"/>)</returns>		 
        public string GetBoardName(int boardId)
        {
            string responseJson = this.boardService.GetBoardName(boardId);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return CreateResponseJson(response.Code, response.Message);
        }

        /// <summary>		 
        /// This method transfers a board ownership.		 
        /// </summary>		 
        /// <param name="currentOwnerEmail">Email of the current owner. Must be logged in</param>		 
        /// <param name="newOwnerEmail">Email of the new owner</param>		 
        /// <param name="boardName">The name of the board</param>		 
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>		 
        public string TransferOwnership(string currentOwnerEmail, string newOwnerEmail, string boardName)
        {
            string responseJson = this.boardService.TransferBoardOwnership(currentOwnerEmail, newOwnerEmail, boardName);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return CreateResponseJson(response.Code, response.Message);
        }

        ///<summary>This method loads all persisted data.		 
        ///<para>		 
        ///<b>IMPORTANT:</b> When starting the system via the GradingService - do not load the data automatically, only through this method.		 
        ///In some cases we will call LoadData when the program starts and in other cases we will call DeleteData. Make sure you support both options.		 
        ///</para>		 
        /// </summary>		 
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>		 
        public string LoadData()
        {
            string responseJson = this.serviceManager.LoadData();
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return CreateResponseJson(response.Code, response.Message);
        }

        ///<summary>This method deletes all persisted data.		 
        ///<para>		 
        ///<b>IMPORTANT:</b>		 
        ///In some cases we will call LoadData when the program starts and in other cases we will call DeleteData. Make sure you support both options.		 
        ///</para>		 
        /// </summary>		 
        ///<returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>		 
        public string DeleteData()
        {
            string responseJson = this.serviceManager.DeleteData();
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return CreateResponseJson(response.Code, response.Message);
        }
    }
}
