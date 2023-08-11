using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.TaskServices;
using System.Text.Json;

namespace BackendTests.TaskTests
{
    /// <summary>
    /// Class <c>ListAllInProgresTest</c> tests successful and unsuccessful 
    /// operations related to geting all of the users in progress tasks. 
    /// Namely, this class test requirement 17
    /// </summary>
    internal class ListAllInProgressTest : AbstractTaskTest
    {
        /// <value>
        /// <c>email</c> the email used in this test
        /// </value>
        private const string EMAIL = EMAIL4;

        /// <summary>
        /// This method runs the tests related to geting all of the users in progress tasks - requirement 22
        /// </summary>
        public override void RunTests()
        {
            Response response1 =JsonSerializer.Deserialize<Response>(taskService.GetInProgressByUser(EMAIL));
           //tries to get a non existing list of tasks and checks if it failed
            if(response1.Code != ResponseCode.OperationFailedCode)
            {
                printTestFailed("List all in progress test has failed. got all in progress tasks from non existing user");
                return;
            }
            
            RegisterAndAddTask(EMAIL); // Create a user, board and task.
            AssignOtherUserToTask(EMAIL);

            userService.Register(EMAIL3, PASSWORD); // again, another user.
            boardService.AddBoard(EMAIL3, BOARD_NAME+ "3");
            taskService.AddTask(EMAIL3, BOARD_NAME+ "3", DUE_DATE, "title", "decription");
            boardService.JoinBoard(EMAIL2, FIRST_BOARD_ID+1); // Other User join Board
            taskService.AssignTask(EMAIL3, BOARD_NAME + "3", FIRST_TASK_ID, EMAIL2);  // Assign Task to other User

            // Checks if the owner's InProgress column is empty
            Response response2 = JsonSerializer.Deserialize<Response>(taskService.GetInProgressByUser(EMAIL));

            if (response2.Code == ResponseCode.OperationSucceededCode)
            {
                //checks if indeed retured zero tasks
                if (JsonSerializer.Deserialize<TaskToSend[]>(response2.Message.ToString()).Length == 0)
                    printTestSucceeded("List all in progress test has succeeded");
                else
                    printTestFailed("List all in progress test has failed. didnt return 0 Tasks");
            }
            else
            {
                printTestFailed("List all in progress test has failed. error code: " + response2.Message);
            }

            taskService.MoveTask(EMAIL2, BOARD_NAME, FIRST_TASK_ID); // Move Task using other user for first board
            taskService.MoveTask(EMAIL2, BOARD_NAME + "3", FIRST_TASK_ID); // Move Task using other user for second board

            Response response3 = JsonSerializer.Deserialize<Response>(taskService.GetInProgressByUser(EMAIL2)); // should work with 2 task.

            //tries to get an existing list of tasks and checks if it succeeded
            if (response3.Code == ResponseCode.OperationSucceededCode)
            {
                //check 2 tasks are returned
                if (JsonSerializer.Deserialize<TaskToSend[]>(response3.Message.ToString()).Length == 2)
                    printTestSucceeded("List all in progress test has succeeded");
                else
                {
                    printTestFailed($"List all in progress test has failed. didnt return the Task; code '{response3.Message}'");
                    return;
                }
            }

            Logout(EMAIL2); //logs out the user

            //tries to get list of tasks from a logged-out user and checks if it failed
            Response response4 = JsonSerializer.Deserialize<Response>(taskService.GetInProgressByUser(EMAIL2));

            if (response4.Code != ResponseCode.OperationFailedCode)
            {
                printTestFailed("List all in progress test has failed. called the function GetInProgressByUser() from a logged-out user");
                return;
            }
            else
            {
                printTestSucceeded("Failed to list in progress tasks for logged out user. " + response4.Message);
            }

            printTestSucceeded("All GetInProgressByUser() tests passed succeffully");
        }
    }
}
