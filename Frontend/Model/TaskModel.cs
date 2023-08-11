using System;
using System.Text.Json.Serialization;

namespace Frontend.Model
{
    /// <summary>
    /// Task model, saves the task data from the backend.
    /// </summary>
    public class TaskModel : NotifiableModelObject
    {
        private const string ID_PROPERTY = "Id";
        private const string CREATION_TIME_PROPERTY = "CreationTime";
        private const string TITLE_PROPERTY = "Title";
        private const string DESCRIPTION_PROPERTY = "Description";
        private const string DUE_DATE_PROPERTY = "DueDate";
        private const string ASSIGNEE_PROPERTY = "Assignee";

        /// <value>
        /// <c>Id</c> represents the id of the task
        /// </value>
        private int id;
        public int Id
        {
            get => id; 
            set
            {
                this.id = value;
                RaisePropertyChanged(ID_PROPERTY);
            }
        }
        private const int DEFAULT_ID = -1;

        /// <value>
        /// <c>CreationTime</c> represents the creation time of the task
        /// </value>
        private DateTime creationTime;
        public DateTime CreationTime 
        { 
            get => creationTime;
            set
            {
                this.creationTime = value;
                RaisePropertyChanged(CREATION_TIME_PROPERTY);
            }
        }
        /// <value>
        /// <c>Title</c> represents the title of the task
        /// </value>
        private string title;
        public string Title {
            get => title;
            set
            {
                title = value;
                RaisePropertyChanged(TITLE_PROPERTY);
            }
        }
        /// <value>
        /// <c>Description</c> represents the description of the task
        /// </value>
        private string description;
        public string Description { get => description;
            set
            { 
                description = value;
                RaisePropertyChanged(DESCRIPTION_PROPERTY);
            }
        }

        /// <value>
        /// <c>DueDate</c> represents the due date of the task
        /// </value>
        private DateTime dueDate;
        public DateTime DueDate 
        { 
            get => dueDate;
            set
            {
                this.dueDate = value;
                RaisePropertyChanged(DUE_DATE_PROPERTY);
            }
        }
        /// <summary>
        /// <c>Assignee</c> represents the assignee of the task
        /// </summary>
        private string? assignee;
        public string? Assignee {
            get => this.assignee;
            set
            {
                assignee = value;
                RaisePropertyChanged(ASSIGNEE_PROPERTY);
            }
        }

        /// <summary>
        /// <c>TaskToSend</c> constructor, initialising all of the fields with the given values
        /// </summary>
        /// <param name="controller">The backend controller</param>
        /// <param name="Id">The id of the task</param>
        /// <param name="CreationTime">The creation time of the task</param>
        /// <param name="DueDate">The due date of the task</param>
        /// <param name="Title">The title of the task</param>
        /// <param name="Description">The description of the task</param>
        /// <param name="Assignee">The assignee of the task</param>
        public TaskModel(BackendController controller, int Id, DateTime CreationTime,
            DateTime DueDate, string Title, string Description, string Assignee) : base(controller)
        {
            this.id = Id;
            this.creationTime = CreationTime;
            this.dueDate = DueDate;
            this.title = Title;
            this.description = Description;
            this.assignee = Assignee;
        }

        /// <summary>
        /// <c>TaskToSend</c> constructor, initialising all of the fields with the default values
        /// </summary>
        public TaskModel() : base(new BackendController())
        {
            this.id = DEFAULT_ID;
            this.creationTime = new DateTime();
            this.dueDate = new DateTime();
            this.title = null;
            this.description = null;
            this.assignee = null;
        }
    }
}
