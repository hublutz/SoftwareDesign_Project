using System;
using System.Text.Json.Serialization;

namespace IntroSE.Kanban.Backend.ServiceLayer.TaskServices
    { /// <summary>
      /// Class <c>TaskToSend</c> represents Service Layer task
      /// data returned from task to related operations
      /// </summary>
    public class TaskToSend
    {
        /// <value>
        /// <c>Id</c> represents the id of the task
        /// </value>
        public int Id { get; }
        /// <value>
        /// <c>CreationTime</c> represents the creation time of the task
        /// </value>
        public DateTime CreationTime { get; }
        /// <value>
        /// <c>Title</c> represents the title of the task
        /// </value>
        public string Title { get; }
        /// <value>
        /// <c>Description</c> represents the description of the task
        /// </value>
        public string Description { get; }
        /// <value>
        /// <c>DueDate</c> represents the due date of the task
        /// </value>
        public DateTime DueDate { get; }
        /// <summary>
        /// <c>Assignee</c> represents the assignee of the task
        /// </summary>
        public string Assignee { get; }

        /// <summary>
        /// <c>TaskToSend</c> constructor, initialising all of the fields
        /// </summary>
        /// <param name="Id">The id of the task</param>
        /// <param name="CreationTime">The creation time of the task</param>
        /// <param name="DueDate">The due date of the task</param>
        /// <param name="Title">The title of the task</param>
        /// <param name="Description">The description of the task</param>
        /// <param name="Assignee">The assignee of the task</param>
        [JsonConstructor]
        public TaskToSend(int Id, DateTime CreationTime, DateTime DueDate, string Title, string Description, string Assignee)
        {
            this.Id = Id;
            this.CreationTime = CreationTime;
            this.DueDate  = DueDate;
            this.Title = Title;
            this.Description = Description;
            this.Assignee = Assignee;
        }
    }
}
