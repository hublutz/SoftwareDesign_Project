using IntroSE.Kanban.Backend.ServiceLayer;
using System.Text.Json;

namespace BackendTests.BoardTests
{
    /// <summary>
    /// Class <c>TestGetColumn</c> is used to test successful and unsuccessful operations for
    /// getting board's columns
    /// </summary>
    internal class TestGetColumn : AbstractBoardTest
    {
        private const int BACKLOG_COLUMN_ID = 0;
        private const int IN_PROGRESS_COLUMN_ID = 1;
        private const int DONE_COLUMN_ID = 2;
        private const int INVALID_COLUMN_ID = 100;

        /// <summary>
        /// this method runs tests related to getting columns of boards or column names
        /// </summary>
        public override void RunTests()
        {
            this.userService.Register(EMAIL1, VALID_PASSWORD);
            this.addBoard(EMAIL1, BOARD_NAME);

            this.testSuccessfulGetColumn(EMAIL1);
            this.testGetColumnToNonExistingUser();
            this.testGetColumnToNonExistingBoard();
            this.testGetInvalidColumn();
            this.testGetColumnWhenLoggedOut();

            this.userService.Register(EMAIL2, VALID_PASSWORD);
            this.testGetColumnToNonBoardMember();
            // Add the user to the board and test get column
            int boardId = this.extractBoardId(EMAIL1);
            this.boardService.JoinBoard(EMAIL2, boardId);
            this.testSuccessfulGetColumn(EMAIL2);
        }

        /// <summary>
        /// This method gets a board's column using <c>BoardService</c>
        /// </summary>
        /// <param name="email">The user's email</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The id of the column. Starts with 0 and ascends by 1</param>
        /// <returns>The response of the operation from the Service Layer</returns>
        private Response getColumn(string email, string boardName, int columnOrdinal)
        {
            string responseJson = this.boardService.GetColumn(email, boardName, columnOrdinal);
            return JsonSerializer.Deserialize<Response>(responseJson);
        }

        /// <summary>
        /// This method gets a board's column name using <c>BoardService</c>
        /// </summary>
        /// <param name="email">The user's email</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The id of the column. Starts with 0 and ascends by 1</param>
        /// <returns>The response of the operation from the Service Layer</returns>
        private Response getColumnName(string email, string boardName, int columnOrdinal)
        {
            string responseJson = this.boardService.GetColumnName(email, boardName, columnOrdinal);
            return JsonSerializer.Deserialize<Response>(responseJson);
        }

        /// <summary>
        /// This method tests successful operation for getting a column by id
        /// </summary>
        /// <param name="email">The email of the user who is a board member</param>
        /// <param name="columnOrdinal">The id of the column. Starts with 0 and ascends by 1</param>
        private void testSuccessfulGetColumn(string email, int columnOrdinal)
        {
            Response response = this.getColumn(email, BOARD_NAME, columnOrdinal);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestSucceeded("Getting column of id " + columnOrdinal + " was successful");
            }
            else
            {
                printTestFailed("Failed to get column of id " + columnOrdinal +
                    ". Error: " + response.Message);
            }

            response = this.getColumnName(email, BOARD_NAME, columnOrdinal);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestSucceeded("Getting column name of id " + columnOrdinal + " was successful");
            }
            else
            {
                printTestFailed("Failed to get column name of id " + columnOrdinal +
                    ". Error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests successful getting of all of a board's columns and their names
        /// </summary>
        /// <param name="email">The email of the user who is a board member</param>
        private void testSuccessfulGetColumn(string email)
        {
            this.testSuccessfulGetColumn(email, BACKLOG_COLUMN_ID);
            this.testSuccessfulGetColumn(email, IN_PROGRESS_COLUMN_ID);
            this.testSuccessfulGetColumn(email, DONE_COLUMN_ID);
        }

        /// <summary>
        /// This method tests failure of getting a column and column name if the user doesn't exist
        /// </summary>
        private void testGetColumnToNonExistingUser()
        {
            Response response = this.getColumn(EMAIL2, BOARD_NAME, BACKLOG_COLUMN_ID);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Got column, however the user doesn't exist");
            }
            else
            {
                printTestSucceeded("Failed to get column of for non-existing user. Error: " + response.Message);
            }

            response = this.getColumnName(EMAIL2, BOARD_NAME, BACKLOG_COLUMN_ID);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Got the column's name, however the user doesn't exist");
            }
            else
            {
                printTestSucceeded("Failed to get the column's name for non-existing user. Error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests failure of getting a column and its name if the board doesn't exist
        /// </summary>
        private void testGetColumnToNonExistingBoard()
        {
            Response response = this.getColumn(EMAIL1, BOARD_NAME + "123", BACKLOG_COLUMN_ID);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Got column, however the board doesn't exist");
            }
            else
            {
                printTestSucceeded("Failed to get column of for a board that doesn't exist. Error: " + response.Message);
            }

            response = this.getColumnName(EMAIL1, BOARD_NAME + "123", BACKLOG_COLUMN_ID);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Got the column's name, however the board doesn't exist");
            }
            else
            {
                printTestSucceeded("Failed to the get column's name for a board that doesn't exist. Error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests failure of getting a column and its name if the column id is invalid
        /// </summary>
        private void testGetInvalidColumn()
        {
            Response response = this.getColumn(EMAIL1, BOARD_NAME, INVALID_COLUMN_ID);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Got column, however the column ordinal is invalid");
            }
            else
            {
                printTestSucceeded("Failed to get column with invalid id. Error: " + response.Message);
            }

            response = this.getColumnName(EMAIL1, BOARD_NAME, INVALID_COLUMN_ID);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Got the column's name, however the column ordinal is invalid");
            }
            else
            {
                printTestSucceeded("Failed to get the column's name since the id is invalid. Error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests getting a column or its name fails when the user
        /// logged out
        /// </summary>
        private void testGetColumnWhenLoggedOut()
        {
            this.userService.Logout(EMAIL1);
            Response response = this.getColumn(EMAIL1, BOARD_NAME, IN_PROGRESS_COLUMN_ID);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Got column, however the user is logged out");
            }
            else
            {
                printTestSucceeded("Failed to get column for logged out user. Error: " + response.Message);
            }

            response = this.getColumnName(EMAIL1, BOARD_NAME, IN_PROGRESS_COLUMN_ID);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Got the column's name, however the user logged out");
            }
            else
            {
                printTestSucceeded("Failed to get the column's name to logged out user. Error: " + response.Message);
            }
            this.userService.Login(EMAIL1, VALID_PASSWORD);
        }

        /// <summary>
        /// This method tests a user that isn't a board member can't get a column or its name
        /// </summary>
        private void testGetColumnToNonBoardMember()
        {
            Response response = this.getColumn(EMAIL2, BOARD_NAME, BACKLOG_COLUMN_ID);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Got column, however the user isn't a board member");
            }
            else
            {
                printTestSucceeded("Failed to get column of for non-board member. Error: " + response.Message);
            }

            response = this.getColumnName(EMAIL2, BOARD_NAME, BACKLOG_COLUMN_ID);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Got the column's name, however the board doesn't exist");
            }
            else
            {
                printTestSucceeded("Failed to the get column's name for non-board member. Error: " + response.Message);
            }
        }
    }
}
