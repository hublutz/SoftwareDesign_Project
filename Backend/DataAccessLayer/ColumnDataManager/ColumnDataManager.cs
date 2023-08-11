
using System.Collections.Generic;
using System.Data.SQLite;

namespace IntroSE.Kanban.Backend.DataAccessLayer.ColumnDataManager
{
    /// <summary>
    /// the data manager class, preforms operation on the Column table in the DB 
    /// </summary>
    internal class ColumnDataManager : AbstractDataManager<ColumnDTO>
    {
        /// <summary>
        /// the name of the columns table
        /// </summary>
        public const string COLUMNS_TABLE_NAME = "Columns";
        /// <summary>
        /// The column name of board id
        /// </summary>
        public const string BOARD_ID_COLUMN_NAME = "BOARD_ID";

        /// <summary>
        /// val constans used in insert DTO
        /// </summary>
        private readonly string NAME_VAL = "nameVal";
        private readonly string TASK_AMOUNT_VAL = "taskAmountVal";
        private readonly string BOARD_ID_VAL = "boardIdVal";
        private readonly string TASK_STATE_VAL = "taskStateVal";

        /// <summary>
        /// columns ordinal numbers
        /// </summary>
        private readonly int TASK_STATE_COLUMN = 0;
        private readonly int NAME_COLUMN = 1;
        private readonly int TASK_AMOUNT_COLUMN = 2;
        private readonly int BOARD_ID_COLUMN = 3;

        /// <summary>
        /// the constractor of the DM
        /// </summary>
        public ColumnDataManager() : base(COLUMNS_TABLE_NAME) { }

        /// <summary>
        /// inserts a DTO to the DB
        /// </summary>
        /// <param name="dto">the dto to insert</param>
        /// <returns>returns trues if operation has succeeded and false otherwise</returns>
        public override bool InsertDTO(ColumnDTO dto)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {this.tableName} ({ColumnDTO.NAME_COLUMN},{ColumnDTO.TASK_AMOUNT_COLUMN}," +
                        $"{ColumnDTO.BOARD_ID_COLUMN},{ColumnDTO.TASK_STATE_COLUMN}) " + $"VALUES (@{NAME_VAL}, @{TASK_AMOUNT_VAL}, @{BOARD_ID_VAL}, @{TASK_STATE_VAL}); ";

                    SQLiteParameter nameParam = new SQLiteParameter(@"" + NAME_VAL, dto.Name);
                    SQLiteParameter taskAmountParam = new SQLiteParameter(@"" + TASK_AMOUNT_VAL, dto.TaskAmountLimit);
                    SQLiteParameter boardIdParam = new SQLiteParameter(@"" + BOARD_ID_VAL, dto.BoardId);
                    SQLiteParameter taskStateParam = new SQLiteParameter(@"" + TASK_STATE_VAL, dto.TaskState);

                    command.Parameters.Add(nameParam);
                    command.Parameters.Add(taskAmountParam);
                    command.Parameters.Add(boardIdParam);
                    command.Parameters.Add(taskStateParam);

                    command.Prepare();
                    res = command.ExecuteNonQuery();
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                return res > 0;
            }
        }
        /// <summary>
        /// convrts a SQLiteDataReader to a ColumnDTO
        /// </summary>
        /// <param name="reader">the SQLiteDataReader</param>
        /// <returns>the ColumnDTO</returns>
        protected override ColumnDTO convertReaderToDTO(SQLiteDataReader reader)
        {
            return new ColumnDTO(reader.GetInt32(TASK_STATE_COLUMN), reader.GetString(NAME_COLUMN), 
                reader.GetInt32(BOARD_ID_COLUMN), reader.GetInt32(TASK_AMOUNT_COLUMN));
        }

        /// <summary>
        /// gets colums of a board
        /// </summary>
        /// <param name="boardId">the id of the board</param>
        /// <returns>a list of ColumnDTO </returns>
        public List<ColumnDTO> SelectBoardColumns(int boardId) {
            List <ColumnDTO> result = new List <ColumnDTO>();
            using (var connection = new SQLiteConnection(connectionString))
            {

                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"SELECT * FROM {this.tableName} WHERE {BOARD_ID_COLUMN_NAME} =@{BOARD_ID_VAL};";
                SQLiteDataReader dataReader = null;
                try
                {
                    command.Parameters.Add(new SQLiteParameter(BOARD_ID_VAL, boardId));
                    connection.Open();
                    command.Prepare();
                    dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        result.Add(convertReaderToDTO(dataReader));
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
            return result;
        }
        /// <summary>
        /// deletes colums of a board
        /// </summary>
        /// <param name="boardId">the id of the board</param>
        /// <returns>returns trues if operation has succeeded and false otherwise</returns>
        public bool DeleteBoardColumns(int boardId) {
            int res = -1;
            using (var connection = new SQLiteConnection(connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"DELETE FROM {this.tableName} WHERE {BOARD_ID_COLUMN_NAME} =@{BOARD_ID_VAL};"
                };
                try
                {
                    command.Parameters.Add(new SQLiteParameter(BOARD_ID_VAL, boardId));
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

        /// <summary>
        /// an override to the update method in abstract data manager. The method fined the attribute by the board its lockated in and its state and updates a attribute of the column 
        /// </summary> 
        ///<param name="boardId">the id of the board the column is in</param>
        /// <param name="taskState">the state of the column</param>
        /// <param name="attributeName">the name of the column of the atribute to update</param>
        /// <param name="attributeValue">the new atribute value</param>
        /// <returns>returns trues if operation has succeeded and false otherwise</returns>
        public bool UpdateDTO(int boardId, int taskState , string attributeName, object attributeValue)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand()
                {
                    Connection = connection,
                    CommandText =
                    $"UPDATE {this.tableName} SET [{attributeName}]=@{attributeName} WHERE {ColumnDTO.TASK_STATE_COLUMN}=@{TASK_STATE_VAL} " +
                    $"AND {ColumnDTO.BOARD_ID_COLUMN}=@{BOARD_ID_VAL}"
                };
                try
                {
                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    command.Parameters.Add(new SQLiteParameter(TASK_STATE_VAL, taskState));
                    command.Parameters.Add(new SQLiteParameter(BOARD_ID_VAL, boardId));
    

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
