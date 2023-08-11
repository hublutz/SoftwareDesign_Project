using IntroSE.Kanban.Backend.BusinessLayer.BoardController;
using IntroSE.Kanban.Backend.BusinessLayer.UserController;
using IntroSE.Kanban.Backend.DataAccessLayer.BoardDataManager;
using IntroSE.Kanban.Backend.DataAccessLayer.ColumnDataManager;
using IntroSE.Kanban.Backend.DataAccessLayer.TaskDataManager;
using IntroSE.Kanban.Backend.DataAccessLayer.UserDataManager;
using IntroSE.Kanban.Backend.ServiceLayer.TaskServices;
using System;
using System.Text.Json;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// Class <c>ServiceManager</c> contains all Service Layer services
    /// so all the services have corresponding data and Business Layer controllers
    /// </summary>
    public class ServiceManager
    {
        private BoardController boardController;
        private UserController userController;


        /// <value>
        /// Service responsible for Board operations
        /// </value>
        public BoardService.BoardService BoardService { get; }

        /// <value>
        /// Service responsible for Task operations
        /// </value>
        public TaskService TaskService { get; }

        /// <value>
        /// Service responsible for User operations
        /// </value>
        public UserService.UserService UserService { get; }

        /// <summary>
        /// Constuctor of <c>ServiceManager</c> initializing all Service Layer
        /// Services
        /// </summary>
        public ServiceManager()
        {
            this.boardController = new BoardController();
            this.userController = new UserController(boardController);

            this.BoardService = new BoardService.BoardService(boardController);
            this.TaskService = new TaskService(boardController);
            this.UserService = new UserService.UserService(userController);
        }

        ///<summary>This method loads all persisted data.
        /// </summary>		 
        /// <returns>An empty response, unless an error occurs </returns>		 
        public string LoadData()
        {
            Response responseUser = JsonSerializer.Deserialize<Response>(UserService.LoadData());
            Response responseBoard = JsonSerializer.Deserialize<Response>(BoardService.LoadData());
            Response response;

            if (responseBoard.Code == ResponseCode.OperationSucceededCode && responseUser.Code == ResponseCode.OperationSucceededCode)
            {
                response = new Response(ResponseCode.OperationSucceededCode);
            }
            else 
            { 
                response = new Response(ResponseCode.OperationFailedCode, "user load data response massage: "+ responseUser.Message
                    +", board response message " + responseBoard.Message);    
            }

            return response.ToJson();
        }

        ///<summary>This method deletes all persisted data.	
        /// </summary>		 
        ///<returns>An empty response, unless an error occurs </returns>		 
        public string DeleteData()
        {
            Response response;
            try
            {
                TaskDataManager taskDataManager = new TaskDataManager();
                ColumnDataManager columnDataManager = new ColumnDataManager();
                BoardDataManager boardDataManager = new BoardDataManager();
                UserDataManager userDataManager = new UserDataManager();

                taskDataManager.DeleteData();
                columnDataManager.DeleteData();
                boardDataManager.DeleteData();
                userDataManager.DeleteData();

                response = new Response(ResponseCode.OperationSucceededCode);
            }
            catch (Exception ex) 
            {
                response = new Response(ResponseCode.OperationFailedCode, ex.Message);
            }
            
            return response.ToJson();
        }
    }
}
