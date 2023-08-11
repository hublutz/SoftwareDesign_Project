using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.TaskServices;
using System.Text.Json;

namespace BackendTests.BoardTests
{
    /// <summary>
    /// Class <c>TestDeleteBoard</c> tests successful and unsuccessful operations
    /// of deleting boards.
    /// Namely, this class tests requirements 9 and 11
    /// </summary>
    internal class TestDeleteBoard : AbstractBoardTest
    {
        /// <value>
        /// field <c>boardId</c> is used to store a board's id
        /// </value>
        private int boardId;

        /// <summary>
        /// This method runs the tests related to deleting boards - requirement 9
        /// </summary>
        public override void RunTests()
        {
            this.userService.Register(EMAIL1, VALID_PASSWORD);
            this.testDeletingNonExistingBoard();
            this.addBoard(EMAIL1, BOARD_NAME);
            this.testSuccessfulDeleteBoard();
            this.testDeletingBoardToNonExistingUser();

            // Re-create the board
            this.addBoard(EMAIL1, BOARD_NAME);

            this.userService.Logout(EMAIL1);
            this.testDeletingBoardWhenLoggedOut();
            this.userService.Login(EMAIL1, VALID_PASSWORD);

            this.boardId = this.extractBoardId(EMAIL1);
            this.userService.Register(EMAIL2, VALID_PASSWORD);
            this.boardService.JoinBoard(EMAIL2, this.boardId);
            this.testDeletingBoardByBoardMember();

            this.testDeletingBoardRemovesFromMember();
        }

        /// <summary>
        /// This method deletes a board from a user using <c>BoardService</c>
        /// </summary>
        /// <param name="email">The user's email</param>
        /// <param name="boardName">The board name to delete</param>
        /// <returns>The response of the deletion operation from the Service Layer</returns>
        private Response deleteBoard(string email, string boardName)
        {
            string responseJson = this.boardService.DeleteBoard(email, boardName);
            return JsonSerializer.Deserialize<Response>(responseJson);
        }

        /// <summary>
        /// This method tests deleting a board from a user, when the board doesn't
        /// exist
        /// </summary>
        private void testDeletingNonExistingBoard()
        {
            Response response = this.deleteBoard(EMAIL1, BOARD_NAME);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Deleted board, however it doesn't exist");
            }
            else
            {
                printTestSucceeded("Failed to delete non - existing board.Error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests successful deletion of board from a user
        /// </summary>
        private void testSuccessfulDeleteBoard()
        {
            Response response = this.deleteBoard(EMAIL1, BOARD_NAME);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestSucceeded("Deleted board successfully");
            }
            else
            {
                printTestFailed("Failed to delete board. Error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests failure of deleting a board from a user that doesn't exist
        /// </summary>
        private void testDeletingBoardToNonExistingUser()
        {
            Response response = this.deleteBoard(EMAIL2, BOARD_NAME);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Deleted board, however the user doesn't exist");
            }
            else
            {
                printTestSucceeded("Failed to delete board for non - existing user.Error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests deleteing a board fails when the user is logged out
        /// </summary>
        private void testDeletingBoardWhenLoggedOut()
        {
            Response response = this.deleteBoard(EMAIL1, BOARD_NAME);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("A logged out user can't delete a board");
            }
            else
            {
                printTestSucceeded("A logged out user failed to delete board. Error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests a board member who isn't the owner can't delete the board
        /// </summary>
        private void testDeletingBoardByBoardMember()
        {
            Response response = this.deleteBoard(EMAIL2, BOARD_NAME);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("A board member who isn't the owner deleted the board");
            }
            else
            {
                printTestSucceeded("A board member who isn't the owner failed to delete the board. Error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests deleting a board, and then checking it doesn't appear in the member's
        /// boards
        /// </summary>
        public void testDeletingBoardRemovesFromMember()
        {
            this.deleteBoard(EMAIL1, BOARD_NAME);
            // the second user should have no boards
            string responseJson = this.boardService.GetAllUserBoards(EMAIL2);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                string[] boards = JsonSerializer.Deserialize<string[]>(response.Message.ToString());
                if (boards.Length == 0)
                {
                    printTestSucceeded("The joined user has no boards");
                }
                else
                {
                    printTestFailed("The joined user is a member, but the board was deleted");
                }
            }
            else
            {
                printTestFailed("Failed to get boards of user " + EMAIL2 + ". Error: " + response.Message);
            }
        }
    }
}
