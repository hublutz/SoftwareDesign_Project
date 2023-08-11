using IntroSE.Kanban.Backend.ServiceLayer;
using System.Text.Json;

namespace BackendTests.BoardTests
{   /// <summary>
    /// test requirements 13 
    /// </summary>
    internal class TestTransferBoardOwnership : AbstractBoardTest
    {
        public override void RunTests()
        {
            userService.Register(EMAIL1, VALID_PASSWORD);
            userService.Register(EMAIL2, VALID_PASSWORD);
            addBoard(EMAIL1, BOARD_NAME);

            TestTransferOwnerShipFromOwnerToNotAMember();

            boardService.JoinBoard(EMAIL2, extractBoardId(EMAIL1));
            TestTransferOwnerShipFromNotOwner();
            TestTransferOwnerShipNotValidBoardName();
            TestTransferOwnerShipNotValidEmail();
            TestTransferOwnerShipWithLoggedOutOwner();

            userService.Login(EMAIL1, VALID_PASSWORD);
            TestTransferOwnerShipValid();
            boardService.TransferBoardOwnership(EMAIL2, EMAIL1, BOARD_NAME);

            //Tests transferring board ownership when the new owner is loggedout, should work.
            userService.Logout(EMAIL2);
            TestTransferOwnerShipValid();
        }
        /// <summary>
        /// transfer ownership
        /// </summary>
        /// <param name="email1">email of the original owner</param>
        /// <param name="email2">email of the new owner</param>
        /// <param name="boardName">the board name</param>
        /// <returns>the response of the action</returns>
        private Response TransferOwnership(string email1, string email2, string boardName)
        {
            string responseJson = boardService.TransferBoardOwnership(email1, email2, boardName);
            return JsonSerializer.Deserialize<Response>(responseJson);
        }

        /// <summary>
        /// tests a ownership transfer from owner to a non board member.
        /// </summary>
        private void TestTransferOwnerShipFromOwnerToNotAMember()
        {
            Response response = TransferOwnership(EMAIL1, EMAIL2, BOARD_NAME);

            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("ownership was transferred to a non board member");
            }
            else
            {
                printTestSucceeded("ownership didnt transfer to a non board member, as expected");
            }
        }

        /// <summary>
        /// tests transferring ownership when owner is logged out
        /// </summary>
        private void TestTransferOwnerShipWithLoggedOutOwner()
        {
            this.userService.Logout(EMAIL1);
            Response response = TransferOwnership(EMAIL1, EMAIL2, BOARD_NAME);

            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("ownership was successfully transferred inspite of the user being logged out");
            }
            else
            {
                printTestSucceeded("ownership didnt transfer");
            }
            this.userService.Login(EMAIL1, VALID_PASSWORD);
        }

        /// <summary>
        /// tests a ownership transfer to a non existing email
        /// </summary>
        private void TestTransferOwnerShipNotValidEmail()
        {
            Response response = TransferOwnership(EMAIL1, EMAIL2 + " 1123", BOARD_NAME);

            if (response.Code == ResponseCode.OperationFailedCode)
            {
                printTestSucceeded("ownership didnt transfer ");
            }
            else
            {
                printTestFailed("ownership was successfully transferred inspite of invalid email");
            }
        }

        /// <summary>
        /// tests a ownership transfership with not a valid board name
        /// </summary>
        private void TestTransferOwnerShipNotValidBoardName()
        {
            Response response = TransferOwnership(EMAIL1, EMAIL2, BOARD_NAME + "1223");

            if (response.Code == ResponseCode.OperationFailedCode)
            {
                printTestSucceeded("ownership didnt transfer ");
            }
            else
            {
                printTestFailed("ownership was successfully transferred inspite of invalid email");
            }
        }

        /// <summary>
        /// tests a ownership transfership from not an owner
        /// </summary>
        private void TestTransferOwnerShipFromNotOwner()
        {
            Response response = TransferOwnership(EMAIL2, EMAIL1, BOARD_NAME);

            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestFailed("ownership transfer from not owner");
            }
            else
            {
                printTestSucceeded("ownership didnt transfer");
            }
        }


        /// <summary>
        /// tests a ownership transfer from owner to a board member.
        /// </summary>
        private void TestTransferOwnerShipValid()
        {
            Response response = TransferOwnership(EMAIL1, EMAIL2, BOARD_NAME);

            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                printTestSucceeded("ownership was transferred to a board member, as expected");
            }
            else
            {
                printTestFailed("ownership didnt transfer to a board member");
            }
        }
    }
}
