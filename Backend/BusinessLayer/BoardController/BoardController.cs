using IntroSE.Kanban.Backend.BusinessLayer.UserController;

using System;
using System.Collections.Generic;

using log4net;
using System.Linq;
using IntroSE.Kanban.Backend.DataAccessLayer.BoardDataManager;
using IntroSE.Kanban.Backend.DataAccessLayer.ColumnDataManager;

namespace IntroSE.Kanban.Backend.BusinessLayer.BoardController
{
    /// <summary>
    /// Class <see cref="BoardController"/> is responsible for mangaging all 
    /// the boards in the <c>BusinessLayer</c>
    /// </summary>
    internal class BoardController
    {
        /// <summary>
        /// Saves the Id the next board that will be created will have
        /// </summary>
        private int nextBoardId;

        /// <summary>
        /// <c>boardsAndIds</c> is a dictionary that connects between the
        /// BoardId and the Board itself.
        /// </summary>
        private Dictionary<int, Board.Board> boardsAndIds;

        /// <summary>
        /// the dictionary that stores all of the users in the system
        /// </summary>
        private Dictionary<string, User> users;

        /// <summary>
        /// The Logger instance
        /// </summary>
        private static readonly ILog Log = 
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The default Id value for the first created board in the system.
        /// </summary>
        private const int DEFAULT_FIRST_BOARD_ID = 0;

        /// <summary>
        /// Initalizes the <see cref="usersAndBoards"/> field.
        /// </summary>
        public BoardController()
        {
            this.boardsAndIds = new Dictionary<int, Board.Board>();
            this.users = new Dictionary<string, User>();
            this.nextBoardId = DEFAULT_FIRST_BOARD_ID;
        }

        /// <summary>
        ///  and a new Item to the field <see cref="users"/>.
        /// *Should be called from the <see cref="UserController.UserController"/>
        /// </summary>
        /// <param name="user"> The user to add</param>
        /// <exception cref="Exception">When the user already exists</exception>
        public void AddUserToBoardControllersUserRegistry(User user)
        {
            if(this.users.ContainsKey(user.Email))
            {
                Log.Error($"Failed to add user '{user.Email}' to the BoardController's user registry as it already exists.");
                throw new Exception("The following user already exists:  " + user.Email);
            }

            users[user.Email] = user;
            Log.Info($"Successfully added user '{user.Email}' to the BoardController's user registry.");
        }

        /// <summary>
        /// Creates a new board and adds the board and the Id to the dictionary field <see cref="boardsAndIds"/>;   
        /// </summary>
        /// <param name="email"> the owners email and identifier</param>
        /// <param name="boardName"> the board to add</param>
        /// <exception cref="Exception">When the board name is illegal or already exists</exception>
        public void AddBoard(string email, string boardName)
        {
            ThrowUserDoesntExistOrLoggedInException(email, "AddBoard()");
            if (string.IsNullOrWhiteSpace(boardName))
            {
                Log.Error("Failed to create a board because the given name is empty or null");
                throw new Exception("boardName can't be empty or null");
            }
            bool boardExists = GetAllUserBoards(email).Exists(x => x.Name == boardName);

            if (boardExists)
            {
                Log.Error($"Failed to create a board with the name '{boardName}' under user '{email}' as " +
                    "the user already got a board with said name");
                throw new Exception("Illegal board name:  " + boardName);
            }

            Board.Board board = new Board.Board(boardName, nextBoardId, email);
            boardsAndIds.Add(nextBoardId++, board);
            
            Log.Info($"Successfully created a board named '{boardName}' under user '{email}'.");
        }

        /// <summary>
        /// Deletes an existing board and removes the board and the Id from the dictionary field <see cref="boardsAndIds"/>;   
        /// </summary>
        /// <param name="email"> the owner's email and identifier</param>
        /// <param name="boardName"> the name of the board to delete</param>
        /// <exception cref="Exception">When the board name is illegal or doesn't exist</exception>
        public void DeleteBoard(string email, string boardName)
        {
            ThrowUserDoesntExistOrLoggedInException(email, "DeleteBoard()");
            if (string.IsNullOrWhiteSpace(boardName))
            {
                Log.Error("Failed to Delete a board because the given name is empty or null");
                throw new Exception("boardName can't be empty or null");
            }
            Board.Board board = GetAllUserBoards(email).Find(x => x.Name == boardName);

            if (board == null)
            {
                Log.Error($"Failed to Delete board '{boardName}' as the user '{email}' is not a member of the board.");
                throw new Exception("The given user is not a member of a board named " + boardName);
            }

            if(board.BoardOwnerEmail !=  email)
            {
                Log.Error($"Failed to Delete board '{boardName}' as the user '{email}' is not the owner of the board.");
                throw new Exception("The given user is not the owner of a board named " + boardName);
            }

            boardsAndIds.Remove(board.Id);
            Log.Info($"Successfully deleted the board with the Id '{board.Id}' from user '{email}'.");
        }

        /// <summary>
        /// Gets an email and returns all of the user's board.
        /// </summary>
        /// <param name="email"> The user's email and identifier</param>
        /// <returns>A list containing all the boards the user is a member of.</returns>
        internal List<Board.Board> GetAllUserBoards(string email)
        {
            ThrowUserDoesntExistOrLoggedInException(email, "GetAllUserBoards()");

            List<Board.Board> boards = boardsAndIds.Values.ToList().FindAll(x => x.IsUserEnrolled(email));

            Log.Info($"Successfully returned all boards the user '{email}' is a member of.");
            return boards;
        }

        /// <summary>
        /// Gets an email and name of a board and returns the board said user is a member of.
        /// </summary>
        /// <param name="email"> The user's email and identifier</param>
        /// <param name="boardName"> The name of the board that will be returned</param>
        /// <returns>The board with the name of <c>boardName</c> that belongs to the 
        /// user with the <c>email</c></returns>
        internal Board.Board GetBoard(string email, string boardName)
        {
            int boardId = getBoardId(email, boardName);
            return boardsAndIds[boardId];
        }

        /// <summary>
        /// Makes a list containing a user's InProgress tasks and returns it.
        /// </summary>
        /// <param name="email">The user's email and identifier</param>
        /// <returns><c>List</c>&lt;Task&gt; containing the InProgress tasks of a user</returns>
        internal List<Task.Task> GetAllInProgressByUser(string email)
        {
            List<Task.Task> tasks = new List<Task.Task>();

            foreach (Board.Board board in GetAllUserBoards(email))
            {
                tasks.AddRange(board.GetColumn((int)Task.TaskState.InProgress));
            }

            tasks.RemoveAll(x => x.Assignee != email);

            Log.Info($"Successfully returned all the 'InProgress' tasks of user '{email}'.");
            return tasks;
        }

        /// <summary>
        /// Given an email, boardName and columnOrdinal - returns the column's limit;
        /// </summary>
        /// <param name="email"> The user's email and identifier</param>
        /// <param name="boardName"> Name of a board</param>
        /// <param name="columnOrdinal"> Value representing the column whos limit will be returned</param>
        /// <returns>The fitting column's Limit.</returns>
        /// <exception cref="Exception">When the given columnOrdinal is invalid.</exception>
        public int GetColumnLimit(string email, string boardName, int columnOrdinal)
        {
            Board.Board board = GetBoard(email, boardName);

            int columnLimit = board.GetColumnLimit(columnOrdinal);
            Log.Info($"Successfully returned the column of ordinal {columnOrdinal}'s limit" +
                $" from the board '{boardName}' under user '{email}'.");
            return columnLimit;
        }

        /// <summary>
        /// Given an email, boardName and columnOrdinal - returns the column;
        /// </summary>
        /// <param name="email"> The user's email and identifier</param>
        /// <param name="boardName"> Name of a board</param>
        /// <param name="columnOrdinal"> Value representing the column whose limit shall be returned</param>
        /// <returns>The fitting column</returns>
        /// <exception cref="Exception">When the given columnOrdinal is invalid.</exception>
        internal List<Task.Task> GetColumn(string email, string boardName, int columnOrdinal)
        {
            Board.Board board = GetBoard(email, boardName);

            List<Task.Task> column = board.GetColumn(columnOrdinal);
            Log.Info($"Successfully returned the column of ordinal {columnOrdinal}" +
                $" from the board '{boardName}' under user '{email}'.");
            return column;
        }

        /// <summary>
        /// Given an email, boardName and columnOrdinal - returns the column's name;
        /// </summary>
        /// <param name="email"> The user's email and identifier</param>
        /// <param name="boardName"> Name of a board</param>
        /// <param name="columnOrdinal"> Value representing the column whose name shall be returned</param>
        /// <returns>The fitting column's Name.</returns>
        /// <exception cref="Exception">When the given columnOrdinal is invalid.</exception>
        public string GetColumnName(string email, string boardName, int columnOrdinal)
        {
            Board.Board board = GetBoard(email, boardName);

            string columnName = board.GetColumnName(columnOrdinal);
            Log.Info($"Successfully returned the column's name - '{columnName}'" +
                $" from the board '{boardName}' under user '{email}'.");
            return columnName;
        }

        /// <summary>
        /// Given an email, boardName, columnOrdinal and limit;   
        /// Updates the corresponding column's limit to the given one.
        /// </summary>
        /// <param name="email"> The user's email and identifier</param>
        /// <param name="boardName"> Name of a board</param>
        /// <param name="columnOrdinal"> Value representing the column whos limit will be returned</param>
        /// <param name="limit"> The new limit of the column</param>
        public void LimitColumn(string email, string boardName, int columnOrdinal, int limit)
        {
            Board.Board board = GetBoard(email, boardName);

            board.LimitColumn(columnOrdinal, limit);
            Log.Info($"Successfully changed the column limit of the column with ordinal '{columnOrdinal}'" +
                $" from the board '{boardName}' under user '{email}'.");
        }

        /// <summary>
        /// Lets a user join a board, no need for permission from the owner.
        /// </summary>
        /// <param name="userEmail">The user's email and identifier</param>
        /// <param name="boardId"> The board's Id and identifier</param>
        /// <exception cref="Exception">When the given boardId is invalid or when the user is already in a board with the same name.</exception>
        public void JoinBoard(string userEmail, int boardId)
        {
            ThrowUserDoesntExistOrLoggedInException(userEmail, "JoinBoard()");
            if(!boardsAndIds.ContainsKey(boardId))
            {
                Log.Error($"Tried to Join a Board but the given boardId- '{boardId}' is invalid.");
                throw new Exception("boardId is invalid.");
            }

            string boardName = GetBoardName(boardId);
            if(GetAllUserBoards(userEmail).Exists(x => x.Name == boardName))
            {
                Log.Error($"Tried to Join a Board but the user is already in a board with the same name.");
                throw new Exception("A user can't join two boards with the same name.");
            }

            boardsAndIds[boardId].JoinBoard(userEmail);
            Log.Info(userEmail + " Successfully joined the board with the id: " + boardId);
        }

        /// <summary>
        /// Lets a user leave a board, no need for permission from the owner.
        /// </summary>
        /// <param name="userEmail">The user's email and identifier</param>
        /// <param name="boardId"> The board's Id and identifier</param>
        /// <exception cref="Exception">When the boardId is invalid.</exception>
        public void LeaveBoard(string email, int boardId)
        {
            ThrowUserDoesntExistOrLoggedInException(email, "LeaveBoard()");
            if (!boardsAndIds.ContainsKey(boardId))
            {
                Log.Error($"Tried to Leave a Board but the given boardId- '{boardId}' is invalid.");
                throw new Exception("boardId is invalid.");
            }
            
            boardsAndIds[boardId].LeaveBoard(email);
            Log.Info(email + " Successfully left the board with the id: " + boardId);
        }

        /// <summary>
        /// Lets a member of a board assign a ask to a member of the board.
        /// </summary>
        /// <param name="email">The assigner's email and identifier</param>
        /// <param name="boardName">The name of the board in which the task is located</param>
        /// <param name="taskId">The Id of the task to assign</param>
        /// <param name="emailAssignee">The assignee's email and identifier or null</param>
        /// <exception cref="Exception">When the board name is null</exception>
        public void AssignTask(string email, string boardName, int taskId, string emailAssignee)
        {
            if(emailAssignee != null)
            {
                ThrowUserDoesntExistException(emailAssignee, "AssignTask()");
            }
            ThrowUserDoesntExistOrLoggedInException(email, "AssignTask()");
         
            if(boardName == null) 
            {
                Log.Error("Tried to assign a task but the given board name is null");
                throw new Exception("The board name can't be null");
            }
            int boardId = getBoardId(email, boardName);

            boardsAndIds[boardId].AssignTask(email, taskId, emailAssignee);
            Log.Info($"Successfully assigned the task {taskId} to user {emailAssignee}");
        }

        /// <summary>
        /// This method will transfer the board ownership from the owner to a member of the board.
        /// </summary>
        /// <param name="curentOwnersEmail">The owner's email and identifier</param>
        /// <param name="newOwnersEmail">The email and identifier of the member who'll become the new owner</param>
        /// <param name="boardName">The name of the board to transfer.</param>
        /// <exception cref="Exception">When the board name is null.</exception>
        public void TransferBoardOwnership(string curentOwnersEmail, string newOwnersEmail, string boardName)
        {
            ThrowUserDoesntExistOrLoggedInException(curentOwnersEmail, "TransferBoardOwnership()");
            ThrowUserDoesntExistException(newOwnersEmail, "TransferBoardOwnership()");
            if (boardName == null)
            {
                Log.Error("Tried to transfer a board's ownership but the given board name is null");
                throw new Exception("The board name can't be null");
            }

            int boardId = getBoardId(curentOwnersEmail, boardName);
            Board.Board board = boardsAndIds[boardId];

            board.TransferBoardOwnership(curentOwnersEmail, newOwnersEmail);
        }

        /// <summary>		 
        /// This method returns a board's name		 
        /// </summary>		 
        /// <param name="boardId">The board's ID</param>		 
        /// <returns>The name of the board with the Id "boardId"</returns>		 
        public string GetBoardName(int boardId)
        {
            if (!boardsAndIds.ContainsKey(boardId))
            {
                Log.Error($"Tried to Get Board Name but the given boardId- '{boardId}' is invalid.");
                throw new Exception("boardId is invalid.");
            }

            return boardsAndIds[boardId].Name;
        }

        /// <summary>
        /// loads all board and task data
        /// </summary>
        public void LoadData()
        {
            BoardDataManager boardDataManager = new BoardDataManager(); 
            ColumnDataManager columnDataManager = new ColumnDataManager();
             
            List<BoardDTO> boardDTOs = boardDataManager.LoadData();
            foreach (BoardDTO boardDTO in boardDTOs) {
                this.boardsAndIds.Add(boardDTO.Id, new Board.Board(boardDTO, columnDataManager.SelectBoardColumns(boardDTO.Id)));
            }
            // finding the next board id
            this.nextBoardId = boardDTOs == null || boardDTOs.Count == 0 ? DEFAULT_FIRST_BOARD_ID :
                (boardDTOs.Max(x => x.Id) + 1);
        }


        /// <summary>
        /// Given a user identifier and a board name, checks if there exists 
        /// a board with such a name that the user is a part of
        /// </summary>
        /// <param name="email">The user's email and identifier</param>
        /// <param name="boardName">The name of the board to check</param>
        /// <returns>The Id of the board that fit the description</returns>
        /// <exception cref="Exception">In any case that the boardId can't be returned.</exception>
        private int getBoardId(string email, string boardName)
        {
            ThrowUserDoesntExistOrLoggedInException(email, "getBoardId()");

            if(string.IsNullOrWhiteSpace(boardName))
            {
                Log.Error("Failed to get BoardId as the given BoardName is empty or null");
                throw new Exception("boardName can't be empty or null");
            }

            Board.Board board = boardsAndIds.Values.FirstOrDefault(x => x.Name == boardName && x.IsUserEnrolled(email));

            if (board == null)
            {
                Log.Error($"Failed to get BoardId as the user '{email}' is not a member of the given BoardName.");
                throw new Exception($"The user must be a part of the board with the name '{boardName}'.");
            }

            Log.Info("Successfully returned a board Id.");
            return board.Id;
        }

        /// <summary>
        /// Private method to check if a user is not null, exists and is logged-in 
        /// before trying to execute various other methods.
        /// </summary>
        /// <param name="email"> the user's email and identifier</param>
        /// <exception cref="Exception">When the user is not logged-in</exception>
        private void ThrowUserDoesntExistOrLoggedInException(string email, string nameOfMethod)
        {
            ThrowUserDoesntExistException(email, nameOfMethod);

            if (!users[email].LoggedIn)
            {
                Log.Error($"Failed to execute the method '{nameOfMethod}' because the user '{email}' is not logged-in.");
                throw new Exception("The following user is not logged-in:  " + email);
            }
        }

        /// <summary>
        /// Private method to check if a user is null and exists before trying to execute various other methods.
        /// </summary>
        /// <param name="email"> the user's email and identifier</param>
        /// <exception cref="Exception">When the user is null or doesn't exist</exception>
        private void ThrowUserDoesntExistException(string email, string nameOfMethod)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                Log.Error($"Failed to execute the method '{nameOfMethod}' because the given email is empty or null");
                throw new Exception("The email can't be empty or null.");
            }

            if (!users.ContainsKey(email))
            {
                Log.Error($"Failed to execute the method '{nameOfMethod}' because the user '{email}' doesn't exist.");
                throw new Exception("The following user doesn't exist:  " + email);
            }
        }
    }


}
