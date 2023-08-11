using IntroSE.Kanban.Backend.ServiceLayer;
using System.Xml.Linq;

namespace BackendTests.BoardTests
{
    /// <summary>
    /// Class <c>TestAddBoard</c> tests successful and unsuccessful 
    /// operations related to creating user boards. 
    /// Namely, this class tests requirements 6 and 9
    /// </summary>
    internal class TestAddBoard : AbstractBoardTest
    {
        /// <summary>
        /// This method runs the tests related to creating boards - requirement 9
        /// </summary>
        public override void RunTests()
        {
            // add user to add a board to
            this.userService.Register(EMAIL1, VALID_PASSWORD);

            this.testSuccessfulAddBoard();
            this.testAddBoardToNonExistingUser();
            this.testAddExistingBoard();

            this.testAddExistingBoardToDifferentUser();

            this.testAddBoardWhenLoggedOut();

            // Delete second user's board and test the user can't add a board with the same name of the one he joined
            this.boardService.DeleteBoard(EMAIL2, BOARD_NAME);
            this.testAddBoardWithJoinedBoardName();
        }

        /// <summary>
        /// This method tests a successful creation of a board to an existing user
        /// </summary>
        private void testSuccessfulAddBoard()
        {
            Response response = this.addBoard(EMAIL1, BOARD_NAME);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestSucceeded("Added board sucessfuly to " + EMAIL1);
            }
            else
            {
                printTestFailed("Failed to add board. error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests the if creation of a board fails if the user doesn't exist
        /// </summary>
        private void testAddBoardToNonExistingUser()
        {
            Response response = this.addBoard(EMAIL2, BOARD_NAME);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Added board to " + EMAIL2 + ", but the user doesn't exist");
            }
            else
            {
                printTestSucceeded("Didn't add board to non-existing user. error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests if the creation of a board fails if the user already 
        /// has a board of such name
        /// </summary>
        private void testAddExistingBoard()
        {
            Response response = this.addBoard(EMAIL1, BOARD_NAME);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Added board to " + EMAIL1 + ", but the board already exists");
            }
            else
            {
                printTestSucceeded("Didn't add existing board. error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests adding a board with name which 
        /// exists in another user, to a new and different user
        /// </summary>
        private void testAddExistingBoardToDifferentUser()
        {
            this.userService.Register(EMAIL2, VALID_PASSWORD);
            Response response = this.addBoard(EMAIL2, BOARD_NAME);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestSucceeded("Successfully added two boards with the same name to two different users");
            }
            else
            {
                printTestFailed("Failed to add the board altough it's a different user." +
                    " error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests adding a board fails when the user is logged out
        /// </summary>
        private void testAddBoardWhenLoggedOut()
        {
            this.userService.Logout(EMAIL1);
            Response response = this.addBoard(EMAIL1, BOARD_NAME + "_loggedout");
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("A logged out user can't add a board");
            }
            else
            {
                printTestSucceeded("A logged out user failed to add board. Error: " + response.Message);
            }
            this.userService.Login(EMAIL1, VALID_PASSWORD);
        }

        /// <summary>
        /// This method tests a user can't add a board with the same name of another board
        /// the user has joined
        /// </summary>
        private void testAddBoardWithJoinedBoardName()
        {
            int boardId = this.extractBoardId(EMAIL1);
            this.boardService.JoinBoard(EMAIL2, boardId);
            Response response = this.addBoard(EMAIL2, BOARD_NAME);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Can't add a board with the same name of another board a user joined to");
            }
            else
            {
                printTestSucceeded("Failed to add board with the name of joined one. Error: " + response.Message);
            }
        }
    }
}
