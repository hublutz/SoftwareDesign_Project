using System;

namespace IntroSE.Kanban.Backend.DataAccessLayer.TaskDataManager
{
    /// <summary>
    /// This enum represents the state of a task in the board
    /// </summary>
    internal enum TaskState
    {
        Backlog = 0,
        InProgress = 1,
        Done = 2
    }

    /// <summary>
    /// Class <c>TaskDTO</c> represents a Task in the Data Access Layer
    /// Namely, it represents a row in the Tasks table
    /// </summary>
    internal class TaskDTO
    {
        /// <summary>
        /// Constants representing columns of the Tasks table
        /// </summary>
        public const string ID_COLUMN = "ID";
        public const string CREATION_TIME_COLUMN = "CREATION_TIME";
        public const string DUE_DATE_COLUMN = "DUE_DATE";
        public const string TITLE_COLUMN = "TITLE";
        public const string DESCRIPTION_COLUMN = "DESCRIPTION";
        public const string STATE_COLUMN = "STATE";
        public const string ASSIGNEE_COLUMN = "ASSIGNEE";
        public const string BOARD_ID_COLUMN = "BOARD_ID";

        /// <value>
        /// Field <c>taskDateManager</c> is used to communicate with the database
        /// </value>
        private TaskDataManager taskDataManager;

        /// <value>
        /// Field <c>id</c> represents the id of the task
        /// </value>
        private int id;
        public int Id { get => this.id; }

        /// <value>
        /// Field <c>creationTime</c> represents the creation date of the task
        /// </value>
        private DateTime creationTime;
        public DateTime CreationTime { get => this.creationTime; }

        /// <value>
        /// Field <c>dueDate</c> represents the due date of the task
        /// </value>
        public DateTime DueDate { get; private set; }

        /// <value>
        /// Field <c>title</c> represents the title of the task
        /// </value>
        public string Title { get; private set; }

        /// <value>
        /// Field <c>description</c> represents the description of the task
        /// </value>
        public string Description { get; private set; }

        /// <value>
        /// Field <c>state</c> represents the column the task is at (see <see cref="TaskState"/>)
        /// </value>
        public TaskState State { get; private set; }

        /// <value>
        /// Field <c>assignee</c> represents the user assigned to the task
        /// </value>
        public string Assignee { get; private set; }

        /// <value>
        /// Field <c>boardId</c> represents the id of the board the task belongs to
        /// </value>
        private int boardId;
        public int BoardId { get => this.boardId; }

        /// <summary>
        /// <c>TaskDTO</c> constructor, initializing its fields
        /// and <c>TaskDataManager</c>
        /// </summary>
        /// <param name="id">The id of the task</param>
        /// <param name="creationTime">The creation time of the task, a DateTime object</param>
        /// <param name="dueDate">The due date of the task, a DateTime object</param>
        /// <param name="title">The title of the task</param>
        /// <param name="description">The description of the task</param>
        /// <param name="state">The column of the board the task is at</param>
        /// <param name="assignee">The user assigned to the task</param>
        /// <param name="boardId">The id of the board containing the task</param>
        public TaskDTO(int id, DateTime creationTime, DateTime dueDate,
            string title, string description, int state, string assignee,
            int boardId)
        {
            this.id = id;
            this.creationTime = creationTime;
            this.DueDate = dueDate;
            this.Title = title;
            this.Description = description;
            this.State = (TaskState)state;
            this.Assignee = assignee;
            this.boardId = boardId;

            this.taskDataManager = new TaskDataManager();
        }

        /// <summary>
        /// <c>TaskDTO</c> constructor, initializing its fields
        /// and <c>TaskDataManager</c>
        /// </summary>
        /// <param name="id">The id of the task</param>
        /// <param name="creationTime">The creation time of the task, a string</param>
        /// <param name="dueDate">The due date of the task, a string</param>
        /// <param name="title">The title of the task</param>
        /// <param name="description">The description of the task</param>
        /// <param name="state">The column of the board the task is at</param>
        /// <param name="assignee">The user assigned to the task</param>
        /// <param name="boardId">The id of the board containing the task</param>
        public TaskDTO(int id, string creationTime, string dueDate,
            string title, string description, int state, string assignee,
            int boardId)
        {
            this.id = id;
            this.creationTime = DateTime.Parse(creationTime);
            this.DueDate = DateTime.Parse(dueDate);
            this.Title = title;
            this.Description = description;
            this.State = (TaskState)state;
            this.Assignee = assignee;
            this.boardId = boardId;

            this.taskDataManager = new TaskDataManager();
        }

        /// <summary>
        /// Getter of <c>creationTime</c> as a string
        /// </summary>
        /// <returns>Returns the string creation time of the task</returns>
        public string GetCreationTimeString()
        {
            return this.CreationTime.ToString("O");
        }

        /// <summary>
        /// Getter of <c>dueDate</c> as a string
        /// </summary>
        /// <returns>Returns the string due date of the task</returns>
        public string GetDueDateString()
        {
            return this.DueDate.ToString("O");
        }

        /// <summary>
        /// Setter of <c>dueDate</c> field
        /// </summary>
        /// <param name="dueDate">The new due date of the task</param>
        /// <returns>true if the update in the database succeeded, else false</returns>
        public bool SetDueDate(DateTime dueDate)
        {
            string newDueDate = dueDate.ToString("O");
            bool result = this.taskDataManager.UpdateDTO(this.BoardId, this.id, DUE_DATE_COLUMN, newDueDate);
            if (result)
            {
                this.DueDate = dueDate;
            }

            return result;
        }

        /// <summary>
        /// Setter of <c>title</c> field
        /// </summary>
        /// <param name="title">The new title of the task</param>
        /// <returns>true if the update succeeded, else false</returns>
        public bool SetTitle(string title) 
        {
            bool result = this.taskDataManager.UpdateDTO(this.BoardId, this.id, TITLE_COLUMN, title);
            if (result)
            {
                this.Title = title;
            }

            return result;
        }

        /// <summary>
        /// Setter of <c>description</c> field
        /// </summary>
        /// <param name="description">The new task description</param>
        /// <returns>true if the description was updated successfully, else false</returns>
        public bool SetDescription(string description) 
        {
            bool result = this.taskDataManager.UpdateDTO(this.BoardId, this.id, DESCRIPTION_COLUMN, description);
            if (result)
            {
                this.Description = description;
            }

            return result;
        }

        /// <summary>
        /// Getter of <c>state</c> as int value
        /// </summary>
        /// <returns>The int ordinal of the task state</returns>
        public int GetState()
        {
            return (int)this.State;
        }

        /// <summary>
        /// Setter of <c>state</c> from int value
        /// </summary>
        /// <param name="state">The new state of the task, an int value</param>
        /// <returns>true if the field update succeeded, else false</returns>
        public bool SetState(int state)
        {
            bool result = this.taskDataManager.UpdateDTO(this.BoardId, this.id, STATE_COLUMN, state);
            if (result)
            {
                this.State = (TaskState)state;
            }

            return result;
        }

        /// <summary>
        /// Setter of <c>assignee</c> field
        /// </summary>
        /// <param name="assignee">The new assignee of the task</param>
        /// <returns>true if updating the assignee succeeded, else false</returns>
        public bool SetAssignee(string assignee) 
        {
            bool result = this.taskDataManager.UpdateDTO(this.BoardId, this.id, ASSIGNEE_COLUMN, assignee);
            if (result) 
            {
                this.Assignee = assignee;
            }

            return result;
        }

        /// <summary>
        /// This method inserts the Task DTO to the database
        /// </summary>
        /// <returns>Returns true if the insertion succeeded, else false</returns>
        public bool AddToDatabase()
        {
            return this.taskDataManager.InsertDTO(this);
        }
    }
}
