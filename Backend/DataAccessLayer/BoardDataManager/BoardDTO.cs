
using System.Collections.Generic;
using System.Text.Json;


namespace IntroSE.Kanban.Backend.DataAccessLayer.BoardDataManager
{
    /// <summary>
    /// the BoardDTO class represents a board in the DL
    /// </summary>
    internal class BoardDTO
    {
        /// <summary>
        /// names of the tables columns
        /// </summary>
        public const string ID_COLUMN_NAME = "ID";
        public const string NAME_COLUMN_NAME = "NAME";
        public const string BOARD_OWNER_COLUMN_NAME = "BOARD_OWNER_EMAIL";
        public const string BOARD_USERS_COLUMN_NAME = "BOARD_USERS";

        /// <summary>
        /// the data manager of this boardDTO
        /// </summary>
        private BoardDataManager boardDataManager;
        /// <summary>
        /// the id field and getter of this board
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// the Name field and getter of this board
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// the board owner email field and getter of this board
        /// </summary>
        public string BoardOwnerEmail { get; private set; }
        /// <summary>
        /// the Set of all of this board user emails field and getter
        /// </summary>
        public HashSet<string> BoardUsers { get;}

        /// <summary>
        /// the constractor of the board dto, initializes the BoardDataManager
        /// </summary>
        /// <param name="Id">the Id of the board</param>
        /// <param name="Name">the name of the board</param>
        /// <param name="BoardOwnerEmail">the owners email of this board</param>
        /// <param name="BoardUsers">the set of this board user emails </param>
        public BoardDTO(int Id, string Name, string BoardOwnerEmail, HashSet<string> BoardUsers)
        {
            this.Id = Id;
            this.Name = Name;
            this.BoardUsers = BoardUsers;
            this.BoardOwnerEmail = BoardOwnerEmail;

            this.boardDataManager = new BoardDataManager();
        }

        /// <summary>
        /// Inserts this DTO to the db
        /// </summary>
        /// <returns>returns trues if operation has succeeded and false otherwise</returns>
        public bool InsertDTO()
        {
            return this.boardDataManager.InsertDTO(this);
        }


        /// <summary>
        /// setter for the id field, updates the DB as well
        /// </summary>
        /// <param name="id">the new id</param>
        /// <returns>returns trues if operation has succeeded and false otherwise</returns>
        public bool setId(int id)
        {

            if (this.boardDataManager.UpdateDTO(ID_COLUMN_NAME, this.Id, ID_COLUMN_NAME, id))
            {
                this.Id = id;
                return true;
            }
            return false;
        }
        /// <summary>
        /// setter for the name field, updates the DB as well
        /// </summary>
        /// <param name="name">the new name</param>
        /// <returns>returns trues if operation has succeeded and false otherwise</returns>
        public bool setName(string name)
        {
            
            if (this.boardDataManager.UpdateDTO(ID_COLUMN_NAME, Id, NAME_COLUMN_NAME, name))
            {
                this.Name = name;
                return true;
            }
            return false;


        }
        /// <summary>
        /// setter for the boardOwnerEmail field, updates the DB as well
        /// </summary>
        /// <param name="boardOwnerEmail">the new email</param>
        /// <returns>returns trues if operation has succeeded and false otherwise</returns>
        public bool setBoardOwnerEmail(string boardOwnerEmail)
        {

            if (this.boardDataManager.UpdateDTO(ID_COLUMN_NAME, Id, BOARD_OWNER_COLUMN_NAME, boardOwnerEmail))
            {
                this.BoardOwnerEmail = boardOwnerEmail;
                return true;
            }
            return false;
        }

        /// <summary>
        /// adds a new users email to the BoardUsers, updates the DB as well 
        /// </summary>
        /// <param name="email">the email to add</param>
        /// <returns>returns trues if operation has succeeded and false otherwise</returns>
        public bool AddUserToBoard(string email)
        {
            this.BoardUsers.Add(email);

            if (boardDataManager.UpdateDTO(ID_COLUMN_NAME, Id, BOARD_USERS_COLUMN_NAME, JsonSerializer.Serialize(BoardUsers)))
            {
                return true;
            }
            this.BoardUsers.Remove(email);
            return false;
        }
        /// <summary>
        /// removes a users email to the BoardUsers, updates the DB as well 
        /// </summary>
        /// <param name="email">the email to remove</param>
        /// <returns>returns trues if operation has succeeded and false otherwise</returns>
        public bool RemoveUserToBoard(string email)
        {
            this.BoardUsers.Remove(email);

            if (boardDataManager.UpdateDTO(ID_COLUMN_NAME, Id, BOARD_USERS_COLUMN_NAME, JsonSerializer.Serialize(BoardUsers)))
            {
                return true;
            }
            this.BoardUsers.Add(email);
            return false;
        }

    }
}
