using IntroSE.Kanban.Backend.ServiceLayer;
using System.Text.Json;

namespace BackendTests.TaskTests
{   
    /// <summary>
    /// test the add task method
    /// </summary>
    internal class TestAddTask : AbstractTaskTest
    {
        public override void RunTests()
        {
            this.userService.Register(EMAIL1, PASSWORD);
            this.boardService.AddBoard(EMAIL1, BOARD_NAME);

            this.userService.Register(EMAIL2 , PASSWORD);
            this.boardService.AddBoard(EMAIL1 , BOARD_NAME);
            this.userService.Logout(EMAIL2);

            TestAddTaskSucccess();
            TestAddTaskIlegalEmail();
            TestAddTaskIlegalBoardName();
            TestAddTaskIlegalDueDate();
            TestAddTaskIlegalTitle();
            TestAddTaskIlegalDescription();
            TestAddTaskNotLoginUser();
        }

        /// <summary>
        /// test successfull addition of a task
        /// </summary>
        private void TestAddTaskSucccess() {
            Response  response = AddTask(EMAIL1, BOARD_NAME, DUE_DATE, "title", "description");

            if (response.Code == ResponseCode.OperationSucceededCode) {
                printTestSucceeded("successfully added a new task");
            }
            else
            {
                printTestFailed("couldnt add a new task " + response.Code);
            }
        }
        /// <summary>
        /// test addition of a task with ilegal email
        /// </summary>
        private void TestAddTaskIlegalEmail() {
            Response response = AddTask(EMAIL1+ "112", BOARD_NAME, DUE_DATE, "title", "description");

            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("added a task inspite of not legal boardName");
            }
            else
            {
                printTestSucceeded("couldnt add a new task ");
            }
        
        }
        /// <summary>
        /// test addition of a task with ilegal board name
        /// </summary>
        private void TestAddTaskIlegalBoardName()
        {
            Response response = AddTask(EMAIL1, BOARD_NAME +112, DUE_DATE, "title", "description");

            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("added a task inspite of not legal boardName");
            }
            else
            {
                printTestSucceeded("couldnt add a new task ");
            }
        }
        /// <summary>
        /// test addition of a task with ilegal due date
        /// </summary>
        private void TestAddTaskIlegalDueDate()
        {
            Response response = AddTask(EMAIL1, BOARD_NAME, DUE_DATE +"adadsfasfd43", "title", "description");

            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("added a task inspite of not legal due date");
            }
            else
            {
                printTestSucceeded("couldnt add a new task ");
            }
        }
        /// <summary>
        /// test addition of a task with ilegal title
        /// </summary>
        private void TestAddTaskIlegalTitle()
        {
            Response response = AddTask(EMAIL1, BOARD_NAME, DUE_DATE, "", "description");

            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("added a task inspite of not legal title");
            }
            else
            {
                printTestSucceeded("couldnt add a new task ");
            }
        }
        /// <summary>
        /// test addition of a task with ilegal description
        /// </summary>
        private void TestAddTaskIlegalDescription()
        {
            string plus300Characters = "descriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescriptiondescription";
            Response response = AddTask(EMAIL1, BOARD_NAME, DUE_DATE, "title", plus300Characters);

            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("added a task inspite of not legal description");
            }
            else
            {
                printTestSucceeded("couldnt add a new task ");
            }
        }

        /// <summary>
        /// test addition of a task with logout user
        /// </summary>
        private void TestAddTaskNotLoginUser()
        {
            Response response = AddTask(EMAIL2, BOARD_NAME, DUE_DATE, "title", "description");

            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("added a task inspite of the user not being logged in");
            }
            else
            {
                printTestSucceeded("couldnt add a new task ");
            }
        }
        /// <summary>
        /// adds a task and returns the response
        /// </summary>
        /// <param name="email">the email of the user</param>
        /// <param name="boardName">the board name in wich to add the task</param>
        /// <param name="dueDate">the due date of the new task</param>
        /// <param name="title">the title of the new task</param>
        /// <param name="description">the description of the task</param>
        /// <returns>returns the response that the add task method returns</returns>
        private Response AddTask(string email, string boardName, string dueDate, string title, string description) {
            string responseJson = this.taskService.AddTask(email, boardName, dueDate, title, description);
            return JsonSerializer.Deserialize<Response>(responseJson);
        }
    }
}
