using System.Data.SQLite;

namespace IntroSE.Kanban.Backend.DataAccessLayer.UserDataManager
{
    /// <summary>
    /// Class <c>UserDataManager</c> is responsible for managing
    /// the Users table of the database
    /// </summary>
    internal class UserDataManager : AbstractDataManager<UserDTO>
    {
        /// <summary>
        /// Constants related to the Users database schema
        /// </summary>
        public const string USERS_TABLE = "Users";
        public const string EMAIL_COLUMN = "EMAIL";
        public const string PASSWORD_COLUMN = "PASSWORD";
        public const int EMAIL_COLUMN_ORDINAL = 0;
        public const int PASSWORD_COLUMN_ORDINAL = 1;

        /// <summary>
        /// Constants used for <c>SQLiteParameter</c> annotations
        /// </summary>
        private const string EMAIL_PARAM = "emailParam";
        private const string PASSWORD_PARAM = "passwordParam";

        /// <summary>
        /// <c>UserDataManger</c> constructor, initializing the table name
        /// with <see cref="USERS_TABLE"/>
        /// </summary>
        public UserDataManager() : base(USERS_TABLE) { }

        /// <summary>
        /// This method inserts a new <c>UserDTO</c> to the database
        /// </summary>
        /// <param name="dto">The new <c>UserDTO</c> to insert</param>
        /// <returns>returns true if the insertion succeeded, else false</returns>
        public override bool InsertDTO(UserDTO dto)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                int result = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"INSERT INTO {this.tableName} " +
                    $"({EMAIL_COLUMN}, {PASSWORD_COLUMN}) " +
                    $"VALUES (@{EMAIL_PARAM}, @{PASSWORD_PARAM});";
                try
                {
                    connection.Open();

                    SQLiteParameter emailParam = new SQLiteParameter(EMAIL_PARAM, dto.Email);
                    SQLiteParameter passwordParam = new SQLiteParameter(PASSWORD_PARAM, dto.Password);
                    command.Parameters.Add(emailParam);
                    command.Parameters.Add(passwordParam);

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
        /// This method converts the next row stored in <c>SQLiteDataReader</c>
        /// to a new <c>UserDTO</c>
        /// </summary>
        /// <param name="reader">The reader from <c>SELECT</c> query. Assumed to not be null</param>
        /// <returns>returns a new DTO from the next row in the reader</returns>
        protected override UserDTO convertReaderToDTO(SQLiteDataReader reader)
        {
            return new UserDTO(reader.GetString(EMAIL_COLUMN_ORDINAL), reader.GetString(PASSWORD_COLUMN_ORDINAL));
        }
    }
}
