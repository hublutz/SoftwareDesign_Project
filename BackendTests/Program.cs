using BackendTests.BoardTests;
using BackendTests.TaskTests;
using BackendTests.UserTests;

namespace IntroSE.Kanban.BackendTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n~~~~ Test User Register ~~~~");
            try
            {
                RegisterTests registerTests = new RegisterTests();
                registerTests.RunTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed due to unexpected error: " + ex.Message);
            }

            Console.WriteLine("\n~~~~ Test User Login ~~~~");
            try
            {
                LoginTests loginTests = new LoginTests();
                loginTests.RunTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed due to unexpected error: " + ex.Message);
            }

            Console.WriteLine("\n~~~~ Test User Logout ~~~~");
            try
            {
                LogoutTests logoutTests = new LogoutTests();
                logoutTests.RunTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed due to unexpected error: " + ex.Message);
            }

            Console.WriteLine("\n~~~~ Test Board Creation ~~~~");
            try
            {
                TestAddBoard testAddBoard = new TestAddBoard();
                testAddBoard.RunTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed due to unexpected error: " + ex.Message);
            }

            Console.WriteLine("\n~~~~ Test Board Deletion ~~~~");
            try
            {
                TestDeleteBoard testDeleteBoard = new TestDeleteBoard();
                testDeleteBoard.RunTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed due to unexpected error: " + ex.Message);
            }

            Console.WriteLine("\n~~~~ Test Get All Boards ~~~~");
            try
            {
                TestGetAllUserBoards testGetAllUserBoards = new TestGetAllUserBoards();
                testGetAllUserBoards.RunTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed due to unexpected error: " + ex.Message);
            }

            Console.WriteLine("\n~~~~ Test Set and Get Column Limit ~~~~");
            try
            {
                TestColumnLimit testColumnLimit = new TestColumnLimit();
                testColumnLimit.RunTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed due to unexpected error: " + ex.Message);
            }

            Console.WriteLine("\n~~~~ Test Get Column and Get Column Name ~~~~");
            try
            {
                TestGetColumn testGetColumn = new TestGetColumn();
                testGetColumn.RunTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed due to unexpected error: " + ex.Message);
            }

            Console.WriteLine("\n~~~~ Test Change Done Task ~~~~");
            try
            {
                ChangeDoneTaskTest changeTaskTest = new ChangeDoneTaskTest();
                changeTaskTest.RunTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed due to unexpected error: " + ex.Message);
            }

            Console.WriteLine("\n~~~~ Test Task Editing ~~~~");
            try
            {
                EditableTaskTest editbleTaskTest = new EditableTaskTest();
                editbleTaskTest.RunTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed due to unexpected error: " + ex.Message);
            }

            Console.WriteLine("\n~~~~ Test Listing All In Progress Tasks ~~~~");
            try
            {
                ListAllInProgressTest listAllInProgressTest = new ListAllInProgressTest();
                listAllInProgressTest.RunTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed due to unexpected error: " + ex.Message);
            }

            Console.WriteLine("\n~~~~ Test Moving Task State ~~~~");
            try
            {
                MoveTaskTest moveTaskTest = new MoveTaskTest();
                moveTaskTest.RunTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed due to unexpected error: " + ex.Message);
            }

            Console.WriteLine("\n~~~~ Test Join and Leave Board ~~~~");
            try
            {
                TestJoinAndLeaveBoard testJoinAndLeaveBoard = new TestJoinAndLeaveBoard();
                testJoinAndLeaveBoard.RunTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed due to unexpected error: " + ex.Message);
            }

            Console.WriteLine("\n~~~~ Test transfer Board ownership ~~~~");
            try
            {
                TestTransferBoardOwnership testTransferBoardOwnership = new TestTransferBoardOwnership();
                testTransferBoardOwnership.RunTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed due to unexpected error: " + ex.Message);
            }

            Console.WriteLine("\n~~~~ Test getting Tasks after the User left the Board ~~~~");
            try
            {
                TestUserTasksAfterLeavingABoard testUserTasksAfterLeavingABoard = new TestUserTasksAfterLeavingABoard();
                testUserTasksAfterLeavingABoard.RunTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed due to unexpected error: " + ex.Message);
            }

            Console.WriteLine("\n~~~~ Test Add Task ~~~~");
            try
            {
                TestAddTask testAddTask = new TestAddTask();
                testAddTask.RunTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed due to unexpected error: " + ex.Message);
            }

            Console.WriteLine("\n~~~~ Test Assign Task ~~~~");
            try
            {
                TestAssignTask testAssignTask = new TestAssignTask();
                testAssignTask.RunTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test failed due to unexpected error: " + ex.Message);
            }
        }
    }
}
