using IntroSE.Kanban.Backend.ServiceLayer;
using System.Text.Json;

namespace BackendTests.BoardTests
{
    /// <summary>
    /// Class <c>TestGetAllUserBoards</c> tests successful and unsuccessful operations
    /// for getting all the user's boards.
    /// This class also tests requirement 10
    /// </summary>
    internal class TestGetAllUserBoards : AbstractBoardTest
    {
        private const int DEFAULT_BOARDS_AMOUNT = 0;
        private const int BOARDS_TO_ADD_AMOUNT = 3;

        /// <summary>
        /// This method runs the tests of getting all boards of a user
        /// it also tests requirement 10 (method <see cref="testDeafultBoardsAmount">/>)
        /// </summary>
        public override void RunTests()
        {
            this.userService.Register(EMAIL1, VALID_PASSWORD);

            this.testDeafultBoardsAmount();
            this.testSuccessfulGetAllBoards();
            this.testGetAllBoardsToNonExistingUser();

            this.userService.Register(EMAIL2, VALID_PASSWORD);
            this.addBoard(EMAIL2, EMAIL2 + "_" + BOARD_NAME);
            this.testGetAllBoardsToBoardMember();

            this.testGetAllBoardWhenLoggedOut();
        }

        /// <summary>
        /// This method returns all the boards a user has
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <returns>Response containing the names of the user's boards</returns>
        private Response getAllUserBoards(string email)
        {
            string responseJson = this.boardService.GetAllUserBoards(email);
            return JsonSerializer.Deserialize<Response>(responseJson);
        }

        /// <summary>
        /// This method tests the default amount of boards a new user has is equal
        /// to <c>DEFAULT_BOARDS_AMOUNT</c>
        /// This method tests requirement 10
        /// </summary>
        private void testDeafultBoardsAmount()
        {
            Response response = this.getAllUserBoards(EMAIL1);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                string[] boardNames = JsonSerializer.Deserialize<string[]>(response.Message.ToString());
                if (boardNames.Length == DEFAULT_BOARDS_AMOUNT)
                {
                    printTestSucceeded("The default amount of boards for a new user is correct");
                }
                else
                {
                    printTestFailed("The default amount of boards for a new user is false");
                }
            }
            else
            {
                printTestFailed("Failed to get all the user's boards. Error: " + response.Message);
            }
        }

        /// <summary>
        /// This method adds boards to a user and tests all the boards are returned
        /// from <c>getAllUserBoards</c>
        /// </summary>
        private void testSuccessfulGetAllBoards()
        {
            // add boards to user
            for (int i = 1; i <=  BOARDS_TO_ADD_AMOUNT; i++) 
            {
                this.addBoard(EMAIL1, BOARD_NAME + i);
            }
            // test all boards were received
            Response response = this.getAllUserBoards(EMAIL1);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                string[] boardNames = JsonSerializer.Deserialize<string[]>(response.Message.ToString());
                if (boardNames.Length == BOARDS_TO_ADD_AMOUNT)
                {
                    printTestSucceeded("All " + BOARDS_TO_ADD_AMOUNT + " boards were received");
                }
                else
                {
                    printTestFailed("The operation didn't return the right amount of the user's boards");
                }
            }
            else
            {
                printTestFailed("Failed to get all the user's boards. Error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests <c>getAllUserBoards</c> fails if the user doesn't exist
        /// </summary>
        private void testGetAllBoardsToNonExistingUser()
        {
            Response response = this.getAllUserBoards(EMAIL2);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Got all the user's boards, but the user doesn't exist");
            }
            else
            {
                printTestSucceeded("Failed to get all the boards of the non-existing user. Error: " + response.Message);
            }
        }

        /// <summary>
        /// Test getting all board names returns all the owned boards of a user and all the
        /// boards the user is a member of
        /// </summary>
        private void testGetAllBoardsToBoardMember()
        {
            // Make a user join one board
            int boardId = this.extractBoardId(EMAIL1);
            this.boardService.JoinBoard(EMAIL2, boardId);
            int secondUserAmountOfBoards = 2;

            // test all boards were received - ones the user owns and is a member of
            Response response = this.getAllUserBoards(EMAIL2);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                string[] boardNames = JsonSerializer.Deserialize<string[]>(response.Message.ToString());
                if (boardNames.Length == secondUserAmountOfBoards)
                {
                    printTestSucceeded("Received joined boards and owned boards of " + EMAIL2);
                }
                else
                {
                    printTestFailed("Didn't receive all owned boards and joined boards for " + EMAIL2);
                }
            }
            else
            {
                printTestFailed("Failed to get all the user's boards. Error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests getting all boards fails when the user is logged out
        /// </summary>
        private void testGetAllBoardWhenLoggedOut()
        {
            this.userService.Logout(EMAIL1);
            Response response = this.getAllUserBoards(EMAIL1);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Got all the user's boards, but the user is logged out");
            }
            else
            {
                printTestSucceeded("Failed to get all the boards of the logged out user. Error: " + response.Message);
            }
            this.userService.Login(EMAIL1, VALID_PASSWORD);
        }
    }
}
