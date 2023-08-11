
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    /// <summary>
    /// abstract data manager class
    /// </summary>
    /// <typeparam name="T">the dto type of the class that is using this class</typeparam>
    internal abstract class AbstractDataManager<T>
    {
        /// <summary>
        /// the data bases file name
        /// </summary>
        public const string KANBAN_DB = "kanban.db";
        /// <summary>
        /// the connection string
        /// </summary>
        protected string connectionString;
        /// <summary>
        /// the sql tables name
        /// </summary>
        protected readonly string tableName;


        /// <summary>
        /// constractor initializes the connectionString and tableName parameters
        /// </summary>
        /// <param name="tableName">the name of the table that the operation are operating on</param>
        public AbstractDataManager(string tableName) {
            
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), KANBAN_DB));

            connectionString = $"Data Source={path}; Version=3;";
            this.tableName = tableName;
        }

        /// <summary>
        /// abstract method that should insert a new dto to the table
        /// </summary>
        /// <param name="dto">the dto to insert</param>
        /// <returns>returns true is the dto was successfuly inserted</returns>
        public abstract bool InsertDTO(T dto);
        /// <summary>
        /// abstract method that should convert a SQLiteDataReader to a DTO
        /// </summary>
        /// <param name="reader">the data reader we convevrt to a DTO</param>
        /// <returns>return a DTO</returns>
        protected abstract T convertReaderToDTO(SQLiteDataReader reader);

        /// <summary>
        /// a method that deletes all of the tables rows
        /// </summary>
        /// <returns>returns true is the table was successfuly deleted</returns>
        public bool DeleteData() {
            int res = -1;
            using (var connection = new SQLiteConnection(connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"DELETE FROM {this.tableName};"
                };
                try
                {
                    connection.Open();
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
        /// a method that is used to get the data from the DB
        /// </summary>
        /// <returns>returns a list of all the DTO's in the table</returns>
        public List<T> LoadData() {
            List<T> result = new List<T>();
            using(var connection = new SQLiteConnection(connectionString)){

                SQLiteCommand command = new SQLiteCommand(null,connection);
                command.CommandText = $"SELECT * FROM {this.tableName};";
                SQLiteDataReader dataReader = null;
                try
                {
                    connection.Open();
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
        /// a method that updates a value in the DB 
        /// </summary>
        /// <param name="rowFilteringName">the name of the attribute with which we want to find the row </param>
        /// <param name="rowfilteringValue">the value of that attribute</param>
        /// <param name="attributeName">the name of the attribute that we want to change</param>
        /// <param name="attributeValue">the new value of that attribute</param>
        /// <returns>true if the operation was successful</returns>
        public bool UpdateDTO(string rowFilteringName, object rowfilteringValue,string attributeName, object attributeValue){
            int res = -1;
            using (var connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand()
                {
                    Connection = connection,
                    CommandText =
                    $"UPDATE {this.tableName} SET [{attributeName}]=@{attributeName} WHERE [{rowFilteringName}]=@{rowFilteringName}"
                };
                try
                {
                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    command.Parameters.Add(new SQLiteParameter(rowFilteringName, rowfilteringValue));
                    connection.Open();
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
