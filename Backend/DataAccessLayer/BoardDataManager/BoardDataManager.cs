
using System.Collections.Generic;
using System.Data.SQLite;

using System.Text.Json;


namespace IntroSE.Kanban.Backend.DataAccessLayer.BoardDataManager
{   /// <summary>
    /// the data manager class, preforms operation on the Boards table in the DB 
    /// </summary>
    internal class BoardDataManager : AbstractDataManager<BoardDTO>
    {
        /// <summary>
        /// the name of the boards table
        /// </summary>
        public const string BOARD_TABLE_NAME = "Boards";
        /// <summary>
        /// val constans used in insert DTO
        /// </summary>
        private readonly string ID_VAL = "idVal";
        private readonly string NAME_VAL = "nameVal";
        private readonly string BOARD_OWNER_EMAIL_VAL = "boardOwnerEmailVal";
        private readonly string BOARD_USERS_VAL = "boardUsersVal";

        /// <summary>
        /// columns numbers
        /// </summary>
        private readonly int ID_COLUMN =0;
        private readonly int NAME_COLUMN = 1;
        private readonly int BOARD_OWNER_EMAIL_COLUMN = 2;
        private readonly int BOARD_USERS_COLUMN = 3;

        /// <summary>
        /// the constractor of the DM
        /// </summary>
        public BoardDataManager() : base(BOARD_TABLE_NAME) {}

        /// <summary>
        /// inserts a DTO to the DB
        /// </summary>
        /// <param name="dto">the dto to insert</param>
        /// <returns>returns trues if operation has succeeded and false otherwise</returns>
        public override bool InsertDTO(BoardDTO dto)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null,connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {this.tableName} ({ BoardDTO.ID_COLUMN_NAME},{ BoardDTO.NAME_COLUMN_NAME}," +
                        $"{BoardDTO.BOARD_OWNER_COLUMN_NAME},{BoardDTO.BOARD_USERS_COLUMN_NAME}) " + $"VALUES (@{ID_VAL}, @{NAME_VAL}, @{BOARD_OWNER_EMAIL_VAL}, @{BOARD_USERS_VAL}); ";

                    SQLiteParameter idParam = new SQLiteParameter(@"" + ID_VAL, dto.Id);
                    SQLiteParameter titleParam = new SQLiteParameter(@"" + NAME_VAL,dto.Name );
                    SQLiteParameter bodyParam = new SQLiteParameter(@"" + BOARD_OWNER_EMAIL_VAL, dto.BoardOwnerEmail);
                    SQLiteParameter forumParam = new SQLiteParameter(@"" + BOARD_USERS_VAL, JsonSerializer.Serialize(dto.BoardUsers));
                   
                    command.Parameters.Add(idParam);
                    command.Parameters.Add(titleParam);
                    command.Parameters.Add(bodyParam);
                    command.Parameters.Add(forumParam);
                    
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
        /// convrts a SQLiteDataReader to a BoardDTO
        /// </summary>
        /// <param name="reader">the SQLiteDataReader</param>
        /// <returns>the BoardDTO</returns>
        protected override BoardDTO convertReaderToDTO(SQLiteDataReader reader)
        {
            return new BoardDTO(reader.GetInt32(ID_COLUMN), reader.GetString(NAME_COLUMN), reader.GetString(BOARD_OWNER_EMAIL_COLUMN),
                JsonSerializer.Deserialize<HashSet<string>>(reader.GetString(BOARD_USERS_COLUMN)));
        }
        /// <summary>
        /// deletes a board from the DB
        /// </summary>
        /// <param name="boardId">the id of the board to delete</param>
        /// <returns>returns trues if operation has succeeded and false otherwise</returns>
        public bool DeleteBoard(int boardId)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(connectionString))
            {
                var command = new SQLiteCommand()
                {
                    Connection = connection,
                    CommandText = $"DELETE FROM {this.tableName} WHERE {BoardDTO.ID_COLUMN_NAME} =@{BoardDTO.ID_COLUMN_NAME};"
                };
                try
                {
                    command.Parameters.Add(new SQLiteParameter(BoardDTO.ID_COLUMN_NAME, boardId));
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

    

