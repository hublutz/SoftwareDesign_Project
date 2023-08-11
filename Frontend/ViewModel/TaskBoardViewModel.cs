using Frontend.Model;

namespace Frontend.ViewModel
{
    /// <summary>
    /// Class <c>TaskBoardViewModel</c> is a wrapper class for 
    /// <c>TaskModel</c> performin logic related to <c>BoardView</c>
    /// </summary>
    public class TaskBoardViewModel
    {
        /// <value>
        /// <c>TaskModel</c> is the task the class wraps
        /// </value>
        public TaskModel TaskModel { get; set; }

        private const int SHORT_DESCRIPTION_LENGTH = 35;
        /// <value>
        /// <c>ShortDescription</c> returns the description of the task in a shortened form
        /// </value>
        public string ShortDescription
        {
            get => this.TaskModel.Description.Length > SHORT_DESCRIPTION_LENGTH ?
                this.TaskModel.Description.Substring(0, SHORT_DESCRIPTION_LENGTH) + "..." : 
                this.TaskModel.Description;
        }

        private const string UNASSIGNED_TASK_STRING = "N/A";
        /// <value>
        /// <c>Assignee</c> represents the assignee of the task
        /// </value>
        public string Assignee
        {
            get => this.TaskModel.Assignee == null ? UNASSIGNED_TASK_STRING : 
                this.TaskModel.Assignee;
        }

        /// <value>
        /// Property <c>CreationTimeString</c> returns the creation time as a string
        /// </value>
        public string CreationTimeString { get => this.TaskModel.CreationTime.ToString("d"); }
        /// <value>
        /// Property <c>DueDateString</c> returns the due date as a string
        /// </value>
        public string DueDateString { get => this.TaskModel.DueDate.ToString("g"); }

        /// <summary>
        /// <c>TaskBoardViewModel</c> constructor
        /// </summary>
        /// <param name="taskModel">The task to wrap</param>
        public TaskBoardViewModel(TaskModel taskModel)
        {
            TaskModel = taskModel;
        }
    }
}
