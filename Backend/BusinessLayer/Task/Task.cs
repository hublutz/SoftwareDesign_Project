using System;
using IntroSE.Kanban.Backend.DataAccessLayer.TaskDataManager;
using log4net;


namespace IntroSE.Kanban.Backend.BusinessLayer.Task
{

    /// <summary>
    /// Enum <c>TaskState</c> represents constants indicating the completion state of a task
    /// </summary>
    internal enum TaskState
    {
        Backlog = 0,
        InProgress = 1,
        Done = 2
    }

    /// <summary>
    /// Class <c>Task</c> represents a task that is located in a <c>Board</c>
    /// </summary>
    internal class Task
    {
        /// <summary>
        /// Represents the max length of a tasks' title
        /// </summary>
        private const int MAX_TITLE_LENGTH = 50;
        /// <summary>
        /// Represents the max length of a task's description
        /// </summary>
        private const int MAX_DESCRIPTION_LENGTH = 300;

        /// <summary>
        /// constant that represent the first taskState
        /// </summary>
        private readonly int FIRST_STATE = 0;

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <value>
        /// <c>DateTime</c> a constant representing the creation time of the task
        /// </value>
        private readonly DateTime CREATION_TIME;

        /// <summary>
        ///  <c>Id</c> the field and getter of the task id
        /// </summary>
        public int Id { get;}
        /// <summary>
        ///  <c>DueDate</c> the field getter and setter of the task due date
        /// </summary>
        private DateTime dueDate;
        public DateTime DueDate { get => dueDate; }

        /// <summary>
        ///  <c>Title</c> the field getter and setter of the task title
        /// </summary>
        private string title;
        public string Title
        {
            get => title;
            private set
            {
                this.title = value;
                Log.Info("title was updated successfully task (" + Id + ")");

            }
        }

        /// <summary>
        ///  <c>Description</c> the field getter and setter of the task description
        /// </summary>
        private string description;
        public string Description
        {
            get => description;

            private set
            {
                this.description = value;
                Log.Info( "description was updated successfully task ("+Id+")" );
            }
        }

        /// <summary>
        ///  <c>State</c> the field getter and setter of the task state. 
        ///  the setter is private so the only way to externely update the fieled is <c>UpdateState()</c>
        /// </summary>
        public TaskState State { get; private set; }

        /// <summary>
        ///  <c>CREATION_TIME</c> field getter
        /// </summary>
        public DateTime GetCreationTime() { return CREATION_TIME; }

        /// <summary>
        /// Field <c>assignee</c> represents the user assigned to the task
        /// If the task is unassigned, the value is null
        /// </summary>
        private string assignee;
        public string Assignee { get => assignee;
            private set
            {
                if (TaskState.Done == State)
                {
                    Log.Error("cannot edit a Done task. task " + Id);
                    throw new Exception("cannot edit a Done task. task " + Id);
                }
            }
        }

        /// <summary>
        /// Field <c>taskDTO</c> is a DTO representing the task
        /// </summary>
        private TaskDTO taskDTO;

        /// <summary>
        /// constructor of <c>Task</c>> initialising all of the fields
        /// </summary>
        /// <param name="id">The id of the task</param>
        /// <param name="dueDate">the due date of the task</param>
        /// <param name="title">the title of the task</param>
        /// <param name="description">the description of the task</param>
        /// <param name="boardId">The id of the board containing the task</param>
        public Task(int id, DateTime dueDate, string title, string description, int boardId)
        {
            this.checkValidTitle(title);
            this.checkValidDescription(description);
            this.Id = id;
            this.dueDate = dueDate;
            this.assignee = null;
            this.Title = title;
            this.Description = description;

            this.State = (TaskState) FIRST_STATE;
            CREATION_TIME = DateTime.Now;

            this.taskDTO = new TaskDTO(this.Id, CREATION_TIME, this.DueDate, this.Title, this.Description,
                (int)this.State, this.Assignee, boardId);
            this.taskDTO.AddToDatabase();
        }

        /// <summary>
        /// constructor of <c>Task</c> initialising all of the fields from a
        /// <c>TaskDTO</c> object
        /// Should be called after loading all Data Access Layer persisted data
        /// </summary>
        /// <param name="taskDTO">DTO used to communicate with the Data Access Layer</param>
        public Task(TaskDTO taskDTO)
        {
            this.Id = taskDTO.Id;
            CREATION_TIME = taskDTO.CreationTime;
            this.dueDate = taskDTO.DueDate;
            this.Title = taskDTO.Title;
            this.Description = taskDTO.Description;
            this.State = (TaskState)taskDTO.GetState();
            this.assignee = taskDTO.Assignee;

            this.taskDTO = taskDTO;
        }

        /// <summary>
        /// dueDate setter 
        /// </summary>
        /// <param name="value">the dueDate value in string form</param>
        /// <param name="email">the email of the user whos trying to edit</param>
        /// <exception cref="Exception"> throws an exseption if the input isnt legal</exception>
        public void SetDueDate(string email, string value)
        {
            if (TaskState.Done == State)
            {
                Log.Error("cannot edit a Done task. task " + Id);
                throw new Exception("cannot edit a Done task. task " + Id);
            }
            throwNotTheAssigneeException(email);

            try
            {
                DateTime newDueDate = DateTime.Parse(value).ToLocalTime();
                if (this.taskDTO.SetDueDate(newDueDate))
                {
                    this.dueDate = newDueDate;
                    Log.Info("setting new due date in task " + Id + " has succeeded");
                }
                else
                {
                    Log.Error("Failed to update the due date of the task in the Data Access Layer");
                    throw new Exception("Failed to update the task's due date");
                }
            }
            catch (FormatException e)
            {
                Log.Error("setting new due date in task " + Id + " has failed because of invalid input");
                throw new Exception("setting new due date in task " + Id + "has failed because of invalid input");
            }
        }

        /// <summary>
        /// description setter 
        /// </summary>
        /// <param name="value">the description value in string form</param>
        /// <param name="email">the email of the user whos trying to edit</param>
        /// <exception cref="Exception">Throws exception if updating the task in the Data 
        /// Access Layer failed</exception>
        public void SetDescription(string email, string value)
        {
            throwNotTheAssigneeException(email);
            checkValidDescription(value);
            if (this.taskDTO.SetDescription(value))
            {
                this.Description = value;
            }
            else
            {
                Log.Error("Failed to update the description of the task in the Data Access Layer");
                throw new Exception("Failed to update the task's description");
            }
        }


        /// <summary>
        /// title setter 
        /// </summary>
        /// <param name="value">the title value in string form</param>
        /// <param name="email">the email of the user whos trying to edit</param>
        /// <exception cref="Exception"> throws an exseption if the input isnt legal or
        /// if updating the task in the Data Access Layer failed</exception>
        public void SetTitle(string email, string value)
        {
            throwNotTheAssigneeException(email);
            checkValidTitle(value);
            if (this.taskDTO.SetTitle(value))
            {
                this.Title = value;
            }
            else
            {
                Log.Error("Failed to update the title of the task in the Data Access Layer");
                throw new Exception("Failed to update the task's title");
            }
        }


        /// <summary>
        /// Sets a new assignee; The new Assignee may be null in order to reset the assignee.
        /// </summary>
        /// <param name="assignerEmail">The email of the user who tried to assign an assignee</param>
        /// <param name="emailAssignee">The email of the user to become the new assignee</param>
        /// <exception cref="Exception">Throws exception if updating the task in the Data 
        /// Access Layer failed</exception>
        public void SetAssignee(string assignerEmail, string emailAssignee)
        {
            if (this.Assignee != null)
            {
                throwNotTheAssigneeException(assignerEmail);
            }
            if (this.taskDTO.SetAssignee(emailAssignee))
            {
                assignee = emailAssignee;
            }
            else
            {
                Log.Error("Failed to update the assignee of the task in the Data Access Layer");
                throw new Exception("Failed to update the task's assignee");
            }
        }

        /// <summary>
        /// The method moves the task into the next column (if exist)
        /// </summary>
        /// <exception cref="Exception">Throws exception if updating the task in the Data 
        /// Access Layer failed</exception>
        public void UpdateState()
        {
            TaskState nextTaskState = this.GetNextTaskState();
            if(this.taskDTO.SetState((int)nextTaskState))
            {
                this.State = nextTaskState;
                Log.Info("updated state of task " + Id + " to " + State);
            }
            else
            {
                Log.Error("Failed to update the state of the task in the Data Access Layer");
                throw new Exception("Failed to update the task's state");
            }
        }

        /// <summary>
        /// The method returns the next state of this task (if exsist)
        /// </summary>
        /// <returns> the next state of this task</returns>
        /// <exception cref="Exception"> throws an exseption if there is not a next state</exception>
        public TaskState GetNextTaskState()
        {

            if (Enum.IsDefined(typeof(TaskState), (int)this.State + 1))
            {
                return (TaskState)this.State + 1;
            }
            else
            {
                Log.Error("next state is not defined in task (" + Id + ")");
                throw new Exception("next state is not defined in task (" + Id + ")");
            }
        }

        /// <summary>
        /// Makes sure only the assignee (in case there is one) can edit the tasks details.
        /// </summary>
        /// <param name="email"> The changer's email and identifier</param>
        /// <exception cref="Exception"> Throws an exception when the assignee is not the user who requests a change.</exception>
        private void throwNotTheAssigneeException(string email)
        {
            if (assignee != email)
            {
                Log.Error("Tried to change a task but the user is not the assignee");
                throw new Exception("Not the assignee");
            }
        }

        /// <summary>
        /// Checks if a given title is valid
        /// </summary>
        /// <param name="title">A new title of the task</param>
        /// <exception cref="Exception">Throws exception if the title
        /// is null, white space or too long</exception>
        private void checkValidTitle(string title) 
        {
            if (TaskState.Done == State)
            {
                Log.Error("cannot edit a Done task. task " + Id);
                throw new Exception("cannot edit a Done task. task " + Id);
            }

            if (string.IsNullOrWhiteSpace(title) || title.Length > MAX_TITLE_LENGTH)
            {
                Log.Error($"the title cannot be longer then {MAX_TITLE_LENGTH} characters or empty");
                throw new Exception($"the title cannot be longer then {MAX_TITLE_LENGTH} characters or empty");
            }
        }

        /// <summary>
        /// Checks if a given description is valid
        /// </summary>
        /// <param name="description">A new title of the task</param>
        /// <exception cref="Exception">Throws exception if the description is null or too long</exception>
        private void checkValidDescription(string description)
        {
            if (TaskState.Done == State)
            {
                Log.Error("cannot edit a Done task. task " + Id);
                throw new Exception("cannot edit a Done task. task " + Id);
            }

            if (description == null)
            {
                Log.Error("the description of a task cannot be null");
                throw new Exception("the description of a task cannot be null");
            }

            if (description.Length > MAX_DESCRIPTION_LENGTH)
            {
                Log.Error($"the description cannot be longer then {MAX_DESCRIPTION_LENGTH} characters");
                throw new Exception($"the description cannot be longer then {MAX_DESCRIPTION_LENGTH} characters");
            }
        }
    }
}
