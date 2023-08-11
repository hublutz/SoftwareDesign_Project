namespace Frontend.Model.Board
{
    /// <summary>
    /// Class <c>BoardModel</c> represnts a board in the Model Layer
    /// with its id and name
    /// </summary>
    public class BoardModel
    {
        /// <value>
        /// Field <c>Id</c> represents the id of the board
        /// </value>
        public int Id { get; private set; }

        /// <value>
        /// Field <c>Name</c> represents the name of the board
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// <c>BoardModel</c> constructor
        /// </summary>
        /// <param name="id">The id of the board</param>
        /// <param name="name">The name of the board</param>
        public BoardModel(int id, string name)
        { 
            this.Id = id;
            this.Name = name;
        }
    }
}
