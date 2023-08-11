using Frontend.Model.Board;
using Frontend.Model.User;
using IntroSE.Kanban.Backend.ServiceLayer;

namespace Frontend.Model
{
    /// <summary>
    /// Connects the frontend to the backend
    /// </summary>
    public class BackendController
    {
        /// <value>
        /// The service manager that is used in this class to execute services
        /// </value>
        private ServiceManager serviceManager;

        /// <value>
        /// Field <c>UserController</c> performs user operations using
        /// the Service Layer
        /// </value>
        public UserControllerModel UserController { get; set; }
        /// <value>
        /// Field <c>BoardController</c> performs board operations using
        /// the Service Layer
        /// </value>
        public BoardControllerModel BoardController { get; set; }

        /// <summary>
        /// Constructor that creates a new service manager and initialises the
        /// controllers
        /// </summary>
        public BackendController()
        {
            this.serviceManager = new ServiceManager();
            serviceManager.LoadData();
            this.UserController = new UserControllerModel(this.serviceManager.UserService);
            this.BoardController = new BoardControllerModel(this.serviceManager.BoardService);
        }

    }
}
