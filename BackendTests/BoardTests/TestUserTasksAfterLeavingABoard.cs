
using System.Text.Json;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.TaskServices;

namespace BackendTests.BoardTests
{
    /// <summary>
    /// tests requariment 15
    /// </summary>
    internal class TestUserTasksAfterLeavingABoard : AbstractBoardTest
    {
        private const int BACKLOG_ORDINAL = 0;
        private const int IN_PROGRESS_ORDINAL = 1;
        private const int DONE_ORDINAL = 2;

        /// <summary>
        /// the function that runs the test
        /// </summary>
        public override void RunTests()
        {
            TestUserTasksAfterLeavingInBacklogColumn();
            TestUserTasksAfterLeavingInInprogressColumn();
            TestUserTasksAfterLeavingInDoneColumn();
        }
        /// <summary>
        /// function that gets tasks from a column
        /// </summary>
        /// <param name="email">the user email</param>
        /// <param name="column">the column number</param>
        /// <returns>return an array of the tasks</returns>
        private TaskToSend[] getColumnTasks(string email ,int column) {
            String responseJson = this.boardService.GetColumn(email, BOARD_NAME, column);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            return JsonSerializer.Deserialize < TaskToSend[]>(response.Message.ToString());
        }

        /// <summary>
        /// Test user tasks after leaving In Backlog Column
        /// </summary>
        private void TestUserTasksAfterLeavingInBacklogColumn() {
            this.userService.Register(EMAIL2, VALID_PASSWORD);

            this.boardService.AddBoard(EMAIL2, BOARD_NAME);
            int id = this.getFirstBoardId(EMAIL2);

            this.taskService.AddTask(EMAIL2, BOARD_NAME, "12/12/2099 00:00:01", "title");

            string secoundEmail = EMAIL2 + "10000";
            this.userService.Register(secoundEmail, VALID_PASSWORD);
            this.boardService.JoinBoard(secoundEmail, id);


            this.taskService.AssignTask(EMAIL2, BOARD_NAME, this.GetTaskId(EMAIL2,BOARD_NAME), secoundEmail);
            
            this.boardService.LeaveBoard(secoundEmail, id);

            if(getColumnTasks(EMAIL2, BACKLOG_ORDINAL)[0].Assignee == null)
            {
                this.printTestSucceeded("there is no assine after its original left");
            }
            else
            {
                this.printTestFailed("there is an assine evan after its original left");
            }
        }
        /// <summary>
        /// Test user tasks after leaving In InProgress Column
        /// </summary>
        private void TestUserTasksAfterLeavingInInprogressColumn() {
            string newEmail = EMAIL1 + "1";

            this.userService.Register(newEmail, VALID_PASSWORD);
            this.boardService.AddBoard(newEmail, BOARD_NAME);
            int id = this.getFirstBoardId(newEmail);

            this.taskService.AddTask(newEmail, BOARD_NAME, "12/12/2099 00:00:01", "title");

            string secoundEmail = EMAIL2 + "100";
            this.userService.Register(secoundEmail, VALID_PASSWORD);
            this.boardService.JoinBoard(secoundEmail, id);

            this.taskService.AssignTask(newEmail, BOARD_NAME, this.GetTaskId(EMAIL2, BOARD_NAME), secoundEmail);

            int taskId = GetTaskId(secoundEmail, BOARD_NAME);
            this.taskService.MoveTask(secoundEmail, BOARD_NAME, taskId);

            this.boardService.LeaveBoard(secoundEmail, id);


            if (getColumnTasks(newEmail, IN_PROGRESS_ORDINAL)[0].Assignee == null)
            {
                this.printTestSucceeded("there is no assine after its original left");
            }
            else
            {
                this.printTestFailed("there is an assine evan after its original left");
            }
        }


        /// <summary>
        /// Test user tasks after leaving In Done Column
        /// </summary>
        private void TestUserTasksAfterLeavingInDoneColumn() {
            string newEmail = EMAIL1 + "2";
            this.userService.Register(newEmail, VALID_PASSWORD);

            this.boardService.AddBoard(newEmail, BOARD_NAME);
            int id = this.getFirstBoardId(newEmail );

            this.taskService.AddTask(newEmail, BOARD_NAME, "12/12/2099 00:00:01", "title");

            string secoundEmail = EMAIL2 + "adk";
            this.userService.Register(secoundEmail, VALID_PASSWORD);
            this.boardService.JoinBoard(secoundEmail, id);

            this.taskService.AssignTask(newEmail, BOARD_NAME, this.GetTaskId(newEmail, BOARD_NAME), secoundEmail);

            int taskId = GetTaskId(secoundEmail, BOARD_NAME);
           
            this.taskService.MoveTask(secoundEmail, BOARD_NAME, taskId);
            this.taskService.MoveTask(secoundEmail, BOARD_NAME, taskId);


            this.boardService.LeaveBoard(secoundEmail, id);

            if (getColumnTasks(newEmail, DONE_ORDINAL)[0].Assignee == null)
            {
                this.printTestFailed("there is no assine after its original left inspite this task is in done column");
            }
            else
            {
                this.printTestSucceeded("there is an assine evan after its original left inspite this task is in done column");
            }
        }

        /// <summary>
        /// This method gets the id of a newly created task (identical to the one in AbstractTaskTest).
        /// </summary>
        /// <param name="email">The email of the user that owns the task</param>
        /// <param name="BOARD_NAME">The boared name in which the task is located in</param>
        /// <returns> the id or throws an exseption if cant tell <returns> 
        protected int GetTaskId(string email, string BOARD_NAME)
        {
            string jsonColunm = boardService.GetColumn(email, BOARD_NAME, 0);
            object messege = JsonSerializer.Deserialize<Response>(jsonColunm).Message;
            TaskToSend[] column = JsonSerializer.Deserialize<TaskToSend[]>(messege.ToString());

            if (column.Length != 1)
                throw new Exception("cant tell id becuse more then 1 task in column");
            return column[0].Id;

        }
    }
}
