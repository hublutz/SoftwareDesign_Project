using Frontend.Model;
using Frontend.Model.Board;
using Frontend.Model.User;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Frontend.ViewModel
{
    /// <summary>
    /// Enum <c>TaskState</c> represents constants indicating the 
    /// column ordinals in the board
    /// </summary>
    public enum TaskState
    {
        Backlog = 0,
        InProgress = 1,
        Done = 2
    }

    /// <summary>
    /// Class <c>BoardViewModel</c> is the data context for the
    /// BoardView window
    /// </summary>
    public class BoardViewModel : NotifiableObject
    {
        /// <value>
        /// Property <c>NO_LIMIT</c> represents having no limit to a column's tasks
        /// </value>
        private const int NO_LIMIT = -1;
        
        private const string INFINITY = "∞";

        /// <value>
        /// Property <c>BackendController</c> is used to communicate with the backend
        /// (see <see cref="BackendController"/>)
        /// </value>
        public BackendController BackendController { get; set; }
        /// <value>
        /// Property <c>Board</c> represents the current board
        /// </value>
        public BoardModel Board { get; set; }
        /// <value>
        /// Property <c>User</c> is the user logged in to the system
        /// </value>
        public UserModel User { get; set; }
        /// <value>
        /// Property <c>BacklogTasks</c> represents the backlog column of the board
        /// </value>
        public ObservableCollection<TaskBoardViewModel> BacklogTasks { get; set; }
        /// <value>
        /// Property <c>BacklogName</c> represents the name of the backlog column of the board
        /// </value>
        public string BacklogName { get; set; }

        /// <value>
        /// Property <c>BacklogLimit</c> represents the limit of the backlog column of the board
        /// </value>
        private int backlogLimit;
        public string BacklogLimit {
            get => backlogLimit == NO_LIMIT ?
                INFINITY : backlogLimit.ToString();
        }

        /// <value>
        /// Property <c>InProgressTasks</c> represents the in progress column of the board
        /// </value>
        public ObservableCollection<TaskBoardViewModel> InProgressTasks { get; set; }

        /// <value>
        /// Property <c>InProgressName</c> represents the name of the in progress column of the board
        /// </value>
        public string InProgressName { get; set; }

        /// <value>
        /// Property <c>InProgressLimit</c> represents the limit of the in progress column of the board
        /// </value>
        private int inProgressLimit;
        public string InProgressLimit { get => inProgressLimit == NO_LIMIT ? 
                INFINITY : inProgressLimit.ToString(); }

        /// <value>
        /// Propery <c>DoneTasks</c> represents the done column of the board
        /// </value>
        public ObservableCollection<TaskBoardViewModel> DoneTasks { get; set; }

        /// <value>
        /// Property <c>DoneName</c> represents the name of the done column of the board
        /// </value>
        public string DoneName { get; set; }

        /// <value>
        /// Property <c>DoneLimit</c> represents the limit of the done column of the board
        /// </value>
        private int doneLimit;
        public string DoneLimit {
            get => doneLimit == NO_LIMIT ?
                INFINITY : doneLimit.ToString();
        }

        /// <summary>
        /// <c>BoardViewModel</c> constructor
        /// </summary>
        /// <param name="backendController">The backend controller used to comunicate with the backed</param>
        /// <param name="id">The id of the current board</param>
        /// <param name="name">The name of the current board</param>
        /// <param name="user">The user logged in to the system</param>
        public BoardViewModel(BackendController backendController,
            BoardModel board, UserModel user)
        {
            this.BackendController = backendController;
            this.Board = board;
            this.User = user;

            this.initializeColumns();
        }

        /// <summary>
        /// This method initializes the columns, their limits and names
        /// </summary>
        private void initializeColumns()
        {
            List<TaskBoardViewModel> tempTasksList;

            tempTasksList = this.GetColumn(TaskState.Backlog).Select(task => new TaskBoardViewModel(task)).ToList();
            this.BacklogTasks = new ObservableCollection<TaskBoardViewModel>(tempTasksList);
            this.BacklogName = this.GetColumnName(TaskState.Backlog);
            this.backlogLimit = this.GetColumnLimit(TaskState.Backlog);

            tempTasksList = this.GetColumn(TaskState.InProgress).Select(task => new TaskBoardViewModel(task)).ToList();
            this.InProgressTasks = new ObservableCollection<TaskBoardViewModel>(tempTasksList);
            this.InProgressName = this.GetColumnName(TaskState.InProgress);
            this.inProgressLimit = this.GetColumnLimit(TaskState.InProgress);

            tempTasksList = this.GetColumn(TaskState.Done).Select(task => new TaskBoardViewModel(task)).ToList();
            this.DoneTasks = new ObservableCollection<TaskBoardViewModel>(tempTasksList);
            this.DoneName = this.GetColumnName(TaskState.Done);
            this.doneLimit = this.GetColumnLimit(TaskState.Done);
        }

        /// <summary>
        /// This method gets a column of the current board
        /// </summary>
        /// <param name="columnOrdinal">The id of the column to get</param>
        /// <returns>A list of tasks located in the column</returns>
        public List<TaskModel> GetColumn(TaskState columnOrdinal)
        {
            BoardControllerModel boardController = this.BackendController.BoardController;
            return boardController.GetColumn(this.User.Email, this.Board.Name, (int)columnOrdinal);
        }

        /// <summary>
        /// This method gets the name of a column in the current board
        /// </summary>
        /// <param name="columnOrdinal">The id of the column to get its name</param>
        /// <returns>The name of the column</returns>
        public string GetColumnName(TaskState columnOrdinal)
        {
            BoardControllerModel boardController = this.BackendController.BoardController;
            return boardController.GetColumnName(this.User.Email, this.Board.Name, (int)columnOrdinal);
        }

        /// <summary>
        /// This method gets the limit of a column in the current board
        /// </summary>
        /// <param name="columnOrdinal">The id of the column t get its column</param>
        /// <returns>The limit of the column</returns>
        public int GetColumnLimit(TaskState columnOrdinal)
        {
            BoardControllerModel boardController = this.BackendController.BoardController;
            return boardController.GetColumnLimit(this.User.Email, this.Board.Name, (int)columnOrdinal);
        }
    }
}
