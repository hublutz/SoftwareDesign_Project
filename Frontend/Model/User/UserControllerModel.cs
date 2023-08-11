using IntroSE.Kanban.Backend.ServiceLayer.UserService;
using System;
using System.Text.Json;


namespace Frontend.Model.User
{
    public class UserControllerModel
    {
        /// <value>
        /// Field <c>userService</c> is the service ralated to the user
        /// </value>
        private UserService userService;


        /// <summary>
        /// <c>UserControllerModel</c> constructor
        /// </summary>
        /// <param name="userService">The user service that is used by the UserControllerModel
        public UserControllerModel(UserService userService) {  this.userService = userService; }

        /// <summary>
        /// This method registers a new user
        /// </summary>
        /// <param name="email">The email of the new user</param>
        /// <param name="password">The password of the new user</param>
        /// <returns>returns a <c>UserModel</c> of the new user </returns>
        /// <exception cref="Exception">throw an exeption if the operation has failes</exception>
        public UserModel Register(string email, string password) {

            Response response;
            string responseJson = this.userService.Register(email,password);
            response = JsonSerializer.Deserialize<Response>(responseJson);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                return new UserModel(email,password);
            }
            else
            {
                throw new Exception(response.Message.ToString());
            }
        }
        /// <summary>
        /// This method logins an existing user
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <param name="password">The password of the user</param>
        /// <returns>returns a <c>UserModel</c> of the user </returns>
        /// <exception cref="Exception">throw an exeption if the operation has failes</exception>
        public UserModel Login(string email, string password) {
            Response response;
            string responseJson = this.userService.Login(email, password);
            response = JsonSerializer.Deserialize<Response>(responseJson);
            if (response.Code == ResponseCode.OperationSucceededCode)
            {
                return new UserModel(email, password);
            }
            else
            {
                throw new Exception(response.Message.ToString());
            }
        }
        /// <summary>
        ///  This method logsout an existing user
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <exception cref="Exception">throw an exeption if the operation has failes</exception>
        public void Logout(string email) {

            Response response;
            string responseJson = this.userService.Logout(email);
            response = JsonSerializer.Deserialize<Response>(responseJson);
            if (response.Code != ResponseCode.OperationSucceededCode)
            {
                throw new Exception(response.Message.ToString());
            }
        }

    }
}
