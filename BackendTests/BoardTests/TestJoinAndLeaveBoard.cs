using IntroSE.Kanban.Backend.ServiceLayer;
using System.Text.Json;

namespace BackendTests.BoardTests
{
    /// <summary>
    /// tests requirments 12 14
    /// </summary>
    internal class TestJoinAndLeaveBoard : AbstractBoardTest
    {
        public int NOT_VALID_BOARD_ID = -1;

        /// <summary>
        /// the functions that runs the test
        /// </summary>
        public override void RunTests()
        {
            this.userService.Register(EMAIL1,VALID_PASSWORD);
            this.userService.Register(EMAIL2,VALID_PASSWORD);

            this.TestJoinNonExistingBoard();
            this.TestLeavingNonExistingBoard();

            this.TestJoinAndLeaveExistingBoard();
            this.TestJoinUserWhoIsAlreadyInBoard_AndOwnerLeaving();
            this.TestLeaveTwice();
            TestLeaveJoinWhenLoggedOut();
            TestLeaveJoinUserDoesntExist();


        }
        /// <summary>
        /// tests leaving a board twice
        /// </summary>
        private void TestLeaveTwice()
        {
            this.addBoard(EMAIL1, BOARD_NAME);
            int id = getFirstBoardId(EMAIL1);
            
            this.JoinBoard(EMAIL1, id);
            this.LeaveBoard(EMAIL1, id);

            Response response = this.LeaveBoard(EMAIL1, id);

            if (response.Code == ResponseCode.OperationFailedCode)
            {
                this.printTestSucceeded("The user couldnt leave a board he already left ");
            }
            else
            {
                this.printTestFailed("The user left a board he wasnt in");
            }

        }

        /// <summary>
        /// test join and leave board for not existing user
        /// </summary>
        private void TestLeaveJoinUserDoesntExist()
        {
            int id = getFirstBoardId(EMAIL1);

            Response response1 = this.JoinBoard(EMAIL1+"1221", id);
            Response response2 = this.LeaveBoard(EMAIL1+"1221", id);

            if (response1.Code == ResponseCode.OperationFailedCode)
            {
                this.printTestSucceeded("A non existing user couldnt join a board");
            }
            else
            {
                this.printTestFailed("A non existing user joinned a board");
            }

            if (response2.Code == ResponseCode.OperationFailedCode)
            {
                this.printTestSucceeded("A non existing user couldnt leave a board");
            }
            else
            {
                this.printTestFailed("A non existing user left a board");
            }

        }

        /// <summary>
        /// test joining and leaving a board when logged out
        /// </summary>
        private void TestLeaveJoinWhenLoggedOut()
        {
            this.addBoard(EMAIL1, BOARD_NAME);
            int id = getFirstBoardId(EMAIL1);

            this.userService.Logout(EMAIL1);
            
            Response response1 =this.JoinBoard(EMAIL1, id);

            Response response2 = this.LeaveBoard(EMAIL1, id);

            if (response1.Code == ResponseCode.OperationFailedCode)
            {
                this.printTestSucceeded("The user couldnt join a board when logged out ");
            }
            else
            {
                this.printTestFailed("The user joined a board when logged out");
            }

            if (response2.Code == ResponseCode.OperationFailedCode)
            {
                this.printTestSucceeded("The user couldnt leave a board when logged out");
            }
            else
            {
                this.printTestFailed("The user left a board when logged out");
            }

            this.userService.Login(EMAIL1, VALID_PASSWORD);
        }

        /// <summary>
        /// Tests a user joinning a user who is already in board and an owner leaving
        /// </summary>
        private void TestJoinUserWhoIsAlreadyInBoard_AndOwnerLeaving() {

            this.addBoard(EMAIL1, BOARD_NAME);
            int id = getFirstBoardId(EMAIL1);

            Response response = this.JoinBoard(EMAIL1, id);

            if (response.Code == ResponseCode.OperationFailedCode)
            {
                this.printTestSucceeded("The user couldnt join a board he was already in ");
            }
            else
            {
                this.printTestFailed("The user joined a board he was already in");
            }

            //owner tries to leave his board
            Response response2 = this.LeaveBoard(EMAIL1, id);

            if (response2.Code == ResponseCode.OperationFailedCode)
            {
                this.printTestSucceeded("The owner couldnt leave his board ");
            }
            else
            {
                this.printTestFailed("The owner left his board");
            }
        }
        /// <summary>
        /// Test a user joinning and an user leaving an existing board
        /// </summary>
        private void TestJoinAndLeaveExistingBoard()
        {
            this.addBoard(EMAIL1, BOARD_NAME);
            int id = this.getFirstBoardId(EMAIL1);

            Response response = this.JoinBoard(EMAIL2, id);

            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                this.printTestSucceeded("The user joined a board successfully ");
            }
            else
            {
                this.printTestFailed("The user couldnt join a board");
            }

            Response response2 = this.LeaveBoard(EMAIL2, id);

            if (response2.Code == ResponseCode.OperationSucceededCode)
            {
                this.printTestSucceeded("user successfully left a board");
            }
            else {
                this.printTestFailed("user coulnt leave board" + response2.Message);
            }
        }
        /// <summary>
        /// test a user joining a non existing board
        /// </summary>
        private void TestJoinNonExistingBoard() {

            Response response =this.JoinBoard(EMAIL1, NOT_VALID_BOARD_ID);

            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                this.printTestFailed("The user joined a non existing board");
            }
            else 
            {
                this.printTestSucceeded("The user couldnt join a non existing board");
            }
            
        }
        /// <summary>
        /// a function that adds a user to the board
        /// </summary>
        /// <param name="email">the email of the user to add </param>
        /// <param name="boardId">the id of the board to join</param>
        /// <returns></returns>
        private Response JoinBoard(String email, int boardId)
        {
            String responseJson = this.boardService.JoinBoard(email, boardId);
            return JsonSerializer.Deserialize<Response>(responseJson);
        }

        /// <summary>
        /// test leaving a non existing board
        /// </summary>
        private void TestLeavingNonExistingBoard () {
            Response response = this.LeaveBoard(EMAIL1, NOT_VALID_BOARD_ID);

            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                this.printTestFailed("The user left a non existing board");
            }
            else
            {
                this.printTestSucceeded("The user couldnt leave a non existing board");
            }
        }

        /// <summary>
        /// leaves a user from the board 
        /// </summary>
        /// <param name="email">the email of the user</param>
        /// <param name="id">the id of the board</param>
        /// <returns>returns the response</returns
        private Response LeaveBoard(string email, int boardId)
        {
            String responseJson = this.boardService.LeaveBoard(email, boardId);
            return JsonSerializer.Deserialize<Response>(responseJson);
        }
    }
}
