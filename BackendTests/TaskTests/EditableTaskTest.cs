using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Text.Json;


namespace BackendTests.TaskTests
{
    /// <summary>
    /// Class <c>EditbleTaskTest</c> tests successful and unsuccessful 
    /// operations related to editing existing tasks. 
    /// Namely, this class test requirement 20-21
    /// </summary>
    internal class EditableTaskTest : AbstractTaskTest {
        /// <value>
        /// <c>email</c> the email used in this test
        /// </value>
        private const string EMAIL = EMAIL3;

        /// <summary>
        /// This method runs the tests related to editing existing tasks - requirement 16
        /// </summary>
        public override void RunTests()
        { 
            RegisterAndAddTask(EMAIL); 

            //tries to edit a task with invalid id and checks if failed
            if (!tryToEdit(ResponseCode.OperationFailedCode,EMAIL,BOARD_NAME, -1))
            {
                printTestFailed("Edited a task with invalid id");
                return;
            }

            //tries to edit a task with invalid board name and checks if failed
            if (!tryToEdit(ResponseCode.OperationFailedCode, EMAIL, "invalid board name", GetId()))
            {
                printTestFailed("Edited a task with invalid board name");
                return;
            }
            //tries to edit a task with invalid email and checks if failed
            if (!tryToEdit(ResponseCode.OperationFailedCode, "invalid email", BOARD_NAME, GetId()))
            {
                printTestFailed("Edited a task with invalid email");
                return;
            }

            //tries to edit an exsiting Unassigned task and checks if failed
            if (!tryToEdit(ResponseCode.OperationFailedCode, EMAIL, BOARD_NAME, GetId()))
            {
                return;
            }

            AssignOtherUserToTask(EMAIL);

            //tries to edit an exsiting assigned task using the wrong user and checks if failed
            if (!tryToEdit(ResponseCode.OperationFailedCode, EMAIL, BOARD_NAME, GetId()))
            {
                return;
            }

            //tries to edit an exsiting assigned task using the right user and checks if succeeded
            if (!tryToEdit(ResponseCode.OperationSucceededCode, EMAIL2, BOARD_NAME, GetId()))
            {
                return;
            }

            //tries to edit an exsiting assigned task using the right user but logged-out and checks if failed
            userService.Logout(EMAIL2);
            if (!tryToEdit(ResponseCode.OperationFailedCode, EMAIL2, BOARD_NAME, GetId()))
            {
                return;
            }

            printTestSucceeded("Editble task test has succeeded");
        }

        /// <summary>
        /// This method tries to edit the test in all of the different options
        /// </summary>
        /// <param name="responseCodeExpected"> the response code exs</param>
        /// <param name="email">The email of the use who owns the task</param>
        /// <param name="boardName">The board name in which the task is located</param>
        /// <param name="id">The id of the task</param>
        /// <returns> true if task succeeded and false if failed</returns>
        private Boolean tryToEdit(ResponseCode responseCodeExpected,string email, string boardName ,int id) {
            Response response = JsonSerializer.Deserialize<Response>(taskService.EditTitle(email, boardName, id, "new title"));
            if (response.Code != responseCodeExpected)
            {
                if (responseCodeExpected == ResponseCode.OperationSucceededCode)
                    printTestFailed("Editble task test has failed. error messege: " + response.Message);
                return false;
            }

            response = JsonSerializer.Deserialize<Response>(taskService.EditDueDate(email, boardName, id, "11/11/2299 00:00:01"));
            if (response.Code != responseCodeExpected)
            {
                if (responseCodeExpected == ResponseCode.OperationSucceededCode)
                    printTestFailed("Editble task test has failed. error messege: " + response.Message);
                return false;
            }
            
            response = JsonSerializer.Deserialize<Response>(taskService.EditDescription(email, boardName, id, "new description"));
            if (response.Code != responseCodeExpected)
            {
                if (responseCodeExpected == ResponseCode.OperationSucceededCode)
                    printTestFailed("Editble task test has failed. error messege: " + response.Message);
                return false;
            }
            return true;
        }
    }
}
