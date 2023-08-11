using IntroSE.Kanban.Backend.BusinessLayer.Board;
using IntroSE.Kanban.Backend.BusinessLayer.BoardController;
using IntroSE.Kanban.Backend.BusinessLayer.Task;

using System;
using System.Collections.Generic;
using System.Linq;

namespace IntroSE.Kanban.Backend.ServiceLayer.TaskServices
{

    /// <summary>
    /// Class <c>TaskService</c> represents all the services for Task in the service layer
    /// </summary>
    public class TaskService
	{
        /// <value>
        /// <c>boardController</c> is used to connect with the business layer regarding
        /// board operations
        /// </value>
        private readonly BoardController boardController;

        /// <summary>
        /// Constructor of <c>TaskService</c>, initalizes the <see cref="boardController"/> field.
        /// </summary>
        /// <param name="boardController">The controller regarding Board 
        /// operations in the Business Layer</param>
        internal TaskService(BoardController boardController)
        {
            this.boardController = boardController;
        }

        /// <summary>
        /// This method adds a Task to a Board
        /// </summary>
        /// <param name="userEmail">The user's email. The user that owns the board must be logged in</param>
        /// <param name="boardName">The name of the board to add to </param>
        /// <param name="dueDate">The dueDate of the task </param>
        /// <param name="title">The title of the task </param>
        /// <param name="description">The description of the task. default is empty string</param>
        /// <returns>Returns an empty response json if the addition succeeded. 
        /// Else, the json will include the error</returns>
        public string AddTask(string userEmail, string boardName, string dueDate, string title, string description = "")
        {
            Response response;
            try
            {
                Board board = this.boardController.GetBoard(userEmail, boardName);
                board.AddTask(dueDate, title, description);
                response = new Response(ResponseCode.OperationSucceededCode);
            }
            catch (Exception ex)
            {
                response = new Response(ResponseCode.OperationFailedCode, ex.Message);
            }

            return response.ToJson();
        }

        /// <summary>
        /// The method moves the task into the next column (from ‘backlog’ to ‘in progress’ or from ‘in progress’ to ‘done’)
        /// </summary>
        /// <param name="userEmail">The user's email. The user that owns the board that the task is located in must be logged in</param>
        /// <param name="boardName">The name of the board which the task is located in </param>
        /// <param name="taskId">The id of the task that is being moved </param>
        /// <returns>Returns an empty response json if the movement succeeded. 
        /// Else, the json will include the error</returns>
		public string MoveTask(string userEmail, string boardName, int taskId) 
		{
            Response response;
            try
            {
                Board board = this.boardController.GetBoard(userEmail, boardName);
                board.MoveTaskState(userEmail, taskId);
                response = new Response(ResponseCode.OperationSucceededCode);
            }
            catch (Exception ex)
            {
                response = new Response(ResponseCode.OperationFailedCode, ex.Message);
            }

            return response.ToJson();
        }

        /// <summary>
        /// This messege is used to edit the due date of a task
        /// </summary>
        /// <param name="userEmail">The user's email. The user that owns the board that the task is located in must be logged in</param>
        /// <param name="boardName">The name of the board which the task is located in </param>
        /// <param name="newDate">The new due date</param>
        /// <param name="taskId">The id of the task that is edited </param>
        /// <returns>Returns an empty response json if the editing was successful. 
        /// Else, the json will include the error</returns>
        public string EditDueDate(string userEmail, string boardName, int taskId, string newDate)
        {
            Response response;
            try
            {
                Task task = GetTask(userEmail, boardName, taskId);
                task.SetDueDate(userEmail, newDate);
                response = new Response(ResponseCode.OperationSucceededCode);
            }
            catch (Exception ex)
            {
                response = new Response(ResponseCode.OperationFailedCode, ex.Message);
            }

            return response.ToJson();
        }

        /// <summary>
        /// This messege is used to edit the title of a task
        /// </summary>
        /// <param name="userEmail">The user's email. The user that owns the board that the task is located in must be logged in</param>
        /// <param name="boardName">The name of the board which the task is located in </param>
        /// <param name="newTitle">The new title</param>
        /// <param name="taskId">The id of the task that is edited </param>
        /// <returns>Returns an empty response json if the editing was successful. 
        /// Else, the json will include the error</returns>
		public string EditTitle(string userEmail, string boardName, int taskId, string newTitle)
		{
            Response response;
            try
            {
                Task task = GetTask(userEmail, boardName, taskId);
                task.SetTitle(userEmail, newTitle);
                response = new Response(ResponseCode.OperationSucceededCode);
            }
            catch (Exception ex)
            {
                response = new Response(ResponseCode.OperationFailedCode, ex.Message);
            }

            return response.ToJson();
        }


        /// <summary>
        /// This messege is used to edit the description of a task
        /// </summary>
        /// <param name="userEmail">The user's email. The user that owns the board that the task is located in must be logged in</param>
        /// <param name="boardName">The name of the board which the task is located in </param>
        /// <param name="newDescription">The new description</param>
        /// <param name="taskId">The id of the task that is edited </param>
        /// <returns>Returns an empty response json if the editing is sucsesfull. 
        /// Else, the json will include the error</returns>
        public string EditDescription(string userEmail, string boardName, int taskId, string newDescription)
		{
            Response response;
            try
            {
                Task task = GetTask(userEmail, boardName, taskId);
                task.SetDescription(userEmail, newDescription);
                response = new Response(ResponseCode.OperationSucceededCode);
            }
            catch (Exception ex)
            {
                response = new Response(ResponseCode.OperationFailedCode, ex.Message);
            }

            return response.ToJson();
        }

        /// <summary>
        /// This methods returns all tasks of a user with the state of InProgress.
        /// </summary>
        /// <param name="email">The user's email and identifier</param>
        /// <returns>All the InProgress tasks of a user</returns>
        public string GetInProgressByUser(string email)
        {
            Response response;
            try
            {
                List<Task> tasks = this.boardController.GetAllInProgressByUser(email);

                TaskToSend[] tasksToSend = tasks.Select(task => new TaskToSend(task.Id, 
                    task.GetCreationTime(), task.DueDate, task.Title, task.Description, task.Assignee)).ToArray();

                response = new Response(ResponseCode.OperationSucceededCode, tasksToSend);
            }
            catch (Exception ex)
            {
                response = new Response(ResponseCode.OperationFailedCode, ex.Message);
            }

            return response.ToJson();
        }


        /// <summary>
        /// Used to get a user's task from the Business layer.
        /// </summary>
        /// <param name="email">The user's email and identifier</param>
        /// <param name="boardName">The user's board name and identifier</param>
        /// <param name="taskId">The user's board's task's identifer</param>
        /// <returns>Returns a user's task</returns>
        private Task GetTask(string email, string boardName, int taskId)
        {
            Board board = this.boardController.GetBoard(email, boardName);
            return board.GetTask(taskId);
        }

        /// <summary>		 
        /// This method assigns a task to a user		 
        /// </summary>		 
        /// <param name="email">Email of the user. Must be logged in</param>		 
        /// <param name="boardName">The name of the board</param>		 
        /// <param name="columnOrdinal">The column number. The first column is 0, the number increases by 1 for each column</param>		 
        /// <param name="taskID">The task to be updated identified a task ID</param>        		 
        /// <param name="emailAssignee">Email of the assignee user or null</param>		 
        /// <returns>An empty response Json, unless an error occurs </returns>	
        public string AssignTask(string email, string boardName, int taskID, string emailAssignee)
        {
            Response response;
            try
            {
                this.boardController.AssignTask(email, boardName, taskID, emailAssignee);
                response = new Response(ResponseCode.OperationSucceededCode);
            }
            catch (Exception ex)
            {
                response = new Response(ResponseCode.OperationFailedCode, ex.Message);
            }

            return response.ToJson();
        }
    }
}
