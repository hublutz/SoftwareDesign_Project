using BackendTests.TaskTests;
using IntroSE.Kanban.Backend.ServiceLayer;
using System.Text.Json;

namespace BackendTests.BoardTests
{
    /// <summary>
    /// Class <c>MoveTaskTest</c> tests successful and unsuccessful 
    /// operations related to moving tasks from coloumn to coloumn. 
    /// Namely, this class test requirement 19
    /// </summary>
    internal class MoveTaskTest : AbstractTaskTest
    {
        /// <value>
        /// <c>email</c> the email used in this test
        /// </value>
        private const string EMAIL = EMAIL1;

        private const int BACKLOG_ORDINAL = 0;

        /// <summary>
        /// This method runs the tests related to moving tasks from coloumn to coloumn - requirement 14
        /// </summary>
        public override void RunTests()
        {
            RegisterAndAddTask(EMAIL);
            this.userService.Register(EMAIL4, PASSWORD); // EMAIL4 will be a board member and the assignee
            this.userService.Register(EMAIL3, PASSWORD); // EMAIL3 isn't a board memebr

            int boardId = this.extractBoardId(EMAIL);
            this.boardService.JoinBoard(EMAIL4, boardId);
            this.taskService.AssignTask(EMAIL, BOARD_NAME,
                this.GetId(), EMAIL4);

            this.testUnssucessfulMoveTask();
            this.testSuccessfulMoveTask();
            this.testMovingDoneTask();
        }

        /// <summary>
        /// This method extracts a board id, assuming the user has only one board
        /// </summary>
        /// <param name="email">The email of the board member</param>
        /// <returns>The is of the board</returns>
        private int extractBoardId(string email)
        {
            string responseJson = this.boardService.GetUserBoards(email);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            int boardId = -1;
            try
            {
                int[] boardIds = JsonSerializer.Deserialize<int[]>(response.Message.ToString());
                boardId = boardIds[0];
            }
            catch (Exception e)
            {
                printTestFailed("Could not extract board id " + e.Message);
            }
            return boardId;
        }

        /// <summary>
        /// This method performs rainy tests on Move task, when the parameters are invalid
        /// </summary>
        private void testUnssucessfulMoveTask()
        {
            string responseJson;
            Response response;
            // test the user that isn't the assignee can't move the task
            responseJson = this.taskService.MoveTask(EMAIL, BOARD_NAME, this.GetId());
            response = JsonSerializer.Deserialize<Response>(responseJson);
            if (response.Code == ResponseCode.OperationFailedCode)
            {
                printTestSucceeded("A user that isn't assigned to the task failed to move it. " +  
                    response.Message);
            }
            else
            {
                printTestFailed("A user that isn't assigned to the task has moved it");
            }

            // test a non-member can't move a task
            responseJson = this.taskService.MoveTask(EMAIL3, BOARD_NAME, this.GetId());
            response = JsonSerializer.Deserialize<Response>(responseJson);
            if (response.Code == ResponseCode.OperationFailedCode)
            {
                printTestSucceeded("A user that isn't assigned to the task failed to move it. " +
                    response.Message);
            }
            else
            {
                printTestFailed("A user that isn't assigned to the task has moved it");
            }

            //tries to move a task with invalid id
            responseJson = taskService.MoveTask(EMAIL4, BOARD_NAME, -1);
            response = JsonSerializer.Deserialize<Response>(responseJson);
            if (response.Code == ResponseCode.OperationFailedCode)
            {
                printTestSucceeded("Didn't move task with invalid id. " +
                    response.Message);
            }
            else
            {
                printTestFailed("Task moved with invalid id");
            }

            //tries to move a task with invalid board name
            responseJson = taskService.MoveTask(EMAIL4, "invalid board name", GetId());
            response = JsonSerializer.Deserialize<Response>(responseJson);
            if (response.Code == ResponseCode.OperationFailedCode)
            {
                printTestSucceeded("Didn't move task when the board doesn't exist. " +
                    response.Message);
            }
            else
            {
                printTestFailed("Task moved but the board doesn't exist");
            }

            //tries to move a task with invalid email
            responseJson = taskService.MoveTask("invalid email", BOARD_NAME, GetId());
            response = JsonSerializer.Deserialize<Response>(responseJson);
            if (response.Code == ResponseCode.OperationFailedCode)
            {
                printTestSucceeded("Didn't move task when the user doesn't exist. " +
                    response.Message);
            }
            else
            {
                printTestFailed("Task moved but the user doesn't exist");
            }

            // test moving the task when the assignee is logged out
            this.Logout(EMAIL4);
            responseJson = taskService.MoveTask(EMAIL4, BOARD_NAME, GetId());
            response = JsonSerializer.Deserialize<Response>(responseJson);
            if (response.Code == ResponseCode.OperationFailedCode)
            {
                printTestSucceeded("Didn't move task when the user is logged out. " +
                    response.Message);
            }
            else
            {
                printTestFailed("Task moved but the user is logged out");
            }
            this.userService.Login(EMAIL4, PASSWORD);
        }

        /// <summary>
        /// This method tests moving a task by its assignee is successful
        /// </summary>
        private void testSuccessfulMoveTask()
        {
            string responseJson;
            Response response;

            // test moving a task from backlog to in progress
            responseJson = this.taskService.MoveTask(EMAIL4, BOARD_NAME, this.GetId());
            response = JsonSerializer.Deserialize<Response>(responseJson);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestSucceeded("Moved task from Backlog to In-Progress");
            }
            else
            {
                printTestFailed("Failed to move task from Backlog to In-Progress. Error: " + 
                    response.Message);
            }

            // test moving a task from in progress to done
            responseJson = this.taskService.MoveTask(EMAIL4, BOARD_NAME, this.GetId());
            response = JsonSerializer.Deserialize<Response>(responseJson);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestSucceeded("Moved task from In-Progress to Done");
            }
            else
            {
                printTestFailed("Failed to move task from In-Progress to Done. Error: " +
                    response.Message);
            }
        }

        /// <summary>
        /// This method tests failure when moving a task that is already done
        /// </summary>
        private void testMovingDoneTask()
        {
            string responseJson = this.taskService.MoveTask(EMAIL4, BOARD_NAME, this.GetId());
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("Moved a Done task");
            }
            else
            {
                printTestSucceeded("Failed to move a Done task. Error: " +
                    response.Message);
            }
        }
    }
}
