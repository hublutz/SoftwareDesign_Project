using System;
using IntroSE.Kanban.Backend.BusinessLayer.UserController;

namespace IntroSE.Kanban.Backend.ServiceLayer.UserService
{
    /// <summary>
    /// Class <c>UserService</c> manages user operations in the Service Layer
    /// </summary>
    public class UserService
    {
        /// <summary>
        /// userController that is used in this class to execute services
        /// </summary>
        private UserController userController;
        
        /// <summary>
        /// user service constructor
        /// </summary>
        internal UserService(UserController userController)
        {
            this.userController = userController;
        }

        /// <summary>
        ///  This method logs in an existing user.
        /// </summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <returns>A response with the user's email, unless an error occurs</returns>
        public string Login(string email, string password)
        {
            try
            {
                userController.Login(email, password);
                return new Response(ResponseCode.OperationSucceededCode,email).ToJson();
            }catch(Exception ex)
            {
                return new Response(ResponseCode.OperationFailedCode, ex.Message).ToJson();
            }
        }
        /// <summary>
        /// This method registers a new user to the system.
        /// </summary>
        /// <param name="email">The user email address, used as the username for logging the system.</param>
        /// <param name="password">The user password.</param>
        /// <returns>An empty response, unless an error occurs </returns>
        public string Register(string email, string password)
        {
            try
            {
                userController.Register(email, password);
                return new Response(ResponseCode.OperationSucceededCode).ToJson();
            }
            catch (Exception ex)
            {
                return new Response(ResponseCode.OperationFailedCode, ex.Message).ToJson();
            }
        }
        /// <summary>
        /// This method logs out a logged in user. 
        /// </summary>
        /// <param name="email">The email of the user to log out</param>
        /// <returns>An empty response, unless an error occurs</returns>
        public string Logout(string email)
        {
            try
            {
                userController.Logout(email);
                return new Response(ResponseCode.OperationSucceededCode).ToJson();
            }
            catch (Exception ex)
            {
                return new Response(ResponseCode.OperationFailedCode, ex.Message).ToJson();
            }
        }

        /// <summary>
        /// loads the data in the userController
        /// </summary>
        /// <returns>An empty response Json, unless an error occurs</returns>
        internal string LoadData()
        {
            try
            {
                userController.LoadData();
                return new Response(ResponseCode.OperationSucceededCode).ToJson();
            
            }
            catch (Exception ex)
            {
                return new Response(ResponseCode.OperationFailedCode, ex.Message).ToJson();
            }
        }
    }
}
