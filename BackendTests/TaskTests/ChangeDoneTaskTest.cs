using IntroSE.Kanban.Backend.ServiceLayer;
using System.Text.Json;


namespace BackendTests.TaskTests
{ /// <summary>
  /// Class <c>ChangeTaskTest</c> tests successful and unsuccessful 
  /// operations related to editing existing tasks what are and are not "done". 
  /// Namely, this class test requirement 20
  /// </summary>
    internal class ChangeDoneTaskTest : AbstractTaskTest
    {
        /// <value>
        /// <c>email</c> the email used in this test
        /// </value>
        private const string EMAIL = EMAIL2;
        /// <summary>
        /// This method runs the tests related to editing existing tasks what are and are not "done" - requirement 15
        /// </summary>
        public override void RunTests() {

            RegisterAndAddTask(EMAIL);
            AssignOtherUserToTask(EMAIL);

            Response response1 = JsonSerializer.Deserialize<Response>(taskService.EditDescription(EMAIL2, BOARD_NAME, GetId(), "new decription"));
            //checks if editing had succeeded
            if (response1.Code == ResponseCode.OperationSucceededCode)
            {
                taskService.MoveTask(EMAIL2, BOARD_NAME, GetId());
                taskService.MoveTask(EMAIL2, BOARD_NAME, GetId());
                //moves task to done column and tries to edit again
                Response response2 = JsonSerializer.Deserialize<Response>(taskService.EditDescription(EMAIL2, BOARD_NAME, GetId(), "new decription"));

                //checks if editing had failed
                if (response2.Code == ResponseCode.OperationFailedCode)
                {
                    printTestSucceeded("change task test has succeeded");
                }
                else
                {
                    printTestFailed("change task test has failed. edited a 'done' task");
                }
            }
            else
            {
                printTestFailed("change task test has failed. error message: " + response1.Message);
            }
            
        }
    }
}
