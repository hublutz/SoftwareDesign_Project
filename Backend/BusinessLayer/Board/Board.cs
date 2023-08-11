using System;
using System.Collections.Generic;
using System.Linq;
using IntroSE.Kanban.Backend.DataAccessLayer.BoardDataManager;
using IntroSE.Kanban.Backend.DataAccessLayer.ColumnDataManager;
using IntroSE.Kanban.Backend.DataAccessLayer.TaskDataManager;
using log4net;

namespace IntroSE.Kanban.Backend.BusinessLayer.Board
{
    /// <summary>
    /// Class <c>Board</c> represents a Kanban board in the business layer
    /// </summary>
    internal class Board
    {
        /// <value>
        /// field <c>Id</c> is the Id of the board
        /// </value>
        public int Id { get; }
        /// <value>
        /// field <c>BoardOwnerEmail</c> is the email of the board owner
        /// </value>
        private string boardOwnerEmail;
        public string BoardOwnerEmail { get => this.boardOwnerEmail; }
        /// <value>
        /// field <c>boardUsers</c> is saving the users of the board
        /// </value>
        private HashSet<string> boardUsers;
        /// <value>
        /// field <c>Name</c> is the name of the board
        /// </value>
        public string Name { get; }
        /// <value>
        /// field <c>tasks</c> is a list of all the board's tasks of all columns
        /// </value>
        private List<Task.Task> tasks;
        /// <value>
        /// field <c>nextId</c> determines the id that will be given to a new <c>Task</c> to add
        /// </value>
        private int nextId;
        /// <value>
        /// field <c>columnStatesAndDimntsions</c> is a dictionary that holds the tasks amount
        /// and limit for every column (see <see cref="ColumnDimensions"/> and <see cref="Task.TaskState"/>)
        /// </value>
        private Dictionary<Task.TaskState, ColumnDimensions> columnStatesAndDimensions;

        /// <summary>
        /// Represents the Board as a DTO in the DataAccessLayer
        /// </summary>
        private BoardDTO boardDTO;

        /// <value>
        /// A constant representing the first task id to count from
        /// </value>
        private const int STARTING_TASK_ID= 0;

        /// <value>
        /// Respresents the first column of the board, to which new tasks will be added
        /// </value>
        private const Task.TaskState FIRST_COLUMN = 0;

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// <c>Board</c> constructor
        /// </summary>
        /// <param name="name">The name of the board</param>
        public Board(string name, int boardId, string ownerEmail)
        {
            this.Id = boardId;
            this.Name = name;
            this.boardOwnerEmail = ownerEmail;
            this.boardUsers = new HashSet<string>{ ownerEmail };
            this.tasks = new List<Task.Task>();
            this.nextId = STARTING_TASK_ID;
            this.columnStatesAndDimensions = new Dictionary<Task.TaskState, ColumnDimensions>();

            this.boardDTO = new BoardDTO(boardId,name,ownerEmail, new HashSet<string>());
            boardDTO.InsertDTO();
            boardDTO.AddUserToBoard(this.boardOwnerEmail);

            // initialise all default columns
            this.columnStatesAndDimensions[Task.TaskState.Backlog] = new ColumnDimensions("backlog",Task.TaskState.Backlog,boardId);
            this.columnStatesAndDimensions[Task.TaskState.InProgress] = new ColumnDimensions("in progress",Task.TaskState.InProgress, boardId) ;
            this.columnStatesAndDimensions[Task.TaskState.Done] = new ColumnDimensions("done", Task.TaskState.Done, boardId);
        }

        /// <summary>
        /// Constructor of <c>Board</c>. Initializes its fields from the received
        /// DTO.
        /// This constructor should be used after loading data from the Data Access Layer,
        /// as it doesn't insert the user
        /// </summary>
        /// <param name="boardDTO"><c>BoardDTO</c> that was received from loading the data</param>
        /// <param name="columns"> A list of all the columns in this board in columnDTO format</param>
        
        public Board(BoardDTO boardDTO, List<ColumnDTO> columns)
        {
            this.Id = boardDTO.Id;
            this.boardOwnerEmail = boardDTO.BoardOwnerEmail;
            this.boardUsers = boardDTO.BoardUsers;
            this.Name = boardDTO.Name;
            
            this.columnStatesAndDimensions = new Dictionary<Task.TaskState, ColumnDimensions>();
            this.tasks = new List<Task.Task>();
            this.boardDTO = boardDTO;

            TaskDataManager taskDataManager = new TaskDataManager();
            List<TaskDTO> boardTasks = taskDataManager.SelectBoardTasks(Id);

            foreach (TaskDTO task in boardTasks)
            {
                tasks.Add(new Task.Task(task));
            }

            foreach (ColumnDTO column in  columns)
            {
                int taskAmount = tasks.FindAll(x => (int) x.State == (int) column.TaskState).Count;
                columnStatesAndDimensions.Add((Task.TaskState) ((int) column.TaskState), new ColumnDimensions(column, taskAmount));
            }
            // finding the next task id
            this.nextId = tasks == null || tasks.Count == 0 ? STARTING_TASK_ID : (tasks.Max(x=> x.Id) + 1); 
        }


        /// <summary>
        /// This method add a new task in the first column
        /// </summary>
        /// <param name="dueDate">The due date of the new task</param>
        /// <param name="title">The title of the new task</param>
        /// <param name="description">The description of the new task</param>
        /// <exception cref="Exception">Throws an exception if the given due date is
        /// in an invalid date format, or it the first column is full</exception>
        public void AddTask(string dueDate, string title, string description)
        {
            // parse the given due dates
            DateTime parsedDueDate;
            try
            {
                parsedDueDate = DateTime.Parse(dueDate).ToLocalTime();
            }
            catch (FormatException)
            {
                Log.Error("Failed to add a new task to board " + this.Name + " because an invalid due date was given");
                throw new Exception("Received an invalid due date for the task");
            }

            if (this.columnStatesAndDimensions[FIRST_COLUMN].HasSpaceInColumn())
            {
                Task.Task task = new Task.Task(nextId, parsedDueDate, title, description, this.Id);
                this.tasks.Add(task);
                this.nextId++;
                this.columnStatesAndDimensions[FIRST_COLUMN].AmountOfTasks++;
                Log.Info("Task with id " + task.Id + " added successfully to board " + this.Name);
            }
            else
            {
                Log.Error("Failed to add a new task to board " + this.Name + " because the first column is full");
                throw new Exception("Cannot add the task since first column is full");
            }
        }

        /// <summary>
        /// This method returns the task with the given id
        /// </summary>
        /// <param name="taskId">The id of the task to get</param>
        /// <returns>The task with the id specified</returns>
        /// <exception cref="Exception">Throws an exception
        /// if there is no task with the given id or if that id is invalid</exception>
        public Task.Task GetTask(int taskId)
        {
            if (taskId < STARTING_TASK_ID | taskId >= nextId)
            {
                Log.Error("Failed to get Task as the given taskId is out of bounds");
                throw new Exception("The given taskId is invalid.");
            }

            Task.Task task = this.tasks.Find(x => x.Id == taskId);
            if (task == null)
            {
                Log.Error("Failed to find task with Id " + taskId + " in Board " + this.Name);
                throw new Exception("The given task Id doesn't exist");
            }

            Log.Info("Successfuly returned task with Id " + taskId + " from Board " + this.Name);
            return task;
        }

        /// <summary>
        /// This method moves the given task to the next column:
        /// (see <see cref="Task.Task.UpdateState"/>)
        /// </summary>
        /// <param name="taskId">The id of the task to update its state</param>
        /// <exception cref="Exception">Throws an exception if the user is not the assignee or if the next
        /// column is full/doesn't exist</exception>
        public void MoveTaskState(string email, int taskId)
        {
            Task.Task task = this.GetTask(taskId);
            if (task.Assignee != email)
            {
                Log.Error("Tried to move a task but the user is not the assignee.");
                throw new Exception("The given user is not the assignee.");
            }

            Task.TaskState previousState = task.State;
            Task.TaskState nextState;
            try
            {
                nextState = task.GetNextTaskState();
            }
            catch (Exception) // a Done task doesn't have a next column
            {
                Log.Error("Failed to move task with id " + taskId + " because it is in the last column");
                throw;
            }

            if (this.columnStatesAndDimensions[nextState].HasSpaceInColumn())
            {
                task.UpdateState();
                this.columnStatesAndDimensions[previousState].AmountOfTasks--;
                this.columnStatesAndDimensions[nextState].AmountOfTasks++;
                Log.Info("Successfuly moved task with id " + taskId + " from " + previousState +
                    " to " + nextState + " in board " + this.Name);
            }
            else
            {
                Log.Error("Could not move task with id " + taskId + 
                    " to the next column because the column is full");
                throw new Exception("Cannot move the task to the " +
                    "next column because it is full");
            }
        }

        /// <summary>
        /// This method checks if a given int is a valid taskState
        /// </summary>
        /// <param name="columnState">The column ordinal to check</param>
        /// <exception cref="Exception">Throws an exception if the given column
        /// doesn't exist</exception>
        private void checkValidColumnOrdinal(int columnState)
        {
            if (!Enum.IsDefined(typeof(Task.TaskState), columnState))
            {
                Log.Error("Failed to access column with ordinal " + columnState + " because it doesn't exist");
                throw new Exception("The given column ordinal doesn't exist");
            }
        }

        /// <summary>
        /// This method returns all the tasks in a column, if it exists
        /// </summary>
        /// <param name="columnOrdinal">The column to get all its tasks</param>
        /// <returns>A list of all tasks located in the specified column</returns>
        public List<Task.Task> GetColumn(int columnOrdinal)
        {
            this.checkValidColumnOrdinal(columnOrdinal);
            Task.TaskState taskState = (Task.TaskState)columnOrdinal;
            List<Task.Task> columnTasks = this.tasks.FindAll(
                task => task.State == taskState);
            Log.Info("Successfuly returned column with ordinal " + taskState + " from board " + this.Name);
            return columnTasks;
        }

        /// <summary>
        /// This method returns the name of the given column, if exists
        /// </summary>
        /// <param name="columnOrdinal">The column to get its name</param>
        /// <returns>Returns the name of the given column, if it exists</returns>
        public string GetColumnName(int columnOrdinal)
        {
            this.checkValidColumnOrdinal(columnOrdinal);
            Task.TaskState taskState = (Task.TaskState)columnOrdinal;
            Log.Info("Successfuly returned the name of the column with ordinal " + columnOrdinal + " from board " + this.Name);
            return this.columnStatesAndDimensions[taskState].Name;
        }

        /// <summary>
        /// This method gets the limit of tasks of a certain column,
        /// if the column given exists
        /// </summary>
        /// <param name="columnOrdinal">The column ordinal</param>
        /// <returns>Retunrs the limit of tasks of the column, if exists</returns>
        public int GetColumnLimit(int columnOrdinal)
        {
            this.checkValidColumnOrdinal(columnOrdinal);
            Task.TaskState taskState = (Task.TaskState)columnOrdinal;
            Log.Info("Successfuly returned the limit of the column with ordinal " + columnOrdinal + " from board " + this.Name);
            return this.columnStatesAndDimensions[taskState].TaskAmountLimit;
        }

        /// <summary>
        /// This method changes the limit of tasks for a certain column
        /// </summary>
        /// <param name="columnOrdinal">The column ordinal</param>
        /// <param name="limit">The new limit to set</param>
        /// <exception cref="Exception">Throws an exception if the new 
        /// limit of tasks is lower than the current amount of tasks in the column</exception>
        public void LimitColumn(int columnOrdinal, int limit)
        {
            this.checkValidColumnOrdinal(columnOrdinal);
            Task.TaskState taskState = (Task.TaskState)columnOrdinal;
            int columnTasksAmount = this.columnStatesAndDimensions[taskState].AmountOfTasks;
            if (limit < columnTasksAmount)
            {
                Log.Error("Failed to set the limit of column with ordinal " + columnOrdinal +
                    "in board " + this.Name + " since the new limit is below the current amount of tasks");
                throw new Exception("Cannot change the column's limit because the " +
                    "amount of existing tasks surpasses the new limit");
            }
            Log.Info("Successfuly changed the limit of column with ordinal " + columnOrdinal + " in board" +
                this.Name + "to " + limit);
            this.columnStatesAndDimensions[taskState].TaskAmountLimit = limit;
        }

        /// <summary>
        /// Lets a user join a board, no need for permission from the owner; Excpects the user to exist and be logged in.
        /// </summary>
        /// <param name="userEmail">The user's email and identifier</param>
        /// <exception cref="Exception"> In the case the given user is already a part of the board</exception>
        public void JoinBoard(string userEmail)
        {
            if (IsUserEnrolled(userEmail))
            {
                Log.Error("Failed to join board as the given email is already a part of the board.");
                throw new Exception("Can't join the board twice.");
            }

            if (!boardDTO.AddUserToBoard(userEmail)) {
                Log.Error("Failed to add a user to a board in the DB");
                throw new Exception("Failed to add a user to a board");
            }
            boardUsers.Add(userEmail);
            
            Log.Info($"Successfully added {userEmail} to the board.");
        }

        /// <summary>
        /// Lets a user leave a board, no need for permission from the owner; Excpects the user to exist and be logged in.
        /// </summary>
        /// <param name="userEmail">The user's email and identifier</param>
        /// <exception cref="Exception"> In the case the given user is not a part of the board or if he is the owner of the board</exception>
        public void LeaveBoard(string userEmail)
        {
            if (!IsUserEnrolled(userEmail))
            {
                Log.Error("Failed to leave board as the given email is not a part of the board.");
                throw new Exception("Can't leave a board the user is not a part of.");
            }
            if(BoardOwnerEmail == userEmail)
            {
                Log.Error("Failed to leave board as the user who wishes to leave is the board owner.");
                throw new Exception("The owner of the board can't leave it.");
            }

            if (!boardDTO.RemoveUserToBoard(userEmail))
            {
                Log.Error("Failed to remove a user to a board in the DB");
                throw new Exception("Failed to remove a user to a board");
            }

            tasks.FindAll(x => x.Assignee == userEmail && x.State != Task.TaskState.Done).ForEach(x => x.SetAssignee(userEmail, null));
            boardUsers.Remove(userEmail);
            Log.Info(userEmail + " Successfully left the board.");
        }

        /// <param name="email">The user's email and identifier</param>
        /// <returns> A boolean, True- user is in the board | False- user isn't in the board</returns>
        /// <exception cref="Exception"> In the case the given user email is null</exception>
        public bool IsUserEnrolled(string email)
        {
            if (email == null)
            {
                Log.Error("Failed to check if a user is a member of the board as the given email is null");
                throw new Exception("userEmail can't be null.");
            }
            return boardUsers.Contains(email);
        }

        /// <summary>
        /// <c>email</c> assigns a task to <c>emailAssignee</c>; 
        /// Excpects the assigner to be logged in
        /// </summary>
        /// <param name="assignerEmail">The assigner's email and identifier</param>
        /// <param name="taskId">The Id of the task to assign</param>
        /// <param name="emailAssignee">The assignee's email and identifier or null</param>
        /// <exception cref="Exception">When the users are not members of the board</exception>
        public void AssignTask(string assignerEmail, int taskId, string emailAssignee)
        {
            if (emailAssignee != null && !IsUserEnrolled(emailAssignee))
            {
                Log.Error("Failed to AssignTask as a given email is not a member of the board.");
                throw new Exception("Can't assign a task or get assigned a task if the user is not a part of the board.");
            }

            GetTask(taskId).SetAssignee(assignerEmail, emailAssignee);
        }

        /// <summary>
        /// This method will transfer the board ownership from the owner to a member of the board.
        /// </summary>
        /// <param name="currentOwnersEmail">The owner's email and identifier</param>
        /// <param name="newOwnersEmail">The email and identifier of the member who'll become the new owner</param>
        /// <exception cref="Exception">When the supposed board owner is not really the owner of the board or when 
        /// the new owner is not a member of the board.</exception>
        public void TransferBoardOwnership(string currentOwnersEmail, string newOwnersEmail)
        {
            if(!(boardOwnerEmail == currentOwnersEmail))
            {
                Log.Error("Tried to transfer a board ownership but the given currentOwnerEmail is not the owner of the board");
                throw new Exception("Must provide the owner's email");
            }

            if (!this.IsUserEnrolled(newOwnersEmail))
            {
                Log.Error("Tried to transfer a board's ownership but the new owner isn't a member of the board.");
                throw new Exception("The new owner must be a member of the board.");
            }

            if (!boardDTO.setBoardOwnerEmail(newOwnersEmail))
            {
                Log.Error("Failed to add a user to a board in the DB");
                throw new Exception("Failed to add a user to a board");
            }

            boardOwnerEmail = newOwnersEmail;
            Log.Info($"Successfully set {newOwnersEmail} as the new owner of the board {Id}");
        }
    }
}
