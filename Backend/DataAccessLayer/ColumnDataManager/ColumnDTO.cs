using IntroSE.Kanban.Backend.BusinessLayer.Board;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.ColumnDataManager
{
    /// <summary>
    /// TaskState emum represents the states of the task
    /// </summary>
    internal enum TaskState
    {
        BackLog =0,
        InProgress =1,
        Done =2
    }

    /// <summary>
    /// the ColumnDTO class represents a column in the DL
    /// </summary>
    internal class ColumnDTO
    {
        /// <summary>
        /// names of the tables columns
        /// </summary>
        public const string NAME_COLUMN ="NAME";
        public const string TASK_AMOUNT_COLUMN ="TASK_AMOUNT_LIMIT";
        public const string TASK_STATE_COLUMN ="STATE";
        public const string BOARD_ID_COLUMN ="BOARD_ID";

        /// <summary>
        /// the data manager of this columnDTO
        /// </summary>
        private ColumnDataManager columnDataManager;
        /// <summary>
        /// the taskState field and getter of this column
        /// </summary>
        public TaskState TaskState { get; private set; }
        /// <summary>
        /// the name field and getter of this column
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// the TaskAmountLimit field and getter of this column
        /// </summary>
        public int TaskAmountLimit { get; private set; }
        /// <summary>
        /// the id field and getter of this column
        /// </summary>
        public int BoardId { get; private set; }

        /// <summary>
        /// this columnDTO constactor, initializes the ColumnDataManager
        /// </summary>
        /// <param name="taskState">the task state of this column</param>
        /// <param name="name">the name of this column</param>
        /// <param name="boardId">the id of the board the column belongs to</param>
        /// <param name="taskAmountLimit">the taskAmountLimit of this column</param>
        public ColumnDTO(int taskState, string name, int boardId, int taskAmountLimit)
        {
            this.Name = name;
            this.TaskAmountLimit = taskAmountLimit;
            this.BoardId = boardId;
            this.TaskState = (TaskState) taskState;
            columnDataManager = new ColumnDataManager();
        }

        /// <summary>
        /// column name setter
        /// </summary>
        /// <param name="newName">the new name</param>
        /// <returns>returns trues if operation has succeeded and false otherwise</returns>
        public bool SetName(string newName)
        {
            if (this.columnDataManager.UpdateDTO(this.BoardId, (int)this.TaskState, NAME_COLUMN, newName)) {
                this.Name = newName;
                return true;
            }
            return false;
        }
        /// <summary>
        /// column taskAmountLimit setter
        /// </summary>
        /// <param name="newLimit">the new limit</param>
        /// <returns>returns trues if operation has succeeded and false otherwise</returns>
        public bool SetTaskAmountLimit(int newLimit)
        {
            if (this.columnDataManager.UpdateDTO(this.BoardId, (int) this.TaskState, TASK_AMOUNT_COLUMN, newLimit))
            {
                this.TaskAmountLimit = newLimit;
                return true;
            }
            return false;
        }
        /// <summary>
        /// inserts dto to the DB
        /// </summary>
        /// <returns>returns trues if operation has succeeded and false otherwise</returns>
        public bool InsertDTO() {
            return columnDataManager.InsertDTO(this);
        }

    }
}
