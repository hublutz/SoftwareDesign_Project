namespace IntroSE.Kanban.Backend.ServiceLayer.BoardService
{
    /// <summary>
    /// Class <c>BoardToSend</c> represents Service Layer board
    /// data returned from board related operations
    /// </summary>
    internal class BoardToSend
    {
        /// <value>
        /// <c>NAME</c> represents the name of the board
        /// </value>
        public readonly string NAME;

        /// <summary>
        /// <c>BoardToSend</c> constructor, initialising its name
        /// </summary>
        /// <param name="name">The name of the board</param>
        public BoardToSend(string name)
        {
            this.NAME = name;
        }
    }
}
