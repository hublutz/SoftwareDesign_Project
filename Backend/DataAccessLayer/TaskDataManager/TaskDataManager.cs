using System.Collections.Generic;
using System.Data.SQLite;

namespace IntroSE.Kanban.Backend.DataAccessLayer.TaskDataManager
{
    /// <summary>
    /// Class <c>TaskDataManager</c> handles database operations related to
    /// Tasks table (see <see cref="TASKS_TABLE"/>)
    /// </summary>
    internal class TaskDataManager : AbstractDataManager<TaskDTO>
    {
        /// <summary>
        /// Constants related to Tasks table schema
        /// </summary>
        public const string  TASKS_TABLE = "Tasks";
        public const string  ID_COLUMN = "ID";
        public const string  CREATION_TIME_COLUMN = "CREATION_TIME";
        public const string  DUE_DATE_COLUMN = "DUE_DATE";
        public const string  TITLE_COLUMN = "TITLE";
        public const string  DESCRIPTION_COLUMN = "DESCRIPTION";
        public const string  STATE_COLUMN = "STATE";
        public const string  ASSIGNEE_COLUMN = "ASSIGNEE";
        public const string  BOARD_ID_COLUMN = "BOARD_ID";
        
        /// <summary>
        /// Constants representing the ordinals of each column in Tasks table
        /// </summary>
        public const int ID_ORDINAL = 0;
        public const int CREATION_TIME_ORDINAL = 1;
        public const int DUE_DATE_ORDINAL = 2;
        public const int TITLE_ORDINAL = 3;
        public const int DESCRIPTION_ORDINAL = 4;
        public const int STATE_ORDINAL = 5;
        public const int ASSIGNEE_ORDINAL = 6;
        public const int BOARD_ID_ORDINAL = 7;
        
        /// <summary>
        /// Constants used for <c>SQLiteParameter</c> annotations
        /// </summary>
        public const string ID_PARAM = "idParam";
        public const string CREATION_TIME_PARAM = "creationTimeParam";
        public const string DUE_DATE_PARAM = "dueDateParam";
        public const string TITLE_PARAM = "titleParam";
        public const string DESCRIPTION_PARAM = "descriptionParam";
        public const string STATE_PARAM = "stateParam";
        public const string ASSIGNEE_PARAM = "assigneeParam";
        public const string BOARD_ID_PARAM = "boardIdParam";
        public const string ATTRIBUTE_TO_UPDATE_PARAM = "attributeParam";

        /// <summary>
        /// <c>TaskDataManager</c> constructor, initializes the table
        /// name to <see cref="TASKS_TABLE"/>
        /// </summary>
        public TaskDataManager() : base(TASKS_TABLE) { }

        /// <summary>
        /// This helper method adds all the <c>TaskDTO</c> fields
        /// into an SQLite command
        /// </summary>
        /// <param name="command">The SQLite command</param>
        /// <param name="dto">The task to insert its values</param>
        private void addParameterValuesToCommand(SQLiteCommand command, TaskDTO dto)
        {
            command.Parameters.Add(new SQLiteParameter(ID_PARAM, dto.Id));
            command.Parameters.Add(new SQLiteParameter(CREATION_TIME_PARAM, 
                dto.GetCreationTimeString()));
            command.Parameters.Add(new SQLiteParameter(DUE_DATE_PARAM, 
                dto.GetDueDateString()));
            command.Parameters.Add(new SQLiteParameter(TITLE_PARAM, dto.Title));
            command.Parameters.Add(new SQLiteParameter(DESCRIPTION_PARAM, dto.Description));
            command.Parameters.Add(new SQLiteParameter(STATE_PARAM, dto.GetState()));
            command.Parameters.Add(new SQLiteParameter(ASSIGNEE_PARAM, dto.Assignee));
            command.Parameters.Add(new SQLiteParameter(BOARD_ID_PARAM, dto.BoardId));
        }

        /// <summary>
        /// This method inserts the given task to the Tasks table
        /// </summary>
        /// <param name="dto">The task to insert in the database</param>
        /// <returns>Return true if the insertion succeeded. Else, returns false</returns>
        public override bool InsertDTO(TaskDTO dto)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                int result = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"INSERT INTO {this.tableName} " +
                    $"({ID_COLUMN}, {CREATION_TIME_COLUMN}, {DUE_DATE_COLUMN}, {TITLE_COLUMN}, " +
                    $"{DESCRIPTION_COLUMN}, {STATE_COLUMN}, {ASSIGNEE_COLUMN}, {BOARD_ID_COLUMN}) " +
                    $"VALUES (@{ID_PARAM}, @{CREATION_TIME_PARAM}, @{DUE_DATE_PARAM}, @{TITLE_PARAM}, " +
                    $"@{DESCRIPTION_PARAM}, @{STATE_PARAM}, @{ASSIGNEE_PARAM}, @{BOARD_ID_PARAM});";
                try
                {
                    connection.Open();
                    this.addParameterValuesToCommand(command, dto);
                    command.Prepare();
                    result = command.ExecuteNonQuery();
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                return result > 0;
            }
        }

        /// <summary>
        /// This method converts the next row in the <c>SQLiteDataReader</c>
        /// to a new <c>TaskDTO</c>
        /// </summary>
        /// <param name="reader">The reader received from a select query. 
        /// Assumed to not be null</param>
        /// <returns>A new <c>TaskDTO</c> with the data of the current row</returns>
        protected override TaskDTO convertReaderToDTO(SQLiteDataReader reader)
        {
            int id = reader.GetInt32(ID_ORDINAL);
            string creation = reader.GetString(CREATION_TIME_ORDINAL);
            string due_date = reader.GetString(DUE_DATE_ORDINAL);
            string title = reader.GetString(TITLE_ORDINAL);
            string description = reader.GetString(DESCRIPTION_ORDINAL);
            int state = reader.GetInt32(STATE_ORDINAL);
            string assignee;
            // retrieve assignee only if not null
            if (reader.IsDBNull(ASSIGNEE_ORDINAL))
            {
                assignee = null;
            }
            else
            {
                assignee = reader.GetString(ASSIGNEE_ORDINAL);
            }
            int boardId = reader.GetInt32(BOARD_ID_ORDINAL);
            return new TaskDTO(id, creation, due_date, title, description, state, assignee, boardId);
        }

        /// <summary>
        /// This method returns all the tasks related to the
        /// given board
        /// </summary>
        /// <param name="boardId">The id of the board to get all its tasks</param>
        /// <returns>A list of tasks in contained in the board</returns>
        public List<TaskDTO> SelectBoardTasks(int boardId)
        {
            List<TaskDTO> boardTasks = new List<TaskDTO>();
            using (var connection = new SQLiteConnection(connectionString))
            {

                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"SELECT * FROM {this.tableName} WHERE {BOARD_ID_COLUMN} = @{BOARD_ID_PARAM};";
                SQLiteDataReader dataReader = null;
                try
                {
                    command.Parameters.Add(new SQLiteParameter(BOARD_ID_PARAM, boardId));
                    connection.Open();
                    command.Prepare();
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        boardTasks.Add(convertReaderToDTO(dataReader));
                    }
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Close();
                    }
                    command.Dispose();
                    connection.Close();
                }

            }
            return boardTasks;
        }

        /// <summary>
        /// This method deletes all the tasks related to the given board
        /// </summary>
        /// <param name="boardId">The id of the board to delete its tasks</param>
        /// <returns>True if the deletion succeeded, else false</returns>
        public bool DeleteBoardTasks(int boardId)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                int result = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"DELETE FROM {this.tableName} " +
                    $"WHERE {BOARD_ID_COLUMN}=@{BOARD_ID_PARAM};";
                try
                {
                    connection.Open();
                    command.Parameters.Add(new SQLiteParameter(BOARD_ID_PARAM, boardId));
                    command.Prepare();
                    result = command.ExecuteNonQuery();
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                return result > 0;
            }
        }

        /// <summary>
        /// The method updates the task's attribute by its id and the board its located in
        /// </summary> 
        ///<param name="boardId">the id of the board the column is in</param>
        /// <param name="taskId">the id of the task</param>
        /// <param name="attributeName">the name of the column of the atribute to update</param>
        /// <param name="attributeValue">the new atribute value</param>
        /// <returns>returns trues if operation has succeeded and false otherwise</returns>
        public bool UpdateDTO(int boardId, int taskId, string attributeName, object attributeValue)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand()
                {
                    Connection = connection,
                    CommandText =
                    $"UPDATE {this.tableName} SET [{attributeName}]=@{ATTRIBUTE_TO_UPDATE_PARAM} WHERE {BOARD_ID_COLUMN} = @{BOARD_ID_PARAM} " +
                    $"AND {ID_COLUMN}=@{ID_PARAM};"
                };
                try
                {
                    command.Parameters.Add(new SQLiteParameter(BOARD_ID_PARAM, boardId));
                    command.Parameters.Add(new SQLiteParameter(ID_PARAM, taskId));
                    command.Parameters.Add(new SQLiteParameter(ATTRIBUTE_TO_UPDATE_PARAM, attributeValue));

                    connection.Open();
                    command.Prepare();
                    res = command.ExecuteNonQuery();
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            return res > 0;
        }
    }
}
