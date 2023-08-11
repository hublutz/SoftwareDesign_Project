using IntroSE.Kanban.Backend.BusinessLayer.Board;
using IntroSE.Kanban.Backend.BusinessLayer.BoardController;
using IntroSE.Kanban.Backend.BusinessLayer.Task;
using IntroSE.Kanban.Backend.BusinessLayer.UserController;
using IntroSE.Kanban.Backend.ServiceLayer.TaskServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntroSE.Kanban.Backend.ServiceLayer.BoardService
{
    /// <summary>
    /// Class <c>BoardService</c> operates <c>Board</c> related actions
    /// in the Service Layer
    /// </summary>
    public class BoardService
    {
        /// <value>
        /// <c>BoardController</c> is used to connect with the business layer regarding
        /// board operations
        /// </value>
        private readonly BoardController boardController;

        /// <summary>
        /// Constructor of <c>BoardService</c>
        /// </summary>
        /// <param name="boardController">The controller regarding Board 
        /// operations in the Business Layer</param>
        internal BoardService(BoardController boardController) 
        {
            this.boardController = boardController;
        }

        /// <summary>
        /// This method adds a new board for the given user
        /// </summary>
        /// <param name="email">The user's email. The user must be logged in</param>
        /// <param name="boardName">The new board's name. The user must not have 
        /// an existing board of such name</param>
        /// <returns>Returns an empty response json if the creation succeeded. 
        /// Else, the json will include the error</returns>
        public string AddBoard(string email, string boardName)
        {
            Response response;
            try
            {
                this.boardController.AddBoard(email, boardName);
                response = new Response(ResponseCode.OperationSucceededCode);
            }
            catch (Exception ex)
            {
                response = new Response(ResponseCode.OperationFailedCode, ex.Message);
            }

            return response.ToJson();
        }

        /// <summary>
        /// This method deletes a user's board
        /// </summary>
        /// <param name="email">The user's email. The user must be logged in and own the board</param>
        /// <param name="boardName">The name of the board to delete</param>
        /// <returns>Returns an empty response json if the deletion succeeded. 
        /// Else, the json will include the error</returns>
        public string DeleteBoard(string email, string boardName)
        {
            Response response;
            try
            {
                this.boardController.DeleteBoard(email, boardName);
                response = new Response(ResponseCode.OperationSucceededCode);
            }
            catch (Exception ex)
            {
                response = new Response(ResponseCode.OperationFailedCode, ex.Message);
            }

            return response.ToJson();
        }

        /// <summary>
        /// This method returns all the boards the user owns
        /// </summary>
        /// <param name="email">The user's email. The user needs to be logged in</param>
        /// <returns>Returns a response json with all the user's boards' names, 
        /// if no error occurred</returns>
        public string GetAllUserBoards(string email)
        {
            Response response;
            try
            {
                List<Board> boards = this.boardController.GetAllUserBoards(email);

                List<BoardToSend> boardsToSends = boards.Select(board => new BoardToSend(board.Name)).ToList();
                string[] boardNames = boardsToSends.Select(board => board.NAME).ToArray();

                response = new Response(ResponseCode.OperationSucceededCode, boardNames);
            }
            catch (Exception ex)
            {
                response = new Response(ResponseCode.OperationFailedCode, ex.Message);
            }

            return response.ToJson();
        }

        /// <summary>
        /// This method gets the limit upon the amount of tasks a board's certain
        /// column can have
        /// </summary>
        /// <param name="email">The user email. The user must be logged in and own the board</param>
        /// <param name="boardName">The board's name</param>
        /// <param name="columnOrdinal">The column id. First is 0, and increases by 1 for each column</param>
        /// <returns>A response contaning the column's task limit, if no error happened</returns>
        public string GetColumnLimit(string email, string boardName, 
            int columnOrdinal)
        {
            Response response;
            try
            {
                int columnLimit = this.boardController.GetColumnLimit(email, boardName, columnOrdinal);
                response = new Response(ResponseCode.OperationSucceededCode, columnLimit);
            }
            catch (Exception ex)
            {
                response = new Response(ResponseCode.OperationFailedCode, ex.Message);
            }

            return response.ToJson();
        }

        /// <summary>
        /// This method gets all the tasks in a board's column
        /// </summary>
        /// <param name="email">The user email. The user must be logged in and own the board</param>
        /// <param name="boardName">The board's name</param>
        /// <param name="columnOrdinal">The column id. First is 0, and increases by 1 for each column</param>
        /// <returns>A response json containing all the tasks the column has, unless an error happened</returns>
        public string GetColumn(string email, string boardName, 
            int columnOrdinal)
        {
            Response response;
            try
            {
                List<Task> columnTasks = this.boardController.GetColumn(email, boardName, columnOrdinal);

                TaskToSend[] tasksToSend = columnTasks.Select(task => new TaskToSend(task.Id, task.GetCreationTime(), 
                    task.DueDate, task.Title, task.Description, task.Assignee)).ToArray();

                response = new Response(ResponseCode.OperationSucceededCode, tasksToSend);
            }
            catch (Exception ex)
            {
                response = new Response(ResponseCode.OperationFailedCode, ex.Message);
            }

            return response.ToJson();
        }

        /// <summary>
        /// This method gets the name of a certain board's column
        /// </summary>
        /// <param name="email">The user email. The user must be logged in and own the board</param>
        /// <param name="boardName">The board's name</param>
        /// <param name="columnOrdinal">The column id. First is 0, and increases by 1 for each column</param>
        /// <returns>A response including the column name, if no error occurrs</returns>
        public string GetColumnName(string email, string boardName,
            int columnOrdinal)
        {
            Response response;
            try
            {
                string columnName = this.boardController.GetColumnName(email, boardName, columnOrdinal);
                response = new Response(ResponseCode.OperationSucceededCode, columnName);
            }
            catch (Exception ex)
            {
                response = new Response(ResponseCode.OperationFailedCode, ex.Message);
            }

            return response.ToJson();
        }

        /// <summary>
        /// This method sets the max amount of tasks a board's certain column can have
        /// </summary>
        /// <param name="email">The user email. The user must be logged in and own the board</param>
        /// <param name="boardName">The board's name</param>
        /// <param name="columnOrdinal">The column id. First is 0, and increases by 1 for each column</param>
        /// <param name="limit">The limit to set for the column</param>
        /// <returns>An empty response. Upon error, returns a response with the exception</returns>
        public string LimitColumn(string email, string boardName,
            int columnOrdinal, int limit)
        {
            Response response;
            try
            {
                this.boardController.LimitColumn(email, boardName, columnOrdinal, limit);
                response = new Response(ResponseCode.OperationSucceededCode);
            }
            catch (Exception ex)
            {
                response = new Response(ResponseCode.OperationFailedCode, ex.Message);
            }

            return response.ToJson();
        }

        /// <summary>		 
        /// This method adds a user as member to an existing board.		 
        /// </summary>		 
        /// <param name="email">The email of the user that joins the board. Must be logged in</param>		 
        /// <param name="boardID">The board's ID</param>		 
        /// <returns>An empty response Json, unless an error occurs </returns>
        public string JoinBoard(string email, int boardId)
        {
            Response response;
            try
            {
                this.boardController.JoinBoard(email, boardId);
                response = new Response(ResponseCode.OperationSucceededCode);
            }
            catch (Exception ex)
            {
                response = new Response(ResponseCode.OperationFailedCode, ex.Message);
            }

            return response.ToJson();
        }

        /// <summary>		 
        /// This method removes a user from the members list of a board.		 
        /// </summary>		 
        /// <param name="email">The email of the user. Must be logged in</param>		 
        /// <param name="boardID">The board's ID</param>		 
        /// <returns>An empty response Json, unless an error occurs </returns>	
        public string LeaveBoard(string email, int boardId)
        {
            Response response;
            try
            {
                this.boardController.LeaveBoard(email, boardId);
                response = new Response(ResponseCode.OperationSucceededCode);
            }
            catch (Exception ex)
            {
                response = new Response(ResponseCode.OperationFailedCode, ex.Message);
            }

            return response.ToJson();
        }

        /// <summary>		 
        /// This method returns a list of IDs of all user's boards.		 
        /// </summary>		 
        /// <param name="email">Email of the user. Must be logged in</param>		 
        /// <returns>A Json of a response with a list of IDs of all user's boards, unless an error occurs </returns>
        public string GetUserBoards(string email)
        {
            Response response;
            try
            {
                List<Board> boards = this.boardController.GetAllUserBoards(email);

                int[] boardsIds = boards.Select(board => board.Id).ToArray();

                response = new Response(ResponseCode.OperationSucceededCode, boardsIds);
            }
            catch (Exception ex)
            {
                response = new Response(ResponseCode.OperationFailedCode, ex.Message);
            }

            return response.ToJson();
        }

        /// <summary>		 
        /// This method returns a board's name		 
        /// </summary>		 
        /// <param name="boardId">The board's ID</param>		 
        /// <returns>A response Json with the board's name, unless an error occurs </returns>		 
        public string GetBoardName(int boardId)
        {
            Response response;
            try
            {
                string boardName = this.boardController.GetBoardName(boardId);
                response = new Response(ResponseCode.OperationSucceededCode, boardName);
            }
            catch (Exception ex)
            {
                response = new Response(ResponseCode.OperationFailedCode, ex.Message);
            }

            return response.ToJson();
        }

        /// <summary>		 
        /// This method transfers a board ownership.		 
        /// </summary>		 
        /// <param name="currentOwnerEmail">Email of the current owner. Must be logged in</param>		 
        /// <param name="newOwnerEmail">Email of the new owner</param>		 
        /// <param name="boardName">The name of the board</param>		 
        /// <returns>An empty response Json, unless an error occurs </returns>	
        public string TransferBoardOwnership(string currentOwnerEmail, string newOwnerEmail, string boardName)
        {
            Response response;
            try
            {
                this.boardController.TransferBoardOwnership(currentOwnerEmail, newOwnerEmail, boardName);
                response = new Response(ResponseCode.OperationSucceededCode);
            }
            catch (Exception ex)
            {
                response = new Response(ResponseCode.OperationFailedCode, ex.Message);
            }

            return response.ToJson();
        }

        /// <summary>
        /// loads the data in the boardController
        /// </summary>
        /// <returns>An empty response Json, unless an error occurs</returns>
        internal string LoadData()
        {
            try
            {
                boardController.LoadData();
                return new Response(ResponseCode.OperationSucceededCode).ToJson();

            }
            catch (Exception ex)
            {
                return new Response(ResponseCode.OperationFailedCode, ex.Message).ToJson();
            }
        }

    }
}
