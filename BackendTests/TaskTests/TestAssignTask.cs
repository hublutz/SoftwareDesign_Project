using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.TaskServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace BackendTests.TaskTests
{
    /// <summary>
    /// the tester class for the 23 requirement
    /// </summary>
    internal class TestAssignTask : AbstractTaskTest
    {
        private int boardId;

        /// <summary>
        /// the function that runs the tests
        /// </summary>
        public override void RunTests()
        {
            this.RegisterAndAddTask(EMAIL1);
            this.userService.Register(EMAIL2, PASSWORD);
            this.userService.Register(EMAIL3, PASSWORD);
            boardId = this.getFirstBoardId(EMAIL1);

            this.boardService.JoinBoard(EMAIL2, boardId);

            TestAssignTaskSuccess(EMAIL2, EMAIL1, "Assigning an unassigned task from member to owner");
            TestAssignTaskSuccess(EMAIL1, EMAIL2, "Assigning an assigned task from owner(and assignee) to member");
            this.boardService.JoinBoard(EMAIL3, boardId);
            TestAssignTaskSuccess(EMAIL2, EMAIL3, "Assigning an assigned task from member(and assignee) to member");
            TestAssignTaskSuccess(EMAIL3, EMAIL1, "Assigning an assigned task from member(and assignee) to owner");

            this.taskService.AssignTask(EMAIL1, BOARD_NAME, boardId, EMAIL3);
            this.boardService.LeaveBoard(EMAIL3, boardId);
            TestAssignTaskSuccess(EMAIL1, EMAIL2, "Assigning an unassigned task from owner to member");
            unAssign(EMAIL2, boardId);

            TestAssignTaskUserNotInBoard();
            TestAssignTaskUserDoesNotExist();
            TestAssignTaskAssignerLoggedOut();
            TestAssignTaskAssignerNotAMember();
            TestAssignTaskAssignerDoesNotExist();

            TestAssignTaskDefault();

            TestAssignTaskNonExistingBoard();
            unAssign(EMAIL2, boardId);

            TestAssignTaskNonExistingTask();
        }

        private Response Assign(string email, string boardName, int taskID, string emailAssignee)
        {
            string responseJson = this.taskService.AssignTask(email, boardName, taskID, emailAssignee);
            return JsonSerializer.Deserialize<Response>(responseJson);
        }

        /// <summary>
        /// test a legal assigning of a task
        /// </summary>
        private void TestAssignTaskSuccess(string assigner, string assignee, string comment) {
            Response response = Assign(assigner, BOARD_NAME, this.GetId(), assignee);

            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                if (getTask(0, assigner).Assignee == assignee)
                {
                    printTestSucceeded("Assigned Task succefully, " + comment);
                }
            }
            else
            {
                printTestFailed("Failed to assign task, " + comment);
            }
        }


        /// <summary>
        /// tests assigning a task to a user that is not in a board 
        /// </summary>
        private void TestAssignTaskUserNotInBoard()
        {
            Response response = Assign(EMAIL1, BOARD_NAME, this.GetId(), EMAIL3);

            if (response.Code == ResponseCode.OperationFailedCode)
            {
                printTestSucceeded("Failed to assign task to a non board member");
            }
            else
            {
                printTestFailed("assigned task to a non board member");
            }
        }

        /// <summary>
        /// tests assigning a task to a user that does not exist
        /// </summary>
        private void TestAssignTaskUserDoesNotExist()
        {
            Response response = Assign(EMAIL1, BOARD_NAME, this.GetId(), EMAIL3 + "bbb");

            if (response.Code == ResponseCode.OperationFailedCode)
            {
                printTestSucceeded("Failed to assign task to a non existing user");
            }
            else
            {
                printTestFailed("assigned task to a non existing user");
            }
        }

        /// <summary>
        /// tests assigning a task by a loggeOut user.
        /// </summary>
        private void TestAssignTaskAssignerLoggedOut()
        {
            userService.Logout(EMAIL1);
            Response response = Assign(EMAIL1, BOARD_NAME, this.GetId(), EMAIL2);

            if (response.Code == ResponseCode.OperationFailedCode)
            {
                printTestSucceeded("Failed to assign task by a loggedout user");
            }
            else
            {
                printTestFailed("assigned task by a loggedout user");
            }
            userService.Login(EMAIL1, PASSWORD);
        }

        /// <summary>
        /// tests assigning a task by a non board member
        /// </summary>
        private void TestAssignTaskAssignerNotAMember()
        {
            Response response = Assign(EMAIL3, BOARD_NAME, this.GetId(), EMAIL2);

            if (response.Code == ResponseCode.OperationFailedCode)
            {
                printTestSucceeded("Failed to assign task by a non member");
            }
            else
            {
                printTestFailed("assigned task by a non member");
            }
        }

        /// <summary>
        /// tests assigning a task by a non board member
        /// </summary>
        private void TestAssignTaskAssignerDoesNotExist()
        {
            Response response = Assign(EMAIL3 + "2532", BOARD_NAME, this.GetId(), EMAIL2);

            if (response.Code == ResponseCode.OperationFailedCode)
            {
                printTestSucceeded("Failed to assign task by a non existing user");
            }
            else
            {
                printTestFailed("assigned task by a non existing user");
            }
        }

        /// <summary>
        /// tests the defult assigne of a task
        /// </summary>
        private void TestAssignTaskDefault()
        {
            if (getTask(0, EMAIL1).Assignee == null)
            {
                printTestSucceeded("assigne default is null");
            }
            else
            {
                printTestFailed("assigne default is not null");
            }
        }

        /// <summary>
        /// test assigning a new assigne to a task with non existing board
        /// </summary>
        private void TestAssignTaskNonExistingBoard()
        {
            Response response = Assign(EMAIL1, BOARD_NAME + "ananan", this.GetId(), EMAIL2);

            if (response.Code == ResponseCode.OperationFailedCode)
            {
                printTestSucceeded("Failed to assign task in a non existing board");
            }
            else
            {
                printTestFailed("assigned task in a non existing board");
            }
        }
        /// <summary>
        /// test assigning a new assigne to a non existing task Id
        /// </summary>
        private void TestAssignTaskNonExistingTask()
        {
            Response response = Assign(EMAIL1, BOARD_NAME, this.GetId() + 99, EMAIL2);

            if (response.Code == ResponseCode.OperationFailedCode)
            {
                printTestSucceeded("Failed to assign task with invalid Id");
            }
            else
            {
                printTestFailed("assigned task with an invalid Id");
            }
        }


        /// <summary>
        /// gets a task from a columd given a user email
        /// </summary>
        /// <param name="column">the column in withs the task is located in</param>
        /// <param name="email">the email of the user who owns the task</param>
        /// <returns>the task that is lockated</returns>
        /// <exception cref="Exception"></exception>
        private TaskToSend getTask(int column, string email)
        {
            string responseJson = this.boardService.GetColumn(email, BOARD_NAME, column);
            Response response = JsonSerializer.Deserialize<Response>(responseJson);
            TaskToSend[] ret =JsonSerializer.Deserialize<TaskToSend[]>(response.Message.ToString());
            if (ret.Length != 1) {
                throw new Exception("cant get task");
            }
            return ret[0];
        }


        /// <summary>
        /// get the id of the first board of a given user (identical as in AbstractBoardTest)
        /// </summary>
        /// <param name="userName">the given user</param>
        /// <returns>returns the id of the board</returns>
        /// <exception cref="Exception">throws an exseption if the user doesnt have any boards</exception>
        public int getFirstBoardId(string userName)
        {
            string jsonBoards = this.boardService.GetUserBoards(userName);
            object messege = JsonSerializer.Deserialize<Response>(jsonBoards).Message;
            int[] ids = JsonSerializer.Deserialize<int[]>(messege.ToString());

            if (ids.Length == 0)
                throw new Exception("cant tell id becuse the user doesnt own any boards");
            return ids[0];
        }

        private void unAssign(string email,  int boardId)
        {
            boardService.LeaveBoard(email, boardId);
            boardService.JoinBoard(email, boardId);
        }
    }
}
