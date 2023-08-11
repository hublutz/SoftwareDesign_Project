using IntroSE.Kanban.Backend.ServiceLayer.BoardService;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Frontend.Model.Board
{
    /// <summary>
    /// Class <c>BoardControllerModel</c> is responsible for interacting
    /// with the Backend Service Layer for boards related operations
    /// </summary>
    public class BoardControllerModel
    {
        /// <value>
        /// Field <c>boardService</c> is the service ralated to boards
        /// </value>
        private BoardService boardService;

        /// <summary>
        /// <c>BoardControllerModel</c> constructor
        /// </summary>
        /// <param name="boardService">The board service created by the <c>BackendController</c>
        /// (<see cref="BoardController"/>)</param>
        public BoardControllerModel(BoardService boardService)
        {
            this.boardService = boardService;
        }

        /// <summary>
        /// This method gets all the board ids the user is a member of
        /// </summary>
        /// <param name="email">The user to get the boards of</param>
        /// <returns>Returns an array of board ids the user is a member of, 
        /// unless an error occurred</returns>
        /// <exception cref="Exception">Throws an exception if the Backend informs of an error</exception>
        public int[] GetUserBoardIds(string email)
        {
            Response response;
            string responseJson = this.boardService.GetUserBoards(email);
            response = JsonSerializer.Deserialize<Response>(responseJson);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                int[] boardIds = response.Message != null ? 
                    JsonSerializer.Deserialize<int[]>(response.Message.ToString()) : null;
                return boardIds;
            }
            else
            {
                throw new Exception(response.Message.ToString());
            }
        }

        /// <summary>
        /// This method gets all the boards the user is a member of
        /// </summary>
        /// <param name="email">The user to get the boards of</param>
        /// <returns>Returns a list of boards the user is a member of, 
        /// unless an error occurred</returns>
        public List<BoardModel> GetUserBoards(string email)
        {
            int[] boardIds = this.GetUserBoardIds(email);
            List<BoardModel> boards = new List<BoardModel>();
            Response response;

            foreach (int boardId in boardIds) 
            {
                string boardName = this.GetBoardName(boardId);
                boards.Add(new BoardModel(boardId, boardName));
            }

            return boards;
        }

        /// <summary>
        /// This method gets a name of a board by id
        /// </summary>
        /// <param name="boardId">The id of the board to get its name</param>
        /// <returns>Returns the name of the requested board</returns>
        /// <exception cref="Exception">Throws an exception if the Backend informs of an error</exception>
        public string GetBoardName(int boardId) 
        {
            Response response;
            string responseJson = this.boardService.GetBoardName(boardId);
            response = JsonSerializer.Deserialize<Response>(responseJson);

            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                return response.Message.ToString();
            }
            else
            {
                throw new Exception(response.Message.ToString());
            }
        }

        /// <summary>
        /// This method gets all the task located in the specified column of the board
        /// </summary>
        /// <param name="email">The email of the user, must be a member of the board</param>
        /// <param name="boardName">The board name to get the column of</param>
        /// <param name="columnOrdinal">The id of the column to get</param>
        /// <returns>A list of <c>TaskModel</c> representing the tasks of the column</returns>
        /// <exception cref="Exception">Throws an exception if the operation failed</exception>
        public List<TaskModel> GetColumn(string email, string boardName, int columnOrdinal)
        {
            Response response;
            string responseJson = this.boardService.GetColumn(email, boardName, columnOrdinal);
            response = JsonSerializer.Deserialize<Response>(responseJson);

            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                List<TaskModel> columnTasks = JsonSerializer.Deserialize<List<TaskModel>>(response.Message.ToString());
                return columnTasks;
            }
            else
            {
                throw new Exception(response.Message.ToString());
            }
        }

        /// <summary>
        /// This method gets a name of a column in a board
        /// </summary>
        /// <param name="email">The user enrolled to the board</param>
        /// <param name="boardName">The name of the board containing the column</param>
        /// <param name="columnOrdinal">The ordinal of the column to get</param>
        /// <returns>Returns the name of the requested column</returns>
        /// <exception cref="Exception">Throws an exception if the Backend informs of an error</exception>
        public string GetColumnName(string email, string boardName, int columnOrdinal)
        {
            Response response;
            string responseJson = this.boardService.GetColumnName(email, boardName, columnOrdinal);
            response = JsonSerializer.Deserialize<Response>(responseJson);

            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                return response.Message.ToString();
            }
            else
            {
                throw new Exception(response.Message.ToString());
            }
        }

        /// <summary>
        /// This method gets a limit of a column in a board
        /// </summary>
        /// <param name="email">The user enrolled to the board</param>
        /// <param name="boardName">The name of the board containing the column</param>
        /// <param name="columnOrdinal">The ordinal of the column to get</param>
        /// <returns>Returns the limit of the requested column</returns>
        /// <exception cref="Exception">Throws an exception if the Backend informs of an error</exception>
        public int GetColumnLimit(string email, string boardName, int columnOrdinal)
        {
            Response response;
            string responseJson = this.boardService.GetColumnLimit(email, boardName, columnOrdinal);
            response = JsonSerializer.Deserialize<Response>(responseJson);

            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                return int.Parse(response.Message.ToString());
            }
            else
            {
                throw new Exception(response.Message.ToString());
            }
        }
    }
}
