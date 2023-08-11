using System;
using IntroSE.Kanban.Backend.DataAccessLayer.ColumnDataManager;
using log4net;

namespace IntroSE.Kanban.Backend.BusinessLayer.Board
{
    /// <summary>
    /// Class <c>ColumnDimentions</c> stores the amount of tasks stored in a certain 
    /// column of a <c>Board</c>, and hold the max amount of tasks it may have
    /// </summary>
    internal class ColumnDimensions
    {
        /// <value>
        /// This constant value represents that the certain column doesn't
        /// have a limit on the amount of tasks it may have
        /// </value>
        public const int UNLIMITED_AMOUNT_OF_TASKS = -1;
        /// <value>
        /// This constant represents the amount of tasks in an empty column, namely 0
        /// </value>
        public const int EMPTY_COLUMN_TASKS_AMOUNT = 0;

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <value>
        /// field <c>name</c> contains the name of the column
        /// </value>
        private string name;
        public string Name { get => name; }

        /// <value>
        /// field <c>taskAmountLimit</c> is the max amount of tasks the column may have.
        /// Cannot be negative, unless equals to <see cref="UNLIMITED_AMOUNT_OF_TASKS"/>
        /// </value>
        private int taskAmountLimit;
        /// <summary>
        /// Represents the column as a DTO in the DataAccessLayer
        /// </summary>
        private ColumnDTO columnDTO;

        public int TaskAmountLimit
        {
            get => taskAmountLimit;
            set
            {
                if (value < EMPTY_COLUMN_TASKS_AMOUNT && value != UNLIMITED_AMOUNT_OF_TASKS)
                {
                    Log.Error("The limit of tasks cannot be negative and different from " + UNLIMITED_AMOUNT_OF_TASKS);
                    throw new Exception("The given tasks limit value is invalid");
                }
                if (columnDTO.SetTaskAmountLimit(value))
                {
                    this.taskAmountLimit = value;
                }
                else {
                    Log.Error("Failed to update the task amount limit in the DB");
                    throw new Exception("Failed to update the task amount limit");
                }
                
            }
        }

        /// <vale>
        /// field <c>amountOfTasks</c> is the amount of tasks the column currently has
        /// </value>
        private int amountOfTasks;
        public int AmountOfTasks {
            get => amountOfTasks;
            set
            {
                if (value < EMPTY_COLUMN_TASKS_AMOUNT)
                {
                    Log.Error("The amount ot tasks in a column cannot be negative");
                    throw new Exception("The amount of tasks cannot be negative");
                }
                if (value > this.TaskAmountLimit && this.TaskAmountLimit != UNLIMITED_AMOUNT_OF_TASKS)
                {
                    Log.Error("The amount of tasks in a column cannot exceed the limit");
                    throw new Exception("The amount of tasks cannot exceed the limit");
                }
                this.amountOfTasks = value;
            }
        }

        /// <summary>
        /// <c>ColumnDimensions</c> constructor. Initialises the column to be empty
        /// and so that there is no limit on the tasks it may have
        /// </summary>
        public ColumnDimensions(string name, Task.TaskState columnTaskState, int boardId)
        {
            this.name = name;
            this.amountOfTasks = EMPTY_COLUMN_TASKS_AMOUNT;
            this.taskAmountLimit = UNLIMITED_AMOUNT_OF_TASKS;

            columnDTO = new ColumnDTO((int) columnTaskState, name, boardId, TaskAmountLimit);
            columnDTO.InsertDTO();
        }

        /// <summary>
        /// constractor that is used while loading data from the DB
        /// </summary>
        /// <param name="columnDTO">the dto of the column</param>
        /// <param name="taskAmount">the amount of tasks in the column</param>
        public ColumnDimensions(ColumnDTO columnDTO, int taskAmount)
        {
            this.name = columnDTO.Name;
            this.amountOfTasks = taskAmount;
            this.taskAmountLimit = columnDTO.TaskAmountLimit;
            
            this.columnDTO = columnDTO;
        }

        /// <summary>
        /// Indicates whether the column has reaced its max amount of tasks
        /// </summary>
        /// <returns>true if the column is able to have more tasks, false if 
        /// it reached its limit</returns>
        public bool HasSpaceInColumn()
        {
            return this.TaskAmountLimit == UNLIMITED_AMOUNT_OF_TASKS ||
                this.AmountOfTasks < this.TaskAmountLimit;
        }
    }
}
