using IntroSE.Kanban.Backend.ServiceLayer;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text.Json;
using System.Threading.Channels;

namespace BackendTests.BoardTests
{
    /// <summary>
    /// Class <c>TestColumnLimit</c> tests setting and getting the limit of tasks
    /// a board's column can have.
    /// This class tests requirements 16 and 17
    /// </summary>
    internal class TestColumnLimit : AbstractBoardTest
    {
        private const int BACKLOG_COLUMN_ID = 0;
        private const int IN_PROGRESS_COLUMN_ID = 1;
        private const int DONE_COLUMN_ID = 2;
        private const int INVALID_COLUMN_ID = 100;

        private const int DEFAULT_COLUMN_LIMIT = -1;
        private const int INVALID_COLUMN_LIMIT = -100;
        private const int NEW_BACKLOG_LIMIT = 2;
        private const int NEW_IN_PROGRESS_LIMIT = 4;
        private const int NEW_DONE_LIMIT = 8;

        /// <summary>
        /// This method runs all tests of setting and getting
        /// column task limit.
        /// It tests requirements 11 and 12
        /// </summary>
        public override void RunTests()
        {
            this.userService.Register(EMAIL1, VALID_PASSWORD);
            this.addBoard(EMAIL1, BOARD_NAME);

            this.testDefultColumnLimit();
            this.testSuccessfulGetAndSetLimit(EMAIL1);

            this.testGetLimitToInvalidColumnId();
            this.testGetLimitToNonExistingUser();
            this.testGetLimitToNonExistingBoard();
            this.testGetLimitWhenLoggedOut();

            this.testSetLimitToInvalidColumnId();
            this.testSetLimitToNonExistingUser();
            this.testSetLimitToNonExistingBoard();
            this.testSetInvalidLimit();
            this.testSetLimitWhenLoggedOut();

            this.userService.Register(EMAIL2, VALID_PASSWORD);
            this.testGetLimitToNonBoardMember();
            this.testSetLimitToNonBoardMember();

            int boardId = this.extractBoardId(EMAIL1);
            this.boardService.JoinBoard(EMAIL2, boardId);
            this.testSuccessfulGetAndSetLimit(EMAIL2);
        }

        /// <summary>
        /// Gets response from the Service Layer trying to get the limit of a column in a board
        /// </summary>
        /// <param name="email">The user's email</param>
        /// <param name="board">The board's name</param>
        /// <param name="columnOrdinal">The column id. First is 0 and ascends by 1</param>
        /// <returns>The response of the operation from the Service Layer</returns>
        private Response getColumnLimit(string email, string board, int columnOrdinal)
        {
            string responseJson = this.boardService.GetColumnLimit(email, board, columnOrdinal);
            return JsonSerializer.Deserialize<Response>(responseJson);
        }

        /// <summary>
        /// Gets response from the Service Layer trying to set the limit of a column in a board
        /// </summary>
        /// <param name="email">The user's email</param>
        /// <param name="board">The board's name</param>
        /// <param name="columnOrdinal">The column id. First is 0 and ascends by 1</param>
        /// <param name="limit">The new limit to set to</param>
        /// <returns>The response of the operation from the Service Layer</returns>
        private Response setColumnLimit(string email, string board, int columnOrdinal, int limit)
        {
            string responseJson = this.boardService.LimitColumn(email, board, columnOrdinal, limit);
            return JsonSerializer.Deserialize<Response>(responseJson);
        }

        /// <summary>
        /// This method tests if the limit the System returns for a column is correct
        /// </summary>
        /// <param name="email">The email of the user who is a board member</param>
        /// <param name="columnOrdinal">The column id. First is 0 and ascends by 1</param>
        /// <param name="correctLimit">The excepted limit for the column</param>
        private void assertCorrectColumnLimit(string email, int columnOrdinal, int correctLimit)
        {
            Response response = this.getColumnLimit(email, BOARD_NAME, columnOrdinal);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                try
                {
                    int returnedLimit = int.Parse(response.Message.ToString());
                    if (returnedLimit == correctLimit)
                    {
                        printTestSucceeded("Received the expected column limit");
                    }
                    else
                    {
                        printTestFailed("Received an incorrect column limit");
                    }
                }
                catch (FormatException)
                {
                    printTestFailed("unexpected error: The response didn't return an integer for the limit");
                }
            }
            else
            {
                printTestFailed("Failed to receive the limit of the column.Error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests the default limit value for each column of a board
        /// is equal to <c>DEFAULT_COLUMN_LIMIT</c>
        /// </summary>
        private void testDefultColumnLimit()
        {
            this.assertCorrectColumnLimit(EMAIL1, BACKLOG_COLUMN_ID, DEFAULT_COLUMN_LIMIT);
            this.assertCorrectColumnLimit(EMAIL1, IN_PROGRESS_COLUMN_ID, DEFAULT_COLUMN_LIMIT);
            this.assertCorrectColumnLimit(EMAIL1, DONE_COLUMN_ID, DEFAULT_COLUMN_LIMIT);
        }

        /// <summary>
        /// This method tests if receiving a column's limit fails if the id is invalid
        /// </summary>
        private void testGetLimitToInvalidColumnId()
        {
            Response response = this.getColumnLimit(EMAIL1, BOARD_NAME, INVALID_COLUMN_ID);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Received the column's limit, but the ordinal is invalid");
            }
            else
            {
                printTestSucceeded("Failed to get limit for invalid column id.Error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests if receiving a column's limit fails if the user doesn't exist
        /// </summary>
        private void testGetLimitToNonExistingUser()
        {
            Response response = this.getColumnLimit(EMAIL2, BOARD_NAME, BACKLOG_COLUMN_ID);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Received the column's limit, but the user doesn't exist");
            }
            else
            {
                printTestSucceeded("Failed to get limit for non-existing user. Error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests if receiving a column's limit fails if the board doesn't exist
        /// </summary>
        private void testGetLimitToNonExistingBoard()
        {
            Response response = this.getColumnLimit(EMAIL1, BOARD_NAME + "123", INVALID_COLUMN_ID);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Received the column's limit, but the board doesn't exist");
            }
            else
            {
                printTestSucceeded("Failed to get limit for non - existing board.Error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests getting a column's limit fails when the user is logged out
        /// </summary>
        private void testGetLimitWhenLoggedOut()
        {
            this.userService.Logout(EMAIL1);
            Response response = this.getColumnLimit(EMAIL1, BOARD_NAME, BACKLOG_COLUMN_ID);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Received the column's limit, but the user is logged out");
            }
            else
            {
                printTestSucceeded("Failed to get limit for logged out user. Error: " + response.Message);
            }
            this.userService.Login(EMAIL1, VALID_PASSWORD);
        }

        /// <summary>
        /// This method tests setting a column's limit is successful
        /// </summary>
        /// <param name="email">The email of the user who is a board member</param>
        /// <param name="columnId">The column id. First is 0 and ascends by 1</param>
        /// <param name="limit">The new limit to set</param>
        private void testSuccessfulSetColumnLimit(string email, int columnId, int limit)
        {
            Response respone = this.setColumnLimit(email, BOARD_NAME, columnId, limit);
            if (respone.Code == ResponseCode.OperationSucceededCode)
            {
                printTestSucceeded("Set the column's limit successfully");
            }
            else
            {
                printTestFailed("Failed to change the column's limit. Error: " + respone.Message);
            }
        }

        /// <summary>
        /// This method tests setting all column's limits and that the limits are updated in the system
        /// </summary>
        /// <param name="email">The email of the user who is a board member</param>
        private void testSuccessfulGetAndSetLimit(string email)
        {
            this.testSuccessfulSetColumnLimit(email, BACKLOG_COLUMN_ID, NEW_BACKLOG_LIMIT);
            this.testSuccessfulSetColumnLimit(email, IN_PROGRESS_COLUMN_ID, NEW_IN_PROGRESS_LIMIT);
            this.testSuccessfulSetColumnLimit(email, DONE_COLUMN_ID, NEW_DONE_LIMIT);
            
            this.assertCorrectColumnLimit(email, BACKLOG_COLUMN_ID, NEW_BACKLOG_LIMIT);
            this.assertCorrectColumnLimit(email, IN_PROGRESS_COLUMN_ID, NEW_IN_PROGRESS_LIMIT);
            this.assertCorrectColumnLimit(email, DONE_COLUMN_ID, NEW_DONE_LIMIT);
        }

        /// <summary>
        /// This method tests that setting a column's limit fails if the column ordinal is invalid
        /// </summary>
        private void testSetLimitToInvalidColumnId()
        {
            int newLimit = 5;
            Response response = this.setColumnLimit(EMAIL1, BOARD_NAME, INVALID_COLUMN_ID, newLimit);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Changed the column's limit, but the ordinal is invalid");
            }
            else
            {
                printTestSucceeded("Failed to set limit for invalid column id. Error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests that setting a column's limit fails if the user doesn't exist
        /// </summary>
        private void testSetLimitToNonExistingUser()
        {
            int newLimit = 5;
            Response response = this.setColumnLimit(EMAIL2, BOARD_NAME, BACKLOG_COLUMN_ID, newLimit);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Changed the column's limit, but the user doesn't exist");
            }
            else
            {
                printTestSucceeded("Failed to set limit for non-existing user. Error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests that setting a column's limit fails if the board doesn't exist
        /// </summary>
        private void testSetLimitToNonExistingBoard()
        {
            int newLimit = 5;
            Response response = this.setColumnLimit(EMAIL1, BOARD_NAME + "123", INVALID_COLUMN_ID, newLimit);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Changed the column's limit, but the board doesn't exist");
            }
            else
            {
                printTestSucceeded("Failed to set limit for non-existing board. Error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests that setting a column's limit fails if the new limit is invalid
        /// </summary>
        private void testSetInvalidLimit()
        {
            Response response = this.setColumnLimit(EMAIL1, BOARD_NAME, BACKLOG_COLUMN_ID, INVALID_COLUMN_LIMIT);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Changed the column's limit, but the limit is invalid");
            }
            else
            {
                printTestSucceeded("Failed to set limit to an invalid value. Error: " + response.Message);
            }
        }

        /// <summary>
        /// This method tests setting a limit's column failes when a user is logged out
        /// </summary>
        private void testSetLimitWhenLoggedOut()
        {
            this.userService.Logout(EMAIL1);
            Response response = this.setColumnLimit(EMAIL1, BOARD_NAME, BACKLOG_COLUMN_ID, NEW_BACKLOG_LIMIT);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Changed the column's limit, but the user is logged out");
            }
            else
            {
                printTestSucceeded("Failed to set limit when the user is logged out. Error: " + response.Message);
            }
            this.userService.Login(EMAIL1, VALID_PASSWORD);
        }

        /// <summary>
        /// Tests a user that isn't a board member can't get a column's limit
        /// </summary>
        private void testGetLimitToNonBoardMember()
        {
            Response response = this.getColumnLimit(EMAIL2, BOARD_NAME, BACKLOG_COLUMN_ID);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("A non board member cannot get the limit of a column");
            }
            else
            {
                printTestSucceeded("Non board member failed to get a column's limit. Error: " + 
                    response.Message);
            }
        }

        /// <summary>
        /// Tests a user that isn't a board member can't set a column's limit
        /// </summary>
        private void testSetLimitToNonBoardMember()
        {
            Response response = this.setColumnLimit(EMAIL2, BOARD_NAME, BACKLOG_COLUMN_ID, NEW_BACKLOG_LIMIT);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("A non board member cannot set the limit of a column");
            }
            else
            {
                printTestSucceeded("Non board member failed to set a column's limit. Error: " +
                    response.Message);
            }
        }
    }
}
