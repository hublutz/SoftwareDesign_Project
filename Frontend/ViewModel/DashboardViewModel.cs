using Frontend.Model;
using Frontend.Model.Board;
using Frontend.Model.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Frontend.ViewModel
{
    /// <summary>
    /// Class <c>DashboardViewModel</c> represents the dashboard view model
    /// </summary>
    public class DashboardViewModel : NotifiableObject
    {
        /// <value>
        /// Property <c>backendController</c> represents the backend controller
        /// </value>
        public BackendController BackendController { get; set; }

        /// <value>
        /// Property <c>userModel</c> represents the user model of the logged in user
        /// </value>
        public UserModel UserModel { get; set; }

        /// <value>
        /// Property <c>boardControllerModel</c> represents the board controller model
        /// </value>
        private BoardControllerModel boardController;

        /// <value>
        /// Property <c>Boards</c> represents the boards the user is a member of
        /// </value>
        public ObservableCollection<BoardModel> Boards { get; set; }

        /// <summary>
        /// <c>DashboardViewModel</c> constructor, gets the backend controller and the user model
        /// </summary>
        /// <param name="backendController">The backend controller</param>
        /// <param name="userModel">The user model of the loggedin user</param>
        public DashboardViewModel(BackendController backendController, UserModel userModel)
        {
            this.UserModel = userModel;
            this.BackendController = backendController;
            this.boardController = this.BackendController.BoardController;
            this.Boards = new ObservableCollection<BoardModel>(getUserBoards(userModel));
        }


        /// <summary>
        /// This method gets all the boards the user is a member of.
        /// </summary>
        /// <param name="userModel">The user whose boards we want to get</param>
        /// <returns>All the boards the user is a member of</returns>
        private List<BoardModel> getUserBoards(UserModel userModel)
        { 
            return boardController.GetUserBoards(userModel.Email);
        }

        /// <summary>
        /// Logs out the user from the system
        /// </summary>
        public void Logout()
        {
            this.BackendController.UserController.Logout(UserModel.Email);
        }
    }
}
